using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FreeTools.Core;
using Microsoft.Playwright;

namespace FreeTools.AccessibilityScanner;

internal class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private static async Task<int> Main(string[] args)
    {
        ConsoleOutput.PrintBanner("AccessibilityScanner (FreeTools)", "1.0");

        // Load config from appsettings.json (next to the exe)
        var config = await LoadConfigAsync();
        if (config == null)
        {
            Console.Error.WriteLine("Failed to load appsettings.json — cannot continue.");
            return 1;
        }

        if (config.Sites.Count == 0)
        {
            Console.Error.WriteLine("No sites configured in appsettings.json → Scanner.Sites");
            return 1;
        }

        // Output goes next to Program.cs source: runs/latest/{site-folder}
        var projectDir = FindProjectDir(AppContext.BaseDirectory);
        var runsDir = Path.Combine(projectDir, "runs", "latest");
        ConsoleOutput.PrintConfig("Output", runsDir);
        ConsoleOutput.PrintConfig("Sites", config.Sites.Count.ToString());
        ConsoleOutput.PrintConfig("Settle delay", $"{config.SettleDelayMs}ms");
        ConsoleOutput.PrintConfig("Timeout", $"{config.TimeoutMs}ms");
        ConsoleOutput.PrintConfig("Max concurrency", config.MaxConcurrency.ToString());
        ConsoleOutput.PrintConfig("Headless", config.Headless.ToString());
        ConsoleOutput.PrintDivider();

        // Show configured sites and page counts
        var totalPages = 0;
        foreach (var (url, siteConfig) in config.Sites)
        {
            // +1 for the root page which is always scanned
            var pageCount = 1 + siteConfig.Pages.Count;
            totalPages += pageCount;
            var credLabel = siteConfig.Credentials.Count > 0
                ? $"{siteConfig.Credentials.Count} credential(s)"
                : "no auth";
            Console.WriteLine($"  • {url} — {pageCount} page(s), {credLabel}");
        }

        Console.WriteLine();
        Console.WriteLine($"  Total pages to scan: {totalPages}");
        Console.WriteLine();

        // Clean previous run
        if (Directory.Exists(runsDir))
        {
            Directory.Delete(runsDir, recursive: true);
        }

        try
        {
            Console.WriteLine("[1/3] Ensuring Playwright browsers are installed...");
            var exitCode = Microsoft.Playwright.Program.Main(["install", "chromium"]);
            if (exitCode != 0)
            {
                Console.WriteLine($"  Playwright install returned {exitCode}, continuing anyway...");
            }
            else
            {
                Console.WriteLine("  Chromium ready.");
            }

            Console.WriteLine("[2/3] Launching browser...");
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = config.Headless
            });

            var maxConcurrency = Math.Max(1, config.MaxConcurrency);
            Console.WriteLine($"[3/3] Scanning sites ({maxConcurrency} at a time)...");
            Console.WriteLine();

            var allResults = new ConcurrentBag<SiteResult>();
            var completed = 0;
            var siteTotal = config.Sites.Count;
            var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);

            var tasks = config.Sites.Select(kvp => Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var result = await ScanSiteAsync(browser, kvp.Key, kvp.Value, config, runsDir);
                    allResults.Add(result);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"  [{kvp.Key}] Site scan failed: {ex.Message}");
                    allResults.Add(new SiteResult
                    {
                        Url = kvp.Key,
                        FolderName = new Uri(kvp.Key).Host.Replace('.', '-')
                    });
                }
                finally
                {
                    var done = Interlocked.Increment(ref completed);
                    Console.WriteLine($"  ── Site {done}/{siteTotal} complete: {kvp.Key}");
                    semaphore.Release();
                }
            })).ToArray();

            await Task.WhenAll(tasks);

            // Write run-level summary report
            await WriteRunReportAsync(runsDir, allResults);

            // Print summary
            Console.WriteLine();
            ConsoleOutput.PrintDivider("Summary");

            var grandSuccessCount = 0;
            var grandTotalCount = 0;

            foreach (var site in allResults.OrderBy(r => r.Url))
            {
                Console.WriteLine($"  📂 {site.Url} → {site.FolderName}/");

                foreach (var page in site.Pages.OrderBy(p => p.PagePath))
                {
                    grandTotalCount++;
                    var status = page.Success ? "✅" : "❌";
                    if (page.Success) grandSuccessCount++;

                    Console.WriteLine($"     {status} {page.PagePath}");
                    Console.WriteLine($"        Status:      {page.StatusCode}");
                    Console.WriteLine($"        HTML:        {PathSanitizer.FormatBytes(page.HtmlSize)}");
                    Console.WriteLine($"        Screenshots: {page.Screenshots.Count} ({PathSanitizer.FormatBytes(page.ScreenshotSize)})");
                    Console.WriteLine($"        Images:      {page.Images.Count} ({PathSanitizer.FormatBytes(page.ImagesTotalSize)})");

                    if (page.ConsoleErrors.Count > 0)
                    {
                        Console.WriteLine($"        JS errors:  {page.ConsoleErrors.Count}");
                    }

                    if (page.ConsoleWarnings.Count > 0)
                    {
                        Console.WriteLine($"        JS warns:   {page.ConsoleWarnings.Count}");
                    }

                    if (page.CredentialUsed != null)
                    {
                        Console.WriteLine($"        Auth:       {page.CredentialUsed}");
                    }

                    if (page.ErrorMessage != null)
                    {
                        Console.WriteLine($"        Error:      {page.ErrorMessage}");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine($"Completed: {grandSuccessCount}/{grandTotalCount} pages scanned successfully across {allResults.Count} sites.");
            Console.WriteLine($"Output: {runsDir}");

            return grandSuccessCount == grandTotalCount ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fatal error: {ex.Message}");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    // ========================================================================
    // Site-level orchestration
    // ========================================================================

    private static async Task<SiteResult> ScanSiteAsync(
        IBrowser browser,
        string siteUrl,
        SiteConfig siteConfig,
        ScannerConfig scannerConfig,
        string runsDir)
    {
        var uri = new Uri(siteUrl);
        var siteFolderName = uri.Host.Replace('.', '-');
        var siteDir = Path.Combine(runsDir, siteFolderName);
        Directory.CreateDirectory(siteDir);

        var siteResult = new SiteResult
        {
            Url = siteUrl,
            FolderName = siteFolderName
        };

        // Build the list of pages to scan: root ("/") + configured pages
        var pagePaths = new List<string> { "/" };
        pagePaths.AddRange(siteConfig.Pages);

        foreach (var pagePath in pagePaths)
        {
            try
            {
                var pageResult = await ScanPageAsync(
                    browser, uri, pagePath, siteConfig, scannerConfig, siteDir);
                siteResult.Pages.Add(pageResult);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"  [{uri.Host}] {pagePath} — Skipped (unhandled error): {ex.Message}");
                siteResult.Pages.Add(new PageResult
                {
                    PagePath = pagePath,
                    FullUrl = ResolvePageUrl(uri, pagePath),
                    FolderName = PagePathToFolderName(pagePath),
                    Success = false,
                    ErrorMessage = ex.Message,
                    CapturedAt = DateTime.UtcNow
                });
            }
        }

        // Write site-level summary report
        await WriteSiteReportAsync(siteDir, siteResult);

        return siteResult;
    }

    // ========================================================================
    // Page-level scanning
    // ========================================================================

    private static async Task<PageResult> ScanPageAsync(
        IBrowser browser,
        Uri siteUri,
        string pagePath,
        SiteConfig siteConfig,
        ScannerConfig scannerConfig,
        string siteDir)
    {
        // Resolve the full URL (handles both relative and absolute paths)
        var fullUrl = ResolvePageUrl(siteUri, pagePath);
        var folderName = PagePathToFolderName(pagePath);
        var pageDir = Path.Combine(siteDir, folderName);
        Directory.CreateDirectory(pageDir);

        var result = new PageResult
        {
            PagePath = pagePath,
            FullUrl = fullUrl,
            FolderName = folderName,
            CapturedAt = DateTime.UtcNow
        };

        var actions = new List<string>();
        var infoLines = new List<string>();

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            UserAgent = scannerConfig.UserAgent
        });

        try
        {
            var page = await context.NewPageAsync();

            // Capture JS console messages by level
            List<string> consoleErrors = [];
            List<string> consoleWarnings = [];
            List<string> consoleInfo = [];

            page.Console += (_, msg) =>
            {
                switch (msg.Type)
                {
                    case "error":
                        consoleErrors.Add(msg.Text);
                        break;
                    case "warning":
                        consoleWarnings.Add(msg.Text);
                        break;
                    case "info":
                    case "log":
                        consoleInfo.Add(msg.Text);
                        break;
                }
            };

            Console.WriteLine($"  [{siteUri.Host}] {pagePath} — Navigating...");
            infoLines.Add($"Navigating to: {fullUrl}");
            infoLines.Add($"Started at: {result.CapturedAt:O}");

            // Navigate
            IResponse? response = null;
            try
            {
                response = await page.GotoAsync(fullUrl, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle,
                    Timeout = scannerConfig.TimeoutMs
                });
            }
            catch (TimeoutException)
            {
                Console.WriteLine($"  [{siteUri.Host}] {pagePath} — NetworkIdle timed out — capturing current state...");
                infoLines.Add("WARNING: NetworkIdle timed out — captured current page state");
            }

            result.StatusCode = response?.Status ?? 0;
            infoLines.Add($"Status code: {result.StatusCode}");

            // Check for redirect
            var currentUrl = page.Url;
            if (!string.Equals(fullUrl.TrimEnd('/'), currentUrl.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
            {
                infoLines.Add($"Redirected to: {currentUrl}");
                result.FinalUrl = currentUrl;
            }

            // Settle delay
            await page.WaitForTimeoutAsync(scannerConfig.SettleDelayMs);
            infoLines.Add($"Settle delay: {scannerConfig.SettleDelayMs}ms");

            // Screenshot: page after initial load + settle
            await TakeScreenshotAsync(page, pageDir, result, actions, "page-loaded");

            // Auth flow if credentials configured
            if (siteConfig.Credentials.Count > 0)
            {
                foreach (var cred in siteConfig.Credentials)
                {
                    Console.WriteLine($"  [{siteUri.Host}] {pagePath} — Attempting auth as '{cred.Username}'...");
                    actions.Add($"Attempted login as '{cred.Username}'");
                    var authSuccess = await TryAuthFlowAsync(page, cred, scannerConfig, pageDir, result, actions);
                    if (authSuccess)
                    {
                        result.CredentialUsed = cred.Username;
                        actions.Add($"Auth flow completed for '{cred.Username}'");
                        Console.WriteLine($"  [{siteUri.Host}] {pagePath} — Auth completed for '{cred.Username}'");
                        break;
                    }
                    else
                    {
                        actions.Add($"No login form found for '{cred.Username}'");
                    }
                }
            }

            if (actions.Count == 0)
            {
                actions.Add("No interactions performed — page was captured as-is");
            }

            // Save HTML content
            Console.WriteLine($"  [{siteUri.Host}] {pagePath} — Saving HTML...");
            var htmlContent = await page.ContentAsync();
            var htmlPath = Path.Combine(pageDir, "page.html");
            await File.WriteAllTextAsync(htmlPath, htmlContent);
            result.HtmlSize = new FileInfo(htmlPath).Length;
            infoLines.Add($"HTML size: {PathSanitizer.FormatBytes(result.HtmlSize)}");

            // Download page images
            Console.WriteLine($"  [{siteUri.Host}] {pagePath} — Downloading images...");
            await DownloadPageImagesAsync(page, pageDir, result, actions, infoLines);

            // Page title
            var title = await page.TitleAsync();
            result.Title = title;
            infoLines.Add($"Title: {title}");

            // Response headers
            var headers = response?.Headers;
            infoLines.Add($"Screenshots: {result.Screenshots.Count} ({PathSanitizer.FormatBytes(result.ScreenshotSize)})");
            infoLines.Add($"Completed at: {DateTime.UtcNow:O}");

            // Populate result
            result.ConsoleErrors = consoleErrors;
            result.ConsoleWarnings = consoleWarnings;
            result.Success = result.StatusCode >= 200 && result.StatusCode < 400;

            // Write all output files
            await WritePageOutputAsync(pageDir, result, actions, consoleErrors, consoleWarnings, consoleInfo, infoLines, headers);

            Console.WriteLine($"  [{siteUri.Host}] {pagePath} — Done — {result.StatusCode}");
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            Console.Error.WriteLine($"  [{siteUri.Host}] {pagePath} — Error: {ex.Message}");

            // Still write what we can
            infoLines.Add($"FATAL ERROR: {ex.Message}");
            actions.Add("Scan aborted due to error");
            try
            {
                await WritePageOutputAsync(pageDir, result, actions, [], [], [], infoLines, null);
            }
            catch { /* Don't fail on error reporting */ }
        }
        finally
        {
            await context.CloseAsync();
        }

        return result;
    }

    // ========================================================================
    // Page output files
    // ========================================================================

    private static async Task WritePageOutputAsync(
        string pageDir,
        PageResult result,
        List<string> actions,
        List<string> consoleErrors,
        List<string> consoleWarnings,
        List<string> consoleInfo,
        List<string> infoLines,
        Dictionary<string, string>? headers)
    {
        // errors.log
        var errorsPath = Path.Combine(pageDir, "errors.log");
        if (consoleErrors.Count > 0)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# JavaScript Console Errors — {result.FullUrl}");
            sb.AppendLine($"# Captured: {result.CapturedAt:O}");
            sb.AppendLine($"# Count: {consoleErrors.Count}");
            sb.AppendLine();
            foreach (var error in consoleErrors)
            {
                sb.AppendLine(error);
            }

            await File.WriteAllTextAsync(errorsPath, sb.ToString());
        }
        else
        {
            await File.WriteAllTextAsync(errorsPath, $"# No JavaScript errors captured\n# {result.FullUrl}\n# {result.CapturedAt:O}\n");
        }

        // warnings.log
        var warningsPath = Path.Combine(pageDir, "warnings.log");
        if (consoleWarnings.Count > 0)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# JavaScript Console Warnings — {result.FullUrl}");
            sb.AppendLine($"# Captured: {result.CapturedAt:O}");
            sb.AppendLine($"# Count: {consoleWarnings.Count}");
            sb.AppendLine();
            foreach (var warning in consoleWarnings)
            {
                sb.AppendLine(warning);
            }

            await File.WriteAllTextAsync(warningsPath, sb.ToString());
        }
        else
        {
            await File.WriteAllTextAsync(warningsPath, $"# No JavaScript warnings captured\n# {result.FullUrl}\n# {result.CapturedAt:O}\n");
        }

        // info.log
        var infoPath = Path.Combine(pageDir, "info.log");
        var infoSb = new StringBuilder();
        infoSb.AppendLine($"# Scan Info — {result.FullUrl}");
        infoSb.AppendLine();
        foreach (var line in infoLines)
        {
            infoSb.AppendLine(line);
        }

        if (consoleInfo.Count > 0)
        {
            infoSb.AppendLine();
            infoSb.AppendLine($"# Console log/info messages ({consoleInfo.Count}):");
            foreach (var msg in consoleInfo)
            {
                infoSb.AppendLine($"  {msg}");
            }
        }

        await File.WriteAllTextAsync(infoPath, infoSb.ToString());

        // actions.log
        var actionsPath = Path.Combine(pageDir, "actions.log");
        var actionsSb = new StringBuilder();
        actionsSb.AppendLine($"# Actions Performed — {result.FullUrl}");
        actionsSb.AppendLine($"# Captured: {result.CapturedAt:O}");
        actionsSb.AppendLine();
        foreach (var action in actions)
        {
            actionsSb.AppendLine($"- {action}");
        }

        await File.WriteAllTextAsync(actionsPath, actionsSb.ToString());

        // report.md
        var reportPath = Path.Combine(pageDir, "report.md");
        var reportSb = new StringBuilder();
        var statusEmoji = result.Success ? "✅" : "❌";
        var missingAltCount = result.Images.Count(i => string.IsNullOrWhiteSpace(i.AltText));

        reportSb.AppendLine($"# 📄 Page Scan Report");
        reportSb.AppendLine();
        reportSb.AppendLine($"> **URL:** {result.FullUrl}  ");
        reportSb.AppendLine($"> **Captured:** {result.CapturedAt:yyyy-MM-dd HH:mm:ss} UTC  ");
        reportSb.AppendLine($"> **Status:** {statusEmoji} {result.StatusCode}  ");
        reportSb.AppendLine();
        reportSb.AppendLine("---");
        reportSb.AppendLine();

        // Table of Contents
        reportSb.AppendLine("## 📑 Contents");
        reportSb.AppendLine();
        reportSb.AppendLine("- [Summary](#-summary)");
        reportSb.AppendLine("- [Screenshots](#-screenshots)");
        reportSb.AppendLine("- [Page Images](#-page-images)");
        if (result.ConsoleErrors.Count > 0) reportSb.AppendLine("- [JavaScript Errors](#-javascript-errors)");
        reportSb.AppendLine("- [Actions](#-actions)");
        reportSb.AppendLine("- [Files](#-files)");
        reportSb.AppendLine();
        reportSb.AppendLine("---");
        reportSb.AppendLine();

        // Summary
        reportSb.AppendLine($"## 📋 Summary");
        reportSb.AppendLine();
        reportSb.AppendLine($"| Field | Value |");
        reportSb.AppendLine($"|-------|-------|");
        reportSb.AppendLine($"| URL | {result.FullUrl} |");
        if (result.FinalUrl != null)
        {
            reportSb.AppendLine($"| Redirected To | {result.FinalUrl} |");
        }

        reportSb.AppendLine($"| Title | {result.Title ?? "*(none)*"} |");
        reportSb.AppendLine($"| Status | {statusEmoji} {result.StatusCode} |");
        reportSb.AppendLine($"| HTML Size | {PathSanitizer.FormatBytes(result.HtmlSize)} |");
        reportSb.AppendLine($"| Screenshots | {result.Screenshots.Count} ({PathSanitizer.FormatBytes(result.ScreenshotSize)}) |");
        reportSb.AppendLine($"| Images | {result.Images.Count} ({PathSanitizer.FormatBytes(result.ImagesTotalSize)}) |");
        reportSb.AppendLine($"| Images Missing Alt | {(missingAltCount > 0 ? $"⚠️ {missingAltCount}" : "✅ 0")} |");
        reportSb.AppendLine($"| JS Errors | {(result.ConsoleErrors.Count > 0 ? $"🔴 {result.ConsoleErrors.Count}" : "✅ 0")} |");
        reportSb.AppendLine($"| JS Warnings | {result.ConsoleWarnings.Count} |");
        reportSb.AppendLine($"| Auth | {result.CredentialUsed ?? "none"} |");
        reportSb.AppendLine($"| Captured | {result.CapturedAt:O} |");
        reportSb.AppendLine();

        if (result.ErrorMessage != null)
        {
            reportSb.AppendLine($"> ❌ **Error:** `{result.ErrorMessage}`");
            reportSb.AppendLine();
        }

        // Console errors — accordion
        if (result.ConsoleErrors.Count > 0)
        {
            reportSb.AppendLine($"## 🔴 JavaScript Errors");
            reportSb.AppendLine();
            reportSb.AppendLine("<details>");
            reportSb.AppendLine($"<summary><strong>{result.ConsoleErrors.Count} error(s) detected</strong></summary>");
            reportSb.AppendLine();
            reportSb.AppendLine("```");
            foreach (var error in result.ConsoleErrors.Take(20))
            {
                reportSb.AppendLine(error.Length > 200 ? error[..200] + "..." : error);
            }
            if (result.ConsoleErrors.Count > 20)
            {
                reportSb.AppendLine($"... and {result.ConsoleErrors.Count - 20} more (see errors.log)");
            }
            reportSb.AppendLine("```");
            reportSb.AppendLine();
            reportSb.AppendLine("</details>");
            reportSb.AppendLine();
        }

        // Actions — accordion
        reportSb.AppendLine($"## 🔧 Actions");
        reportSb.AppendLine();
        reportSb.AppendLine("<details>");
        reportSb.AppendLine($"<summary><strong>{actions.Count} action(s) performed</strong></summary>");
        reportSb.AppendLine();
        foreach (var action in actions)
        {
            reportSb.AppendLine($"- {action}");
        }
        reportSb.AppendLine();
        reportSb.AppendLine("</details>");
        reportSb.AppendLine();

        // Screenshots — HTML gallery
        reportSb.AppendLine($"## 📸 Screenshots");
        reportSb.AppendLine();

        if (result.Screenshots.Count > 0)
        {
            reportSb.AppendLine("<table>");
            for (int i = 0; i < result.Screenshots.Count; i += 2)
            {
                reportSb.AppendLine("<tr>");
                for (int j = 0; j < 2; j++)
                {
                    var idx = i + j;
                    if (idx < result.Screenshots.Count)
                    {
                        var shot = result.Screenshots[idx];
                        reportSb.AppendLine("<td align=\"center\" width=\"50%\">");
                        reportSb.AppendLine($"<a href=\"{shot.FileName}\">");
                        reportSb.AppendLine($"<img src=\"{shot.FileName}\" width=\"400\" alt=\"{shot.Label}\" />");
                        reportSb.AppendLine("</a>");
                        reportSb.AppendLine($"<br /><strong>{shot.StepNumber}. {shot.Label}</strong>");
                        reportSb.AppendLine($"<br /><sub>{PathSanitizer.FormatBytes(shot.FileSize)}</sub>");
                        reportSb.AppendLine("</td>");
                    }
                    else
                    {
                        reportSb.AppendLine("<td></td>");
                    }
                }
                reportSb.AppendLine("</tr>");
            }
            reportSb.AppendLine("</table>");
        }
        else
        {
            reportSb.AppendLine("*No screenshots captured.*");
        }
        reportSb.AppendLine();

        // Image gallery — HTML grid with accordions
        reportSb.AppendLine($"## 🖼️ Page Images ({result.Images.Count})");
        reportSb.AppendLine();

        if (result.Images.Count > 0)
        {
            // Summary table in accordion
            reportSb.AppendLine("<details open>");
            reportSb.AppendLine($"<summary><strong>📋 Image Index</strong> — {result.Images.Count} images, {PathSanitizer.FormatBytes(result.ImagesTotalSize)}</summary>");
            reportSb.AppendLine();
            reportSb.AppendLine($"| # | Image | Alt Text | Size |");
            reportSb.AppendLine($"|--:|-------|----------|-----:|");

            var imgNum = 0;
            foreach (var img in result.Images)
            {
                imgNum++;
                var alt = Truncate(img.AltText, 40);
                if (string.IsNullOrWhiteSpace(alt)) alt = "⚠️ *(missing)*";
                reportSb.AppendLine($"| {imgNum} | [{img.FileName}](images/{img.FileName}) | {alt} | {PathSanitizer.FormatBytes(img.FileSize)} |");
            }
            reportSb.AppendLine();
            reportSb.AppendLine("</details>");
            reportSb.AppendLine();

            // Thumbnail gallery — 3-column HTML grid
            reportSb.AppendLine("<details open>");
            reportSb.AppendLine($"<summary><strong>🖼️ Gallery</strong></summary>");
            reportSb.AppendLine();
            reportSb.AppendLine("<table>");
            for (int i = 0; i < result.Images.Count; i += 3)
            {
                reportSb.AppendLine("<tr>");
                for (int j = 0; j < 3; j++)
                {
                    var idx = i + j;
                    if (idx < result.Images.Count)
                    {
                        var img = result.Images[idx];
                        var alt = string.IsNullOrWhiteSpace(img.AltText) ? img.FileName : img.AltText;
                        var altBadge = string.IsNullOrWhiteSpace(img.AltText) ? " ⚠️" : "";
                        reportSb.AppendLine("<td align=\"center\" width=\"33%\">");
                        reportSb.AppendLine($"<a href=\"images/{img.FileName}\">");
                        reportSb.AppendLine($"<img src=\"images/{img.FileName}\" width=\"200\" alt=\"{alt}\" />");
                        reportSb.AppendLine("</a>");
                        reportSb.AppendLine($"<br /><sub>{img.FileName}{altBadge}</sub>");
                        reportSb.AppendLine("</td>");
                    }
                    else
                    {
                        reportSb.AppendLine("<td></td>");
                    }
                }
                reportSb.AppendLine("</tr>");
            }
            reportSb.AppendLine("</table>");
            reportSb.AppendLine();
            reportSb.AppendLine("</details>");
            reportSb.AppendLine();

            // Images missing alt text — accessibility flag
            var noAlt = result.Images.Where(i => string.IsNullOrWhiteSpace(i.AltText)).ToList();
            if (noAlt.Count > 0)
            {
                reportSb.AppendLine("<details>");
                reportSb.AppendLine($"<summary>⚠️ <strong>Images Missing Alt Text</strong> ({noAlt.Count})</summary>");
                reportSb.AppendLine();
                reportSb.AppendLine("| Image | Source URL |");
                reportSb.AppendLine("|-------|-----------|");
                foreach (var img in noAlt)
                {
                    reportSb.AppendLine($"| `{img.FileName}` | {Truncate(img.SourceUrl, 80)} |");
                }
                reportSb.AppendLine();
                reportSb.AppendLine("</details>");
                reportSb.AppendLine();
            }
        }
        else
        {
            reportSb.AppendLine("*No images found on page.*");
            reportSb.AppendLine();
        }

        // Files section
        reportSb.AppendLine($"## 📁 Files");
        reportSb.AppendLine();
        reportSb.AppendLine("| File | Description |");
        reportSb.AppendLine("|------|-------------|");
        foreach (var shot in result.Screenshots)
        {
            reportSb.AppendLine($"| `{shot.FileName}` | {shot.Label} ({PathSanitizer.FormatBytes(shot.FileSize)}) |");
        }
        reportSb.AppendLine($"| `page.html` | Rendered HTML content |");
        reportSb.AppendLine($"| `metadata.json` | Machine-readable scan data |");
        reportSb.AppendLine($"| `errors.log` | JavaScript console errors |");
        reportSb.AppendLine($"| `warnings.log` | JavaScript console warnings |");
        reportSb.AppendLine($"| `info.log` | Navigation and timing details |");
        reportSb.AppendLine($"| `actions.log` | Interactions performed |");
        if (result.Images.Count > 0)
        {
            reportSb.AppendLine($"| `images/` | {result.Images.Count} page images ({PathSanitizer.FormatBytes(result.ImagesTotalSize)}) |");
        }
        reportSb.AppendLine();
        reportSb.AppendLine("---");
        reportSb.AppendLine();
        reportSb.AppendLine("*Generated by AccessibilityScanner (FreeTools) v1.0*");

        await File.WriteAllTextAsync(reportPath, reportSb.ToString());

        // metadata.json
        var metadataPath = Path.Combine(pageDir, "metadata.json");
        var metadata = new
        {
            result.PagePath,
            Url = result.FullUrl,
            result.FinalUrl,
            result.Title,
            result.StatusCode,
            result.Success,
            HtmlSizeBytes = result.HtmlSize,
            ScreenshotSizeBytes = result.ScreenshotSize,
            result.CredentialUsed,
            ConsoleErrorCount = result.ConsoleErrors.Count,
            ConsoleWarningCount = result.ConsoleWarnings.Count,
            Screenshots = result.Screenshots.Select(s => new { s.FileName, s.Label, s.StepNumber, s.FileSize }).ToArray(),
            ImageCount = result.Images.Count,
            ImagesTotalSizeBytes = result.ImagesTotalSize,
            ImagesMissingAlt = result.Images.Count(i => string.IsNullOrWhiteSpace(i.AltText)),
            Images = result.Images.Select(i => new { i.FileName, i.SourceUrl, i.AltText, i.FileSize }).ToArray(),
            result.ErrorMessage,
            Headers = headers,
            result.CapturedAt
        };

        var json = JsonSerializer.Serialize(metadata, JsonOptions);
        await File.WriteAllTextAsync(metadataPath, json);
    }

    // ========================================================================
    // Site-level report (one per site folder)
    // ========================================================================

    private static async Task WriteSiteReportAsync(string siteDir, SiteResult site)
    {
        var sb = new StringBuilder();
        var successCount = site.Pages.Count(p => p.Success);
        var totalCount = site.Pages.Count;
        var totalErrors = site.Pages.Sum(p => p.ConsoleErrors.Count);
        var totalWarnings = site.Pages.Sum(p => p.ConsoleWarnings.Count);
        var totalHtml = site.Pages.Sum(p => p.HtmlSize);
        var totalScreenshots = site.Pages.Sum(p => p.ScreenshotSize);
        var totalImages = site.Pages.Sum(p => p.Images.Count);
        var totalImagesSize = site.Pages.Sum(p => p.ImagesTotalSize);
        var totalMissingAlt = site.Pages.Sum(p => p.Images.Count(i => string.IsNullOrWhiteSpace(i.AltText)));
        var failedPages = site.Pages.Where(p => !p.Success).ToList();
        var pagesWithErrors = site.Pages.Where(p => p.ConsoleErrors.Count > 0).ToList();
        var statusEmoji = successCount == totalCount ? "✅" : "⚠️";
        var successPct = totalCount > 0 ? (successCount * 100.0 / totalCount) : 0;

        sb.AppendLine($"# 🌐 Site Report: {site.Url}");
        sb.AppendLine();
        sb.AppendLine($"> **Status:** {statusEmoji} {successCount}/{totalCount} pages OK  ");
        sb.AppendLine($"> **Folder:** `{site.FolderName}/`  ");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // Summary dashboard
        sb.AppendLine($"## 📋 Summary");
        sb.AppendLine();
        sb.AppendLine("```");
        sb.AppendLine($"Success Rate:  {GenerateProgressBar(successPct, 30)} {successPct:F0}%");
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine($"| Metric | Value |");
        sb.AppendLine($"|--------|-------|");
        sb.AppendLine($"| Pages Scanned | {totalCount} |");
        sb.AppendLine($"| Pages Passed | ✅ {successCount} |");
        sb.AppendLine($"| Pages Failed | {(failedPages.Count > 0 ? $"❌ {failedPages.Count}" : "0")} |");
        sb.AppendLine($"| Total JS Errors | {(totalErrors > 0 ? $"🔴 {totalErrors}" : "0")} |");
        sb.AppendLine($"| Total JS Warnings | {totalWarnings} |");
        sb.AppendLine($"| Total Images | {totalImages} ({PathSanitizer.FormatBytes(totalImagesSize)}) |");
        sb.AppendLine($"| Images Missing Alt | {(totalMissingAlt > 0 ? $"⚠️ {totalMissingAlt}" : "✅ 0")} |");
        sb.AppendLine($"| Total HTML | {PathSanitizer.FormatBytes(totalHtml)} |");
        sb.AppendLine($"| Total Screenshots | {PathSanitizer.FormatBytes(totalScreenshots)} |");
        sb.AppendLine();

        // Page results table
        sb.AppendLine($"## 📑 Pages");
        sb.AppendLine();
        sb.AppendLine($"| Status | Page | HTTP | Title | JS Errors | Images | Missing Alt |");
        sb.AppendLine($"|:------:|------|:----:|-------|:---------:|:------:|:-----------:|");

        foreach (var page in site.Pages.OrderBy(p => p.PagePath))
        {
            var s = page.Success ? "✅" : "❌";
            var title = Truncate(page.Title ?? "*(none)*", 40);
            var missingAlt = page.Images.Count(i => string.IsNullOrWhiteSpace(i.AltText));
            var errBadge = page.ConsoleErrors.Count > 0 ? $"🔴 {page.ConsoleErrors.Count}" : "0";
            var altBadge = missingAlt > 0 ? $"⚠️ {missingAlt}" : "0";
            sb.AppendLine($"| {s} | [{page.PagePath}]({page.FolderName}/report.md) | {page.StatusCode} | {title} | {errBadge} | {page.Images.Count} | {altBadge} |");
        }
        sb.AppendLine();

        // Screenshot gallery — 3-column HTML grid
        sb.AppendLine($"## 📸 Page Screenshots");
        sb.AppendLine();
        sb.AppendLine("Click any thumbnail to view the full page report.");
        sb.AppendLine();

        var pagesWithShots = site.Pages.Where(p => p.Screenshots.Count > 0).OrderBy(p => p.PagePath).ToList();
        if (pagesWithShots.Count > 0)
        {
            sb.AppendLine("<table>");
            for (int i = 0; i < pagesWithShots.Count; i += 3)
            {
                sb.AppendLine("<tr>");
                for (int j = 0; j < 3; j++)
                {
                    var idx = i + j;
                    if (idx < pagesWithShots.Count)
                    {
                        var page = pagesWithShots[idx];
                        var firstShot = page.Screenshots.First();
                        var statusIcon = page.Success ? "✅" : "❌";
                        sb.AppendLine("<td align=\"center\" width=\"33%\">");
                        sb.AppendLine($"<a href=\"{page.FolderName}/report.md\">");
                        sb.AppendLine($"<img src=\"{page.FolderName}/{firstShot.FileName}\" width=\"250\" alt=\"{page.PagePath}\" />");
                        sb.AppendLine("</a>");
                        sb.AppendLine($"<br />{statusIcon} <code>{page.PagePath}</code>");
                        sb.AppendLine("</td>");
                    }
                    else
                    {
                        sb.AppendLine("<td></td>");
                    }
                }
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
        }
        else
        {
            sb.AppendLine("*No screenshots captured.*");
        }
        sb.AppendLine();

        // Failed pages — accordion
        if (failedPages.Count > 0)
        {
            sb.AppendLine($"## ❌ Failed Pages");
            sb.AppendLine();
            sb.AppendLine("<details open>");
            sb.AppendLine($"<summary><strong>{failedPages.Count} page(s) failed</strong></summary>");
            sb.AppendLine();
            sb.AppendLine("| Page | HTTP | Error |");
            sb.AppendLine("|------|:----:|-------|");

            foreach (var page in failedPages)
            {
                var err = Truncate(page.ErrorMessage ?? "—", 60);
                sb.AppendLine($"| [{page.PagePath}]({page.FolderName}/report.md) | {page.StatusCode} | {err} |");
            }
            sb.AppendLine();
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        // JS errors — accordion
        if (pagesWithErrors.Count > 0)
        {
            sb.AppendLine($"## 🔴 JavaScript Errors");
            sb.AppendLine();
            sb.AppendLine("<details>");
            sb.AppendLine($"<summary><strong>{totalErrors} error(s) across {pagesWithErrors.Count} page(s)</strong></summary>");
            sb.AppendLine();

            foreach (var page in pagesWithErrors.OrderByDescending(p => p.ConsoleErrors.Count))
            {
                sb.AppendLine($"**{page.PagePath}** ({page.ConsoleErrors.Count} errors)");
                sb.AppendLine();
                sb.AppendLine("```");
                foreach (var error in page.ConsoleErrors.Take(5))
                {
                    sb.AppendLine(error.Length > 200 ? error[..200] + "..." : error);
                }
                if (page.ConsoleErrors.Count > 5)
                {
                    sb.AppendLine($"... and {page.ConsoleErrors.Count - 5} more (see {page.FolderName}/errors.log)");
                }
                sb.AppendLine("```");
                sb.AppendLine();
            }
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("*Generated by AccessibilityScanner (FreeTools) v1.0*");

        await File.WriteAllTextAsync(Path.Combine(siteDir, "report.md"), sb.ToString());
    }

    // ========================================================================
    // Run-level report (root of runs/latest/)
    // ========================================================================

    private static async Task WriteRunReportAsync(string runsDir, IEnumerable<SiteResult> sites)
    {
        var siteList = sites.OrderBy(s => s.Url).ToList();
        var sb = new StringBuilder();

        var totalSites = siteList.Count;
        var totalPages = siteList.Sum(s => s.Pages.Count);
        var totalSuccess = siteList.Sum(s => s.Pages.Count(p => p.Success));
        var totalFailed = totalPages - totalSuccess;
        var totalErrors = siteList.Sum(s => s.Pages.Sum(p => p.ConsoleErrors.Count));
        var totalWarnings = siteList.Sum(s => s.Pages.Sum(p => p.ConsoleWarnings.Count));
        var totalHtml = siteList.Sum(s => s.Pages.Sum(p => p.HtmlSize));
        var totalScreenshots = siteList.Sum(s => s.Pages.Sum(p => p.ScreenshotSize));
        var totalImages = siteList.Sum(s => s.Pages.Sum(p => p.Images.Count));
        var totalImagesSize = siteList.Sum(s => s.Pages.Sum(p => p.ImagesTotalSize));
        var totalMissingAlt = siteList.Sum(s => s.Pages.Sum(p => p.Images.Count(i => string.IsNullOrWhiteSpace(i.AltText))));
        var successPct = totalPages > 0 ? (totalSuccess * 100.0 / totalPages) : 0;
        var runStatus = totalFailed == 0 ? "✅ All pages passed" : $"⚠️ {totalFailed} page(s) failed";

        sb.AppendLine($"# 📊 Accessibility Scanner — Run Report");
        sb.AppendLine();
        sb.AppendLine($"> **Generated:** {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC  ");
        sb.AppendLine($"> **Status:** {runStatus}  ");
        sb.AppendLine($"> **Sites:** {totalSites} | **Pages:** {totalPages}  ");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // Table of Contents
        sb.AppendLine("## 📑 Contents");
        sb.AppendLine();
        sb.AppendLine("- [Dashboard](#-dashboard)");
        sb.AppendLine("- [Sites](#-sites)");
        sb.AppendLine("- [Screenshot Gallery](#-screenshot-gallery)");
        sb.AppendLine("- [All Pages](#-all-pages)");
        if (totalFailed > 0) sb.AppendLine("- [Failed Pages](#-failed-pages)");
        if (totalErrors > 0) sb.AppendLine("- [JavaScript Errors](#-javascript-errors)");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // Dashboard
        sb.AppendLine($"## 📋 Dashboard");
        sb.AppendLine();
        sb.AppendLine("```");
        sb.AppendLine($"Page Success:     {GenerateProgressBar(successPct, 30)} {successPct:F0}%");
        var altPct = totalImages > 0 ? ((totalImages - totalMissingAlt) * 100.0 / totalImages) : 100;
        sb.AppendLine($"Alt Text Cover:   {GenerateProgressBar(altPct, 30)} {altPct:F0}%");
        sb.AppendLine("```");
        sb.AppendLine();

        sb.AppendLine("| ✅ Passed | ❌ Failed | 🖼️ Images | ⚠️ Missing Alt | 🔴 JS Errors |");
        sb.AppendLine("|:---------:|:---------:|:----------:|:--------------:|:------------:|");
        sb.AppendLine($"| {totalSuccess} | {totalFailed} | {totalImages} | {totalMissingAlt} | {totalErrors} |");
        sb.AppendLine();

        sb.AppendLine($"| Metric | Value |");
        sb.AppendLine($"|--------|-------|");
        sb.AppendLine($"| Sites | {totalSites} |");
        sb.AppendLine($"| Total Pages | {totalPages} |");
        sb.AppendLine($"| Total Images | {totalImages} ({PathSanitizer.FormatBytes(totalImagesSize)}) |");
        sb.AppendLine($"| Total HTML | {PathSanitizer.FormatBytes(totalHtml)} |");
        sb.AppendLine($"| Total Screenshots | {PathSanitizer.FormatBytes(totalScreenshots)} |");
        sb.AppendLine();

        // Sites table
        sb.AppendLine($"## 🌐 Sites");
        sb.AppendLine();
        sb.AppendLine($"| Status | Site | Pages | Passed | Failed | Images | Missing Alt |");
        sb.AppendLine($"|:------:|------|:-----:|:------:|:------:|:------:|:-----------:|");

        foreach (var site in siteList)
        {
            var siteSuccess = site.Pages.Count(p => p.Success);
            var siteFailed = site.Pages.Count - siteSuccess;
            var siteImages = site.Pages.Sum(p => p.Images.Count);
            var siteMissingAlt = site.Pages.Sum(p => p.Images.Count(i => string.IsNullOrWhiteSpace(i.AltText)));
            var s = siteFailed == 0 ? "✅" : "⚠️";

            sb.AppendLine($"| {s} | [{site.Url}]({site.FolderName}/report.md) | {site.Pages.Count} | {siteSuccess} | {siteFailed} | {siteImages} | {siteMissingAlt} |");
        }
        sb.AppendLine();

        // Screenshot gallery — grouped by site, HTML 3-column grid, in accordions
        sb.AppendLine($"## 📸 Screenshot Gallery");
        sb.AppendLine();
        sb.AppendLine($"**{totalPages} pages** across **{totalSites} sites**. Click any thumbnail to view the full page report.");
        sb.AppendLine();

        foreach (var site in siteList)
        {
            var host = new Uri(site.Url).Host;
            var pagesWithShots = site.Pages.Where(p => p.Screenshots.Count > 0).OrderBy(p => p.PagePath).ToList();
            if (pagesWithShots.Count == 0) continue;

            var siteSuccess = site.Pages.Count(p => p.Success);
            var siteFailed = site.Pages.Count - siteSuccess;
            var siteEmoji = siteFailed == 0 ? "✅" : "⚠️";

            sb.AppendLine("<details>");
            sb.AppendLine($"<summary><strong>{siteEmoji} {host}</strong> — {pagesWithShots.Count} page(s)</summary>");
            sb.AppendLine();
            sb.AppendLine("<table>");

            for (int i = 0; i < pagesWithShots.Count; i += 3)
            {
                sb.AppendLine("<tr>");
                for (int j = 0; j < 3; j++)
                {
                    var idx = i + j;
                    if (idx < pagesWithShots.Count)
                    {
                        var page = pagesWithShots[idx];
                        var firstShot = page.Screenshots.First();
                        var statusIcon = page.Success ? "✅" : "❌";
                        sb.AppendLine("<td align=\"center\" width=\"33%\">");
                        sb.AppendLine($"<a href=\"{site.FolderName}/{page.FolderName}/report.md\">");
                        sb.AppendLine($"<img src=\"{site.FolderName}/{page.FolderName}/{firstShot.FileName}\" width=\"250\" alt=\"{host}{page.PagePath}\" />");
                        sb.AppendLine("</a>");
                        sb.AppendLine($"<br />{statusIcon} <code>{page.PagePath}</code>");
                        sb.AppendLine("</td>");
                    }
                    else
                    {
                        sb.AppendLine("<td></td>");
                    }
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine();
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        // All pages flat table — in accordion because it can be huge
        sb.AppendLine($"## 📑 All Pages");
        sb.AppendLine();
        sb.AppendLine("<details>");
        sb.AppendLine($"<summary><strong>{totalPages} pages scanned</strong></summary>");
        sb.AppendLine();
        sb.AppendLine($"| Status | Site | Page | HTTP | Title | Images | Missing Alt |");
        sb.AppendLine($"|:------:|------|------|:----:|-------|:------:|:-----------:|");

        foreach (var site in siteList)
        {
            foreach (var page in site.Pages.OrderBy(p => p.PagePath))
            {
                var s = page.Success ? "✅" : "❌";
                var title = Truncate(page.Title ?? "*(none)*", 35);
                var host = new Uri(site.Url).Host;
                var missingAlt = page.Images.Count(i => string.IsNullOrWhiteSpace(i.AltText));
                sb.AppendLine($"| {s} | {host} | [{page.PagePath}]({site.FolderName}/{page.FolderName}/report.md) | {page.StatusCode} | {title} | {page.Images.Count} | {missingAlt} |");
            }
        }
        sb.AppendLine();
        sb.AppendLine("</details>");
        sb.AppendLine();

        // Failed pages — accordion
        var allFailed = siteList.SelectMany(s => s.Pages.Where(p => !p.Success)
            .Select(p => (Site: s, Page: p))).ToList();

        if (allFailed.Count > 0)
        {
            sb.AppendLine($"## ❌ Failed Pages");
            sb.AppendLine();
            sb.AppendLine("<details open>");
            sb.AppendLine($"<summary><strong>{allFailed.Count} page(s) failed</strong></summary>");
            sb.AppendLine();
            sb.AppendLine("| Site | Page | HTTP | Error |");
            sb.AppendLine("|------|------|:----:|-------|");

            foreach (var (site, page) in allFailed)
            {
                var host = new Uri(site.Url).Host;
                var err = Truncate(page.ErrorMessage ?? "—", 50);
                sb.AppendLine($"| {host} | {page.PagePath} | {page.StatusCode} | {err} |");
            }
            sb.AppendLine();
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        // Top JS error pages — accordion
        var errorPages = siteList.SelectMany(s => s.Pages.Where(p => p.ConsoleErrors.Count > 0)
            .Select(p => (Site: s, Page: p)))
            .OrderByDescending(x => x.Page.ConsoleErrors.Count)
            .Take(15)
            .ToList();

        if (errorPages.Count > 0)
        {
            sb.AppendLine($"## 🔴 JavaScript Errors");
            sb.AppendLine();
            sb.AppendLine("<details>");
            sb.AppendLine($"<summary><strong>Top {errorPages.Count} pages by JS error count</strong></summary>");
            sb.AppendLine();
            sb.AppendLine($"| Errors | Site | Page |");
            sb.AppendLine($"|:------:|------|------|");

            foreach (var (site, page) in errorPages)
            {
                var host = new Uri(site.Url).Host;
                sb.AppendLine($"| 🔴 {page.ConsoleErrors.Count} | {host} | [{page.PagePath}]({site.FolderName}/{page.FolderName}/report.md) |");
            }
            sb.AppendLine();
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("*Generated by AccessibilityScanner (FreeTools) v1.0*");
        sb.AppendLine();
        sb.AppendLine("**[FreeTools](https://github.com/WSU-EIT/FreeTools)** — Open source accessibility scanning tools for .NET projects");

        await File.WriteAllTextAsync(Path.Combine(runsDir, "report.md"), sb.ToString());
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        // Escape pipe characters for markdown tables
        value = value.Replace("|", "\\|");
        return value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";
    }

    private static string GenerateProgressBar(double percentage, int width)
    {
        var filled = (int)(percentage * width / 100.0);
        filled = Math.Clamp(filled, 0, width);
        var empty = width - filled;
        return $"[{new string('█', filled)}{new string('░', empty)}]";
    }

    // ========================================================================
    // Image extraction
    // ========================================================================

    /// <summary>
    /// Find all images on the page, download them to an /images subfolder,
    /// and track them in the result.
    /// </summary>
    private static async Task DownloadPageImagesAsync(
        IPage page,
        string pageDir,
        PageResult result,
        List<string> actions,
        List<string> infoLines)
    {
        var imagesDir = Path.Combine(pageDir, "images");

        // Get all img elements with src attributes
        var imgElements = await page.EvaluateAsync<ImageInfo[]>(@"
            () => Array.from(document.querySelectorAll('img[src]')).map(img => ({
                src: img.src,
                alt: img.alt || '',
                width: img.naturalWidth,
                height: img.naturalHeight
            })).filter(img => img.src && !img.src.startsWith('data:'))
        ");

        if (imgElements == null || imgElements.Length == 0)
        {
            infoLines.Add("Images: 0 found");
            actions.Add("No images found on page");
            return;
        }

        // Deduplicate by URL
        var uniqueImages = imgElements
            .GroupBy(i => i.Src)
            .Select(g => g.First())
            .ToList();

        Directory.CreateDirectory(imagesDir);

        var downloaded = 0;
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(5);

        foreach (var img in uniqueImages)
        {
            try
            {
                var uri = new Uri(img.Src);
                var ext = Path.GetExtension(uri.AbsolutePath).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || ext.Length > 5)
                {
                    ext = ".img";
                }

                // Sanitize filename from the URL path
                var urlFileName = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
                if (string.IsNullOrWhiteSpace(urlFileName) || urlFileName.Length > 60)
                {
                    urlFileName = $"image-{downloaded + 1}";
                }

                // Strip invalid chars
                foreach (var c in Path.GetInvalidFileNameChars())
                {
                    urlFileName = urlFileName.Replace(c, '-');
                }

                var fileName = $"{urlFileName}{ext}";

                // Avoid collisions
                var filePath = Path.Combine(imagesDir, fileName);
                var counter = 1;
                while (File.Exists(filePath))
                {
                    fileName = $"{urlFileName}-{counter}{ext}";
                    filePath = Path.Combine(imagesDir, fileName);
                    counter++;
                }

                var bytes = await httpClient.GetByteArrayAsync(img.Src);
                await File.WriteAllBytesAsync(filePath, bytes);

                var fileSize = bytes.Length;
                result.Images.Add(new ImageEntry
                {
                    FileName = fileName,
                    SourceUrl = img.Src,
                    AltText = img.Alt,
                    FileSize = fileSize
                });

                result.ImagesTotalSize += fileSize;
                downloaded++;
            }
            catch
            {
                // Skip images that fail to download
            }
        }

        infoLines.Add($"Images: {downloaded} downloaded of {uniqueImages.Count} found ({PathSanitizer.FormatBytes(result.ImagesTotalSize)})");
        actions.Add($"Downloaded {downloaded} images to /images/");
    }

    // ========================================================================
    // Auth flow
    // ========================================================================

    /// <summary>
    /// Capture a named screenshot, track it in the result, and log the action.
    /// Returns the step number assigned.
    /// </summary>
    private static async Task<int> TakeScreenshotAsync(
        IPage page,
        string pageDir,
        PageResult result,
        List<string> actions,
        string label)
    {
        var stepNumber = result.Screenshots.Count + 1;
        var fileName = $"{stepNumber:D2}-{SanitizeFileName(label)}.png";
        var filePath = Path.Combine(pageDir, fileName);

        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = filePath,
            FullPage = true
        });

        var fileSize = new FileInfo(filePath).Length;

        result.Screenshots.Add(new ScreenshotEntry
        {
            FileName = fileName,
            Label = label,
            FileSize = fileSize,
            StepNumber = stepNumber
        });

        result.ScreenshotSize += fileSize;
        actions.Add($"Screenshot #{stepNumber}: {label} ({PathSanitizer.FormatBytes(fileSize)})");

        return stepNumber;
    }

    private static string SanitizeFileName(string label)
    {
        // Convert "Page loaded (after settle)" → "page-loaded-after-settle"
        var sanitized = label.ToLowerInvariant();
        sanitized = sanitized.Replace(' ', '-').Replace('(', '-').Replace(')', '-');
        // Collapse multiple dashes and trim
        while (sanitized.Contains("--"))
        {
            sanitized = sanitized.Replace("--", "-");
        }

        return sanitized.Trim('-');
    }

    /// <summary>
    /// Attempt to fill and submit a login form on the current page.
    /// Takes before/after screenshots at each interaction step.
    /// Returns true if a form was found and submitted.
    /// </summary>
    private static async Task<bool> TryAuthFlowAsync(
        IPage page, SiteCredential cred, ScannerConfig config,
        string pageDir, PageResult result, List<string> actions)
    {
        var usernameSelectors = new[]
        {
            "input[name='username']",
            "input[name='Username']",
            "input[name='email']",
            "input[name='Email']",
            "input[name='Input.Email']",
            "input[name='Input.Username']",
            "input[type='email']",
            "input[autocomplete='username']",
            "input[placeholder*='user' i]",
            "input[placeholder*='email' i]"
        };

        var passwordSelectors = new[]
        {
            "input[name='password']",
            "input[name='Password']",
            "input[name='Input.Password']",
            "input[type='password']"
        };

        ILocator? usernameField = null;
        ILocator? passwordField = null;

        foreach (var selector in usernameSelectors)
        {
            try
            {
                var locator = page.Locator(selector).First;
                if (await locator.CountAsync() > 0 && await locator.IsVisibleAsync())
                {
                    usernameField = locator;
                    actions.Add($"Found username field via: {selector}");
                    break;
                }
            }
            catch { /* Continue to next selector */ }
        }

        foreach (var selector in passwordSelectors)
        {
            try
            {
                var locator = page.Locator(selector).First;
                if (await locator.CountAsync() > 0 && await locator.IsVisibleAsync())
                {
                    passwordField = locator;
                    actions.Add($"Found password field via: {selector}");
                    break;
                }
            }
            catch { /* Continue to next selector */ }
        }

        if (usernameField == null || passwordField == null)
        {
            return false;
        }

        // Screenshot: before filling
        await TakeScreenshotAsync(page, pageDir, result, actions, "auth-form-detected");

        await usernameField.FillAsync(cred.Username);
        actions.Add($"Filled username field with '{cred.Username}'");

        await passwordField.FillAsync(cred.Password);
        actions.Add("Filled password field with ****");

        // Screenshot: after filling, before submit
        await TakeScreenshotAsync(page, pageDir, result, actions, "auth-form-filled");

        var submitSelectors = new[]
        {
            "button[type='submit']",
            "input[type='submit']",
            "button:has-text('Log in')",
            "button:has-text('Login')",
            "button:has-text('Sign in')"
        };

        var submitted = false;
        foreach (var selector in submitSelectors)
        {
            try
            {
                var locator = page.Locator(selector).First;
                if (await locator.CountAsync() > 0 && await locator.IsVisibleAsync())
                {
                    await locator.ClickAsync();
                    actions.Add($"Clicked submit button via: {selector}");
                    submitted = true;
                    break;
                }
            }
            catch { /* Continue to next selector */ }
        }

        if (!submitted)
        {
            await passwordField.PressAsync("Enter");
            actions.Add("Pressed Enter on password field (no submit button found)");
        }

        await page.WaitForTimeoutAsync(config.SettleDelayMs);
        actions.Add($"Waited {config.SettleDelayMs}ms for post-login settle");

        // Screenshot: after login result
        await TakeScreenshotAsync(page, pageDir, result, actions, "auth-result");

        return true;
    }

    // ========================================================================
    // URL and folder helpers
    // ========================================================================

    /// <summary>
    /// Resolve a page path to a full URL. Handles both relative ("/about/")
    /// and absolute ("https://other.site.com/page") paths.
    /// </summary>
    private static string ResolvePageUrl(Uri siteUri, string pagePath)
    {
        if (pagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            pagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return pagePath;
        }

        // Relative path — combine with site root
        return new Uri(siteUri, pagePath).ToString();
    }

    /// <summary>
    /// Convert a page path to a flat folder name.
    /// "/" → "_root", "/site/page1" → "site_page1", "/?c=A" → "_qc-A"
    /// </summary>
    private static string PagePathToFolderName(string pagePath)
    {
        // Strip protocol if full URL was given
        if (pagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            pagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(pagePath);
            pagePath = uri.PathAndQuery;
        }

        var trimmed = pagePath.Trim('/');

        if (string.IsNullOrEmpty(trimmed))
        {
            return "_root";
        }

        // Replace path separators and query string chars with safe alternatives
        var folderName = trimmed
            .Replace('/', '_')
            .Replace("?", "_q")
            .Replace('&', '_')
            .Replace('=', '-');

        // Strip any remaining invalid filesystem characters
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            folderName = folderName.Replace(c, '-');
        }

        return folderName;
    }

    // ========================================================================
    // Config loading
    // ========================================================================

    private static async Task<ScannerConfig?> LoadConfigAsync()
    {
        var exeDir = AppContext.BaseDirectory;
        var projectDir = FindProjectDir(exeDir);

        var candidates = new[]
        {
            Path.Combine(exeDir, "appsettings.json"),
            Path.Combine(projectDir, "appsettings.json")
        };

        foreach (var path in candidates)
        {
            if (!File.Exists(path))
            {
                continue;
            }

            Console.WriteLine($"  Loading config: {path}");

            var json = await File.ReadAllTextAsync(path);
            var root = JsonSerializer.Deserialize<AppSettingsRoot>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (root?.Scanner != null)
            {
                return root.Scanner;
            }
        }

        Console.Error.WriteLine("  appsettings.json not found.");
        return null;
    }

    /// <summary>
    /// Walk up from bin/Debug/net10.0 to find the project directory (where .csproj lives).
    /// </summary>
    private static string FindProjectDir(string startDir)
    {
        var dir = startDir;

        while (dir != null)
        {
            if (Directory.GetFiles(dir, "*.csproj").Length > 0)
            {
                return dir;
            }

            dir = Directory.GetParent(dir)?.FullName;
        }

        return Directory.GetCurrentDirectory();
    }
}

// ============================================================================
// Configuration Models (maps to appsettings.json)
// ============================================================================

internal class AppSettingsRoot
{
    public ScannerConfig Scanner { get; set; } = new();
}

internal class ScannerConfig
{
    public int SettleDelayMs { get; set; } = 3000;
    public int TimeoutMs { get; set; } = 10000;
    public int MaxConcurrency { get; set; } = 5;
    public bool Headless { get; set; } = true;
    public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36";
    public Dictionary<string, SiteConfig> Sites { get; set; } = new();
}

internal class SiteConfig
{
    public List<SiteCredential> Credentials { get; set; } = [];
    public List<string> Pages { get; set; } = [];
}

internal class SiteCredential
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

// ============================================================================
// Result Models
// ============================================================================

internal class SiteResult
{
    public string Url { get; set; } = "";
    public string FolderName { get; set; } = "";
    public List<PageResult> Pages { get; set; } = [];
}

internal class PageResult
{
    public string PagePath { get; set; } = "";
    public string FullUrl { get; set; } = "";
    public string? FinalUrl { get; set; }
    public string? Title { get; set; }
    public string FolderName { get; set; } = "";
    public int StatusCode { get; set; }
    public long HtmlSize { get; set; }
    public long ScreenshotSize { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CredentialUsed { get; set; }
    public List<string> ConsoleErrors { get; set; } = [];
    public List<string> ConsoleWarnings { get; set; } = [];
    public List<ScreenshotEntry> Screenshots { get; set; } = [];
    public List<ImageEntry> Images { get; set; } = [];
    public long ImagesTotalSize { get; set; }
    public DateTime CapturedAt { get; set; }
}

/// <summary>
/// Tracks a single screenshot taken during page scanning.
/// </summary>
internal class ScreenshotEntry
{
    public string FileName { get; set; } = "";
    public string Label { get; set; } = "";
    public long FileSize { get; set; }
    public int StepNumber { get; set; }
}

/// <summary>
/// Tracks a single image downloaded from a scanned page.
/// </summary>
internal class ImageEntry
{
    public string FileName { get; set; } = "";
    public string SourceUrl { get; set; } = "";
    public string AltText { get; set; } = "";
    public long FileSize { get; set; }
}

/// <summary>
/// Used for deserializing image info from Playwright's EvaluateAsync.
/// </summary>
internal class ImageInfo
{
    [JsonPropertyName("src")]
    public string Src { get; set; } = "";

    [JsonPropertyName("alt")]
    public string Alt { get; set; } = "";

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
}
