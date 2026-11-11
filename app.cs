#:package Microsoft.Extensions.Hosting@10.*

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

logger.LogInformation("Checking API health at {ApiBaseUrl}", apiBaseAddress);

try
{
    var health = await httpClient.GetFromJsonAsync(
        "/api/health",
        appJsonContext.HealthResponse)
        ?? throw new InvalidOperationException("The API returned an empty health response.");
    logger.LogInformation("API health: {Health}", health.Status);

    var productList = await httpClient.GetFromJsonAsync(
        "/api/products",
        appJsonContext.ProductListResponse)
        ?? throw new InvalidOperationException("The API returned an empty product list.");

    foreach (var product in productList.Products)
    {
        logger.LogInformation(
            "Product {ProductId}: {ProductName} - {Price}",
            product.Id,
            product.Name,
            product.Price);
    }
}
catch (HttpRequestException exception)
{
    logger.LogError(exception, "Could not reach the API at {ApiBaseUrl}", apiBaseAddress);
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
