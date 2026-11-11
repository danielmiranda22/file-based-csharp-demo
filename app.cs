#:package Humanizer@2.*

using Humanizer;

var date = new DateTime(2020, 1, 1);

Console.WriteLine($"Hello, World! {date.Humanize()}");

using var httpClient = new HttpClient
{
    BaseAddress = new Uri("http://localhost:5000")
};

try
{
    var health = await httpClient.GetStringAsync("/api/health");
    Console.WriteLine($"API health: {health}");
}
catch (HttpRequestException exception)
{
    Console.Error.WriteLine($"Could not reach the API: {exception.Message}");
    Environment.ExitCode = 1;
}
