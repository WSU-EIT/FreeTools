# FreeTools Shared Code Analysis

## Overview

This document identifies shared code patterns in FreeTools common across the FreeManager2 ecosystem. FreeTools is unique as it uses a different architecture than the CRM-based projects.

## Shared Pattern Summary

| Pattern | Location | Reuse Potential |
|---------|----------|-----------------|
| CliArgs | `FreeTools.Core/CliArgs.cs` | High |
| ConsoleOutput | `FreeTools.Core/ConsoleOutput.cs` | High |
| PathSanitizer | `FreeTools.Core/PathSanitizer.cs` | High |
| RouteParser | `FreeTools.Core/RouteParser.cs` | Medium |

## Core Utilities

### 1. CliArgs - Command Line Argument Parsing

**Purpose:** Unified CLI argument and environment variable handling.

```csharp
public static class CliArgs
{
    public static string GetEnvOrArg(string envVar, string[] args, int argIndex, string defaultValue);
    public static int GetEnvOrArgInt(string envVar, string[] args, int argIndex, int defaultValue);
    public static bool HasFlag(List<string> args, string flag);
    public static string? GetOption(List<string> args, string option);
    public static string? GetPositional(List<string> args, int index, string? defaultValue = null);
}
```

**Priority Order:**
1. Environment variables (highest)
2. CLI arguments
3. Defaults (lowest)

### 2. ConsoleOutput - Formatted Console Output

**Purpose:** Consistent, thread-safe console output formatting.

```csharp
public static class ConsoleOutput
{
    public static void PrintBanner(string title);
    public static void PrintConfig(string label, string value);
    public static void PrintDivider();
    public static void WriteLine(string message, bool isError = false);
}
```

**Features:**
- Thread-safe with locking
- Error output to stderr
- Consistent formatting

### 3. PathSanitizer - File Path Safety

**Purpose:** Convert routes and strings to safe file system paths.

```csharp
public static class PathSanitizer
{
    public static string RouteToDirectoryPath(string route);
    public static string SanitizeFileName(string fileName);
}
```

### 4. RouteParser - Route CSV Parsing

**Purpose:** Parse route definitions from CSV files.

```csharp
public static class RouteParser
{
    public static async Task<(List<Route> routes, List<string> skipped)> ParseRoutesFromCsvFileAsync(string path);
}
```

## Architecture Differences

FreeTools uses a different architecture than CRM-based projects:

| Aspect | FreeTools | CRM Projects |
|--------|-----------|--------------|
| Pattern | Static utilities | DI with interfaces |
| State | Stateless | Stateful (DbContext) |
| Dependencies | Minimal | Full .NET stack |
| Entry Point | `Main` in each tool | `Program.cs` + DI |
| Configuration | Env vars + CLI | appsettings.json |

## Tool Structure

```
tools/
├── FreeTools.Core/              # Shared utilities
├── FreeTools.AppHost/           # Aspire orchestrator
├── FreeTools.EndpointMapper/    # Route scanner
├── FreeTools.EndpointPoker/     # HTTP tester
├── FreeTools.BrowserSnapshot/   # Screenshot tool
├── FreeTools.WorkspaceInventory/# File inventory
├── FreeTools.WorkspaceReporter/ # Report generator
└── FreeTools.Tests/             # Tests
```

## Integration with CRM Projects

FreeTools can be used to:
- Test CRM API endpoints (EndpointPoker)
- Generate screenshots of CRM pages (BrowserSnapshot)
- Inventory CRM codebase (WorkspaceInventory)
- Scan Razor routes (EndpointMapper)

## Consolidation Recommendations

### For FreeTools
1. FreeTools.Core is already consolidated
2. Could add more shared utilities as needed
3. Keep CLI-focused architecture

### For Other Projects
1. CLI argument patterns from FreeTools could be adopted
2. ConsoleOutput pattern useful for CLI-based utilities
3. Path sanitization patterns are reusable

## Potential Shared Library

Consider extracting to `FreeManager.Cli.Core`:
- Command line argument parsing
- Console output formatting
- Path sanitization
- Route parsing

## Code Metrics

| Metric | Value |
|--------|-------|
| Shared patterns used | 4 (CLI-specific) |
| Lines in FreeTools.Core | ~300 |
| Reuse in CRM projects | Low (different architecture) |
| Consolidation priority | Low (already consolidated) |
