#:package Humanizer@2.*
#:package Microsoft.Extensions.Hosting@10.*

using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var date = new DateTime(2020, 1, 1);

Console.WriteLine($"Hello, World! {date.Humanize()}");

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

logger.LogInformation("Checking API health at {ApiBaseUrl}", apiBaseAddress);

try
{
    var health = await httpClient.GetStringAsync("/api/health");
    logger.LogInformation("API health: {Health}", health);
}
catch (HttpRequestException exception)
{
    logger.LogError(exception, "Could not reach the API at {ApiBaseUrl}", apiBaseAddress);
    Environment.ExitCode = 1;
}
