# FreeTools.AppHost — Aspire Pipeline Orchestrator

> **Purpose:** Orchestrates the FreeTools pipeline using .NET Aspire to test web applications.  
> **Version:** 1.0  
> **Last Updated:** 2025-12-19

---

## Overview

**FreeTools.AppHost** is an Aspire orchestrator that:

1. **Starts** the target web application (FreeCRM-main by default)
2. **Runs** the tools pipeline in sequence
3. **Collects** all outputs to `FreeTools.Docs/runs/{timestamp}/`
4. **Copies** latest outputs to `FreeTools.Docs/latest/`

---

## Pipeline Phases

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         FreeTools Pipeline                                  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  Phase 0: Start Web App                                                     │
│  ┌─────────────────────┐                                                    │
│  │  FreeCRM-main/CRM   │ ◄─── Target web application                        │
│  │  (https://5001)     │                                                    │
│  └─────────────────────┘                                                    │
│           │                                                                 │
│           ▼                                                                 │
│  Phase 1: EndpointMapper ───────────────────► pages.csv                     │
│           │                                                                 │
│           ├──► Phase 2: WorkspaceInventory ─► workspace-inventory.csv       │
│           │                                                                 │
│           ▼                                                                 │
│  Phase 3: EndpointPoker ────────────────────► snapshots/*.html              │
│           │                                                                 │
│           ▼                                                                 │
│  Phase 4: BrowserSnapshot ──────────────────► snapshots/*.png               │
│           │                                                                 │
│           ▼                                                                 │
│  Phase 5: WorkspaceReporter ────────────────► LatestReport.md               │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## Usage

### Run the Pipeline

```bash
cd tools/FreeTools.AppHost
dotnet run
```

This will:
1. Start FreeCRM on https://localhost:5001
2. Run all tools in sequence
3. Write outputs to `FreeTools.Docs/runs/2025-12-19_143052/`
4. Copy to `FreeTools.Docs/latest/`

### View Outputs

```bash
# Latest run
ls tools/FreeTools.Docs/latest/

# Historical runs
ls tools/FreeTools.Docs/runs/
```

---

## Configuration

### Change Target Project

Edit `Program.cs` to point to a different project:

```csharp
// Change this line:
var targetProjectRoot = Path.GetFullPath(Path.Combine(toolsRoot, "..", "FreeCRM-main"));

// To your project:
var targetProjectRoot = Path.GetFullPath(Path.Combine(toolsRoot, "..", "MyProject"));
```

Also update the project reference in `.csproj`:

```xml
<ProjectReference Include="..\..\MyProject\MyProject.csproj" />
```

### Environment Variables

The AppHost sets these for each tool:

| Tool | Variables |
|------|-----------|
| EndpointMapper | args: `[targetRoot, pagesCsv]` |
| WorkspaceInventory | args: `[targetRoot, inventoryCsv]` |
| EndpointPoker | `BASE_URL`, `CSV_PATH`, `OUTPUT_DIR`, `MAX_THREADS`, `START_DELAY_MS` |
| BrowserSnapshot | `BASE_URL`, `CSV_PATH`, `OUTPUT_DIR`, `SCREENSHOT_BROWSER`, `MAX_THREADS` |
| WorkspaceReporter | `REPO_ROOT`, `OUTPUT_PATH`, `WORKSPACE_CSV`, `PAGES_CSV`, `SNAPSHOTS_DIR` |

---

## Output Structure

Each run creates:

```
FreeTools.Docs/runs/2025-12-19_143052/
├── pages.csv                    # All Blazor routes
├── workspace-inventory.csv      # File inventory with metrics
├── snapshots/
│   ├── /
│   │   ├── default.html        # HTTP response
│   │   └── default.png         # Screenshot
│   ├── Account/
│   │   └── Login/
│   │       ├── default.html
│   │       └── default.png
│   └── ...
└── LatestReport.md              # Summary report
```

---

## Requirements

- **.NET 10 SDK**
- **.NET Aspire workload**: `dotnet workload install aspire`
- **Playwright browsers**: Run once after restore:
  ```bash
  pwsh tools/FreeTools.BrowserSnapshot/bin/Debug/net10.0/playwright.ps1 install
  ```

---

## Customization

### Adding a New Target Project

1. Add project reference to `.csproj`
2. Update `targetProjectRoot` in `Program.cs`
3. Update the `AddProject<>` call for the web app

### Changing Tool Order

Modify the `WaitFor()` dependencies in `Program.cs`:

```csharp
var myTool = builder.AddProject<Projects.MyTool>("my-tool")
    .WaitFor(endpointMapper)  // Run after EndpointMapper
    .WaitFor(webApp);         // Wait for web app
```

### Parallel Execution

Tools with no dependencies run in parallel by default. Use `WaitFor()` to enforce ordering.
