using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Content;
using Portfolio.Web.Models;

namespace Portfolio.Web.Controllers;

/// <summary>
/// The site's only controller: serves the single portfolio page and the not-found page.
/// </summary>
/// <remarks>
/// This is the "C" in ASP.NET Core MVC (ADR-0002). It stays deliberately thin:
/// it asks <see cref="IPortfolioContent"/> for data, packs it into one view model,
/// and lets the Razor views render it. No copy or claim lives here.
/// </remarks>
public sealed class HomeController(IPortfolioContent content) : Controller
{
    /// <summary>
    /// The whole portfolio page, served at <c>/</c>.
    /// </summary>
    public IActionResult Index()
    {
        // STEP 1 of 2: gather every section's data from the content service.
        var model = new PortfolioViewModel(content.Profile, content.Projects, content.Experience, content.Skills);

        // STEP 2 of 2: hand the single model to Views/Home/Index.cshtml.
        return View(model);
    }

    /// <summary>
    /// A friendly not-found page at <c>/not-found</c>.
    /// </summary>
    /// <remarks>
    /// The static export writes this page out as <c>404.html</c>, which GitHub Pages
    /// serves for any unknown address. It is a normal 200 response here on purpose:
    /// the exporter treats any non-200 as a failed export.
    /// </remarks>
    [Route("not-found")]
    public IActionResult NotFoundPage() => View("NotFound", content.Profile);
}
