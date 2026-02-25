using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using FreeExamples.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FreeExamples.Server.Services;

/// <summary>
/// Service that clones public git repositories and loads their entire contents into memory.
/// Shallow-clones via git CLI (depth=1), reads all files into an in-memory tree, then
/// immediately deletes the temp directory. Nothing stays on disk.
/// Emits real-time progress via SignalR so the client can show each step.
/// </summary>
public class GitBrowserService : IDisposable
{
    /// <summary>
    /// Represents one node in the in-memory file tree (either a directory or a file).
    /// </summary>
    private class MemoryNode
    {
        public string Name { get; set; } = "";
        public string RelativePath { get; set; } = "";
        public bool IsDirectory { get; set; }
        public long Size { get; set; }
        public string Extension { get; set; } = "";
        public bool IsBinary { get; set; }
        public string? Content { get; set; }

        /// <summary>Children keyed by name (only populated for directories).</summary>
        public Dictionary<string, MemoryNode>? Children { get; set; }
    }

    private readonly ConcurrentDictionary<string, MemoryNode> _repos = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _cloneLock = new(1, 1);
    private readonly IHubContext<freeexamplesHub, IsrHub> _signalR;
    private bool _disposed;

    private const long MaxFileSize = 512 * 1024; // 512 KB — skip larger files

    public GitBrowserService(IHubContext<freeexamplesHub, IsrHub> signalR)
    {
        _signalR = signalR;
    }

    /// <summary>
    /// Sends a clone progress update to all clients via SignalR.
    /// </summary>
    private async Task SendProgress(string message)
    {
        try {
            await _signalR.Clients.All.SignalRUpdate(new DataObjects.SignalRUpdate {
                UpdateType = DataObjects.SignalRUpdateType.GitCloneProgress,
                Message = message,
            });
        } catch {
            // Best-effort — don't let SignalR failures break the clone
        }
    }

    /// <summary>
    /// Ensures a repo is loaded into memory. Shallow-clones to a temp dir, reads everything
    /// into an in-memory tree, then deletes the temp dir immediately.
    /// </summary>
    private async Task<MemoryNode> EnsureLoadedAsync(string repoUrl)
    {
        if (_repos.TryGetValue(repoUrl, out var cached))
        {
            await SendProgress("Repository already cached in memory.");
            return cached;
        }

        await _cloneLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (_repos.TryGetValue(repoUrl, out cached))
            {
                await SendProgress("Repository already cached in memory.");
                return cached;
            }

            var tempDir = Path.Combine(Path.GetTempPath(), "FreeExamples-GitBrowser", Guid.NewGuid().ToString("N")[..12]);
            Directory.CreateDirectory(tempDir);

            try
            {
                // ── Step 1: Shallow clone ──
                await SendProgress("Starting shallow clone (depth=1, single branch, no tags)...");

                var sw = Stopwatch.StartNew();
                var (exitCode, output) = await RunGitAsync(
                    $"clone --depth 1 --single-branch --no-tags \"{repoUrl}\" \"{tempDir}\"",
                    onStdErr: async line => {
                        if (!string.IsNullOrWhiteSpace(line))
                            await SendProgress(line.Trim());
                    });

                sw.Stop();

                if (exitCode != 0)
                {
                    await SendProgress($"Clone failed (exit code {exitCode}). Is the URL a valid public repo?");
                    throw new InvalidOperationException($"git clone failed with exit code {exitCode}: {output}");
                }

                await SendProgress($"Clone finished in {sw.Elapsed.TotalSeconds:F1}s — reading files into memory...");

                // ── Step 2: Read entire tree into memory ──
                var root = new MemoryNode
                {
                    Name = "",
                    RelativePath = "",
                    IsDirectory = true,
                    Children = new(StringComparer.OrdinalIgnoreCase),
                };

                // counters: [0]=fileCount, [1]=dirCount, [2]=totalBytes
                var counters = new long[3];

                await ReadDirectoryIntoMemory(tempDir, tempDir, root, counters);

                await SendProgress($"Loaded {counters[0]:N0} files, {counters[1]:N0} folders ({FormatBytes(counters[2])}) into memory.");

                // ── Step 3: Delete temp dir ──
                await SendProgress("Cleaning up temp directory...");
                DeleteDirectoryBestEffort(tempDir);
                await SendProgress("Done — serving entirely from memory.");

                _repos[repoUrl] = root;
                return root;
            }
            catch
            {
                // Always clean up on failure
                DeleteDirectoryBestEffort(tempDir);
                throw;
            }
        }
        finally
        {
            _cloneLock.Release();
        }
    }

    /// <summary>
    /// Recursively reads a directory into the in-memory tree.
    /// </summary>
    private async Task ReadDirectoryIntoMemory(string rootPath, string currentPath, MemoryNode parentNode,
        long[] counters)
    {
        // Directories first
        foreach (var dir in Directory.GetDirectories(currentPath).OrderBy(d => Path.GetFileName(d), StringComparer.OrdinalIgnoreCase))
        {
            var name = Path.GetFileName(dir);
            if (name.StartsWith('.'))
                continue; // Skip .git etc.

            var relativePath = Path.GetRelativePath(rootPath, dir).Replace('\\', '/');
            var dirNode = new MemoryNode
            {
                Name = name,
                RelativePath = relativePath,
                IsDirectory = true,
                Children = new(StringComparer.OrdinalIgnoreCase),
            };

            parentNode.Children![name] = dirNode;
            counters[1]++;

            await ReadDirectoryIntoMemory(rootPath, dir, dirNode, counters);
        }

        // Files
        foreach (var file in Directory.GetFiles(currentPath).OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase))
        {
            var name = Path.GetFileName(file);
            var relativePath = Path.GetRelativePath(rootPath, file).Replace('\\', '/');
            var fi = new FileInfo(file);
            var ext = fi.Extension.ToLowerInvariant();

            var fileNode = new MemoryNode
            {
                Name = name,
                RelativePath = relativePath,
                IsDirectory = false,
                Size = fi.Length,
                Extension = ext,
            };

            if (fi.Length > MaxFileSize)
            {
                fileNode.IsBinary = true;
                fileNode.Content = $"[File too large to display: {fi.Length:N0} bytes]";
            }
            else
            {
                var bytes = await File.ReadAllBytesAsync(file);

                // Binary detection: check for null bytes in the first 8KB
                var checkLen = Math.Min(bytes.Length, 8192);
                bool isBinary = false;
                for (int i = 0; i < checkLen; i++)
                {
                    if (bytes[i] == 0) { isBinary = true; break; }
                }

                if (isBinary)
                {
                    fileNode.IsBinary = true;
                    fileNode.Content = $"[Binary file: {fi.Length:N0} bytes]";
                }
                else
                {
                    fileNode.Content = Encoding.UTF8.GetString(bytes);
                }
            }

            parentNode.Children![name] = fileNode;
            counters[0]++;
            counters[2] += fi.Length;
        }
    }

    /// <summary>
    /// Navigates the in-memory tree to find a node at the given path.
    /// </summary>
    private static MemoryNode? FindNode(MemoryNode root, string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return root;

        var current = root;
        foreach (var segment in path.Split('/'))
        {
            if (string.IsNullOrEmpty(segment)) continue;
            if (current.Children == null || !current.Children.TryGetValue(segment, out var child))
                return null;
            current = child;
        }
        return current;
    }

    /// <summary>
    /// Lists entries (files/folders) at a given path within the repo — from memory.
    /// </summary>
    public async Task<List<DataObjects.GitRepoEntry>> BrowseAsync(string repoUrl, string? path)
    {
        var root = await EnsureLoadedAsync(repoUrl);
        var node = FindNode(root, path);

        if (node == null || !node.IsDirectory || node.Children == null)
            return new List<DataObjects.GitRepoEntry>();

        var entries = new List<DataObjects.GitRepoEntry>();

        // Directories first, then files (already sorted during load)
        foreach (var child in node.Children.Values.Where(c => c.IsDirectory))
        {
            entries.Add(new DataObjects.GitRepoEntry
            {
                Name = child.Name,
                Path = child.RelativePath,
                IsDirectory = true,
            });
        }

        foreach (var child in node.Children.Values.Where(c => !c.IsDirectory))
        {
            entries.Add(new DataObjects.GitRepoEntry
            {
                Name = child.Name,
                Path = child.RelativePath,
                IsDirectory = false,
                Size = child.Size,
                Extension = child.Extension,
            });
        }

        return entries;
    }

    /// <summary>
    /// Reads the content of a single file from the in-memory tree.
    /// </summary>
    public async Task<DataObjects.GitFileContent> GetFileAsync(string repoUrl, string filePath)
    {
        var root = await EnsureLoadedAsync(repoUrl);
        var node = FindNode(root, filePath);

        var result = new DataObjects.GitFileContent
        {
            Path = filePath,
            Name = Path.GetFileName(filePath),
            Extension = Path.GetExtension(filePath).ToLowerInvariant(),
        };

        if (node == null || node.IsDirectory)
        {
            result.Content = "File not found.";
            return result;
        }

        result.Size = node.Size;
        result.IsBinary = node.IsBinary;
        result.Content = node.Content ?? "";
        return result;
    }

    /// <summary>
    /// Runs a git command and returns the exit code and combined output.
    /// Streams stderr lines to the caller for real-time progress.
    /// </summary>
    private static async Task<(int ExitCode, string Output)> RunGitAsync(string arguments, Func<string, Task>? onStdErr = null)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = new Process { StartInfo = psi };
        var outputBuilder = new StringBuilder();

        process.Start();

        // Read stdout in background
        var stdoutTask = Task.Run(async () => {
            while (await process.StandardOutput.ReadLineAsync() is { } line)
                outputBuilder.AppendLine(line);
        });

        // Read stderr line-by-line for progress
        while (await process.StandardError.ReadLineAsync() is { } line)
        {
            outputBuilder.AppendLine(line);
            if (onStdErr != null)
                await onStdErr(line);
        }

        await stdoutTask;
        await process.WaitForExitAsync();

        return (process.ExitCode, outputBuilder.ToString());
    }

    private static void DeleteDirectoryBestEffort(string path)
    {
        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
        }
        catch { /* Best-effort cleanup */ }
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _repos.Clear();
        _cloneLock.Dispose();
    }
}
