#:sdk Microsoft.NET.Sdk.Web

using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, HealthJsonContext.Default));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/health", () => TypedResults.Ok(new HealthResponse("ok")))
    .WithName("GetHealth")
    .WithSummary("Checks whether the API is responding.")
    .Produces<HealthResponse>();

app.Run();

/// <summary>Health information returned by the API.</summary>
public sealed record HealthResponse(string Status);

[JsonSerializable(typeof(HealthResponse))]
internal partial class HealthJsonContext : JsonSerializerContext
{
}
