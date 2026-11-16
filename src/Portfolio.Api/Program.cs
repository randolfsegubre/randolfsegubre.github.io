using Portfolio.Api.Export;
using Portfolio.Business;
using Portfolio.Business.Content;
using Portfolio.Business.Services;

// Presentation layer of the portfolio's .NET backend (ADR-0011, ADR-0012, ADR-0013). The front end is the Nuxt app in
// /web; this app serves the content as JSON and runs the build commands. It talks only to the business layer: it
// never names a table, a repository or a connection. It has two ways to run: build commands that do a job and exit,
// and a web server.

// STEP 1 of 5: register the services. The connection string comes from configuration (the environment variable
// ConnectionStrings__Portfolio, or user secrets) and is never kept in the repository; it is only looked up when the
// database is first used, so a test that swaps in its own content source needs none.
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
builder.Services.AddPortfolioContent(services =>
    services.GetRequiredService<IConfiguration>().GetConnectionString("Portfolio")
        ?? throw new InvalidOperationException("No connection string. Set ConnectionStrings__Portfolio, for example: Server=localhost;Database=PortfolioSite;Integrated Security=true;TrustServerCertificate=true"));
var app = builder.Build();

// STEP 2 of 5: build commands. They can be combined and always run in this order: --migrate creates or updates the
// database, --seed <folder> makes the database say what the JSON files in the folder say, and --export-content <file>
// writes the content document the Nuxt build reads (the same one GET /api/content serves) without starting a web
// server, because GitHub Pages can only serve static files (ADR-0001). Folders and files must be absolute paths, since
// `dotnet run` starts in the project folder and a relative path would land somewhere surprising.
var migrate = args.Contains("--migrate");
var seedFolder = ValueOf("--seed");
var exportFile = ValueOf("--export-content");

if (migrate || seedFolder is not null || exportFile is not null)
{
    await using var scope = app.Services.CreateAsyncScope();

    if (migrate)
    {
        await scope.ServiceProvider.GetRequiredService<IContentMaintenance>().MigrateAsync();
        Console.WriteLine("The content database is up to date.");
    }

    if (seedFolder is not null)
    {
        await scope.ServiceProvider.GetRequiredService<IContentMaintenance>().SeedFromFilesAsync(seedFolder);
        Console.WriteLine($"Seeded the content database from {seedFolder}");
    }

    if (exportFile is not null)
    {
        var content = await scope.ServiceProvider.GetRequiredService<IContentSource>().LoadAsync();
        await ContentExporter.WriteAsync(content, exportFile, DateTime.UtcNow.Year);
        Console.WriteLine($"Wrote the content to {Path.GetFullPath(exportFile)}");
    }

    return;
}

// STEP 3 of 5: web server mode. Read the content once, now, so a database that is missing or empty stops the app
// with a clear message instead of failing the first visitor. After this the database is not touched again.
IPortfolioContent siteContent;
await using (var scope = app.Services.CreateAsyncScope())
{
    siteContent = await scope.ServiceProvider.GetRequiredService<IContentSource>().LoadAsync(app.Lifetime.ApplicationStopping);
}

// STEP 4 of 5: security. Outside development, tell browsers to use HTTPS only (HSTS) and send them there. The
// API only ever returns JSON, so every response also carries headers that forbid sniffing the type, forbid being
// framed, and allow nothing to load from it, and the referrer is never sent.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Content-Type-Options"] = "nosniff";
    headers["Referrer-Policy"] = "no-referrer";
    headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";
    headers["Cross-Origin-Resource-Policy"] = "same-origin";
    await next();
});

// STEP 5 of 5: the one endpoint, read-only, the same document the export writes (same builder, same serializer
// settings). No CORS policy is registered, so a browser on another site cannot read it.
app.MapGet("/api/content", () =>
    Results.Text(SiteContentBuilder.ToJson(SiteContentBuilder.Build(siteContent, DateTime.UtcNow.Year)), "application/json"));

app.Run();

// The value after a flag such as --seed, which must be an absolute path; null when the flag is not given.
string? ValueOf(string flag)
{
    var index = Array.IndexOf(args, flag);
    if (index < 0)
    {
        return null;
    }

    var value = index + 1 < args.Length ? args[index + 1] : null;
    if (string.IsNullOrWhiteSpace(value) || value.StartsWith("--", StringComparison.Ordinal) || !Path.IsPathRooted(value))
    {
        throw new ArgumentException($"{flag} needs an absolute path, for example: {flag} /full/path");
    }

    return value;
}

/// <summary>
/// Exposes the entry point to the test project so it can host the app in memory.
/// </summary>
public partial class Program;
