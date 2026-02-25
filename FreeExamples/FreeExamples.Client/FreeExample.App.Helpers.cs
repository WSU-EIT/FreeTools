using System.Net.NetworkInformation;

namespace FreeExamples.Client;

public static partial class Helpers
{
    public static List<DataObjects.MenuItem> MyMenuItemsApp {
        get {
            // Add any app-specific top-level menu items here.
            var output = new List<DataObjects.MenuItem>();



            output.Add(new DataObjects.MenuItem {
                Title = "Examples Dashboard",
                Icon = "Home",
                PageNames = new List<string> { "examplesdashboard" },
                SortOrder = 100,
                url = Helpers.BuildUrl("Examples/Dashboard"),
                AppAdminOnly = false,
            });

            output.Add(new DataObjects.MenuItem {
                Title = "Sample Items",
                Icon = "Files",
                PageNames = new List<string> { "sampleitems", "editsampleitem" },
                SortOrder = 200,
                url = Helpers.BuildUrl("Examples/SampleItems"),
                AppAdminOnly = false,
            });

            output.Add(new DataObjects.MenuItem {
                Title = "File Demo",
                Icon = "Files",
                PageNames = new List<string> { "filedemo" },
                SortOrder = 300,
                url = Helpers.BuildUrl("Examples/FileDemo"),
                AppAdminOnly = false,
            });

            output.Add(new DataObjects.MenuItem {
                Title = "Bootstrap Showcase",
                Icon = "Settings",
                PageNames = new List<string> { "bootstrapshowcase" },
                SortOrder = 400,
                url = Helpers.BuildUrl("Examples/BootstrapShowcase"),
                AppAdminOnly = false,
            });

            output.Add(new DataObjects.MenuItem {
                Title = "Charts Dashboard",
                Icon = "Home",
                PageNames = new List<string> { "chartsdashboard" },
                SortOrder = 500,
                url = Helpers.BuildUrl("Examples/ChartsDashboard"),
                AppAdminOnly = false,
            });

            output.Add(new DataObjects.MenuItem {
                Title = "Code Editor",
                Icon = "Settings",
                PageNames = new List<string> { "codeeditor" },
                SortOrder = 600,
                url = Helpers.BuildUrl("Examples/CodeEditor"),
                AppAdminOnly = false,
            });

            output.Add(new DataObjects.MenuItem {
                Title = "SignalR Demo",
                Icon = "Settings",
                PageNames = new List<string> { "signalrdemo" },
                SortOrder = 700,
                url = Helpers.BuildUrl("Examples/SignalRDemo"),
                AppAdminOnly = false,
            });

            output.Add(new DataObjects.MenuItem {
                Title = "Timer Demo",
                Icon = "Settings",
                PageNames = new List<string> { "timerdemo" },
                SortOrder = 800,
                url = Helpers.BuildUrl("Examples/TimerDemo"),
                AppAdminOnly = false,
            });

            output.Add(new DataObjects.MenuItem {
                Title = "Network Graph",
                Icon = "Settings",
                PageNames = new List<string> { "networkgraph" },
                SortOrder = 900,
                url = Helpers.BuildUrl("Examples/NetworkGraph"),
                AppAdminOnly = false,
            });

            output.Add(new DataObjects.MenuItem {
                Title = "Signature Demo",
                Icon = "Settings",
                PageNames = new List<string> { "signaturedemo" },
                SortOrder = 1000,
                url = Helpers.BuildUrl("Examples/SignatureDemo"),
                AppAdminOnly = false,
            });

            output.Add(new DataObjects.MenuItem {
                Title = "Wizard Demo",
                Icon = "Settings",
                PageNames = new List<string> { "wizarddemo" },
                SortOrder = 1100,
                url = Helpers.BuildUrl("Examples/WizardDemo"),
                AppAdminOnly = false,
            });

            output.Add(new DataObjects.MenuItem {
                Title = "Git Browser",
                Icon = "Settings",
                PageNames = new List<string> { "gitbrowser" },
                SortOrder = 1200,
                url = Helpers.BuildUrl("Examples/GitBrowser"),
                AppAdminOnly = false,
            });

            output.Add(new DataObjects.MenuItem {
                Title = "API Key Demo",
                Icon = "Settings",
                PageNames = new List<string> { "apikeydemo" },
                SortOrder = 1300,
                url = Helpers.BuildUrl("Examples/ApiKeyDemo"),
                AppAdminOnly = false,
            });

            return output;
        }
    }

}
