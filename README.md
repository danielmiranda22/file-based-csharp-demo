# File-based C# demo

This repo is a small example of running C# directly from `.cs` files, without creating a `.csproj` project.

## What we are building

There are two independent apps:

- `Api.cs` is an ASP.NET Core API. It exposes `GET /api/health` and `GET /api/products`.
- `App.cs` is a console app. It calls the API over HTTP, reads its JSON responses, and displays them with Spectre.Console.

```text
App.cs  -- HTTP / JSON -->  Api.cs
```

Keeping them separate shows how a client app talks to a real server without a direct project reference.
The product list is kept in memory to keep this first example small; no database is needed.

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

The client should show the API health and a table with the demo products.

You can also find example requests for the API endpoints in `Api.http`.

## Configuration and logging

`App.cs` uses the .NET Generic Host. It provides logging and loads the API address from `appsettings.json`.

The API listens at `http://localhost:5000` by default, and `appsettings.json` gives the client the same address. No environment variables are needed for the local demo.

For another environment, environment variables can override these defaults: `ASPNETCORE_URLS` changes where the API listens, and `Api__BaseUrl` overrides `Api:BaseUrl` for the client. The two addresses must match.

## Demo steps

1. Create a C# app in a single file.
2. Create a minimal API.
3. Add a health endpoint.
4. Make the client call the API.
5. Load configuration and add logging.
6. Return JSON from the API and read it in the client.
7. Return demo products and display them in the client.
8. Use Spectre.Console to present the results in the terminal.

`ApiJsonContext` and `AppJsonContext` provide JSON metadata for the API and client. Each app keeps its own types because they communicate over HTTP, not through shared C# types.
