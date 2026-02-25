using System.Net.NetworkInformation;

namespace FreeExamples.Client;

public static partial class Helpers
{
    public static Dictionary<string, List<string>> AppIcons {
        get {
            Dictionary<string, List<string>> icons = new Dictionary<string, List<string>> {
                { "fa:fa-solid fa-home", new List<string> { "IconName1", "IconName2" }},
            };

            return icons;
        }
    }

    public static bool AppMethod()
    {
        return true;
    }

    // {{ModuleItemStart:Tags}}
    public static List<DataObjects.Tag> AvailableTagListApp(DataObjects.TagModule? Module, List<Guid> ExcludeTags)
    {
        var output = new List<DataObjects.Tag>();

        if (Module != null) {
            switch (Module) {
                //case DataObjects.TagModule.AppTagType:
                //    output = Model.Tags.Where(x => !ExcludeTags.Contains(x.TagId) && x.UseInAppTagType == true)
                //        .OrderBy(x => x.Name)
                //        .ToList();
                //    break;

                default:
                    break;
            }
        }

        return output;
    }
    // {{ModuleItemEnd:Tags}}

    private static List<string> GetDeletedRecordTypesApp()
    {
        var output = new List<string>();

        // Add any app-specific deleted record types here.

        return output;
    }

    /// <summary>
    /// Gets the deleted records for a specific app type.
    /// </summary>
    /// <param name="deletedRecords">The DeletedRecords object.</param>
    /// <param name="type">The item type.</param>
    /// <returns>A nullable list of DeletedRecordItem objects.</returns>
    public static List<DataObjects.DeletedRecordItem>? GetDeletedRecordsForAppType(DataObjects.DeletedRecords deletedRecords, string type)
    {
        List<DataObjects.DeletedRecordItem>? output = null;

        switch (StringLower(type)) {
            //case "this":
            //    output = deletedRecords.That;
            //    break;

            default:
                break;
        }

        return output;
    }

    /// <summary>
    /// Gets the language tag for deleted records based on the app type.
    /// </summary>
    /// <param name="type">The item type.</param>
    /// <returns>The language tag for the item type.</returns>
    public static string GetDeletedRecordsLanguageTagForAppType(string type)
    {
        string output = String.Empty;

        switch (StringLower(type)) {
            //case "this":
            //    output = "That";
            //    break;

            default:
                break;
        }

        return output;
    }

    public static List<DataObjects.MenuItem> MenuItemsApp {
        get {
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

            return output;
        }
    }

    public static List<DataObjects.MenuItem> MenuItemsAdminApp {
        get {
            // Add any app-specific admin menu items here.
            var output = new List<DataObjects.MenuItem>();

            return output;
        }
    }

    public static async Task ProcessSignalRUpdateApp(DataObjects.SignalRUpdate update)
    {
        // Process any SignalR updates specific to your app here. See the main ProcessSignalRUpdate method for an example in the MainLayout.razor page.

        if (update != null && (update.TenantId == null || update.TenantId == Model.TenantId)) {
            var itemId = update.ItemId;
            string message = update.Message.ToLower();
            var userId = update.UserId;

            switch (update.UpdateType) {
                default:
                    // Since this is called only from the default method in the main handler here,
                    // we can assume that the update type is not recognized by this app.
                    await Helpers.ConsoleLog("Unknown SignalR Update Type Received");
                    break;
            }
        }
    }

    public static async Task ProcessSignalRUpdateAppUndelete(DataObjects.SignalRUpdate update)
    {
        await Task.Delay(0); // Simulate a delay since this method has to be async. This can be removed once you implement your await logic.

        switch (Helpers.StringLower(update.Message)) {
            case "this":
                // Add code to reload your app-specific data based on the undelete type.
                break;
        }
    }

    private async static Task ReloadModelApp(DataObjects.BlazorDataModelLoader? blazorDataModelLoader)
    {
        // Called from the main ReloadModel method in Helpers to load app-specific data.
    }

    private static void UpdateModelDeletedRecordCountsForAppItems(DataObjects.DeletedRecords deletedRecords)
    {
        // Model.DeletedRecordCounts.MyValue = deletedRecords.MyValue.Count();
    }

}
