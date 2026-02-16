# Page Scan Report

| Field | Value |
|-------|-------|
| URL | https://catalog.wsu.edu/courses/ |
| Title |  |
| Status | ✅ 200 |
| HTML Size | 169.9 KB |
| Screenshots | 1 (65.3 KB) |
| JS Errors | 12 |
| JS Warnings | 2 |
| Auth | none |
| Captured | 2026-02-16T20:11:58.1693129Z |

## JavaScript Errors

- `crit: Microsoft.AspNetCore.Components.WebAssembly.Rendering.WebAssemblyRenderer[100]
      Unhandled exception rendering component: Object reference not set to an instance of an object.
System.NullReferenceException: Object reference not set to an instance of an object.
   at CatalogRewrite.Client.Pages.Catalog.CoursesList.BuildRenderTree(RenderTreeBuilder __builder)
   at Microsoft.AspNetCore.Components.ComponentBase.<.ctor>b__7_0(RenderTreeBuilder builder)
   at Microsoft.AspNetCore.Components.Rendering.ComponentState.RenderIntoBatch(RenderBatchBuilder batchBuilder, RenderFragment renderFragment, Exception& renderFragmentException)`
- `(null)`
- `Unhandled Exception:`
- `System.ArgumentNullException: Value cannot be null. (Parameter 'source')`
- `   at System.Linq.ThrowHelper.ThrowArgumentNullException(ExceptionArgument argument)`
- `   at System.Linq.Enumerable.TryGetFirst[CourseListData](IEnumerable`1 source, Boolean& found)`
- `   at System.Linq.Enumerable.FirstOrDefault[CourseListData](IEnumerable`1 source)`
- `   at CatalogRewrite.Client.Pages.Catalog.Courses.OnInitialized()`
- `   at System.Threading.Tasks.Task.<>c.<ThrowAsync>b__128_1(Object state)`
- `   at System.Threading.QueueUserWorkItemCallbackDefaultContext.Execute()`
- `   at System.Threading.ThreadPoolWorkQueue.Dispatch()`
- `   at System.Threading.ThreadPool.BackgroundJobHandler()`

## Actions

- Screenshot #1: page-loaded (65.3 KB)

## Screenshots

### 1. page-loaded

![page-loaded](01-page-loaded.png)


## Files

- `01-page-loaded.png` — page-loaded (65.3 KB)
- `page.html` — rendered HTML content
- `metadata.json` — machine-readable scan data
- `errors.log` — JavaScript console errors
- `warnings.log` — JavaScript console warnings
- `info.log` — navigation and timing details
- `actions.log` — interactions performed on the page
