# 100 — Focus Group Setup: FreeTools Suite Review

> **Document ID:** 100
> **Category:** Focus Group Setup
> **Purpose:** Comprehensive review of FreeTools documentation and testing suite
> **Date:** 2025-12-30
> **Predicted Outcome:** Identify gaps, issues, and improvement opportunities
> **Actual Outcome:** (To be filled after session)
> **Resolution:** (To be filled after session)

---

## Objective

Review the FreeTools suite to:
1. Assess what's working well
2. Identify current pain points and limitations
3. Propose improvements and new features
4. Prioritize a roadmap for enhancements

---

## Roster

| Role | Focus Area |
|------|------------|
| **[Core]** | Tool architecture, pipeline design, code quality |
| **[UX]** | Developer experience, output usability, report clarity |
| **[Ops]** | Reliability, timing, deployment, CI/CD integration |
| **[Docs]** | Documentation completeness, accuracy |
| **[Quality]** | Testing coverage, edge cases, error handling |
| **[Sanity]** | Complexity check, simplicity, "are we overcomplicating?" |
| **[CTO]** | Final decisions (human input when needed) |

---

## Scope

### What FreeTools Does Today

| Tool | Purpose | Output |
|------|---------|--------|
| **EndpointMapper** | Discovers Blazor @page routes | `pages.csv` |
| **WorkspaceInventory** | Scans codebase for file metrics | `workspace-inventory.csv` |
| **EndpointPoker** | HTTP GET tests on routes | `*.html` snapshots |
| **BrowserSnapshot** | Playwright screenshots | `*.png` files |
| **WorkspaceReporter** | Generates markdown report | `{Project}-Report.md` |
| **AppHost** | Aspire orchestration | Pipeline coordination |

### Current Use Case (from CTO)

> "I'm mostly using this suite of tools as a way to take a screenshot of someone trying to access every page on my site. I'm using it to verify that my endpoints that require auth don't render anything to unauth'd users and slapping together a report I can just quickly glance at seeing the screenshots as thumbnails."

---

## Known Issues (from CTO)

### 1. Timing Problems
> "Sometimes the pages don't load fast enough and the screenshot is a blank page."

**Symptoms:**
- Blank screenshots
- Inconsistent results between runs
- Pages look different in browser vs screenshot

### 2. Dynamic Content
> "Most the sites I am testing are not static pages but dynamic and might just not be running API requests or something because it detects me as a bot."

**Symptoms:**
- Missing dynamic content in screenshots
- API calls not completing before capture
- Possible bot detection interference

### 3. Authentication Limitation
> "One I know of is not being able to login as part of the tests, so we can only test the pages that don't require auth."

**Impact:**
- Cannot test authenticated user experience
- Cannot verify protected pages render correctly FOR authenticated users
- Only testing "unauthorized access" scenario

---

## Materials for Discussion

### Files to Review

| Category | Files |
|----------|-------|
| **Core** | `FreeTools.Core/*.cs` |
| **Browser Tool** | `FreeTools.BrowserSnapshot/Program.cs` |
| **HTTP Tool** | `FreeTools.EndpointPoker/Program.cs` |
| **Orchestration** | `FreeTools.AppHost/Program.cs` |
| **Reporting** | `FreeTools.WorkspaceReporter/Program.cs` |
| **Sample Output** | `Docs/runs/BlazorApp1/main/latest/` |

### Current Configuration Options

| Variable | Tool | Current Default |
|----------|------|-----------------|
| `START_DELAY_MS` | All | 2000-8000ms |
| `MAX_THREADS` | Multiple | 10 |
| `SCREENSHOT_BROWSER` | Browser | chromium |
| `WaitUntil` | Browser | `WaitUntilState.Load` |
| `Timeout` | Browser | 60000ms |

---

## Questions for Discussion

### Core Architecture
1. Is the 5-phase pipeline design appropriate?
2. Should tools be more independently runnable?
3. Is Aspire orchestration adding value or complexity?

### Timing & Reliability
1. What's causing blank screenshots?
2. Is `WaitUntilState.Load` sufficient for SPAs?
3. Should we add retry logic?
4. Should we add "page ready" detection?

### Authentication
1. How could we support logged-in screenshot capture?
2. Cookie injection? Pre-authenticated browser context?
3. Multiple user personas (admin, user, guest)?

### Bot Detection
1. Are we being detected as a bot?
2. Should we add realistic browser fingerprinting?
3. Should we slow down requests to appear more human?

### Missing Features
1. Visual regression testing (diff screenshots)?
2. Accessibility auditing (axe-core)?
3. Performance metrics (Lighthouse)?
4. API endpoint testing (not just Blazor routes)?
5. Console error capture?
6. Network request logging?

---

## Success Criteria

By end of session:
- [ ] Current issues documented with root causes
- [ ] Prioritized list of improvements
- [ ] Clear recommendations for timing issues
- [ ] Authentication support decision (do it / defer / alternative)
- [ ] Roadmap with P1/P2/P3 priorities

---

*Created: 2025-12-30*
*Maintained by: [Quality]*
