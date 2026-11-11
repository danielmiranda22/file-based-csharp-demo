# File-based C# demo

This repository is a small example of running C# directly from `.cs` files, without creating a `.csproj` project.

## What we are building

There are two independent apps:

- `Api.cs` is an ASP.NET Core API. It exposes `GET /api/health`.
- `App.cs` is a console app. It calls the API over HTTP and reads its JSON response.

```text
App.cs  -- HTTP / JSON -->  Api.cs
```

Keeping them separate shows how a client app talks to a real server without a direct project reference.

## How file-based C# works

The directives at the top of each file tell .NET what the app needs:

- `#:sdk` selects the SDK for the file.
- `#:package` adds a NuGet package.

`dotnet run` builds and runs the file. This demo needs the .NET 10 SDK or later.

## Run the demo

Open two terminals in the repository folder.

In the first terminal, start the API:

```powershell
dotnet run .\Api.cs
```

In the second terminal, run the client:

```powershell
dotnet run .\App.cs
```

The client should log `API health: ok`.

You can also find example requests for both API routes in `Api.http`.

## Configuration and logging

`App.cs` uses the .NET Generic Host. It provides logging and loads the API address from `appsettings.json`.

The default address is `http://localhost:5000`. If you change the API port, set the matching environment variables in the two terminals:

```powershell
# API terminal
$env:ASPNETCORE_URLS = "http://localhost:5001"
dotnet run .\Api.cs
```

```powershell
# Client terminal
$env:Api__BaseUrl = "http://localhost:5001"
dotnet run .\App.cs
```

Environment variables override values from `appsettings.json`. You do not need to set them when using the default address.

## Demo steps

1. Create a C# app in a single file.
2. Create a minimal API.
3. Add a health endpoint.
4. Make the client call the API.
5. Load configuration and add logging.
6. Return JSON from the API and read it in the client.

`HealthJsonContext` and `AppJsonContext` provide JSON metadata for the API and client. Each app keeps its own types because they communicate over HTTP, not through shared C# types.
