#:package Microsoft.Extensions.Hosting@10.*
#:package Spectre.Console@0.57.2

using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

var builder = Host.CreateApplicationBuilder(args);
var apiBaseUrl = builder.Configuration["Api:BaseUrl"];

if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiBaseAddress))
{
    throw new InvalidOperationException(
        "Configuration value 'Api:BaseUrl' must be an absolute URL.");
}

using var host = builder.Build();
var logger = host.Services
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("App");

using var httpClient = new HttpClient
{
    BaseAddress = apiBaseAddress
};

var appJsonContext = new AppJsonContext(
    new JsonSerializerOptions(JsonSerializerDefaults.Web));

logger.LogDebug("Requesting API data from {ApiBaseUrl}", apiBaseAddress);

try
{
    var health = await httpClient.GetFromJsonAsync(
        "/api/health",
        appJsonContext.HealthResponse)
        ?? throw new InvalidOperationException("The API returned an empty health response.");
    AnsiConsole.MarkupLine($"[green]API health:[/] {Markup.Escape(health.Status)}");

    var productList = await httpClient.GetFromJsonAsync(
        "/api/products",
        appJsonContext.ProductListResponse)
        ?? throw new InvalidOperationException("The API returned an empty product list.");

    var table = new Table();
    table.AddColumn("ID");
    table.AddColumn("Name");
    table.AddColumn("Price");

    foreach (var product in productList.Products)
    {
        table.AddRow(
            product.Id.ToString(CultureInfo.InvariantCulture),
            Markup.Escape(product.Name),
            product.Price.ToString("0.00", CultureInfo.InvariantCulture));
    }

    AnsiConsole.Write(table);
}
catch (HttpRequestException exception)
{
    logger.LogError(exception, "Could not reach the API at {ApiBaseUrl}", apiBaseAddress);
    AnsiConsole.MarkupLine("[red]Could not reach the API.[/]");
    AnsiConsole.MarkupLine("[dim]Check that Api.cs is running and Api:BaseUrl is correct.[/]");
    Environment.ExitCode = 1;
}

/// <summary>Health information returned by the API.</summary>
public sealed record HealthResponse(string Status);

/// <summary>Product information returned by the API.</summary>
public sealed record ProductResponse(int Id, string Name, decimal Price);

/// <summary>A list of products returned by the API.</summary>
public sealed record ProductListResponse(ProductResponse[] Products);

[JsonSerializable(typeof(HealthResponse))]
[JsonSerializable(typeof(ProductListResponse))]
internal partial class AppJsonContext : JsonSerializerContext
{
}
