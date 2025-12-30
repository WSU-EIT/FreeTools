# 104 — Implementation Plan: FreeTools P1 & P2

> **Document ID:** 104
> **Category:** Implementation Plan
> **Purpose:** Detailed task breakdown for approved P1 and P2 improvements
> **Date:** 2025-12-30
> **Related Docs:** 100-103 (Focus Group Series)
> **Status:** AWAITING CTO APPROVAL

---

## Scope

| Phase | Description | Status |
|-------|-------------|--------|
| **P1** | Fix screenshot timing issues | 🟡 Planned |
| **P2** | Improve reporting & observability | 🟡 Planned |
| **P3** | Authentication support | ❌ Deferred (out of scope) |

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        CURRENT FLOW (PROBLEM)                           │
└─────────────────────────────────────────────────────────────────────────┘

  Browser                    Blazor App                    Screenshot
    │                            │                             │
    │── Navigate ──────────────▶ │                             │
    │                            │                             │
    │◀── HTML Shell (empty) ─────│                             │
    │                            │                             │
    │    [Load Event Fires] ─────┼─────────────────────────────┤
    │                            │                             │
    │    wait 1500ms ────────────┼─────────────────────────────┤
    │                            │                             │
    │                            │◀── WASM Download ───────────│
    │                            │◀── Runtime Init ────────────│
    │                            │◀── Component Render ────────│
    │                            │◀── API Data Fetch ──────────│
    │                            │                             │
    │                            │    [SCREENSHOT TAKEN] ──────┼──▶ ❌ BLANK!
    │                            │                             │
    │                            │◀── Final Render ────────────│
    │                            │                             │
    └────────────────────────────┴─────────────────────────────┘


┌─────────────────────────────────────────────────────────────────────────┐
│                         NEW FLOW (SOLUTION)                             │
└─────────────────────────────────────────────────────────────────────────┘

  Browser                    Blazor App                    Screenshot
    │                            │                             │
    │── Navigate ──────────────▶ │                             │
    │                            │                             │
    │◀── HTML Shell (empty) ─────│                             │
    │                            │                             │
    │◀── WASM Download ──────────│                             │
    │◀── Runtime Init ───────────│                             │
    │◀── Component Render ───────│                             │
    │◀── API Data Fetch ─────────│                             │
    │◀── Final Render ───────────│                             │
    │                            │                             │
    │    [NetworkIdle] ──────────┼─────────────────────────────┤
    │                            │                             │
    │    wait 3000ms (settle) ───┼─────────────────────────────┤
    │                            │                             │
    │                            │    [SCREENSHOT TAKEN] ──────┼──▶ ✅ GOOD!
    │                            │                             │
    └────────────────────────────┴─────────────────────────────┘
```

---

## Phase 1: Fix Screenshot Timing

### Task 1.1: Change Wait Strategy

**What:** Replace `WaitUntilState.Load` with `WaitUntilState.NetworkIdle`

**Why:** `Load` fires when HTML loads (empty shell for SPAs). `NetworkIdle` waits until network is quiet for 500ms, meaning all resources are loaded.

**File:** `FreeTools.BrowserSnapshot/Program.cs`

**Change:**
```csharp
// BEFORE (line ~208)
var response = await page.GotoAsync(url, new PageGotoOptions
{
    WaitUntil = WaitUntilState.Load,
    Timeout = 60000
});

// AFTER
var response = await page.GotoAsync(url, new PageGotoOptions
{
    WaitUntil = WaitUntilState.NetworkIdle,
    Timeout = 60000
});
```

**Risk:** May hang on SignalR connections. Mitigated by existing 60s timeout.

---

### Task 1.2: Add Configurable Settle Delay

**What:** Make the post-navigation wait configurable, increase default from 1500ms to 3000ms.

**Why:** Even after NetworkIdle, JavaScript may still be running animations or final renders.

**File:** `FreeTools.BrowserSnapshot/Program.cs`

**Changes:**

1. Add env var parsing at startup:
```csharp
var settleDelayEnv = Environment.GetEnvironmentVariable("PAGE_SETTLE_DELAY_MS");
var settleDelay = int.TryParse(settleDelayEnv, out var sd) && sd > 0 ? sd : 3000;
```

2. Display in config output:
```csharp
ConsoleOutput.PrintConfig("SETTLE_DELAY", $"{settleDelay}ms");
```

3. Pass to capture method and use:
```csharp
// BEFORE
await page.WaitForTimeoutAsync(1500);

// AFTER
await page.WaitForTimeoutAsync(settleDelay);
```

---

### Task 1.3: Add Retry for Small Screenshots

**What:** If screenshot is suspiciously small (< 10KB), retry once with longer delay.

**Why:** A blank page screenshot is typically 1-5KB. A real page is usually 50KB+.

**File:** `FreeTools.BrowserSnapshot/Program.cs`

**Logic:**
```
┌─────────────────────────────────────┐
│         Take Screenshot             │
└─────────────────┬───────────────────┘
                  │
                  ▼
        ┌─────────────────┐
        │  Size < 10KB?   │
        └────────┬────────┘
                 │
       ┌─────────┴─────────┐
       │ YES               │ NO
       ▼                   ▼
┌──────────────┐    ┌──────────────┐
│ Wait 3s more │    │   Success    │
│ Retry once   │    │              │
└──────┬───────┘    └──────────────┘
       │
       ▼
┌──────────────┐
│ Take Again   │
│ Mark if still│
│ small        │
└──────────────┘
```

**Implementation:**
```csharp
// After first screenshot
var fi = new FileInfo(screenshotPath);
if (fi.Length < 10 * 1024) // < 10KB
{
    result.RetryAttempted = true;
    await page.WaitForTimeoutAsync(3000); // Extra wait
    await page.ScreenshotAsync(new PageScreenshotOptions
    {
        Path = screenshotPath,
        FullPage = true
    });
    fi = new FileInfo(screenshotPath);
}

result.FileSize = fi.Length;
result.IsSuspiciouslySmall = fi.Length < 10 * 1024;
```

---

### Task 1.4: Update ScreenshotResult Class

**What:** Add new properties to track retry and suspicious status.

**File:** `FreeTools.BrowserSnapshot/Program.cs`

**Add to `ScreenshotResult` class:**
```csharp
public bool RetryAttempted { get; set; }
public bool IsSuspiciouslySmall { get; set; }
public List<string> ConsoleErrors { get; set; } = new();
```

---

## Phase 2: Improve Reporting

### Task 2.1: Capture Console Errors

**What:** Listen for JavaScript console errors during page load.

**Why:** Helps debug why a page might be blank (JS errors preventing render).

**File:** `FreeTools.BrowserSnapshot/Program.cs`

**Implementation:**
```csharp
var consoleErrors = new List<string>();
page.Console += (_, msg) =>
{
    if (msg.Type == "error")
        consoleErrors.Add(msg.Text);
};

// After screenshot
result.ConsoleErrors = consoleErrors;
```

---

### Task 2.2: Write Screenshot Metadata File

**What:** Save a JSON file alongside each screenshot with metadata.

**Why:** Reporter needs this data to build the Screenshot Health section.

**File:** `FreeTools.BrowserSnapshot/Program.cs`

**Output format:** `{route}/metadata.json`
```json
{
  "route": "/Account/Login",
  "url": "https://localhost:5001/Account/Login",
  "statusCode": 200,
  "fileSize": 45231,
  "isSuspiciouslySmall": false,
  "retryAttempted": false,
  "consoleErrors": [],
  "capturedAt": "2025-12-30T10:15:00Z"
}
```

---

### Task 2.3: Add Screenshot Health Section to Reporter

**What:** New section in the report showing screenshot capture results.

**Why:** Surfaces failures without manual inspection of all thumbnails.

**File:** `FreeTools.WorkspaceReporter/Program.cs`

**Output:**
```markdown
## 📊 Screenshot Health

| Status | Count | Description |
|--------|-------|-------------|
| ✅ Success | 28 | Screenshots > 10KB |
| ⚠️ Suspicious | 3 | Screenshots < 10KB (possible blank) |
| 🔄 Retried | 2 | Required retry attempt |
| ❌ Failed | 1 | Capture errors |
| 🔴 JS Errors | 4 | Pages with console errors |

<details>
<summary>⚠️ Suspicious Screenshots (3)</summary>

| Route | Size | Status | Console Errors |
|-------|------|--------|----------------|
| /Account/Login | 2.1 KB | ⚠️ | None |
| /Account/Register | 1.8 KB | ⚠️ | None |
| /weather | 3.2 KB | ⚠️ | 1 error |

</details>
```

---

### Task 2.4: Update TOC and Section Ordering

**What:** Add Screenshot Health to table of contents, place after routes section.

**File:** `FreeTools.WorkspaceReporter/Program.cs`

**TOC addition:**
```csharp
sb.AppendLine("- [Screenshot Health](#-screenshot-health)");
```

---

## Summary of File Changes

| File | Tasks | Lines Changed (Est.) |
|------|-------|---------------------|
| `FreeTools.BrowserSnapshot/Program.cs` | 1.1, 1.2, 1.3, 1.4, 2.1, 2.2 | ~80 |
| `FreeTools.WorkspaceReporter/Program.cs` | 2.3, 2.4 | ~100 |

**Total estimated changes:** ~180 lines

---

## Testing Plan

### After P1:
1. Run AppHost against BlazorApp1
2. Verify no blank screenshots
3. Check console output shows settle delay
4. Verify retry logic triggers on intentionally slow pages

### After P2:
1. Verify metadata.json files created in snapshots/
2. Verify Screenshot Health section in report
3. Verify suspicious screenshots are flagged
4. Verify console errors are captured and displayed

---

## Rollout

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   Implement     │────▶│     Test        │────▶│     Commit      │
│   P1 Tasks      │     │   BlazorApp1    │     │   to main       │
└─────────────────┘     └─────────────────┘     └─────────────────┘
                                                        │
                                                        ▼
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   Implement     │────▶│     Test        │────▶│     Commit      │
│   P2 Tasks      │     │   Full Report   │     │   to main       │
└─────────────────┘     └─────────────────┘     └─────────────────┘
```

---

## Open Questions

None — scope is clear and approved.

---

## Approval Checklist

- [ ] **CTO approves this plan**
- [ ] P1 tasks are clear
- [ ] P2 tasks are clear
- [ ] Estimated effort is acceptable (~3-4 hours total)
- [ ] Testing approach is sufficient

---

*Created: 2025-12-30*
*Status: AWAITING CTO APPROVAL*
