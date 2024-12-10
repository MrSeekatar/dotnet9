# .NET 9 Playground <!-- omit in toc -->

- [Aspire](#aspire)
- [.NET 9 Features Explored](#net-9-features-explored)
- [C# 13 Features Explored](#c-13-features-explored)
- [Running the app](#running-the-app)
- [API Endpoints](#api-endpoints)
  - [/api.html UI](#apihtml-ui)
  - [/scalar/v1 UI](#scalarv1-ui)
- [Links](#links)

This is my annual repo that explores some of the more interesting new .NET and C# features. This year is a bit lighter on new features, but I took this opportunity to play with Aspire.

## Aspire

[Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview) is an orchestrator that allows a developer to run multiple apps locally, automatically connecting them together, and adding tons of observability with its own console. It uses containers to do all this magic, so does require Docker Desktop or some other Docker runtime to be installed locally. It's kinda like Docker Compose on steroids.

The main reason I wanted to play with Aspire was to see how to integrate it with an existing backend app and what effect it has for deploying to Kubernetes. The high level steps I followed were:

1. Create a new backend API using my own dotnet template. For lack of a better name I called it the Box server. (I'd already used Widget and Thingy in other projects)
1. Add a Blazor front end using `dotnet new blazor`
    - Added a new page for calling the Box API CRUD endpoints.
    - Added a new page for SignalR testing.
1. Followed the [tutorial](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/add-aspire-existing-app?tabs=unix&pivots=dotnet-cli) to add Aspire to the existing apps which was:
    - `sudo dotnet workload uninstall aspire` # remove the old version since 9 doesn't use it3x
    - `dotnet new install Aspire.ProjectTemplates` # install latest Aspire 9 templates
    - `dotnet new aspire-apphost -o Box.AppHost`
        - add to solution
    - Add UI and API as project refs to `Box.AppHost` so can launch them
        - `dotnet add ./Box.AppHost/Box.AppHost.csproj reference ./BoxUI/BoxUI.csproj`
        - `dotnet add ./Box.AppHost/Box.AppHost.csproj reference ./BoxServerApi/BoxServerApi.csproj`
    - `dotnet new aspire-servicedefaults -o Box.ServiceDefaults`
        - add to solution
    - add `Box.ServiceDefaults` ref to both API and UI so they can play in Aspire
        - `dotnet add ./BoxUI/BoxUI.csproj reference ./Box.ServiceDefaults/Box.ServiceDefaults.csproj`
        - `dotnet add ./BoxServerApi/BoxServerApi.csproj reference ./Box.ServiceDefaults/Box.ServiceDefaults.csproj`
    - add `builder.AddServiceDefaults();` after `CreateBuilder()` in UI and API
    - Suppress `CS1591` in the API since generated code doesn't add XML comments
    - Edit [Box.AppHost/Program.cs](src/Box.AppHost/Program.cs) to add the API, and UI the refers to the API

At this point I could do `dotnet run` from `src/Box.AppHost` and it will launch its console, the API and the UI. From the console you can see status, messages, logs, and metrics. (Note trying to launch from Rider gave me an error `Unable to get the project output for net8.0` )

I also wanted to see how, if at all, this affected running the API in K8s, since that is my typical deployment environment at work. I could run the app in Docker or Kubernetes without any problems.

## .NET 9 Features Explored

Here are some of the more interesting new features in [.NET 9](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/overview) and [ASP.NET 9](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-9.0)

- slnx files greatly simply sln files. Still a preview feature and no official doc yet, but many blog posts. Rider and VS support it.
  - [dotnet9.slnx](src/dotnet9.slnx)
- [Static Delivery Optimization]([doclink](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-9.0?view=aspnetcore-8.0#static-asset-delivery-optimization)) adds compression and better caching headers to help browsers.
  - [src/BoxUI/Program.cs](src/BoxUI/Program.cs)
- [Generate OpenAPI documents](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-9.0&tabs=visual-studio) replaces [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) to generate the OpenAPI document at runtime (or buildtime). You can then plug in your own UI such as [SwaggerUI](https://github.com/swagger-api/swagger-ui). I added [Elements](https://github.com/stoplightio/elements) and [Scalar](https://github.com/scalar/scalar).
  - [src/BoxServerApi/Program.cs](src/BoxServerApi/Program.cs)
  - [src/BoxServerApi/wwwroot/api.html](src/BoxServerApi/wwwroot/api.html) for Elements
- [Hybrid Cache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-9.0) (Currently still in preview with first release of .NET 9) combines in-memory and distributed caching.
  - [src/BoxServer/Repositories/BoxRepository.cs](src/BoxServer/Repositories/BoxRepository.cs)
  - [src/BoxServerApi/Program.cs](src/BoxServerApi/Program.cs) Redis setup
- [Polymorphic type support in SignalR Hubs](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-9.0?view=aspnetcore-9.0#polymorphic-type-support-in-signalr-hubs)
  - [src/BoxUI/Components/Pages/Messages.razor](src/BoxUI/Components/Pages/Messages.razor)
  - [src/BoxServerApi/Models/Message.cs](src/BoxServerApi/Models/Message.cs)
  - [src/BoxServerApi/Controllers/BoxController.cs](src/BoxServerApi/Controllers/BoxController.cs)

I have samples of these in the [Notebook](dotnet9.dib)

- [LINQ](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/libraries#linq)
  - [CountBy](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.countby)
  - [AggregateBy](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.aggregateby)
- [Base64Url](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/libraries#base64url)
- [OrderedDictionary](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/libraries#ordereddictionarytkey-tvalue) add access to values by index.
- [Guid.CreateVersion7](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/libraries#systemguid) for generating version 7 GUIDs that are time-ordered.

## C# 13 Features Explored

There are handful of new [features](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13) in C# 13. Nothing too dramatic, and I've used a few of them.

- [`params` can take collections](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13#params-collections)
  - [src/BoxServer/Services/BoxProcessor.cs](src/BoxServer/Services/BoxProcessor.cs)
- [New `Lock` object](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13#new-lock-object)
  - [src/BoxServer/Services/BoxProcessor.cs](src/BoxServer/Services/BoxProcessor.cs)
- [`partial` can be used on properties](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13#more-partial-members)
  - [src/BoxServer/Services/BoxProcessor.cs](src/BoxServer/Services/BoxProcessor.cs)
  - [src/BoxServer/Services/BoxProcessorGenerated.cs](src/BoxServer/Services/BoxProcessorGenerated.cs)
- [`field` keyword (preview)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/field)
  - [src/BoxServer/Services/BoxProcessor.cs](src/BoxServer/Services/BoxProcessor.cs)

## Running the app

You do need Redis running for the API to start up. I run it locally in Docker. `appsettings.json` has the connection string for Redis, which is where Aspire seems to want it. I did try having Aspire start it up in AppHost, but it would timeout.

To run the Aspire app do `dotnet run` from `src/Box.AppHost`. Or use the helper script:

```powershell
./run.ps1 runAppHost
```

It will log a message with the URL to open in the browser.

```plaintext
info: Aspire.Hosting.DistributedApplication[0]
      Login to the dashboard at https://localhost:17264/login?t=9c67f29218ad2e0805247cfd890ff35a
```

From the console you can see the status of the API and UI, and click on the links to open the UI. To see caching in action, you can get the list or individual Box by id and the first time it will log a message about cache miss. If you add, update, or delete a Box it invalidates the cache for the list. If you restart the app without restarting Redis, the items will still be in the cache, but out-of-sync with the faked out database in the repository.

## API Endpoints

In addition to the CRUD endpoint for Boxes that are called by the UI, there are two Swagger UI replacements that show the API endpoints.

### /api.html UI

A Swagger UI replacement using [Elements](https://github.com/stoplightio/elements)

### /scalar/v1 UI

A Swagger UI replacement using [Scalar](https://github.com/scalar/scalar)

## Links

Previous playgrounds:

- [.NET 8 playground](https://github.com/seekatar/dotnet8)
- [.NET 7 playground](https://github.com/seekatar/dotnet7)

Other links:

- [What's new in .NET 9](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/overview)
- [What's new in ASP.NET Core 9.0](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-9.0)
- [What's new in C# 13](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13)
- [NET Aspire overview](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
- [Rider: Support for SLNX Solution Files](https://blog.jetbrains.com/dotnet/2024/10/04/support-for-slnx-solution-files/)
- [What's New for OpenAPI with .NET 9 - blog](https://blog.martincostello.com/whats-new-for-openapi-with-dotnet-9/) with explanation of why and gaps.
- [Refit](https://github.com/reactiveui/refit) an easy-to-use REST client used to call the API from the UI
