using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Portfolio.Web.Content;
using Portfolio.Web.Export;

// Composition root for the portfolio site (ADR-0002).
// STEP 1 of 4: register the services the app needs.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IPortfolioContent, PortfolioContent>();

// STEP 2 of 4: build the app and set up the request pipeline: static files, then MVC routing.
var app = builder.Build();
app.UseStaticFiles();
app.MapControllers();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

// STEP 3 of 4: export mode. `dotnet run -- --export <folder>` starts the app on a free
// local port, saves every page as static HTML into the folder, and exits. This is how
// the site reaches GitHub Pages, which can only serve static files (ADR-0001).
var exportIndex = Array.IndexOf(args, "--export");
if (exportIndex >= 0)
{
    var outputDirectory = exportIndex + 1 < args.Length
        ? args[exportIndex + 1]
        : throw new ArgumentException("--export needs an output folder, for example: --export \"$PWD/dist\"");

    // `dotnet run` starts in the project folder, so a relative path would land in the wrong place.
    if (!Path.IsPathRooted(outputDirectory))
    {
        throw new ArgumentException($"--export needs an absolute folder path, but got '{outputDirectory}'. Use \"$PWD/dist\" from the repository root.");
    }

    app.Urls.Add("http://127.0.0.1:0");
    await app.StartAsync();

    var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First();
    using var client = new HttpClient { BaseAddress = new Uri(address) };
    await StaticExporter.ExportAsync(client, app.Environment.WebRootPath, outputDirectory);
    await app.StopAsync();

    Console.WriteLine($"Exported {StaticExporter.Pages.Count} pages to {Path.GetFullPath(outputDirectory)}");
    return;
}

// STEP 4 of 4: normal mode: serve the site for local development.
app.Run();

/// <summary>
/// Exposes the entry point to the test project so it can host the app in memory.
/// </summary>
public partial class Program;
