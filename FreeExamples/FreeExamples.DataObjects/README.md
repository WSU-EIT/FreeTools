# FreeExamples.DataObjects

> Shared data transfer objects, request/response models, and caching utilities used by both the server and Blazor WASM client.

**Target:** .NET 10 · **Type:** Class Library

---

## What This Project Contains

| Area | Description |
|------|-------------|
| **DTOs** | Data transfer objects shared between server and client |
| **Request/Response** | Typed API request and response models |
| **Caching** | In-memory caching utilities (`System.Runtime.Caching`) |
| **Enums & Constants** | Shared enumerations and constant values |

---

## Key Dependencies

| Package | Purpose |
|---------|---------|
| `System.Runtime.Caching` | In-memory object caching |

---

## Design Notes

This project has **no dependency** on EF Core, ASP.NET Core, or Blazor — it contains only plain C# classes that can be referenced from any layer. Both the server (`FreeExamples`) and client (`FreeExamples.Client`) reference this project for shared type definitions.

---

*Part of the [FreeExamples](..) suite.*
