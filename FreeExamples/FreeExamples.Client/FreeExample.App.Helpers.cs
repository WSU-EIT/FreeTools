using System.Net.NetworkInformation;

namespace FreeExamples.Client;

public static partial class Helpers
{
    public static List<DataObjects.MenuItem> MyMenuItemsApp {
        get {
            var output = new List<DataObjects.MenuItem>();

            // ── 1. Dashboard & Data ──
            output.Add(new DataObjects.MenuItem {
                Title = "Dashboard & Data",
                Icon = "DashboardData",
                PageNames = new List<string> { "examplesdashboard", "sampleitems", "editsampleitem", "sampleitemsv1", "sampleitemsv2", "sampleitemsv3", "sampleitemsv4", "sampleitemsv5", "searchautocomplete", "comparisontable", "itemcards" },
                SortOrder = 100,
                url = Helpers.BuildUrl("Examples/Dashboard"),
                AppAdminOnly = false,
                DropdownItems = new List<DataObjects.MenuItem> {
                    MakeSubItem("Sample Items", "sampleitems", "Examples/SampleItems", "Files"),
                    MakeSubItem("V1: Card View", "sampleitemsv1", "Examples/SampleItemsV1"),
                    MakeSubItem("V2: Split Panel", "sampleitemsv2", "Examples/SampleItemsV2"),
                    MakeSubItem("V3: Grouped View", "sampleitemsv3", "Examples/SampleItemsV3"),
                    MakeSubItem("V4: Timeline", "sampleitemsv4", "Examples/SampleItemsV4"),
                    MakeSubItem("V5: Stats Dashboard", "sampleitemsv5", "Examples/SampleItemsV5"),
                    MakeSubItem("Search & Autocomplete", "searchautocomplete", "Examples/SearchAutocomplete"),
                    MakeSubItem("Comparison Table", "comparisontable", "Examples/ComparisonTable"),
                    MakeSubItem("Item Cards", "itemcards", "Examples/ItemCards"),
                },
            });

            // ── 2. Files & Media ──
            output.Add(new DataObjects.MenuItem {
                Title = "Files & Media",
                Icon = "FilesMedia",
                PageNames = new List<string> { "filedemo", "filedemov1", "filedemov2", "filedemov3", "filedemov4", "filedemov5", "filedemov6", "imagegallery", "carousel", "signaturedemo", "signaturev1", "signaturev2", "signaturev3", "signaturev4", "signaturev5" },
                SortOrder = 200,
                url = Helpers.BuildUrl("Examples/FileDemo"),
                AppAdminOnly = false,
                DropdownItems = new List<DataObjects.MenuItem> {
                    MakeSubItem("V1: Profile Photo", "filedemov1", "Examples/FileDemoV1"),
                    MakeSubItem("V2: Document Library", "filedemov2", "Examples/FileDemoV2"),
                    MakeSubItem("V3: Resume Upload", "filedemov3", "Examples/FileDemoV3"),
                    MakeSubItem("V4: Bulk Import", "filedemov4", "Examples/FileDemoV4"),
                    MakeSubItem("V5: Case Attachments", "filedemov5", "Examples/FileDemoV5"),
                    MakeSubItem("V6: Upload Policy", "filedemov6", "Examples/FileDemoV6"),
                    MakeSubItem("Image Gallery", "imagegallery", "Examples/ImageGallery"),
                    MakeSubItem("Carousel", "carousel", "Examples/Carousel"),
                    MakeSubItem("Signature Demo", "signaturedemo", "Examples/SignatureDemo"),
                    MakeSubItem("Sig V1: Job Application", "signaturev1", "Examples/SignatureV1"),
                    MakeSubItem("Sig V2: Doc Acknowledgment", "signaturev2", "Examples/SignatureV2"),
                    MakeSubItem("Sig V3: Upload & Sign", "signaturev3", "Examples/SignatureV3"),
                    MakeSubItem("Sig V4: GLBA Consent", "signaturev4", "Examples/SignatureV4"),
                    MakeSubItem("Sig V5: Contract Signing", "signaturev5", "Examples/SignatureV5"),
                },
            });

            // ── 3. UI Components ──
            output.Add(new DataObjects.MenuItem {
                Title = "UI Components",
                Icon = "UIComponents",
                PageNames = new List<string> { "bootstrapshowcase", "bootstrapv1", "bootstrapv2", "bootstrapv3", "bootstrapv4", "bootstrapv5", "bootstrapv6", "bootstrapv7", "bootstrapv8", "bootstrapv9", "bootstrapv10", "bootstrapv11", "bootstrapv12", "kanbanboard", "statusboard", "pipelinetracker", "wizarddemo", "commandpalette", "commentthread", "chatview" },
                SortOrder = 300,
                url = Helpers.BuildUrl("Examples/BootstrapShowcase"),
                AppAdminOnly = false,
                DropdownItems = new List<DataObjects.MenuItem> {
                    MakeSubItem("V1: Email Builder", "bootstrapv1", "Examples/BootstrapV1"),
                    MakeSubItem("V2: Settings Page", "bootstrapv2", "Examples/BootstrapV2"),
                    MakeSubItem("V3: Error Pages", "bootstrapv3", "Examples/BootstrapV3"),
                    MakeSubItem("V4: User Profile", "bootstrapv4", "Examples/BootstrapV4"),
                    MakeSubItem("V5: Pricing Page", "bootstrapv5", "Examples/BootstrapV5"),
                    MakeSubItem("V6: Filter Panel", "bootstrapv6", "Examples/BootstrapV6"),
                    MakeSubItem("V7: Settings Admin", "bootstrapv7", "Examples/BootstrapV7"),
                    MakeSubItem("V8: Offcanvas Sidebar", "bootstrapv8", "Examples/BootstrapV8"),
                    MakeSubItem("V9: Master-Detail", "bootstrapv9", "Examples/BootstrapV9"),
                    MakeSubItem("V10: Modals & Toasts", "bootstrapv10", "Examples/BootstrapV10"),
                    MakeSubItem("V11: Data Table", "bootstrapv11", "Examples/BootstrapV11"),
                    MakeSubItem("V12: Request Form", "bootstrapv12", "Examples/BootstrapV12"),
                    MakeSubItem("Kanban Board", "kanbanboard", "Examples/KanbanBoard"),
                    MakeSubItem("Status Board", "statusboard", "Examples/StatusBoard"),
                    MakeSubItem("Pipeline Tracker", "pipelinetracker", "Examples/PipelineTracker"),
                    MakeSubItem("Wizard Demo", "wizarddemo", "Examples/WizardDemo"),
                    MakeSubItem("Command Palette", "commandpalette", "Examples/CommandPalette"),
                    MakeSubItem("Comment Thread", "commentthread", "Examples/CommentThread"),
                    MakeSubItem("Chat View", "chatview", "Examples/ChatView"),
                },
            });

            // ── 4. Charts & Visualizations ──
            output.Add(new DataObjects.MenuItem {
                Title = "Charts & Viz",
                Icon = "ChartsViz",
                PageNames = new List<string> { "chartsdashboard", "chartsv1", "chartsv2", "chartsv3", "chartsv4", "chartsv5", "networkgraph", "networkgraphv1", "networkgraphv2" },
                SortOrder = 400,
                url = Helpers.BuildUrl("Examples/ChartsDashboard"),
                AppAdminOnly = false,
                DropdownItems = new List<DataObjects.MenuItem> {
                    MakeSubItem("V1: Sales Analytics", "chartsv1", "Examples/ChartsV1"),
                    MakeSubItem("V2: Enrollment Stats", "chartsv2", "Examples/ChartsV2"),
                    MakeSubItem("V3: System Health", "chartsv3", "Examples/ChartsV3"),
                    MakeSubItem("V4: HR Analytics", "chartsv4", "Examples/ChartsV4"),
                    MakeSubItem("V5: Web Analytics", "chartsv5", "Examples/ChartsV5"),
                    MakeSubItem("Network Graph", "networkgraph", "Examples/NetworkGraph"),
                    MakeSubItem("Net V1: Org Chart", "networkgraphv1", "Examples/NetworkGraphV1"),
                    MakeSubItem("Net V2: Dependency Map", "networkgraphv2", "Examples/NetworkGraphV2"),
                },
            });

            // ── 5. Code & Real-Time ──
            output.Add(new DataObjects.MenuItem {
                Title = "Code & Real-Time",
                Icon = "CodeRealTime",
                PageNames = new List<string> { "codeeditor", "codeeditorv1", "codeeditorv2", "codeeditorv3", "codeeditorv4", "codeeditorv5", "codeplayground", "signalrdemo", "signalrv1", "signalrv2", "signalrv3", "signalrv4", "signalrv5", "timerdemo", "timerv1", "timerv2", "timerv3", "timerv4", "timerv5", "gitbrowser", "apikeydemo" },
                SortOrder = 500,
                url = Helpers.BuildUrl("Examples/CodeEditor"),
                AppAdminOnly = false,
                DropdownItems = new List<DataObjects.MenuItem> {
                    MakeSubItem("V1: SQL Query Builder", "codeeditorv1", "Examples/CodeEditorV1"),
                    MakeSubItem("V2: API Tester", "codeeditorv2", "Examples/CodeEditorV2"),
                    MakeSubItem("V3: Config Editor", "codeeditorv3", "Examples/CodeEditorV3"),
                    MakeSubItem("V4: Diff Viewer", "codeeditorv4", "Examples/CodeEditorV4"),
                    MakeSubItem("V5: Template Engine", "codeeditorv5", "Examples/CodeEditorV5"),
                    MakeSubItem("Code Playground", "codeplayground", "Examples/CodePlayground"),
                    MakeSubItem("SignalR Demo", "signalrdemo", "Examples/SignalRDemo"),
                    MakeSubItem("SR V1: Live Notifications", "signalrv1", "Examples/SignalRV1"),
                    MakeSubItem("SR V2: Online Users", "signalrv2", "Examples/SignalRV2"),
                    MakeSubItem("SR V3: Live Poll", "signalrv3", "Examples/SignalRV3"),
                    MakeSubItem("SR V4: Activity Feed", "signalrv4", "Examples/SignalRV4"),
                    MakeSubItem("SR V5: Live Scoreboard", "signalrv5", "Examples/SignalRV5"),
                    MakeSubItem("Timer Demo", "timerdemo", "Examples/TimerDemo"),
                    MakeSubItem("Timer V1: Pomodoro", "timerv1", "Examples/TimerV1"),
                    MakeSubItem("Timer V2: Session Timeout", "timerv2", "Examples/TimerV2"),
                    MakeSubItem("Timer V3: Auto-Refresh", "timerv3", "Examples/TimerV3"),
                    MakeSubItem("Timer V4: Quiz Timer", "timerv4", "Examples/TimerV4"),
                    MakeSubItem("Timer V5: Event Countdown", "timerv5", "Examples/TimerV5"),
                    MakeSubItem("Git Browser", "gitbrowser", "Examples/GitBrowser"),
                    MakeSubItem("API Key Demo", "apikeydemo", "Examples/ApiKeyDemo"),
                },
            });

            return output;
        }
    }

    private static DataObjects.MenuItem MakeSubItem(string title, string pageName, string route, string? icon = null)
    {
        return new DataObjects.MenuItem {
            Title = title,
            Icon = icon ?? "",
            PageNames = new List<string> { pageName },
            url = Helpers.BuildUrl(route),
            AppAdminOnly = false,
        };
    }

}
