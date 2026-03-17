# Proposed Example Page Categories

> 10 new entity-driven example categories, each with its own data object, CRUD endpoints, and multiple page variants — following the same `SampleItems` pattern but covering **different real-world university scenarios** with **different field types, form patterns, and UI layouts**.

---

## What We Already Have

| Category | Pages | Pattern |
|---|---|---|
| Dashboard & Data | SampleItems + V1–V5, Search, Comparison, ItemCards | CRUD table, card grid, split panel, accordion, timeline, stats |
| Files & Media | FileDemo + V1–V6, Gallery, Carousel, Signature + V1–V5 | Upload, drag-drop, preview, digital signatures |
| UI Components | Bootstrap V1–V12, Kanban, Status Board, Pipeline, Wizard, Command Palette, Comments, Chat | Layout patterns, drag-drop, status workflows |
| Charts & Viz | Charts + V1–V5, NetworkGraph + V1–V2 | Bar, line, pie, radar, org chart, dependency map |
| Code & Real-Time | CodeEditor + V1–V5, Playground, SignalR + V1–V5, Timer + V1–V5, Git Browser, API Keys | Monaco, real-time push, countdown/polling |

**What's missing:** More entity types with forms that collect information, store it in the database, and transform it later. Different data shapes, different form complexity levels, different workflow patterns — things someone at a university could copy and adapt for real projects.

---

## Proposed Categories

### 1. Work Orders (Facilities Maintenance)

**Why it's different:** Status-machine workflow with assignment. Every university has a facilities team — "the toilet on floor 3 is broken" needs to become a tracked, assigned, completed record. Introduces a **multi-party workflow** (requester → dispatcher → technician → closer) that SampleItems doesn't have.

**Data shape:**
- `WorkOrderId` (Guid), `TenantId` (Guid)
- `Title` (string), `Description` (string, multiline)
- `Building` (string — dropdown), `Floor` (string), `RoomNumber` (string)
- `Category` (enum: Plumbing, Electrical, HVAC, Custodial, Grounds, Locksmith, Other)
- `Urgency` (enum: Low, Normal, High, Emergency)
- `Status` (enum: Submitted → Assigned → InProgress → OnHold → Completed → Closed)
- `AssignedTo` (string — worker name/team)
- `RequestedBy` (string), `RequestedDate` (DateTime)
- `CompletedDate` (DateTime?), `CompletionNotes` (string?)
- `EstimatedHours` (decimal?), `ActualHours` (decimal?)
- Audit fields (Added, LastModified, Deleted, etc.)

**Page variants:**
- **Work Orders** — Main CRUD table with status/urgency/building filters, sortable columns
- **V1: Submission Form** — Public-facing form with building/floor/room cascading dropdowns, photo attachment, urgency selector with color-coded descriptions
- **V2: Dispatch Board** — Kanban-style board grouped by status, drag to assign/advance, worker assignment dropdown per card
- **V3: Technician View** — Mobile-friendly card list filtered to "my assignments," swipe-to-complete, timer for hours tracking
- **V4: Metrics Dashboard** — Avg time-to-completion by category, open vs. closed trends, busiest buildings chart, overdue count

**Bootstrap patterns showcased:** Cascading dropdowns, status badge pipeline, responsive card actions, color-coded urgency indicators

---

### 2. Event Registration

**Why it's different:** Date/time-heavy with capacity management. Introduces **RSVP/attendance tracking** — a record that references another record. The core challenge is "how many seats are left" and "who actually showed up," which is a different data transformation than simple CRUD.

**Data shape:**
- `EventId` (Guid), `TenantId` (Guid)
- `Title` (string), `Description` (string, multiline/rich text)
- `EventType` (enum: Workshop, Seminar, Social, Meeting, Training, Conference)
- `Location` (string), `RoomCapacity` (int)
- `StartDate` (DateTime), `EndDate` (DateTime)
- `IsRecurring` (bool), `RecurrencePattern` (string?)
- `RegistrationDeadline` (DateTime?)
- `MaxAttendees` (int), `CurrentRegistrations` (int — computed)
- `RequiresApproval` (bool), `IsPublic` (bool)
- `ContactName` (string), `ContactEmail` (string)
- `Status` (enum: Draft, Published, Full, InProgress, Completed, Cancelled)
- Audit fields

**Attendee sub-record:**
- `AttendeeId`, `EventId`, `Name`, `Email`, `Department`, `DietaryNeeds` (string?), `AccessibilityNeeds` (string?), `Status` (Registered, Waitlisted, Confirmed, Attended, NoShow, Cancelled), `RegisteredDate`, `CheckedInDate`

**Page variants:**
- **Event Registration** — Main list with date range filter, type filter, status badges, upcoming/past toggle
- **V1: Event Detail & RSVP** — Public event page with countdown to start, capacity progress bar, RSVP form with dietary/accessibility fields, waitlist indicator
- **V2: Calendar View** — Month/week/day grid showing events as colored blocks, click to expand detail, filter by event type
- **V3: Attendee Manager** — Table of registrants per event, check-in toggle, bulk email, export to CSV, attendance rate gauge
- **V4: Event Builder** — Multi-section form to create/edit event: basics → schedule → capacity → registration options → notifications, with live preview card

**Bootstrap patterns showcased:** Date/time pickers, progress bars for capacity, calendar grid layout, multi-section accordion forms, countdown components

---

### 3. Equipment Checkout

**Why it's different:** Lending/borrowing lifecycle with due dates and overdue tracking. Introduces a **transaction log** pattern — the same item gets checked out and returned repeatedly, building history. The "is it available right now?" question requires querying current state from transaction history.

**Data shape:**
- `EquipmentId` (Guid), `TenantId` (Guid)
- `Name` (string), `Description` (string?)
- `Category` (enum: Laptop, Projector, Camera, Microphone, Tablet, Hotspot, Adapter, Other)
- `SerialNumber` (string?), `AssetTag` (string)
- `Location` (string — where it lives when not checked out)
- `Condition` (enum: New, Good, Fair, NeedsRepair, Retired)
- `PurchaseDate` (DateTime?), `PurchasePrice` (decimal?)
- `IsAvailable` (bool — computed from checkout history)
- `PhotoUrl` (string?)
- Audit fields

**Checkout sub-record:**
- `CheckoutId`, `EquipmentId`, `BorrowerName`, `BorrowerEmail`, `BorrowerDepartment`, `CheckoutDate` (DateTime), `DueDate` (DateTime), `ReturnDate` (DateTime?), `ConditionAtCheckout` (enum), `ConditionAtReturn` (enum?), `Notes` (string?)

**Page variants:**
- **Equipment Checkout** — Main inventory table with availability filter (Available/Checked Out/In Repair/Retired), category filter, sortable columns
- **V1: Checkout Form** — Borrower lookup (autocomplete by name/email), equipment selector with availability indicator, date picker for due date, condition assessment radio buttons, terms acknowledgment checkbox
- **V2: My Checkouts** — Borrower's view: current checkouts with due-date countdown badges (green/yellow/red), return button, checkout history accordion
- **V3: Overdue Report** — Filtered list of overdue items, days overdue calculated column, borrower contact info, bulk reminder email, sorted by most overdue first
- **V4: Asset Detail** — Single equipment page: photo, specs, full checkout history timeline, condition change log, maintenance notes

**Bootstrap patterns showcased:** Availability badges, countdown-to-due-date indicators, condition radio button groups, before/after comparison, history timeline

---

### 4. Budget Requests (Purchase/Procurement)

**Why it's different:** Multi-line-item forms with calculated totals. Introduces **dynamic row add/remove** inside a form — a parent record with N child line items. Also introduces an **approval chain** (requestor → supervisor → finance) which is a different workflow than the single-status-field approach.

**Data shape:**
- `BudgetRequestId` (Guid), `TenantId` (Guid)
- `Title` (string), `Justification` (string, multiline)
- `Department` (string), `FiscalYear` (string — e.g. "FY2025")
- `RequestedBy` (string), `RequestedDate` (DateTime)
- `Status` (enum: Draft, Submitted, SupervisorApproved, FinanceReview, Approved, Denied, Completed)
- `TotalAmount` (decimal — computed sum of line items)
- `ApprovedAmount` (decimal?)
- `SupervisorName` (string?), `SupervisorApprovedDate` (DateTime?)
- `FinanceReviewerName` (string?), `FinanceDecisionDate` (DateTime?)
- `DenialReason` (string?)
- `AccountCode` (string — GL account)
- Audit fields

**Line item sub-record:**
- `LineItemId`, `BudgetRequestId`, `Description` (string), `Vendor` (string?), `Quantity` (int), `UnitPrice` (decimal), `LineTotal` (decimal — computed), `Category` (enum: Supplies, Equipment, Software, Travel, Services, Other), `Notes` (string?)

**Page variants:**
- **Budget Requests** — Main list with fiscal year filter, status filter, department filter, total column with currency formatting, status badge pipeline
- **V1: Request Builder** — Multi-line item form: add/remove rows, auto-calculated line totals and grand total, vendor autocomplete, GL account picker, justification textarea, running total in sticky footer
- **V2: Approval Queue** — Supervisor/finance view: pending requests sorted by date, expandable to see line items, approve/deny buttons with comment modal, batch approve checkbox
- **V3: Budget Overview** — Department-level summary: requested vs. approved vs. spent, bar chart by category, fiscal year comparison, burn rate gauge
- **V4: Request Detail** — Full view of a single request: line item table, approval timeline (who approved when with comments), attached receipts, print-friendly layout

**Bootstrap patterns showcased:** Dynamic row add/remove, calculated input fields, currency formatting, approval step indicator, sticky footer totals, print stylesheet

---

### 5. Room Reservations

**Why it's different:** Time-slot-based booking with conflict detection. Introduces a **calendar-driven UI** where the primary interaction isn't a form but clicking on a time grid. The key data challenge is "does this overlap with an existing booking?" — a constraint validation that goes beyond simple field-level validation.

**Data shape:**
- `ReservationId` (Guid), `TenantId` (Guid)
- `RoomId` (Guid — FK), `RoomName` (string)
- `Title` (string — what the meeting/event is for)
- `ReservedBy` (string), `Department` (string)
- `StartTime` (DateTime), `EndTime` (DateTime)
- `IsRecurring` (bool), `RecurrenceRule` (string?)
- `AttendeesCount` (int)
- `SetupRequested` (flags/list: Projector, Whiteboard, VideoConference, Catering, Podium)
- `Status` (enum: Confirmed, Tentative, Cancelled)
- `Notes` (string?)
- Audit fields

**Room sub-record:**
- `RoomId`, `Name`, `Building`, `Floor`, `Capacity` (int), `HasProjector` (bool), `HasVideoConference` (bool), `HasWhiteboard` (bool), `IsActive` (bool)

**Page variants:**
- **Room Reservations** — Main list of upcoming reservations, filter by room/building/date, conflict indicators
- **V1: Weekly Calendar** — Week-at-a-glance grid: rooms on Y-axis, hours on X-axis, reservations as colored blocks, click empty slot to book, hover for details popover
- **V2: Booking Form** — Room finder: filter by capacity + equipment needs → available rooms list → select time slot → confirm. Live conflict check on date/time change
- **V3: Room Directory** — All rooms as cards: photo, capacity, equipment icons, today's schedule mini-timeline, "Book Now" button
- **V4: My Reservations** — User's upcoming and past bookings, cancel/edit buttons, recurring series management, iCal export

**Bootstrap patterns showcased:** Time-slot grid, equipment checkbox/toggle group, conflict alert banners, popover detail cards, room capacity badges

---

### 6. Course Evaluations (Surveys)

**Why it's different:** Dynamic form rendering from stored question definitions. Introduces a **form builder/renderer** pattern where the form structure itself is data. The same page renders completely differently depending on which evaluation template is loaded. Aggregated results transform individual responses into statistical summaries.

**Data shape:**
- `EvaluationId` (Guid), `TenantId` (Guid)
- `Title` (string — e.g. "Fall 2025 — CS 101 — Dr. Smith")
- `CourseCode` (string), `CourseName` (string), `InstructorName` (string)
- `Term` (string — e.g. "Fall 2025"), `Department` (string)
- `OpenDate` (DateTime), `CloseDate` (DateTime)
- `IsAnonymous` (bool), `IsOpen` (bool — computed from dates)
- `ResponseCount` (int), `EnrollmentCount` (int)
- `TemplateId` (Guid — which question set to use)
- Audit fields

**Question sub-record:**
- `QuestionId`, `TemplateId`, `QuestionText` (string), `QuestionType` (enum: Likert5, Likert7, MultipleChoice, YesNo, FreeText, Rating10), `IsRequired` (bool), `DisplayOrder` (int), `Options` (string? — JSON for MC choices)

**Response sub-record:**
- `ResponseId`, `EvaluationId`, `SubmittedDate`, `Answers` (list of `{ QuestionId, Value }`)

**Page variants:**
- **Course Evaluations** — Admin list: term/department filter, response rate column with progress bar, open/closed status, bulk open/close
- **V1: Take Evaluation** — Student-facing: renders questions dynamically by type (radio buttons for Likert, textareas for free text, star rating for Rating10), progress indicator, submit with confirmation
- **V2: Results Summary** — Instructor/admin view: per-question averages with horizontal bar charts, distribution histograms, free-text comments list, comparison to department average
- **V3: Template Builder** — Drag-and-drop question ordering, add/remove questions, question type selector, preview rendered form, save as template
- **V4: Response Rate Tracker** — Dashboard showing response rates by course, department rollup, reminder scheduling, completion trends over time

**Bootstrap patterns showcased:** Dynamic form rendering, star rating inputs, horizontal bar charts for Likert averages, drag-and-drop ordering, progress stepper

---

### 7. Scholarship Applications

**Why it's different:** Multi-step wizard with eligibility gating. Introduces a **wizard/stepper pattern** where Step 2 isn't even visible until Step 1 passes validation. Also introduces **review committee scoring** — multiple reviewers independently score the same application, then scores are aggregated for batch decisions.

**Data shape:**
- `ApplicationId` (Guid), `TenantId` (Guid)
- `ScholarshipId` (Guid — which scholarship), `ScholarshipName` (string)
- `ApplicantName` (string), `ApplicantEmail` (string), `StudentId` (string)
- `GPA` (decimal), `Major` (string), `ClassYear` (enum: Freshman, Sophomore, Junior, Senior, Graduate)
- `FinancialNeed` (bool), `IsFirstGeneration` (bool)
- `EssayText` (string, multiline — 500 word max)
- `ResumeFileId` (Guid?), `TranscriptFileId` (Guid?)
- `ReferenceName` (string), `ReferenceEmail` (string), `ReferenceRelationship` (string)
- `Status` (enum: InProgress, Submitted, UnderReview, Awarded, Denied, Withdrawn)
- `SubmittedDate` (DateTime?)
- Audit fields

**Review sub-record:**
- `ReviewId`, `ApplicationId`, `ReviewerName`, `AcademicScore` (int 1-5), `EssayScore` (int 1-5), `NeedScore` (int 1-5), `OverallScore` (int 1-5), `Comments` (string?), `Recommendation` (enum: StrongYes, Yes, Maybe, No), `ReviewedDate`

**Page variants:**
- **Scholarship Applications** — Admin list: scholarship filter, status filter, average score column, sortable by GPA/score/date
- **V1: Application Wizard** — Multi-step form: Eligibility Check (GPA + class year gate) → Personal Info → Essay → Documents (file upload) → References → Review & Submit. Progress bar across top, step validation before advancing
- **V2: Review Portal** — Reviewer interface: assigned applications queue, read-only view of application, scoring rubric form (1-5 per criteria), recommendation dropdown, comments, submit review
- **V3: Committee Dashboard** — All applications with aggregated scores: avg overall, score distribution, reviewer agreement indicator, side-by-side comparison of top candidates, batch award/deny checkboxes
- **V4: Applicant Status** — Applicant's view: application progress stepper, status updates, notification preferences, withdraw button

**Bootstrap patterns showcased:** Multi-step wizard with validation gates, rubric scoring grid, side-by-side comparison layout, progress stepper, file upload integration

---

### 8. Parking Permits

**Why it's different:** Renewal-cycle entity with vehicle sub-records and violation tracking. Introduces **permit generation** (a read-only formatted output from stored data) and **violation/appeal workflow** — a record spawning child dispute records. Also shows seasonal/annual renewal patterns.

**Data shape:**
- `PermitId` (Guid), `TenantId` (Guid)
- `PermitNumber` (string — auto-generated display number like "P-2025-0042")
- `HolderName` (string), `HolderEmail` (string), `HolderType` (enum: Student, Faculty, Staff, Visitor)
- `LotPreference` (string — dropdown of lot names)
- `PermitType` (enum: Annual, Semester, Monthly, Daily, ADA)
- `VehicleMake` (string), `VehicleModel` (string), `VehicleYear` (int), `VehicleColor` (string), `LicensePlate` (string), `State` (string)
- `StartDate` (DateTime), `EndDate` (DateTime)
- `Fee` (decimal), `PaymentStatus` (enum: Pending, Paid, Waived, Refunded)
- `Status` (enum: Active, Expired, Suspended, Revoked, PendingRenewal)
- Audit fields

**Violation sub-record:**
- `ViolationId`, `PermitId`, `ViolationDate`, `ViolationType` (enum: Expired, WrongLot, NoPermit, FireLane, ADAViolation, Other), `FineAmount` (decimal), `Location` (string), `Description` (string?), `Status` (enum: Issued, Appealed, Upheld, Dismissed, Paid), `AppealText` (string?), `AppealDate` (DateTime?), `ResolutionNotes` (string?)

**Page variants:**
- **Parking Permits** — Main list: type/status/lot filters, expiration date highlighting (red if <30 days), payment status badges
- **V1: Permit Application** — Multi-section form: holder info → vehicle info (make/model/year/color/plate) → lot selection with availability indicator → permit type with fee display → payment confirmation
- **V2: My Permit** — Permit holder view: permit card (printable, styled like a real parking pass), vehicle info, renewal button when within 30 days of expiration, violation history
- **V3: Violation Manager** — Enforcement view: issue new violation form, violation list with appeal status, appeal review with approve/dismiss, fine payment tracking
- **V4: Lot Utilization** — Dashboard: permits per lot vs. capacity, revenue by permit type, violation trends, expiring-soon count, renewal rate

**Bootstrap patterns showcased:** Auto-generated display numbers, printable permit card layout, expiration countdown badges, appeal workflow, lot capacity gauges

---

### 9. Help Desk Tickets

**Why it's different:** Threaded conversation with internal vs. public visibility. Introduces **cascading category/subcategory dropdowns**, **SLA timer tracking** (time elapsed since submission), and **canned response templates**. The conversation pattern is fundamentally different from simple CRUD — each ticket accumulates replies over time.

**Data shape:**
- `TicketId` (Guid), `TenantId` (Guid)
- `TicketNumber` (string — auto-generated like "HD-2025-1234")
- `Subject` (string), `Description` (string, multiline)
- `Category` (enum: Hardware, Software, Network, Access, Email, Printing, Other)
- `SubCategory` (string — dependent on Category)
- `Priority` (enum: Low, Medium, High, Critical)
- `Status` (enum: New, Open, AwaitingUser, AwaitingStaff, Resolved, Closed)
- `SubmittedBy` (string), `SubmittedByEmail` (string), `Department` (string)
- `AssignedTo` (string?), `AssignedGroup` (string?)
- `SlaDeadline` (DateTime — computed from priority: Critical=4hrs, High=8hrs, Medium=24hrs, Low=72hrs)
- `ResolvedDate` (DateTime?), `ClosedDate` (DateTime?)
- `SatisfactionRating` (int? — 1-5 after resolution)
- Audit fields

**Reply sub-record:**
- `ReplyId`, `TicketId`, `AuthorName`, `Body` (string), `IsInternal` (bool — staff-only note vs. public reply), `CreatedDate`, `Attachments` (list?)

**Page variants:**
- **Help Desk Tickets** — Main queue: status/priority/category filters, SLA countdown column (green/yellow/red), assigned-to filter, bulk assign
- **V1: Submit Ticket** — User-facing form: category dropdown → subcategory cascading dropdown, priority selector with SLA expectation display, rich text description, file attachment, similar-tickets suggestion (knowledge base)
- **V2: Ticket Detail** — Threaded conversation view: alternating bubbles for user vs. staff replies, internal notes in yellow background (staff only), reply box with canned response dropdown, status change buttons, SLA countdown timer in header
- **V3: Agent Dashboard** — Support staff view: my open tickets, unassigned queue, SLA breaches highlighted, quick-reply from list, satisfaction scores
- **V4: Reports** — Metrics: avg resolution time by category, SLA compliance %, tickets by category pie chart, satisfaction rating distribution, busiest hours heatmap

**Bootstrap patterns showcased:** Cascading dependent dropdowns, chat-bubble conversation threading, internal-note highlighting, SLA countdown timers, satisfaction star rating, canned response selector

---

### 10. Employee Onboarding

**Why it's different:** Checklist/task-completion pattern with multi-party responsibility. Introduces **progress tracking across a set of required tasks** where different people are responsible for different items. The data shape is a parent record (the new hire) with N checklist items, each independently completable. Shows "percentage done" aggregation.

**Data shape:**
- `OnboardingId` (Guid), `TenantId` (Guid)
- `EmployeeName` (string), `EmployeeEmail` (string), `EmployeeTitle` (string)
- `Department` (string), `HireDate` (DateTime), `StartDate` (DateTime)
- `SupervisorName` (string), `MentorName` (string?)
- `EmploymentType` (enum: FullTime, PartTime, Temporary, GradAssistant, StudentWorker)
- `Status` (enum: Pending, InProgress, Completed, Withdrawn)
- `CompletionPercentage` (int — computed from checklist)
- `Notes` (string?)
- Audit fields

**Checklist item sub-record:**
- `ChecklistItemId`, `OnboardingId`
- `TaskName` (string), `Description` (string?)
- `Category` (enum: HR, IT, Facilities, Department, Training, Compliance)
- `AssignedTo` (string — who's responsible: HR, IT, Supervisor, Employee)
- `DueDate` (DateTime?), `CompletedDate` (DateTime?)
- `IsRequired` (bool), `IsCompleted` (bool)
- `CompletedBy` (string?), `Notes` (string?)
- `DisplayOrder` (int)

**Page variants:**
- **Employee Onboarding** — Main list: department filter, status filter, completion % progress bar column, start date sorting, overdue task count badge
- **V1: Onboarding Wizard** — HR creates new onboarding: employee info → select checklist template → customize tasks → assign due dates → activate. Auto-generates standard checklist items
- **V2: Task Tracker** — Checklist view for a single employee: grouped by category (HR / IT / Facilities / Training), checkbox to mark complete, overdue items highlighted red, overall progress bar, responsible party shown per task
- **V3: My Onboarding** — New employee's self-service view: their personal checklist with only their tasks, document upload slots (I-9, W-4, direct deposit form, emergency contacts), training module links, mentor contact card
- **V4: Department Overview** — Manager/HR dashboard: all active onboardings in department, completion heatmap, bottleneck identification (which category has the most overdue items), average days-to-complete

**Bootstrap patterns showcased:** Checklist with progress bars, grouped task categories, completion percentage badges, document collection slots, heatmap-style overview, mentor/contact cards

---

## Summary Grid

| # | Category | Core Data Challenge | Key Form Pattern | Key UI Pattern | Unique Bootstrap Feature |
|---|---|---|---|---|---|
| 1 | Work Orders | Status workflow + assignment | Cascading location dropdowns | Dispatch kanban board | Status badge pipeline |
| 2 | Event Registration | Date/time + capacity constraints | Multi-section event builder | Calendar grid view | Capacity progress bars |
| 3 | Equipment Checkout | Lending lifecycle + availability | Condition assessment radios | Overdue countdown badges | Before/after comparison |
| 4 | Budget Requests | Multi-line items + calculated totals | Dynamic row add/remove | Approval step indicator | Sticky footer totals |
| 5 | Room Reservations | Time-slot conflicts | Equipment checkbox toggles | Weekly time grid | Conflict alert banners |
| 6 | Course Evaluations | Dynamic form from question definitions | Rendered Likert/rating/text inputs | Distribution bar charts | Star rating inputs |
| 7 | Scholarship Applications | Multi-step with eligibility gating | Wizard with validation gates | Rubric scoring grid | Side-by-side comparison |
| 8 | Parking Permits | Renewal cycles + violations | Vehicle info multi-field | Printable permit card | Auto-generated display # |
| 9 | Help Desk Tickets | Threaded conversation + SLA | Cascading category/subcategory | Chat-bubble threading | SLA countdown timer |
| 10 | Employee Onboarding | Checklist completion tracking | Document collection slots | Grouped task progress | Completion heatmap |

## Implementation Notes

Each category follows the established pattern:
- **DataObjects:** New class in `FreeExamples.DataObjects` (like `SampleItem`)
- **Filter DTO:** New filter class extending `Filter` (like `FilterSampleItems`)
- **DataAccess:** New CRUD methods in `FreeExamples.DataAccess` (GetMany/SaveMany/DeleteMany)
- **API Endpoints:** Three endpoints per entity per `copilot-instructions.md`
- **Pages:** Hub page + V1–V4 variants, each with `ExampleNav`, `InfoTip`, `AboutSection`
- **Seed Data:** Realistic sample records generated on first load (like existing SampleItems seed)
- **No external dependencies:** Everything built with Bootstrap 5 + Font Awesome + vanilla Blazor
