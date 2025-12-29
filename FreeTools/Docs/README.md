# FreeTools.Docs — Centralized Tool Output Repository

> **Purpose:** Central location for all FreeTools pipeline outputs, organized by run timestamp.  
> **Version:** 1.0  
> **Last Updated:** 2025-12-19

---

## Overview

**FreeTools.Docs** is the output repository for the FreeTools pipeline. All tools write their outputs here, organized by timestamp.

---

## Directory Structure

```
FreeTools.Docs/
├── latest/                    # Symlink/copy of most recent run
│   ├── pages.csv              # Route inventory
│   ├── workspace-inventory.csv
│   ├── snapshots/             # Screenshots and HTML
│   └── LatestReport.md        # Summary report
│
├── runs/                      # Historical runs by timestamp
│   ├── 2025-12-19_143052/     # Format: YYYY-MM-DD_HHMMSS
│   │   ├── pages.csv
│   │   ├── workspace-inventory.csv
│   │   ├── snapshots/
│   │   └── LatestReport.md
│   │
│   ├── 2025-12-19_150000/
│   │   └── ...
│   └── ...
│
└── README.md
```

---

## Output Files

| File | Source Tool | Description |
|------|-------------|-------------|
| `pages.csv` | EndpointMapper | All Blazor routes with auth requirements |
| `workspace-inventory.csv` | WorkspaceInventory | File inventory with metrics |
| `snapshots/` | EndpointPoker + BrowserSnapshot | HTML and PNG for each route |
| `LatestReport.md` | WorkspaceReporter | Aggregated markdown report |

---

## Usage

### From AppHost (Recommended)

The AppHost automatically creates timestamped run folders:

```bash
cd tools/FreeTools.AppHost
dotnet run
```

Outputs go to `FreeTools.Docs/runs/YYYY-MM-DD_HHMMSS/`

### Manual Tool Runs

Set `OUTPUT_ROOT` to direct outputs here:

```bash
$env:OUTPUT_ROOT = "path/to/FreeTools.Docs/runs/$(Get-Date -Format 'yyyy-MM-dd_HHmmss')"

dotnet run --project FreeTools.EndpointMapper
dotnet run --project FreeTools.EndpointPoker
dotnet run --project FreeTools.BrowserSnapshot
dotnet run --project FreeTools.WorkspaceReporter
```

---

## Retention

Historical runs are kept indefinitely by default. To clean up old runs:

```bash
# Keep only last 10 runs
Get-ChildItem -Path runs -Directory | 
    Sort-Object Name -Descending | 
    Select-Object -Skip 10 | 
    Remove-Item -Recurse -Force
```

---

## Git Ignore

Add to `.gitignore` to exclude outputs from version control:

```gitignore
# FreeTools outputs
tools/FreeTools.Docs/runs/
tools/FreeTools.Docs/latest/
```

Or keep them tracked for historical reference — your choice.
