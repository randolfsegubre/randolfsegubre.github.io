using Portfolio.Api.Content;
using Portfolio.Api.Export;

// Composition root for the portfolio's .NET backend (ADR-0011). The front end is the Nuxt app in /web; this app
// owns the content and serves it as JSON. It has two modes.

// STEP 1 of 4: export mode. `dotnet run -- --export-content <file>` writes the content document to a file and
// exits, without starting a web server. The Nuxt build reads that file, and the result is what GitHub Pages
// serves, because Pages can only serve static files (ADR-0001).
var exportIndex = Array.IndexOf(args, "--export-content");
if (exportIndex >= 0)
{
    var outputFile = exportIndex + 1 < args.Length
        ? args[exportIndex + 1]
        : throw new ArgumentException("--export-content needs an output file, for example: --export-content /full/path/content.json");

    await ContentExporter.WriteAsync(new PortfolioContent(), outputFile, DateTime.UtcNow.Year);
    Console.WriteLine($"Wrote the content to {Path.GetFullPath(outputFile)}");
    return;
}

// STEP 2 of 4: normal mode: register the content service. The Server header is switched off, because it tells a
// visitor which software to attack and helps nobody.
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
builder.Services.AddSingleton<IPortfolioContent, PortfolioContent>();
var app = builder.Build();

// STEP 3 of 4: security. Outside development, tell browsers to use HTTPS only (HSTS) and send them there. The
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

// STEP 4 of 4: the one endpoint, read-only, the same document the export writes (same builder, same serializer
// settings). No CORS policy is registered, so a browser on another site cannot read it.
app.MapGet("/api/content", (IPortfolioContent content) =>
    Results.Text(SiteContentBuilder.ToJson(SiteContentBuilder.Build(content, DateTime.UtcNow.Year)), "application/json"));

app.Run();

/// <summary>
/// Exposes the entry point to the test project so it can host the app in memory.
/// </summary>
public partial class Program;
