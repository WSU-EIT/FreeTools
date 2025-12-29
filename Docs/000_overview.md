# FreeTools

Workspace analysis and testing suite for Blazor/ASP.NET Core applications.

## Summary

Comprehensive CLI toolset for analyzing project structure, discovering Blazor routes, testing endpoints, capturing browser screenshots, and generating automated markdown reports. Orchestrated via Microsoft Aspire AppHost.

**Application Type:** .NET 10.0 CLI Tools with Aspire Orchestration

## Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 10.0 | Runtime framework |
| C# | 14.0 | Language version |
| Aspire.AppHost.Sdk | 9.2.0 | Pipeline orchestration |
| Aspire.Hosting.AppHost | 9.2.0 | App host runtime |
| Microsoft.Playwright | 1.56.0 | Browser automation |
| Microsoft.CodeAnalysis.CSharp | 4.12.0 | C# syntax analysis |
| FileSystemGlobbing | 9.0.0 | Pattern matching |
| MSTest.Sdk | 4.0.1 | Unit testing |

## Project Structure

```
FreeTools/
├── FreeTools.Core/                 # Shared utilities
│   ├── ConsoleOutput.cs
│   ├── CliArgs.cs
│   ├── RouteParser.cs
│   └── PathSanitizer.cs
│
├── FreeTools.AppHost/              # Aspire orchestrator
│   └── Program.cs                  # 5-phase pipeline
│
├── Phase 1: Static Analysis
│   ├── FreeTools.EndpointMapper/   # Route discovery
│   │   └── Program.cs              # → pages.csv
│   │
│   └── FreeTools.WorkspaceInventory/ # File metrics
│       └── Program.cs              # → workspace-inventory.csv
│
├── Phase 2-3: Testing
│   ├── FreeTools.EndpointPoker/    # HTTP testing
│   │   └── Program.cs              # → *.html
│   │
│   └── FreeTools.BrowserSnapshot/  # Screenshots
│       └── Program.cs              # → *.png
│
├── Phase 4: Reporting
│   ├── FreeTools.WorkspaceReporter/# Report generation
│   │   └── Program.cs              # → LatestReport.md
│   │
│   └── FreeTools.Docs/             # Output storage
│       ├── runs/
│       └── latest/
│
└── FreeTools.Tests/                # Unit tests
```

## Pipeline Execution

```
[0] Launch FreeCRM (InMemory)
  ↓
[1] Static Analysis (Parallel)
  ├─ EndpointMapper → pages.csv
  └─ WorkspaceInventory → workspace-inventory.csv
  ↓
[2] HTTP Testing
  └─ EndpointPoker → snapshots/*.html
  ↓
[3] Browser Screenshots
  └─ BrowserSnapshot → snapshots/*.png
  ↓
[4] Report Generation
  └─ WorkspaceReporter → LatestReport.md
```

## Key Features

### Static Analysis
- [x] File system scanning
- [x] Blazor route discovery
- [x] C# syntax analysis
- [x] File metrics (size, lines)
- [x] Kind classification
- [x] Namespace extraction

### Route Discovery
- [x] @page directive parsing
- [x] Route parameter detection
- [x] Auth requirement detection
- [x] Project classification

### HTTP Testing
- [x] HTTP GET requests
- [x] Configurable timeout
- [x] Response caching
- [x] MIME type verification

### Browser Automation
- [x] Playwright screenshots
- [x] Multi-browser (Chromium, Firefox, WebKit)
- [x] Viewport configuration
- [x] Full-page capture

### Reporting
- [x] Markdown generation
- [x] CSV data interchange
- [x] Progress tracking
- [x] Screenshot galleries
- [x] File statistics

### Configuration
- [x] Environment variables
- [x] CLI arguments
- [x] Azure DevOps integration
- [x] Custom viewports
- [x] Browser selection

