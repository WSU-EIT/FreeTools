# FreeCRM Hook: DataController.App.cs

> Server API controller hooks — custom authentication, API endpoints, SignalR processing.

**File:** `{ProjectName}/Controllers/DataController.App.cs`
**Class:** `partial class DataController`
**Complexity:** MEDIUM — 3 hook methods

---

## Hook Methods

### Authenticate_App

```csharp
private DataObjects.User Authenticate_App()
```

**Called:** From the `DataController` constructor during request initialization.
**Use for:** Custom authentication beyond the standard JWT/cookie auth — API keys, custom tokens, service-to-service auth.

**Suggested tie-in:**

```csharp
// In DataController.App.cs:
private DataObjects.User Authenticate_App()
{
    var output = new DataObjects.User();
    output = MyAuthenticateApp();  // ← tie-in
    return output;
}

// In {ProjectName}.App.DataController.Auth.cs:
private DataObjects.User MyAuthenticateApp()
{
    var output = new DataObjects.User();
    var token = HeaderValue("X-Api-Key");
    if (!String.IsNullOrWhiteSpace(token)) {
        DataObjects.User user = da.ValidateApiKey(token);
        if (user.ActionResponse.Result) {
            output = user;
        }
    }
    return output;
}
```

**Custom file:** `{ProjectName}.App.DataController.Auth.cs`

---

### YourEndpoint (Example Template)

```csharp
[HttpGet]
[Authorize]
[Route("~/api/Data/YourEndpoint/")]
public ActionResult<DataObjects.BooleanResponse> YourEndpoint()
```

**Called:** N/A — this is an example template showing how to add endpoints.
**Use for:** Delete this example and replace with your actual endpoints.

**For new endpoints, use partial classes directly — no hook modification needed:**

```csharp
// In {ProjectName}.App.API.cs (your file — no hook file change):
namespace {ProjectName}.Server.Controllers;

public partial class DataController
{
    [HttpPost("api/Data/GetSourceSystems")]
    public async Task<ActionResult<DataObjects.SourceSystemFilterResult>> GetSourceSystems(
        [FromBody] DataObjects.SourceSystemFilter filter)
    {
        return Ok(await da.GetSourceSystemsAsync(filter));
    }

    [HttpGet("api/Data/GetDashboardStats")]
    [Authorize]
    public async Task<ActionResult<DataObjects.DashboardStats>> GetDashboardStats()
    {
        return Ok(await da.GetDashboardStatsAsync());
    }
}
```

**Custom files:**

| Scenario | File Name |
|----------|-----------|
| General API endpoints | `{ProjectName}.App.API.cs` |
| Feature-specific endpoints | `{ProjectName}.App.API.{Feature}.cs` |
| Large feature set | `{ProjectName}.App.{Feature}Controller.cs` (standalone controller) |

**Real-world:** FreeCICD has 15+ endpoints in `FreeCICD.App.API.cs`. FreeGLBA uses a standalone `GlbaController` + partial `DataController` with 25+ CRUD endpoints in `FreeGLBA.App.DataController.cs`.

---

### SignalRUpdateApp

```csharp
private async Task<bool> SignalRUpdateApp(DataObjects.SignalRUpdate update)
```

**Called:** From `DataController.cs` when a SignalR update is sent.
**Use for:** App-specific server-side SignalR processing — broadcasting to custom groups, filtering updates, adding metadata.

**Suggested tie-in:**

```csharp
// In DataController.App.cs:
private async Task<bool> SignalRUpdateApp(DataObjects.SignalRUpdate update)
{
    bool processedInApp = false;
    processedInApp = await MySignalRUpdateApp(update);  // ← tie-in
    return processedInApp;
}

// In {ProjectName}.App.SignalR.cs:
private async Task<bool> MySignalRUpdateApp(DataObjects.SignalRUpdate update)
{
    bool processedInApp = false;
    if (update.UpdateType == DataObjects.SignalRUpdateType.PipelineLiveStatusUpdate) {
        // Custom broadcasting logic
        await _signalR.Clients.Group("PipelineMonitor").SignalRUpdate(update);
        processedInApp = true;
    }
    return processedInApp;
}
```

**Return `true`** if your app handled the broadcast. The framework will skip its default broadcast logic.
**Return `false`** (default) to let the framework handle it normally.

**Custom file:** `{ProjectName}.App.SignalR.cs`

---

## Suggested File Names

| Scenario | File Name |
|----------|-----------|
| Custom authentication | `{ProjectName}.App.DataController.Auth.cs` |
| General API endpoints | `{ProjectName}.App.API.cs` |
| Feature-grouped endpoints | `{ProjectName}.App.API.{Feature}.cs` |
| Server-side SignalR | `{ProjectName}.App.SignalR.cs` |
| Standalone new controller | `{ProjectName}.App.{Feature}Controller.cs` |

---

*Category: 006_architecture*
*Source: `ReferenceProjects/FreeCRM-main/CRM/Controllers/DataController.App.cs`*
