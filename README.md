# BoxServer API Project <!-- omit in toc -->

This is the repo for the BoxServer project. It was generated initially by the Loyal dotnet template, then adapted to show off .NET 9 and Aspire

## Running the API

### Required Environment Variables

This app uses `IConfiguration` so these values may be in appsettings, shared.appsettings, environment variables, etc. Here is a sample `shared.appsettings.Development.json` file with the minimum values required that you can fill out and put in your "code" directory (be sure not to commit it!)

```json
{

}
```

### Endpoints

These are the endpoints that are available when running this API:

| Endpoint                                   | Description                                                                                |
| ------------------------------------------ | ------------------------------------------------------------------------------------------ |
| https://localhost:44300/api/health/        | Minimal health check, auth required                                                        |
| https://localhost:44300/api/health/details | Health check with details, auth required                                                   |
| https://localhost:44300/api/v1/*           | Base Url for API calls, see the [tests](src/test/integration/ApiTest.cs) for example calls |
| https://localhost:44300/docs/index.html    | Swagger endpoint                                                                           |
| https://localhost:44300/warmup             | Returns 200 if running                                                                     |

### Using run.ps1

This helper PowerShell script has the following commands, which are wrappers around other commands with all their parameters nicely filled out for you. You can run multiple comma-separated tasks. Use tab-completion to see the options and parameters.
<!-- Get-Content ./run.ps1 | Where-Object { $_ -match "^\s+'([\w+-]+)' {" } -->
| Task            | Description                                                              |
| --------------- | ------------------------------------------------------------------------ |
| build           | Builds the project                                                       |
| testUnit        | Runs unit tests                                                          |
| testIntegration | Runs integration tests. The API must be running (see run)                |
| run             | Runs the API synchronously                                               |
| buildDocker     | Builds the docker image                                                  |
| runDocker       | Runs the API in a docker container synchronously, call buildDocker first |
| buildPerses     | Builds the Perses Docker image                                           |
| generate        | Generates the API code from the ./doc/openapi.yaml file                  |
| updateDb        | Runs Perses to update the database schema                                |
| bootstrapDb     | Runs Perses to seed the database                                         |

## Tour of the Repo

<!-- tree -P *.csproj  -I bin -I obj -P *.ps1 -P *.y* -->
```text
.
├── DevOps
│   ├── boxserver-api
│   │   ├── build.yml                               # AzDO Pipeline Yaml for Build
│   │   ├── deploy.yml                              # AzDO Pipeline Yaml for Deployment
│   │   ├── integration-test-steps.yml              # AzDO Pipeline Yaml for Integration test
│   │   ├── values.yaml                             # Helm values file for use in Helm Chart
│   │   └── variables                               # Variable files used in build and deploy yaml
│   │       ├── variables-common.yml
│   │       ├── variables-dev.yml
│   │       ├── variables-development.yml
│   │       ├── variables-preprod.yml
│   │       ├── variables-prod.yml
│   │       └── variables-test.yml
│   └── boxserver-models
│       └── build.yml                               # AzDO Pipeline Yaml for Building NuGet package
├── Scripts                                         # SQL Scripts
│   ├── BoxServer                                    # Perses scripts added to pipeline are artifact for deploy
│   │   └── client
│   ├── int-test-bootstrap                          # integration test bootstrap data
│   │   └── client
│   └── util                                        # helper scripts used by developers
├── doc
│   └── OAS
│       └── openapi.yaml                            # OAS file for generating controllers and models
├── run.ps1                                         # helper for running local tasks, like builds, Perses, deploys, etc.
└── src
    ├── BoxServer                                    # Core of the application
    │   ├── BoxServer.csproj
    │   ├── Interfaces
    │   └── Repositories
    ├── BoxServerApi                                 # ASP.NET API
    │   ├── BoxServerApi.csproj
    │   ├── Controllers
    │   └── Properties
    ├── BoxServerModels                              # Models, mostly generated, could be NuGet for use by other projects
    │   ├── BoxServerModels.csproj
    │   └── ModelsGenerated
    └── test                                        # XUnit tests
        ├── integration                             # integration tests run in integration test pipeline against running app
        │   └── integration.csproj
        └── unit                                    # Unit tests run in build container
            └── unit.csproj
```
