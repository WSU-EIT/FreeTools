using System.Collections.Concurrent;

namespace FreeExamples;

public partial interface IDataAccess
{
    List<DataObjects.SampleItem> GetSampleItems(List<Guid>? ids);
    List<DataObjects.SampleItem> SaveSampleItems(List<DataObjects.SampleItem> items, DataObjects.User? currentUser);
    DataObjects.BooleanResponse DeleteSampleItems(List<Guid>? ids);
    DataObjects.SampleDashboard GetSampleDashboard();
}

public partial class DataAccess
{
    // In-memory store for sample data. Not persisted to DB — reseeds on app restart.
    private static readonly ConcurrentDictionary<Guid, DataObjects.SampleItem> _sampleItems = new();
    private static bool _sampleDataSeeded = false;

    private void SeedSampleDataIfNeeded()
    {
        if (_sampleDataSeeded) return;
        _sampleDataSeeded = true;

        var categories = new[] { "Engineering", "Marketing", "Operations", "Finance", "Support" };
        var statuses = new[] {
            DataObjects.SampleItemStatus.Draft,
            DataObjects.SampleItemStatus.Active,
            DataObjects.SampleItemStatus.Completed,
            DataObjects.SampleItemStatus.Archived,
        };
        var names = new[] {
            "Website Redesign", "Q4 Budget Review", "Server Migration",
            "Onboarding Workflow", "API Documentation", "Security Audit",
            "Mobile App Launch", "Data Pipeline", "CI/CD Setup",
            "Customer Portal", "Email Templates", "Performance Review",
            "Inventory System", "Compliance Report", "Training Materials",
            "Brand Guidelines", "Help Desk Rollout", "Backup Strategy",
            "Dashboard Widgets", "SSO Integration", "Load Testing",
            "Release Notes", "Vendor Evaluation", "Cost Analysis",
            "Network Diagram",
        };

        var rng = new Random(42); // Deterministic seed for repeatable demo data

        for (int i = 0; i < names.Length; i++) {
            var id = Guid.NewGuid();
            var daysAgo = rng.Next(1, 180);
            var added = DateTime.UtcNow.AddDays(-daysAgo);

            _sampleItems[id] = new DataObjects.SampleItem {
                SampleItemId = id,
                TenantId = Guid.Empty,
                Name = names[i],
                Description = $"Sample description for {names[i]}. This is demo data generated on startup.",
                Category = categories[rng.Next(categories.Length)],
                Status = statuses[rng.Next(statuses.Length)],
                Priority = rng.Next(1, 6),
                Amount = Math.Round((decimal)(rng.NextDouble() * 10000), 2),
                Enabled = rng.NextDouble() > 0.2,
                DueDate = rng.NextDouble() > 0.3 ? DateTime.UtcNow.AddDays(rng.Next(-30, 90)) : null,
                Added = added,
                AddedBy = "System",
                LastModified = added.AddDays(rng.Next(0, Math.Max(1, daysAgo / 2))),
                LastModifiedBy = "System",
            };
        }
    }

    // --- GetMany: null/empty → all, IDs → filtered ---
    public List<DataObjects.SampleItem> GetSampleItems(List<Guid>? ids)
    {
        SeedSampleDataIfNeeded();

        if (ids != null && ids.Count > 0) {
            return _sampleItems.Values
                .Where(x => ids.Contains(x.SampleItemId))
                .OrderBy(x => x.Name)
                .ToList();
        }

        return _sampleItems.Values.OrderBy(x => x.Name).ToList();
    }

    // --- SaveMany: PK exists → update, empty/new PK → insert ---
    public List<DataObjects.SampleItem> SaveSampleItems(List<DataObjects.SampleItem> items, DataObjects.User? currentUser)
    {
        SeedSampleDataIfNeeded();

        var saved = new List<DataObjects.SampleItem>();
        string modifiedBy = currentUser?.DisplayName() ?? "Unknown";

        foreach (var item in items) {
            if (item.SampleItemId == Guid.Empty) {
                item.SampleItemId = Guid.NewGuid();
            }

            if (_sampleItems.TryGetValue(item.SampleItemId, out var existing)) {
                // Update
                existing.Name = item.Name;
                existing.Description = item.Description;
                existing.Category = item.Category;
                existing.Status = item.Status;
                existing.Priority = item.Priority;
                existing.Amount = item.Amount;
                existing.Enabled = item.Enabled;
                existing.DueDate = item.DueDate;
                existing.LastModified = DateTime.UtcNow;
                existing.LastModifiedBy = modifiedBy;
                saved.Add(existing);
            } else {
                // Insert
                item.Added = DateTime.UtcNow;
                item.AddedBy = modifiedBy;
                item.LastModified = DateTime.UtcNow;
                item.LastModifiedBy = modifiedBy;
                _sampleItems[item.SampleItemId] = item;
                saved.Add(item);
            }
        }

        return saved;
    }

    // --- DeleteMany: must provide IDs explicitly ---
    public DataObjects.BooleanResponse DeleteSampleItems(List<Guid>? ids)
    {
        SeedSampleDataIfNeeded();

        var output = new DataObjects.BooleanResponse();

        if (ids == null || ids.Count == 0) {
            output.Messages.Add("You must provide IDs to delete. Use GetSampleItems first to find them.");
            return output;
        }

        int deleted = 0;
        foreach (var id in ids) {
            if (_sampleItems.TryRemove(id, out _)) {
                deleted++;
            }
        }

        output.Result = true;
        output.Messages.Add($"Deleted {deleted} item(s).");
        return output;
    }

    // --- Dashboard aggregate ---
    public DataObjects.SampleDashboard GetSampleDashboard()
    {
        SeedSampleDataIfNeeded();

        var items = _sampleItems.Values.ToList();

        var dashboard = new DataObjects.SampleDashboard {
            TotalItems = items.Count,
            ActiveItems = items.Count(x => x.Status == DataObjects.SampleItemStatus.Active),
            CompletedItems = items.Count(x => x.Status == DataObjects.SampleItemStatus.Completed),
            DraftItems = items.Count(x => x.Status == DataObjects.SampleItemStatus.Draft),
            ArchivedItems = items.Count(x => x.Status == DataObjects.SampleItemStatus.Archived),
        };

        dashboard.ByCategory = items
            .GroupBy(x => x.Category ?? "Uncategorized")
            .Select(g => new DataObjects.SampleCategorySummary {
                Category = g.Key,
                Count = g.Count(),
                TotalAmount = g.Sum(x => x.Amount),
            })
            .OrderBy(x => x.Category)
            .ToList();

        dashboard.ByMonth = items
            .GroupBy(x => x.Added.ToString("yyyy-MM"))
            .Select(g => new DataObjects.SampleTimelineSummary {
                Month = g.Key,
                Added = g.Count(),
                Completed = g.Count(x => x.Status == DataObjects.SampleItemStatus.Completed),
            })
            .OrderBy(x => x.Month)
            .ToList();

        return dashboard;
    }
}
