using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using FreeTools.Core;
using Microsoft.Playwright;

namespace FreeTools.BrowserSnapshot;

internal class Program
{
    private static readonly object _consoleLock = new();

    private static async Task<int> Main(string[] args)
    {
        // Robustness: Optional startup delay (default 5 seconds for server warmup)
        var delayEnv = Environment.GetEnvironmentVariable("START_DELAY_MS");
        var startupDelay = int.TryParse(delayEnv, out var delayMs) && delayMs > 0 ? delayMs : 5000;
        
        Console.WriteLine($"Waiting {startupDelay}ms for server to be ready...");
        await Task.Delay(startupDelay);

        var baseUrl = CliArgs.GetEnvOrArg("BASE_URL", args, 0, "https://localhost:5001");
        var csvPath = CliArgs.GetEnvOrArg("CSV_PATH", args, 1, "pages.csv");
        var outputDir = CliArgs.GetEnvOrArg("OUTPUT_DIR", args, 2, "page-snapshots");
        var browserEnv = Environment.GetEnvironmentVariable("SCREENSHOT_BROWSER");
        var viewportEnv = Environment.GetEnvironmentVariable("SCREENSHOT_VIEWPORT");
        
        // Default to 10 threads for parallel processing
        var maxThreads = Math.Max(1, CliArgs.GetEnvOrArgInt("MAX_THREADS", args, 3, 10));

        var browserName = string.IsNullOrWhiteSpace(browserEnv)
            ? "chromium"
            : browserEnv.Trim().ToLowerInvariant();

        ConsoleOutput.PrintBanner("BrowserSnapshot (FreeTools)");
        ConsoleOutput.PrintConfig("BASE_URL", baseUrl);
        ConsoleOutput.PrintConfig("CSV_PATH", csvPath);
        ConsoleOutput.PrintConfig("OUTPUT_DIR", outputDir);
        ConsoleOutput.PrintConfig("BROWSER", browserName);
        ConsoleOutput.PrintConfig("VIEWPORT", string.IsNullOrWhiteSpace(viewportEnv) ? "(default)" : viewportEnv);
        ConsoleOutput.PrintConfig("MAX_THREADS", maxThreads.ToString());
        ConsoleOutput.PrintDivider();

        if (!File.Exists(csvPath))
        {
            Console.Error.WriteLine($"CSV file not found: {csvPath}");
            return 1;
        }

        var (routes, skippedRoutes) = await RouteParser.ParseRoutesFromCsvFileAsync(csvPath);

        foreach (var skipped in skippedRoutes)
        {
            Console.WriteLine($"  [SKIP] {skipped} - has route parameters");
        }

        if (skippedRoutes.Count > 0)
        {
            Console.WriteLine($"Skipped {skippedRoutes.Count} routes with parameters.");
        }

        if (routes.Count == 0)
        {
            Console.WriteLine("No screenshottable routes found in CSV.");
            return 0;
        }

        Console.WriteLine($"Found {routes.Count} routes to screenshot with {maxThreads} parallel workers.");

        Directory.CreateDirectory(outputDir);

        (int? viewportWidth, int? viewportHeight) = ParseViewport(viewportEnv);

        try
        {
            Console.WriteLine("[1/4] Ensuring Playwright browsers are installed...");
            await EnsurePlaywrightBrowsersInstalledAsync(browserName);
            
            Console.WriteLine("[2/4] Initializing Playwright...");
            using var playwright = await Playwright.CreateAsync();

            var browserType = browserName switch
            {
                "firefox" => playwright.Firefox,
                "webkit" => playwright.Webkit,
                _ => playwright.Chromium
            };

            Console.WriteLine($"[3/4] Launching {browserName} (headless)...");
            await using var browser = await browserType.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            Console.WriteLine("[4/4] Capturing screenshots for each route...");
            Console.WriteLine();

            var totalCount = routes.Count;
            var errorCount = 0;
            var httpErrorCount = 0;
            var successCount = 0;

            // Track results by index for ordered output
            var results = new ConcurrentDictionary<int, ScreenshotResult>();
            var nextIndexToWrite = 0;
            var writeLock = new object();

            var semaphore = new SemaphoreSlim(maxThreads);

            var tasks = routes.Select((route, index) => Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var result = await CaptureScreenshotAsync(
                        browser, route, index, totalCount, baseUrl, outputDir,
                        viewportWidth, viewportHeight);

                    // Store result
                    results[index] = result;

                    // Update counters
                    if (result.IsSuccess)
                    {
                        Interlocked.Increment(ref successCount);
                    }
                    else if (result.IsHttpError)
                    {
                        Interlocked.Increment(ref httpErrorCount);
                    }
                    else if (result.IsError)
                    {
                        Interlocked.Increment(ref errorCount);
                    }

                    // Try to write results in order
                    WriteResultsInOrder(results, ref nextIndexToWrite, writeLock);
                }
                finally
                {
                    semaphore.Release();
                }
            })).ToArray();

            await Task.WhenAll(tasks);

            // Write any remaining results
            WriteResultsInOrder(results, ref nextIndexToWrite, writeLock, flush: true);

            Console.WriteLine();
            Console.WriteLine($"Screenshot capture complete. Processed {totalCount} routes:");
            Console.WriteLine($"  - Successful (2xx/3xx): {successCount}");
            Console.WriteLine($"  - HTTP errors (4xx/5xx): {httpErrorCount}");
            Console.WriteLine($"  - Browser/timeout errors: {errorCount}");

            if (errorCount > 0)
            {
                Console.WriteLine("WARNING: Some browser/timeout errors occurred (non-fatal).");
            }

            Console.WriteLine("Completed successfully.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Fatal error in BrowserSnapshot:");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static async Task<ScreenshotResult> CaptureScreenshotAsync(
        IBrowser browser,
        string route,
        int index,
        int totalCount,
        string baseUrl,
        string outputDir,
        int? viewportWidth,
        int? viewportHeight)
    {
        var result = new ScreenshotResult
        {
            Index = index,
            Route = route,
            Number = index + 1,
            TotalCount = totalCount
        };

        var contextOptions = new BrowserNewContextOptions();
        if (viewportWidth.HasValue && viewportHeight.HasValue)
        {
            contextOptions.ViewportSize = new ViewportSize
            {
                Width = viewportWidth.Value,
                Height = viewportHeight.Value
            };
        }

        var context = await browser.NewContextAsync(contextOptions);
        try
        {
            var page = await context.NewPageAsync();
            var url = RouteParser.BuildUrl(baseUrl, route);
            result.Url = url;

            try
            {
                var response = await page.GotoAsync(url, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.Load,
                    Timeout = 60000
                });

                result.StatusCode = response?.Status ?? 0;

                if (result.StatusCode >= 200 && result.StatusCode < 400)
                {
                    result.IsSuccess = true;
                }
                else if (result.StatusCode >= 400)
                {
                    result.IsHttpError = true;
                }

                // Wait for page to settle
                await page.WaitForTimeoutAsync(1500);

                var screenshotPath = PathSanitizer.GetOutputFilePath(outputDir, route, "default.png");
                PathSanitizer.EnsureDirectoryExists(screenshotPath);

                await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = screenshotPath,
                    FullPage = true
                });

                var fi = new FileInfo(screenshotPath);
                result.ScreenshotPath = screenshotPath;
                result.FileSize = fi.Length;
            }
            catch (TimeoutException)
            {
                result.IsError = true;
                result.ErrorMessage = "Navigation timed out";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.ErrorMessage = ex.Message;
            }
        }
        finally
        {
            await context.CloseAsync();
        }

        return result;
    }

    private static void WriteResultsInOrder(
        ConcurrentDictionary<int, ScreenshotResult> results,
        ref int nextIndexToWrite,
        object writeLock,
        bool flush = false)
    {
        lock (writeLock)
        {
            // Write all consecutive results starting from nextIndexToWrite
            while (results.TryGetValue(nextIndexToWrite, out var result))
            {
                WriteResult(result);
                results.TryRemove(nextIndexToWrite, out _);
                nextIndexToWrite++;
            }

            // If flushing, write any remaining out-of-order results
            if (flush && results.Count > 0)
            {
                foreach (var kvp in results.OrderBy(k => k.Key))
                {
                    WriteResult(kvp.Value);
                }
                results.Clear();
            }
        }
    }

    private static void WriteResult(ScreenshotResult result)
    {
        Console.WriteLine($"[{result.Number}/{result.TotalCount}] {result.Route}");
        Console.WriteLine($"  URL: {result.Url}");
        
        if (result.IsError)
        {
            Console.WriteLine($"  !! {result.ErrorMessage}");
        }
        else
        {
            Console.WriteLine($"  -> Status: {result.StatusCode}");
            if (!string.IsNullOrEmpty(result.ScreenshotPath))
            {
                Console.WriteLine($"  -> Saved: {Path.GetFileName(result.ScreenshotPath)} ({PathSanitizer.FormatBytes(result.FileSize)})");
            }
        }
        Console.WriteLine();
    }

    private static async Task EnsurePlaywrightBrowsersInstalledAsync(string browserName)
    {
        var exitCode = Microsoft.Playwright.Program.Main(["install", browserName]);
        
        if (exitCode != 0)
        {
            Console.WriteLine($"Playwright install returned exit code {exitCode}, but continuing anyway...");
        }
        else
        {
            Console.WriteLine($"Playwright {browserName} browser ready.");
        }
        
        await Task.CompletedTask;
    }

    private static (int? width, int? height) ParseViewport(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return (null, null);

        var parts = value.Split('x', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 2 &&
            int.TryParse(parts[0], out var w) &&
            int.TryParse(parts[1], out var h))
        {
            return (w, h);
        }

        return (null, null);
    }
}

// Result tracking for ordered output
internal class ScreenshotResult
{
    public int Index { get; set; }
    public int Number { get; set; }
    public int TotalCount { get; set; }
    public string Route { get; set; } = "";
    public string Url { get; set; } = "";
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public bool IsHttpError { get; set; }
    public bool IsError { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ScreenshotPath { get; set; }
    public long FileSize { get; set; }
}
