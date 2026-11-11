#:sdk Microsoft.NET.Sdk.Web

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApiJsonContext.Default));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/health", () => TypedResults.Ok(new HealthResponse("ok")))
    .WithName("GetHealth")
    .WithSummary("Checks whether the API is responding.")
    .Produces<HealthResponse>();

var products = new[]
{
    new ProductResponse(1, "Keyboard", 49.99m),
    new ProductResponse(2, "Mouse", 24.99m)
};

app.MapGet("/api/products", () => TypedResults.Ok(new ProductListResponse(products)))
    .WithName("GetProducts")
    .WithSummary("Returns the demo products.")
    .Produces<ProductListResponse>();

app.MapGet("/api/products/{id:int}", Results<Ok<ProductResponse>, NotFound> (int id) =>
    {
        var product = products.FirstOrDefault(product => product.Id == id);
        return product is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(product);
    })
    .WithName("GetProductById")
    .WithSummary("Returns a product by ID.")
    .Produces<ProductResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.Run();

/// <summary>Health information returned by the API.</summary>
public sealed record HealthResponse(string Status);

/// <summary>Product information returned by the API.</summary>
public sealed record ProductResponse(int Id, string Name, decimal Price);

/// <summary>A list of products returned by the API.</summary>
public sealed record ProductListResponse(ProductResponse[] Products);

[JsonSerializable(typeof(HealthResponse))]
[JsonSerializable(typeof(ProductResponse))]
[JsonSerializable(typeof(ProductListResponse))]
internal partial class ApiJsonContext : JsonSerializerContext
{
}
