using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeExamples.Server.Controllers;

/// <summary>
/// Three-endpoint CRUD pattern for SampleData: GetMany, SaveMany, DeleteMany.
/// Plus a dashboard aggregate endpoint shared across demo pages.
/// </summary>
public partial class DataController
{
    /// <summary>
    /// GetMany: POST null/empty list → all items, or list of IDs → matching items.
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("~/api/Data/GetSampleItems")]
    public ActionResult<List<DataObjects.SampleItem>> GetSampleItems(List<Guid>? ids)
    {
        return Ok(da.GetSampleItems(ids));
    }

    /// <summary>
    /// SaveMany: POST a list of items. If SampleItemId exists → update, otherwise → insert.
    /// Caller may provide their own ID or leave it empty for auto-generation.
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("~/api/Data/SaveSampleItems")]
    public ActionResult<List<DataObjects.SampleItem>> SaveSampleItems(List<DataObjects.SampleItem> items)
    {
        return Ok(da.SaveSampleItems(items, CurrentUser));
    }

    /// <summary>
    /// DeleteMany: POST a list of IDs to delete. Empty/null → error.
    /// To delete all: GetSampleItems([]) first, then pass IDs.
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("~/api/Data/DeleteSampleItems")]
    public ActionResult<DataObjects.BooleanResponse> DeleteSampleItems(List<Guid>? ids)
    {
        return Ok(da.DeleteSampleItems(ids));
    }

    /// <summary>
    /// Dashboard aggregate data — status counts, category breakdowns, timeline.
    /// Shared across dashboard, charts, and reporting demo pages.
    /// </summary>
    [HttpGet]
    [Authorize]
    [Route("~/api/Data/GetSampleDashboard")]
    public ActionResult<DataObjects.SampleDashboard> GetSampleDashboard()
    {
        return Ok(da.GetSampleDashboard());
    }
}
