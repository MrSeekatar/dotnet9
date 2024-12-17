# .NET 9 Playground <!-- omit in toc -->

- [Aspire](#aspire)
- [.NET 9 Features Explored](#net-9-features-explored)
- [C# 13 Features Explored](#c-13-features-explored)
- [API Endpoints](#api-endpoints)
  - [/api.html UI](#apihtml-ui)
  - [/scalar/v1 UI](#scalarv1-ui)
- [Links](#links)

> [.NET 8 playground](https://github.com/seekatar/dotnet8)<br>
> [.NET 7 playground](https://github.com/seekatar/dotnet7)

This is my annual repo that explores some of the more interesting new .NET and C# features. In addition to some .NET 9 and C# 13 features, I also played with Aspire.

## Aspire

[Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview) is an orchestrator that allows a developer to run multiple apps locally, automatically connecting them together, and adding tons of observability with its own console. It uses containers to do all this magic, so does require Docker Desktop, or some other Docker runtime to be installed locally. It's kinda like Docker Compose on steroids.

The main reason I wanted to play with Aspire was to see how to integrate it with existing backend apps and what effect it has for deploying to Kubernetes. The high level steps I followed were:

1. Create a new backend API using my own dotnet template. For lack of a better name I called it the Box server. (I'd already used Widget and Thingy in other projects)
1. Add a Blazor front end using `dotnet new blazor`
    - Added a new page for calling the Box API CRUD endpoints.
1. Followed the [tutorial](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/add-aspire-existing-app?tabs=unix&pivots=dotnet-cli) to add Aspire to the existing apps:
    - `dotnet new aspire-apphost -o Box.AppHost`
        - update tfm to 9
        - add to solution
    - Add UI and API as project refs to AppHost so can launch them
        - `dotnet add ./Box.AppHost/Box.AppHost.csproj reference ./BoxUI/BoxUI.csproj`
        - `dotnet add ./Box.AppHost/Box.AppHost.csproj reference ./BoxServerApi/BoxServerApi.csproj`
    - dotnet new aspire-servicedefaults -o Box.ServiceDefaults
        - update tfm to 9
        - add to solution
    - add ServiceDefaults ref to both API and UI so can play in Aspire
        - `dotnet add ./BoxUI/BoxUI.csproj reference ./Box.ServiceDefaults/Box.ServiceDefaults.csproj`
        - `dotnet add ./BoxServerApi/BoxServerApi.csproj reference ./Box.ServiceDefaults/Box.ServiceDefaults.csproj`
    - add `builder.AddServiceDefaults();` after `CreateBuilder()` in UI and API
    - Suppress `CS1591` in the API since generated code doesn't add XML comments
    - Edit [Box.AppHost/Program.cs](src/Box.AppHost/Program.cs) to add the API, and UI the refers to the API

At this point I could do `dotnet run` from `src/Box.AppHost` and it will launch its console, the API and the UI. From the console you can see status, messages, logs, and metrics. (Note trying to launch from Rider gave me an error `Unable to get the project output for net8.0` )

I also wanted to see how, if at all, this affected running the API in K8s, since that is the typical deployment environment. I could run the app in Docker or Kubernetes without any problems.

## .NET 9 Features Explored

- slnx files greatly simply sln files. Still a preview feature and no official doc yet, but many blog posts. Rider and VS support it.
  - [dotnet9.slnx](src/dotnet9.slnx)
- [Static Delivery Optimization]([doclink](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-9.0?view=aspnetcore-8.0#static-asset-delivery-optimization)) (doc) adds compression and better caching headers to help browsers.
  - [src/BoxUI/Program.cs](src/BoxUI/Program.cs#L44)
- [Generate OpenAPI documents](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-9.0&tabs=visual-studio) replaces [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) to generate the OpenAPI document at runtime (or buildtime). You can then plug in your own UI such as [SwaggerUI](https://github.com/swagger-api/swagger-ui). I added [Elements](https://github.com/stoplightio/elements) and [Scalar](https://github.com/scalar/scalar).
  - [src/BoxServerApi/Program.cs](src/BoxServerApi/Program.cs#L31)
  - [src/BoxServerApi/wwwroot/api.html](src/BoxServerApi/wwwroot/api.html) for Elements
- SignalR has some enhancements, like polymorphic serialization I didn't use them here.
- [DisableHttpMetricsAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.disablehttpmetricsattribute?view=aspnetcore-9.0) [ASP.NET Core Metrics](https://learn.microsoft.com/en-us/aspnet/core/log-mon/metrics/metrics?view=aspnetcore-9.0)
- [Hybrid Cache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-9.0) (Currently still in preview with first release of .NET 9) combines in-memory and distributed caching.
- LINQ
  - [CountBy](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.countby)
  - [AggregateBy](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.aggregateby)
- [Base64Url](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.text.base64url)
- [OrderedDictionary](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ordereddictionary-2?view=net-9.0) add access to values by index.
- [Guid.CreateVersion7](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/libraries#systemguid) for generating version 7 GUIDs that are time-ordered.

## C# 13 Features Explored

There are handful of new [features](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13) in C# 13. Nothing too dramatic, and I've used a few of them.

- [`params` can take collections](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13#params-collections)
  - [src/BoxServer/Services/BoxProcessor.cs](src/BoxServer/Services/BoxProcessor.cs#L30)
- [New `Lock` object](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13#new-lock-object)
  - [src/BoxServer/Services/BoxProcessor.cs](src/BoxServer/Services/BoxProcessor.cs#L18)
- [`partial` can be used on properties](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13#more-partial-members)
  - [src/BoxServer/Services/BoxProcessor.cs](src/BoxServer/Services/BoxProcessor.cs#L10)
  - [src/BoxServer/Services/BoxProcessorGenerated.cs](src/BoxServer/Services/BoxProcessorGenerated.cs#L10)
- [`field` keyword (preview)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/field)
  - [src/BoxServer/Services/BoxProcessor.cs](src/BoxServer/Services/BoxProcessor.cs#L18)

## API Endpoints

### /api.html UI

A Swagger UI replacement using [Elements](https://github.com/stoplightio/elements)

### /scalar/v1 UI

A Swagger UI replacement using [Scalar](https://github.com/scalar/scalar)

## Links

- [What's new in .NET 9](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/overview)
- [What's new in ASP.NET Core 9.0](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-9.0)
- [What's new in C# 13](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13)
- [NET Aspire overview](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
- [Rider: Support for SLNX Solution Files](https://blog.jetbrains.com/dotnet/2024/10/04/support-for-slnx-solution-files/)
- [What's New for OpenAPI with .NET 9 - blog](https://blog.martincostello.com/whats-new-for-openapi-with-dotnet-9/) with explanation of why and gaps.
- [Refit](https://github.com/reactiveui/refit) an easy-to-use REST client used to call the API from the UI
