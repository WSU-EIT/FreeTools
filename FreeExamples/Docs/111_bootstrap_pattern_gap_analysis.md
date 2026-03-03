# 111 — Bootstrap Pattern Gap Analysis

> **Generated:** Research scan of `_Repos/` production systems (30 projects, 3,592 `.razor` + 622 `.cshtml` files)
> vs. existing FreeExamples pages (68 `.razor` example pages).

---

## 1. Production Bootstrap Usage Summary (Top Patterns by Count)

| Pattern | `.razor` Count | `.cshtml` Count | Production Projects Using It |
|---|---|---|---|
| `form-control` | 4,876 | 2,912 | All |
| `form-check-input` / `form-check` | 2,847 / 2,818 | 655 / 590 | All |
| `form-switch` | 2,652 | — | All Blazor |
| `form-check-label` | 2,416 | 173 | All |
| `btn-dark` | 2,022 | 415 | All (primary action) |
| `btn-success` | 1,814 | 224 | All (Save) |
| `btn-sm` | 1,539 | 115 | All |
| `form-select` | 1,536 | 151 | All |
| `btn-primary` | 1,421 | 182 | All |
| `btn-group` | 1,047 | 113 | Helpdesk, Credentials, Tasks |
| `card-body` / `card-header` | 1,007 / 789 | 69 / 69 | All |
| `btn-xs` | 885 | 79 | All (inline table actions) |
| `table` / `table-sm` | 812 / 711 | 148 / 68 | All |
| `alert-danger` | 718 | 43 | All (validation) |
| `badge` | 693 | — | All |
| `table-dark` (header row) | 486 | 51 | All |
| `btn-warning` | 579 | 38 | All (Re-open, Process) |
| `btn-danger` | 420 | 174 | All (Delete) |
| `input-group` / `input-group-text` | 341 / 375 | 99 / 92 | All |
| `tab-pane` / `nav-tabs` | 315 / 66 | 31 / — | Helpdesk Settings, Flex, Smartsheets |
| `dropdown-item` / `dropdown-menu` | 309 / 141 | 124 / 16 | All (saved filters, menus) |
| `spinner-border` | 139 | — | All Blazor |
| `modal-*` | 89 each | — | Helpdesk, GLBA, CICD, Smartsheets |
| `offcanvas-*` | 77 each | — | All Blazor (user menu, quick actions) |
| `card-title` / `card-footer` | 85 / 41 | — | Multiple |
| `list-group-item` / `list-group-flush` | 80 / 37 | — | Multiple |
| `toast-*` | 41 each | — | All Blazor (MainLayout) |
| `navbar-*` | 42–82 | — | All Blazor (MainLayout) |
| `pagination` / `pagination-sm` | 20 / 16 | — | Smartsheets, Helpdesk |
| `accordion-*` | 11–18 | — | Estimate, Smartsheets, Touchpoints |
| `form-floating` | 26 | — | Identity/Auth pages |
| `progress-bar` | 31 | — | Multiple |
| `form-range` | 10 | — | Helpdesk, Flex |
| `breadcrumb` | — | — | FileDemoV2, GitBrowser only |
| `carousel` | — | — | Dashboard, Carousel only |

---

## 2. Master Page Inventory — All Existing FreeExamples Pages

### Legend
- ✅ = Featured / primary focus
- ◻ = Present but incidental
- — = Not present

| # | Page Name | About / Title | form-control | form-select | form-check / switch | btn-group | modal | offcanvas | accordion | tabs | toast | pagination | progress | alerts | badges | dropdown | table | card | list-group | spinner | breadcrumb | input-group | form-floating | form-range | carousel | collapse | navbar |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 | **Dashboard** | FreeExamples Home | — | — | — | — | ◻ | — | — | — | — | ◻ | ◻ | — | ◻ | — | ◻ | ◻ | — | — | — | — | — | — | ◻ | — | — |
| 2 | **SampleItems** | CRUD List (main) | ✅ | ✅ | ✅ | ✅ | — | — | — | — | — | ✅ | — | — | — | ✅ | ✅ | — | — | — | — | — | — | — | — | — | — |
| 3 | **SampleItemsV1** | Card Grid View | — | ◻ | — | — | — | — | — | — | — | — | — | — | ◻ | — | ◻ | ✅ | — | — | — | — | — | — | — | — | — |
| 4 | **SampleItemsV2** | Split Panel View | ✅ | — | — | — | — | — | — | — | — | — | — | — | ◻ | — | ◻ | ◻ | ✅ | — | — | — | — | — | — | — | — |
| 5 | **SampleItemsV3** | Grouped Accordion View | — | — | — | — | — | — | ✅ | — | — | — | — | — | ◻ | — | ◻ | ◻ | — | — | — | — | — | — | — | ✅ | — |
| 6 | **SampleItemsV4** | Timeline View | — | — | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 7 | **SampleItemsV5** | Stats Dashboard View | — | — | — | — | — | — | — | — | — | — | ◻ | — | ◻ | — | ◻ | ✅ | — | — | — | — | — | — | — | — | — |
| 8 | **EditSampleItem** | Single Item CRUD Form | ✅ | ✅ | ✅ | ✅ | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| 9 | **BootstrapShowcase** | All Bootstrap Components | ✅ | ✅ | ✅ | — | ✅ | — | ✅ | ✅ | ✅ | — | ✅ | ✅ | ✅ | — | ✅ | ✅ | — | ✅ | — | ✅ | — | — | — | ✅ | — |
| 10 | **BootstrapV1** | Email Template Builder | ✅ | ✅ | ✅ | — | — | — | — | — | — | — | — | ◻ | ◻ | — | ✅ | ✅ | — | — | — | — | — | — | — | — | — |
| 11 | **BootstrapV2** | Settings Page | ✅ | ✅ | ✅ | ✅ | — | — | — | ✅ | — | — | — | ◻ | — | — | — | ◻ | — | — | — | ✅ | — | ✅ | — | — | — |
| 12 | **BootstrapV3** | Error Pages | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| 13 | **BootstrapV4** | User Profile | — | — | — | — | — | — | — | ✅ | — | — | — | — | ◻ | ✅ | — | ✅ | ✅ | — | — | — | — | — | — | — | — |
| 14 | **BootstrapV5** | Pricing Page | — | — | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 15 | **FileDemo** | File Operations | — | — | — | ✅ | — | — | — | — | — | — | — | — | ◻ | — | ✅ | ✅ | — | — | — | — | — | — | — | — | — |
| 16 | **FileDemoV1** | Profile Photo Upload | ✅ | — | — | — | — | — | — | — | — | — | — | — | — | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 17 | **FileDemoV2** | Document Library | — | — | — | — | — | — | — | — | — | — | — | — | — | — | ✅ | ✅ | — | — | ✅ | — | — | — | — | — | — |
| 18 | **FileDemoV3** | Application Checklist | — | — | — | — | — | — | — | — | — | — | ✅ | — | — | — | — | ✅ | — | — | — | — | — | — | — | — |
| 19 | **FileDemoV4** | Bulk Data Import | — | — | — | — | — | — | — | — | — | — | — | ✅ | ◻ | — | ✅ | ✅ | — | — | — | — | — | — | — | — | — |
| 20 | **FileDemoV5** | Ticket Attachments | — | — | — | — | — | — | — | — | — | — | ◻ | — | ◻ | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 21 | **SignatureDemo** | Signature Pad | — | — | — | — | — | — | — | — | — | — | — | — | ◻ | — | ◻ | ✅ | — | — | — | — | — | — | — | — | — |
| 22 | **SignatureV1** | Job Application | ✅ | ✅ | ✅ | — | — | — | — | — | — | — | — | ✅ | — | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 23 | **SignatureV2** | Document Acknowledgment | ✅ | — | ✅ | — | — | — | — | — | — | — | — | ✅ | ◻ | — | ✅ | ✅ | — | — | — | — | — | — | — | — | — |
| 24 | **SignatureV3** | Upload & Sign | ✅ | — | — | — | — | — | — | — | — | — | — | ✅ | — | — | — | ✅ | ✅ | — | — | ✅ | — | — | — | — | — |
| 25 | **SignatureV4** | GLBA Compliance Gate | ✅ | ✅ | — | — | ✅ | — | — | — | — | — | — | ✅ | ◻ | — | ✅ | ✅ | — | — | — | — | — | — | — | — | — |
| 26 | **SignatureV5** | Contract Section Initials | ✅ | — | — | — | — | — | — | — | — | — | ◻ | ✅ | ◻ | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 27 | **ChartsDashboard** | Charts Overview | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 28 | **ChartsV1** | Sales Analytics | — | — | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | ✅ | ✅ | — | — | — | — | — | — | — | — |
| 29 | **ChartsV2** | University Enrollment | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 30 | **ChartsV3** | Infrastructure Monitoring | — | — | — | — | — | — | — | — | — | — | ◻ | — | ◻ | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 31 | **ChartsV4** | HR / People Analytics | — | — | — | — | — | — | — | — | — | — | ◻ | — | ◻ | — | — | ✅ | ✅ | — | — | — | — | — | — | — | — |
| 32 | **ChartsV5** | Web Analytics | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 33 | **CodeEditor** | Monaco Code Editor | — | ✅ | — | — | — | — | — | — | — | — | — | — | — | ✅ | — | — | — | — | — | — | — | — | — | — | — |
| 34 | **CodeEditorV1** | SQL Query Builder | — | — | — | — | — | — | — | — | — | — | — | — | ◻ | — | ✅ | ✅ | — | — | — | — | — | — | — | — | — |
| 35 | **CodeEditorV2** | API Tester | ✅ | ✅ | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 36 | **CodeEditorV3** | Config Editor | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | ✅ | ✅ | — | — | — | — | — | — | — | — |
| 37 | **CodeEditorV4** | Diff Viewer | — | — | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 38 | **CodeEditorV5** | Template Engine | ✅ | — | — | — | — | — | — | — | — | — | — | — | — | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 39 | **SignalRDemo** | Real-time SignalR | ✅ | ✅ | ✅ | — | — | — | — | — | ✅ | — | — | — | ◻ | — | ✅ | ✅ | — | ✅ | — | — | — | — | — | — | — |
| 40 | **SignalRV1** | Live Notifications | — | — | — | — | — | — | — | — | ✅ | — | — | ✅ | — | — | — | — | — | — | — | — | — | — | — | — | — |
| 41 | **SignalRV2** | Online Presence | — | — | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 42 | **SignalRV3** | Live Polling | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 43 | **SignalRV4** | Activity Feed | — | — | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | — | — | — | — | — | — | — | — | — | — |
| 44 | **SignalRV5** | Live Scoreboard | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | — | ✅ | ✅ | — | — | — | — | — | — | — | — | — |
| 45 | **TimerDemo** | Timer/Debounce | ✅ | — | ✅ | — | — | — | — | — | — | — | ◻ | — | — | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 46 | **TimerV1** | Pomodoro Timer | — | — | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | — | — | — | — | — | — | — | — | — | — |
| 47 | **TimerV2** | Session Timeout | — | — | — | — | ✅ | — | — | — | — | — | — | ✅ | — | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 48 | **TimerV3** | Auto-Refresh | — | ✅ | — | — | — | — | — | — | — | — | ◻ | — | — | — | — | ✅ | — | ✅ | — | — | — | — | — | — | — |
| 49 | **TimerV4** | Timed Quiz | — | — | ✅ | — | — | — | — | — | — | — | ◻ | — | ◻ | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 50 | **TimerV5** | Event Countdown | — | — | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | — | — | — | — | — | — | — | — | — | — |
| 51 | **NetworkGraph** | SVG Network | — | — | — | ✅ | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| 52 | **NetworkGraphV1** | Org Chart | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 53 | **NetworkGraphV2** | Dependency Map | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — | ✅ | ✅ | — | — | — | — | — | — | — | — |
| 54 | **WizardDemo** | Multi-step Wizard | ✅ | ✅ | — | — | — | — | — | — | — | — | ◻ | — | ◻ | — | ✅ | ✅ | — | — | ✅ | — | — | — | — | — | — |
| 55 | **KanbanBoard** | Drag & Drop Board | ✅ | ✅ | — | — | — | — | — | — | — | — | — | — | ◻ | — | ◻ | ✅ | — | — | — | ✅ | — | — | — | — | — |
| 56 | **SearchAutocomplete** | Typeahead Search | ✅ | — | — | — | — | — | — | — | — | — | — | — | ◻ | ✅ | — | ✅ | — | ✅ | — | ✅ | — | — | — | — | — |
| 57 | **GitBrowser** | Git Repo Browser | ✅ | — | — | — | — | — | — | — | — | — | ◻ | — | ◻ | — | — | ✅ | ✅ | ✅ | ✅ | ✅ | — | — | — | — | — |
| 58 | **ApiKeyDemo** | API Key Middleware | ✅ | ✅ | — | — | — | — | — | — | ◻ | — | — | ✅ | ◻ | — | ✅ | ✅ | — | ✅ | — | ✅ | — | — | — | — | — |
| 59 | **Carousel** | Image Carousel | — | ✅ | — | — | — | — | — | — | — | — | — | — | ◻ | — | — | ✅ | — | — | — | — | — | — | ✅ | — | — |
| 60 | **ChatView** | Chat Interface | ✅ | ✅ | — | — | — | — | — | — | — | — | ◻ | — | ◻ | — | — | ✅ | — | ✅ | — | ✅ | — | — | — | — | — |
| 61 | **CommandPalette** | VS Code-style Palette | ✅ | — | — | — | ✅ | — | — | — | — | — | — | — | ◻ | ✅ | ✅ | ✅ | — | ✅ | — | ✅ | — | — | — | — | — |
| 62 | **CommentThread** | Threaded Comments | ✅ | ✅ | — | — | — | — | — | — | — | — | — | — | ◻ | ✅ | — | ✅ | — | ✅ | — | — | — | — | — | — | — |
| 63 | **ComparisonTable** | Side-by-side Compare | — | — | ✅ | — | — | — | — | — | — | — | — | — | ◻ | — | ✅ | ✅ | — | — | — | — | — | — | — | — | — |
| 64 | **ImageGallery** | Image Gallery | — | — | — | ✅ | ✅ | — | — | — | — | — | — | — | ◻ | — | — | ✅ | — | — | — | — | — | — | — | — | — |
| 65 | **ItemCards** | Item Card Layouts | — | — | ✅ | — | — | — | — | — | — | — | ◻ | — | ◻ | — | ◻ | ✅ | ✅ | — | — | — | — | — | — | — | — |
| 66 | **PipelineTracker** | CI/CD Pipeline | — | ✅ | — | — | — | — | — | — | — | — | ✅ | ✅ | ✅ | — | — | ✅ | — | ✅ | — | — | — | — | — | — | — |
| 67 | **StatusBoard** | System Status | — | ✅ | — | — | — | — | — | — | — | — | ◻ | — | ◻ | — | ✅ | ✅ | — | ✅ | — | — | — | — | — | — | — |
| 68 | **CodePlayground** | Live Code Runner | — | ✅ | — | ✅ | — | — | — | ✅ | — | — | — | ✅ | ✅ | ✅ | ✅ | ✅ | — | ✅ | — | — | — | — | — | — | — |

---

## 3. Gap Analysis — Production Patterns Missing from FreeExamples

### 3A. Bootstrap Components with ZERO or Minimal Coverage

| Missing Pattern | Production Usage | Where in Production | Priority |
|---|---|---|---|
| **Offcanvas** (slide-out panel) | 77 instances | Helpdesk MainLayout (user menu, quick add user), all Blazor apps | 🔴 HIGH |
| **Form-floating** (floating labels) | 26 instances | Identity/Auth pages (Login, Register, ChangePassword) | 🟡 MEDIUM |
| **Nav-pills** (pill-style nav) | 66+ `nav-tabs` but 0 pills | Smartsheets, Flex5 | 🟡 MEDIUM |
| **Navbar** (top navigation bar) | 42–82 instances | All Blazor MainLayouts | 🟡 MEDIUM |
| **Modal with footer** (`modal-footer`, `modal-lg`) | 89 modal instances | Helpdesk, GLBA, CICD Import | 🟡 MEDIUM |
| **Btn-toolbar** (button toolbar) | 10 instances | SAP pagination, Helpdesk | 🟢 LOW |
| **Split-button dropdown** (`dropdown-toggle-split`) | Used in Helpdesk Save button | Helpdesk Request page (Save / Save Without Notification) | 🔴 HIGH |
| **Date pickers in forms** | Pervasive | Helpdesk filters, Tasks, Credentials | 🟡 MEDIUM |
| **Multi-select `<select multiple>` with categories** | Helpdesk, TouchPoints | Filter pages | 🟡 MEDIUM |

### 3B. Functional Patterns with ZERO or Minimal Coverage

| Missing Pattern | Production Example | Where in Production | Priority |
|---|---|---|---|
| **Advanced Filter Panel** (multi-field filter with saved filters dropdown) | Helpdesk Index, TouchPoints Search | Helpdesk, TouchPoints, SAP | 🔴 HIGH |
| **Settings Page with Tabbed Sections** (admin-style tabs + form fields) | Helpdesk Settings (7+ tabs: General, Theme, Auth, Email, etc.) | All Blazor apps | 🔴 HIGH |
| **Offcanvas Quick-Action Sidebar** (slide-out form for quick add) | Helpdesk MainLayout ("Add User" offcanvas) | Helpdesk, Credentials | 🔴 HIGH |
| **Toast Notification System** (color-coded, auto-dismiss) | MainLayout toast container | All Blazor apps | 🟡 MEDIUM |
| **Inline Table Actions** (Edit/Delete buttons per row with `btn-xs`) | DependencyManager, Helpdesk, Credentials | All data-management apps | 🟡 MEDIUM |
| **Master-Detail with Edit/View Toggle** (readonly vs editable form) | Credentials EditCredential (AllowEdit toggle) | Credentials, DependencyManager | 🔴 HIGH |
| **Bulk Select / Bulk Actions** (checkbox column + batch operations) | TouchPoints, Helpdesk | Multiple admin pages | 🟡 MEDIUM |
| **Form Layout: Table-based Label/Value** (table with label column + input column) | Helpdesk NewRequest | Helpdesk, Tasks | 🟡 MEDIUM |
| **Confirmation Delete Pattern** (`DeleteConfirmation` component) | Used across all apps | All CRUD pages | 🟢 Exists in EditSampleItem |
| **UndeleteMessage Pattern** (soft-delete with undo) | Helpdesk, Tasks, Credentials | All CRUD apps | 🟡 MEDIUM |
| **Saved Filters Dropdown** (common filters + custom saved filters) | Helpdesk Index | Helpdesk | 🟡 MEDIUM |
| **User Lookup / Typeahead for entity references** | Helpdesk NewRequest (UserLookup component) | Helpdesk, Tasks | 🟢 Exists in SearchAutocomplete |
| **Multi-step Import with Modal** (URL/File → Validate → Import → Done) | FreeCICD Import modal | FreeCICD | 🟡 MEDIUM |
| **Accordion-based Financial/Data Sections** | Estimate (Financial Aid accordion) | Estimate | 🟢 Exists in SampleItemsV3 |
| **Role-based UI visibility** (`if (Model.TechOrAdmin)`, `if (Model.User.Admin)`) | All production apps | Pervasive | 🟡 MEDIUM |

---

## 4. Proposed New Example Pages

Based on the gap analysis above, here are the recommended new pages to build, ordered by impact:

| # | Proposed Page | Route | Mirrors Production Pattern From | Key Bootstrap Components | Key Functional Patterns |
|---|---|---|---|---|---|
| 1 | **BootstrapV6** — Advanced Filter Panel | `/Examples/BootstrapV6` | Helpdesk Index, TouchPoints Search, SAP Filter | `form-control`, `form-select`, `form-check form-switch`, `btn-group`, `dropdown-menu`, `dropdown-item`, `input-group`, `badge` | Multi-field filters, saved filters dropdown, date range, keyword search, filter reset, record count display |
| 2 | **BootstrapV7** — Settings Admin Panel | `/Examples/BootstrapV7` | Helpdesk Settings, all Blazor app Settings pages | `nav-tabs`, `tab-pane`, `form-control`, `form-select`, `form-check form-switch`, `form-floating`, `card`, `alert-info`, `btn-success` | Tabbed settings, toggle switches in sections, save all, required field validation, conditional tab visibility |
| 3 | **BootstrapV8** — Offcanvas & Sidebar Actions | `/Examples/BootstrapV8` | Helpdesk MainLayout (Quick Add User, User Menu), all Blazor offcanvas | `offcanvas`, `offcanvas-end`, `offcanvas-header`, `offcanvas-body`, `offcanvas-title`, `btn-close`, `form-control`, `list-group`, `badge` | Right-side slide-out form, quick-add entity, user profile sidebar, language selector, tenant switcher simulation |
| 4 | **BootstrapV9** — Master-Detail Edit Form | `/Examples/BootstrapV9` | Credentials EditCredential, DependencyManager EditDependency | `form-control`, `form-select`, `form-check form-switch`, `card-header`, `card-body`, `card-footer`, `alert-warning`, `alert-danger`, `btn-group`, `dropdown-toggle-split`, `table table-sm` | Read-only vs edit toggle, conditional field rendering, split-button save, inline table with add/remove rows, soft-delete/undelete message, card sections for grouping |
| 5 | **BootstrapV10** — Modal Workflows & Toasts | `/Examples/BootstrapV10` | Helpdesk modals, FreeCICD Import wizard modal, MainLayout toasts | `modal`, `modal-dialog`, `modal-lg`, `modal-header`, `modal-body`, `modal-footer`, `toast`, `toast-container`, `alert-*`, `progress-bar`, `spinner-border`, `btn-toolbar` | Confirmation modal, large modal with steps (import flow), toast notification system with variants (success/danger/warning), auto-dismiss toasts, stacked toasts |
| 6 | **BootstrapV11** — Data Table with Inline Actions | `/Examples/BootstrapV11` | Helpdesk Request list, DependencyManager table, Tasks list | `table`, `table-sm`, `table-dark`, `table-hover`, `table-striped`, `table-responsive`, `btn-xs`, `btn-group`, `badge`, `pagination`, `pagination-sm`, `form-check-input` | Sortable columns, row-level action buttons (Edit/Delete/Duplicate), bulk checkbox select, pagination with page-size selector, column header filters, export button, responsive wrapper |
| 7 | **BootstrapV12** — Request Submission Form | `/Examples/BootstrapV12` | Helpdesk NewRequest, TouchPoints forms | `table` (label/value layout), `form-control`, `form-select`, `form-check form-switch`, `form-range`, `nav-pills`, `input-group`, `btn-success`, `btn-primary`, `alert-danger` | Table-based form layout (like Helpdesk), conditional fields based on selection, required field highlighting with CSS class, person-affected dropdown switching UI, file attachment area, urgent flag toggle |

---

## 5. Coverage Matrix After Proposed Pages

| Bootstrap Component | Before (68 pages) | After (+7 pages) | Production Coverage |
|---|---|---|---|
| Offcanvas | ❌ 0 pages | ✅ BootstrapV8 | ✅ |
| Form-floating | ❌ 0 pages | ✅ BootstrapV7 | ✅ |
| Nav-pills | ❌ 0 pages | ✅ BootstrapV12 | ✅ |
| Navbar | ❌ 0 pages | ◻ (not applicable — in MainLayout, not example pages) | N/A |
| Modal with footer | ❌ 0 pages | ✅ BootstrapV10 | ✅ |
| Modal-lg | ❌ 0 pages | ✅ BootstrapV10 | ✅ |
| Btn-toolbar | ❌ 0 pages | ✅ BootstrapV10 | ✅ |
| Split-button dropdown | ❌ 0 pages | ✅ BootstrapV9 | ✅ |
| Table-hover / table-striped | ◻ incidental | ✅ BootstrapV11 | ✅ |
| Table-responsive | ❌ 0 pages | ✅ BootstrapV11 | ✅ |
| Pagination (dedicated) | ◻ Dashboard only | ✅ BootstrapV11 | ✅ |
| Form-range | ◻ BootstrapV2 only | ✅ BootstrapV12 | ✅ |
| Saved filters dropdown | ❌ 0 pages | ✅ BootstrapV6 | ✅ |
| Offcanvas quick-action form | ❌ 0 pages | ✅ BootstrapV8 | ✅ |
| Toast notification system | ◻ BootstrapShowcase only | ✅ BootstrapV10 | ✅ |
| Edit/View toggle | ❌ 0 pages | ✅ BootstrapV9 | ✅ |
| Inline table actions (btn-xs) | ❌ 0 pages | ✅ BootstrapV11 | ✅ |
| Multi-field filter panel | ❌ 0 pages | ✅ BootstrapV6 | ✅ |
| Tabbed settings page | ◻ BootstrapV2 light | ✅ BootstrapV7 (full) | ✅ |
| Bulk select / batch ops | ❌ 0 pages | ✅ BootstrapV11 | ✅ |

---

## 6. Source Production Files Referenced

| Project | Key Files Examined | Patterns Found |
|---|---|---|
| **Helpdesk4** | `Request.razor`, `Index.razor`, `NewRequest.razor`, `Stats.razor`, `Settings.razor`, `MainLayout.razor` | Split-button save, saved filters, offcanvas, toasts, tabs, dropdowns, modals, table forms, pagination, date pickers, conditional rendering |
| **Credentials** | `EditCredential.razor`, `MainLayout.razor` | Edit/view toggle, form-switch in alerts, card sections, file management, type-specific conditional fields |
| **DependencyManager** | `EditDependency.razor` | Inline table add/remove, card with colored header, btn-xs actions |
| **Tasks** | `EditTask.razor` | Status badges, form-switch, conditional buttons, processing flags |
| **Estimate** | `Estimate.razor`, `SlateEstimate.razor` | Accordion sections, financial tables, modal for campus contacts |
| **TouchPoints** | `_partialSearch.cshtml` | Multi-field filter, multi-select dropdowns, form-switch, btn-group, export, date pickers |
| **SAP** | `_partialHome.cshtml` | Form-switch with scaled inputs, pagination, record count, filter apply |
| **AcademicCalendarPetitions** | `FreeCICD.App.UI.Import.razor`, `Settings.razor`, `MainLayout.razor` | Import wizard modal, offcanvas, nav-tabs settings |
| **FreeForm** | `_partialFormEditor.cshtml` | Complex form builder patterns |
