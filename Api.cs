#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/health", () => "ok")
    .WithName("GetHealth")
    .WithSummary("Checks whether the API is responding.");

app.Run();
