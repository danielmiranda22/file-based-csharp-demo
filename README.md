# File-based C# demo

Explore C# file-based apps: small C# programs that can run directly from `.cs` files without a checked-in `.csproj`.

## What this demo does

- `Api.cs` runs a small ASP.NET Core API with health and product endpoints.
- `App.cs` calls the API over HTTP, reads its JSON responses, and displays the results with Spectre.Console.
- Product data is kept in memory, so the demo does not need a database.

The client and API are separate apps. They communicate over HTTP:

```text
App.cs  -- HTTP / JSON -->  Api.cs
```

The `#:sdk` and `#:package` directives at the top of a file declare its SDK and NuGet packages. `dotnet run` builds and runs the file.

## Run it

You need the .NET 10 SDK or later. Open two terminals in the repo folder.

Start the API in the first terminal:

```powershell
dotnet run .\Api.cs
```

Run the client in the second terminal:

```powershell
dotnet run .\App.cs
```

The client reads the API address from `appsettings.json`. The local default is `http://localhost:5000`, so no environment variables are needed.

## Files

- `Api.cs` — API endpoints.
- `App.cs` — console client.
- `appsettings.json` — client configuration.
- `Api.http` — example requests for the API.
