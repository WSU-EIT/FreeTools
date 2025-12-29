# WorkspaceReporter — Dashboard & Report Generator

> **Purpose:** Generates a comprehensive markdown report (LatestReport.md) aggregating outputs from all other tools, including screenshot galleries, code statistics, and route analysis.  
> **Last Reviewed:** 2025-12-19  
> **Version:** 1.1

---

## Overview

The **WorkspaceReporter** tool is a development utility that:

- **Aggregates data:** Reads outputs from WorkspaceInventory, PageScanner, PageTester, and PageScreenshoter
- **Generates statistics:** File counts, line counts, code distribution charts
- **Creates galleries:** Screenshot thumbnails with clickable full-size images
- **Analyzes routes:** Auth requirements, route distribution by area
- **Outputs markdown:** GitHub/Azure DevOps compatible report with collapsible sections

---

## ⚠️ Scope Limitations

This tool focuses on **Blazor pages only**:

| Included | Not Included |
|----------|--------------|
| ✅ Blazor pages (`@page` directives) | ❌ API endpoints (`/api/*`) |
| ✅ Razor components | ❌ Dynamic routes (MapGet/MapPost) |
| ✅ C# source files | ❌ Routes with parameters (`{id}`) — skipped |
| ✅ Configuration files | |

Routes with parameters (e.g., `/Account/Manage/{Id}`) are detected and listed separately but not tested or screenshotted.

---

## Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 10.0 | Runtime framework |

---

## Usage

### Via Aspire AppHost (Recommended)

The tool runs automatically as **Phase 5** after all other tools complete:

```bash
cd UnifiedAppHost
dotnet run
```

### Standalone

```bash
cd tools/WorkspaceReporter
dotnet run [repoRoot] [outputPath]
```

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `REPO_ROOT` | Repository root directory | Auto-detect |
| `OUTPUT_PATH` | Output markdown file path | `<repo>/LatestReport.md` |
| `WORKSPACE_CSV` | Path to workspace-inventory.csv | `<repo>/workspace-inventory.csv` |
| `WORKSPACE_CSHARP_CSV` | Path to C# files CSV | `<repo>/workspace-inventory-csharp.csv` |
| `WORKSPACE_RAZOR_CSV` | Path to Razor files CSV | `<repo>/workspace-inventory-razor.csv` |
| `PAGES_CSV` | Path to pages.csv | `<web>/pages.csv` |
| `SNAPSHOTS_DIR` | Path to page-snapshots directory | `<web>/page-snapshots` |
| `WEB_PROJECT_ROOT` | Path to web project | `<repo>/Web/FreeTools.Web` |

---

## Output Format

The generated `LatestReport.md` includes:

### 📁 Workspace Overview
- Total files, lines, characters, size

### 📈 File Statistics
- File counts by category (CSharpSource, RazorPage, Config, etc.)
- Percentage distribution with progress bars

### 📊 Code Distribution
- ASCII bar charts showing lines of code by category
- Extension distribution

### 📏 Largest Files
- Top 15 C# files by line count
- Top 15 Razor files by line count

### 🛤️ Page Routes
- Route summary (total, public, protected)
- Access distribution chart
- Collapsible route listings by area

### 📸 Screenshot Gallery
- Organized by page area
- Clickable thumbnails (250px width)
- Full-size image links

---

## Integration

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           Tool Pipeline                                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  PHASE 3-4: Data Generation                                                 │
│  ┌─────────────────┐  ┌────────────┐  ┌────────────────┐  ┌──────────────┐  │
│  │WorkspaceInventory│  │ PageScanner│  │  PageTester    │  │PageScreenshoter│
│  │                 │  │            │  │                │  │              │  │
│  │ workspace-*.csv │  │ pages.csv  │  │  *.html files  │  │  *.png files │  │
│  └────────┬────────┘  └─────┬──────┘  └───────┬────────┘  └──────┬───────┘  │
│           │                 │                 │                  │          │
│           └─────────────────┴─────────────────┴──────────────────┘          │
│                                       │                                     │
│  PHASE 5: Report Generation           ▼                                     │
│  ┌───────────────────────────────────────────────────────────────────────┐  │
│  │                        WorkspaceReporter                              │  │
│  │                                                                       │  │
│  │  Reads all outputs → Generates LatestReport.md                        │  │
│  └───────────────────────────────────────────────────────────────────────┘  │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## Sample Output

```markdown
# 📊 FreeTools Workspace Report

> **Generated:** 2025-12-19 10:30:00
> **Repository:** Public2

## 📁 Workspace Overview

| Metric | Value |
|--------|-------|
| **Total Files** | 523 |
| **Total Lines** | 45,230 |
| **Total Size** | 2.1 MB |

## 📊 Code Distribution

```
CSharpSource         ████████████████████████████████████████ 28,450
RazorPage            ████████████████████                     12,340
Config               █████████                                 3,200
...
```

---

*Part of the FreeTools suite*
