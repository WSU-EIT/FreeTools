using System.Net.NetworkInformation;

namespace FreeExamples.Client;

public static partial class Helpers
{
    public static Dictionary<string, List<string>> AppIcons {
        get {
            Dictionary<string, List<string>> icons = new Dictionary<string, List<string>> {
                // Dashboard & Data
                { "fa:fa-solid fa-table-columns",      new List<string> { "ExamplesDashboard", "DashboardData" }},
                { "fa:fa-solid fa-list",                new List<string> { "SampleItems", "EditSampleItem" }},
                { "fa:fa-solid fa-grip",                new List<string> { "SampleItemsV1" }},
                { "fa:fa-solid fa-columns",             new List<string> { "SampleItemsV2" }},
                { "fa:fa-solid fa-layer-group",         new List<string> { "SampleItemsV3", "ItemCards" }},
                { "fa:fa-solid fa-timeline",            new List<string> { "SampleItemsV4" }},
                { "fa:fa-solid fa-chart-bar",           new List<string> { "SampleItemsV5" }},
                { "fa:fa-solid fa-magnifying-glass",    new List<string> { "SearchAutocomplete" }},
                { "fa:fa-solid fa-table-cells",         new List<string> { "ComparisonTable", "StatusBoard" }},

                // Files & Media
                { "fa:fa-solid fa-file-arrow-up",       new List<string> { "FileDemo", "FilesMedia" }},
                { "fa:fa-solid fa-camera",              new List<string> { "FileDemoV1" }},
                { "fa:fa-solid fa-folder-open",         new List<string> { "FileDemoV2" }},
                { "fa:fa-solid fa-file-lines",          new List<string> { "FileDemoV3" }},
                { "fa:fa-solid fa-file-import",         new List<string> { "FileDemoV4" }},
                { "fa:fa-solid fa-paperclip",           new List<string> { "FileDemoV5" }},
                { "fa:fa-solid fa-shield-halved",       new List<string> { "FileDemoV6" }},
                { "fa:fa-solid fa-images",              new List<string> { "ImageGallery" }},
                { "fa:fa-solid fa-film",                new List<string> { "Carousel" }},
                { "fa:fa-solid fa-signature",           new List<string> { "SignatureDemo" }},
                { "fa:fa-solid fa-file-signature",      new List<string> { "SignatureV1" }},
                { "fa:fa-solid fa-file-contract",       new List<string> { "SignatureV2" }},
                { "fa:fa-solid fa-cloud-arrow-up",      new List<string> { "SignatureV3" }},
                { "fa:fa-solid fa-scale-balanced",      new List<string> { "SignatureV4" }},
                { "fa:fa-solid fa-stamp",               new List<string> { "SignatureV5" }},

                // UI Components
                { "fa:fa-solid fa-palette",             new List<string> { "BootstrapShowcase", "UIComponents" }},
                { "fa:fa-solid fa-envelope-open-text",  new List<string> { "BootstrapV1" }},
                { "fa:fa-solid fa-gear",                new List<string> { "BootstrapV2" }},
                { "fa:fa-solid fa-triangle-exclamation", new List<string> { "BootstrapV3" }},
                { "fa:fa-solid fa-id-card",             new List<string> { "BootstrapV4" }},
                { "fa:fa-solid fa-tag",                 new List<string> { "BootstrapV5" }},
                { "fa:fa-solid fa-filter",              new List<string> { "BootstrapV6" }},
                { "fa:fa-solid fa-gears",               new List<string> { "BootstrapV7" }},
                { "fa:fa-solid fa-bars-staggered",      new List<string> { "BootstrapV8" }},
                { "fa:fa-solid fa-pen-to-square",       new List<string> { "BootstrapV9" }},
                { "fa:fa-solid fa-window-restore",      new List<string> { "BootstrapV10" }},
                { "fa:fa-solid fa-table",               new List<string> { "BootstrapV11" }},
                { "fa:fa-solid fa-file-pen",            new List<string> { "BootstrapV12" }},
                { "fa:fa-solid fa-th-large",            new List<string> { "KanbanBoard" }},
                { "fa:fa-solid fa-bars-progress",       new List<string> { "PipelineTracker" }},
                { "fa:fa-solid fa-hat-wizard",          new List<string> { "WizardDemo" }},
                { "fa:fa-solid fa-terminal",            new List<string> { "CommandPalette" }},
                { "fa:fa-solid fa-comments",            new List<string> { "CommentThread" }},
                { "fa:fa-solid fa-message",             new List<string> { "ChatView" }},

                // Charts & Viz
                { "fa:fa-solid fa-chart-pie",           new List<string> { "ChartsDashboard", "ChartsViz" }},
                { "fa:fa-solid fa-chart-line",          new List<string> { "ChartsV1" }},
                { "fa:fa-solid fa-chart-area",          new List<string> { "ChartsV2" }},
                { "fa:fa-solid fa-heartbeat",           new List<string> { "ChartsV3" }},
                { "fa:fa-solid fa-users",               new List<string> { "ChartsV4" }},
                { "fa:fa-solid fa-globe",               new List<string> { "ChartsV5" }},
                { "fa:fa-solid fa-diagram-project",     new List<string> { "NetworkGraph" }},
                { "fa:fa-solid fa-sitemap",             new List<string> { "NetworkGraphV1" }},
                { "fa:fa-solid fa-share-nodes",         new List<string> { "NetworkGraphV2" }},

                // Code & Real-Time
                { "fa:fa-solid fa-code",                new List<string> { "CodeEditor", "CodeRealTime" }},
                { "fa:fa-solid fa-database",            new List<string> { "CodeEditorV1" }},
                { "fa:fa-solid fa-vial",                new List<string> { "CodeEditorV2" }},
                { "fa:fa-solid fa-file-code",           new List<string> { "CodeEditorV3" }},
                { "fa:fa-solid fa-code-compare",        new List<string> { "CodeEditorV4" }},
                { "fa:fa-solid fa-wand-magic-sparkles", new List<string> { "CodeEditorV5" }},
                { "fa:fa-solid fa-laptop-code",         new List<string> { "CodePlayground" }},
                { "fa:fa-solid fa-tower-broadcast",     new List<string> { "SignalRDemo" }},
                { "fa:fa-solid fa-bell",                new List<string> { "SignalRV1" }},
                { "fa:fa-solid fa-user-group",          new List<string> { "SignalRV2" }},
                { "fa:fa-solid fa-square-poll-vertical", new List<string> { "SignalRV3" }},
                { "fa:fa-solid fa-rss",                 new List<string> { "SignalRV4" }},
                { "fa:fa-solid fa-trophy",              new List<string> { "SignalRV5" }},
                { "fa:fa-solid fa-stopwatch",           new List<string> { "TimerDemo" }},
                { "fa:fa-solid fa-tomato",              new List<string> { "TimerV1" }},
                { "fa:fa-solid fa-hourglass-half",      new List<string> { "TimerV2" }},
                { "fa:fa-solid fa-rotate",              new List<string> { "TimerV3" }},
                { "fa:fa-solid fa-clock",               new List<string> { "TimerV4" }},
                { "fa:fa-solid fa-calendar-day",        new List<string> { "TimerV5" }},
                { "fa:fa-solid fa-code-branch",         new List<string> { "GitBrowser" }},
                { "fa:fa-solid fa-key",                 new List<string> { "ApiKeyDemo" }},
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
            // Add any app-specific top-level menu items here.
            var output = new List<DataObjects.MenuItem>();

            output.AddRange(Helpers.MyMenuItemsApp);

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
                case DataObjects.SignalRUpdateType.SampleItemSaved:
                case DataObjects.SignalRUpdateType.SampleItemDeleted:
                    // Let page-level SignalR subscribers handle the update.
                    // The framework calls Model.SignalRUpdate(update) after this method.
                    break;
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
