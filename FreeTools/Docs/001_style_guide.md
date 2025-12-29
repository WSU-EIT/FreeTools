# FreeTools Style Guide

## Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Classes | PascalCase | `CliArgs`, `ConsoleOutput`, `PathSanitizer`, `RouteParser` |
| Methods | PascalCase | `HasFlag()`, `GetOption()`, `GetEnvOrArg()` |
| Properties | PascalCase | `FilePath`, `LineCount`, `RequiresAuth` |
| Private Fields | _camelCase | `_lock`, `_consoleLock` |
| Local Variables | camelCase | `baseUrl`, `processedCount`, `errorCount` |
| Constants | UPPERCASE | `DefaultIncludePatterns`, `DefaultMaxParseSizeBytes` |

## File Organization

```
tools/
├── FreeTools.Core/              # Shared utilities (no dependencies)
│   ├── CliArgs.cs
│   ├── ConsoleOutput.cs
│   ├── PathSanitizer.cs
│   └── RouteParser.cs
├── FreeTools.AppHost/           # Aspire orchestrator
├── FreeTools.EndpointMapper/    # Razor page scanner
├── FreeTools.EndpointPoker/     # HTTP endpoint tester
├── FreeTools.BrowserSnapshot/   # Screenshot tool
├── FreeTools.WorkspaceInventory/# File inventory
├── FreeTools.WorkspaceReporter/ # Report generator
├── FreeTools.Tests/             # Test project
└── FreeTools.slnx               # Solution file
```

### Organization Principles
- One concern per file
- Namespace = Directory structure
- Flat structure by logical concern
- Single `Program.cs` entry point per tool

## Code Formatting

### Brace Style (Allman)
```csharp
public static bool HasFlag(List<string> args, string flag)
{
    var index = args.FindIndex(a => a.Equals(flag, StringComparison.OrdinalIgnoreCase));
    if (index >= 0)
    {
        args.RemoveAt(index);
        return true;
    }
    return false;
}
```

### Expression Bodies (Short Methods)
```csharp
public static string? GetPositional(List<string> args, int index, string? defaultValue = null)
    => args.Count > index ? args[index] : defaultValue;

public static string BuildUrl(string baseUrl, string route)
    => baseUrl.TrimEnd('/') + "/" + route.TrimStart('/');
```

### Indentation
- 4 spaces (standard .NET)
- UTF-8 with BOM
- Unix line endings (LF)

## Common Patterns

### Async/Await
```csharp
private static async Task<int> Main(string[] args)
{
    var (routes, skippedRoutes) = await RouteParser.ParseRoutesFromCsvFileAsync(csvPath);

    using var httpClient = new HttpClient(httpClientHandler)
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    var tasks = routes.Select(async route =>
    {
        await semaphore.WaitAsync();
        try { /* work */ }
        finally { semaphore.Release(); }
    }).ToArray();

    await Task.WhenAll(tasks);
}
```

### Null Handling
```csharp
var envRoot = Environment.GetEnvironmentVariable("FREETOOLS_ROOT")
    ?? (args.Length > argIndex ? args[argIndex] : null)
    ?? defaultValue;

var contentType = response.Content.Headers.ContentType?.MediaType ?? "unknown";
```

### Switch Expressions
```csharp
private static string ClassifyByExtension(string extension) => extension switch
{
    ".razor" => "RazorComponent",
    ".cs" => "CSharpSource",
    ".csproj" => "ProjectFile",
    ".json" or ".config" or ".xml" => "Config",
    _ => "Other"
};

return bytes switch
{
    >= GB => $"{bytes / (double)GB:F1} GB",
    >= MB => $"{bytes / (double)MB:F1} MB",
    >= KB => $"{bytes / (double)KB:F1} KB",
    _ => $"{bytes} bytes"
};
```

### Exception Handling
```csharp
try
{
    var response = await page.GotoAsync(url, new PageGotoOptions
    {
        WaitUntil = WaitUntilState.Load,
        Timeout = 60000
    });
}
catch (TimeoutException)
{
    Interlocked.Increment(ref errorCount);
    ConsoleOutput.WriteLine("  !! Navigation timed out", isError: true);
}
catch (Exception ex)
{
    ConsoleOutput.WriteLine($"  !! Error: {ex.Message}", isError: true);
}
```

### Threading & Concurrency
```csharp
var semaphore = new SemaphoreSlim(maxThreads);
Interlocked.Increment(ref processedCount);

private static readonly object _lock = new();
lock (_lock)
{
    Console.WriteLine(message);
}
```

### Compiled Regex
```csharp
var pageRegex = new Regex(@"@page\s+""(?<route>[^""]+)""", RegexOptions.Compiled);
```

### CLI Argument Pattern
```csharp
var baseUrl = CliArgs.GetEnvOrArg("BASE_URL", args, 0, "https://localhost:5001");
var maxThreads = Math.Max(1, CliArgs.GetEnvOrArgInt("MAX_THREADS", args, 3, 100));

ConsoleOutput.PrintBanner("EndpointPoker (FreeTools)");
ConsoleOutput.PrintConfig("Base URL", baseUrl);
ConsoleOutput.PrintDivider();
```

### Environment Configuration Priority
1. Environment variables (highest)
2. CLI arguments
3. Defaults (lowest)

## Documentation

### XML Comments
```csharp
/// <summary>
/// Command-line argument parsing utilities.
/// </summary>
public static class CliArgs { }

/// <summary>
/// Get a string value from environment variable or CLI arg, with fallback default.
/// </summary>
public static string GetEnvOrArg(string envVar, string[] args, int argIndex, string defaultValue)

/// <summary>
/// Convert a route to a safe directory path for output files.
/// Example: "/Account/Login" -> "Account\Login" (on Windows)
/// </summary>
public static string RouteToDirectoryPath(string route)
```

### Inline Comments
```csharp
// Try environment variable first
var envRoot = Environment.GetEnvironmentVariable("FREETOOLS_ROOT");

// Walk up from base directory
var dir = AppContext.BaseDirectory;
```

## Technology Stack

```xml
<TargetFramework>net10.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings>
<Nullable>enable</Nullable>
<LangVersion>14.0</LangVersion>
```

### Key Dependencies
- **FreeTools.Core**: No external dependencies (pure .NET)
- **WorkspaceInventory**: Microsoft.CodeAnalysis.CSharp, FileSystemGlobbing
- **BrowserSnapshot**: Microsoft.Playwright
- **AppHost**: Aspire.Hosting.AppHost 9.2.0

## Return Code Convention
- `0`: Success
- `1`: Failure

```csharp
return errorCount > 0 ? 1 : 0;
```

## Key Characteristics

1. CLI-first design
2. Static utility classes (stateless, functional approach)
3. No IoC container (lightweight, dependency-free)
4. Parallel task execution with semaphores
5. Modern C# 14 features
6. Aspire orchestration for complex pipelines
7. Environment variable + CLI arg configuration
