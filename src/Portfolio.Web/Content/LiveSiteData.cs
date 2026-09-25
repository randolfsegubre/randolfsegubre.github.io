using Portfolio.Web.Models;

namespace Portfolio.Web.Content;

/// <summary>
/// Live public websites the owner contributed to at Hotelplan UK (client work, through Cloud Employee).
/// </summary>
/// <remarks>
/// The owner confirmed on 2026-09-25 that naming these sites is fine, on the condition that only
/// publicly available information is shown and nothing private, internal or confidential.
/// So: names and public addresses only, plus his plain-language part. No repository names,
/// ticket numbers, hosting addresses, screenshots of internal tools, or configuration detail.
/// The Inghams features named here (the wishlist, the resort and country pages) and the
/// Santa's Lapland description (family Lapland holidays with flights, hotels and a private trip
/// to see Santa) were checked against each public site's own text on 2026-09-25. The owner
/// corrected the Santa's Lapland address that day to santaslapland.com.
/// </remarks>
public static class LiveSiteData
{
    public static IReadOnlyList<LiveSite> Sites { get; } =
    [
        new LiveSite(
            Id: "inghams",
            Name: "Inghams",
            Address: "inghams.co.uk",
            Href: "https://www.inghams.co.uk/",
            Summary: "The customer-facing website of Inghams, which describes itself as the ski, walking and Lapland holiday experts, with Hotelplan UK.",
            Role: "One of a team of developers on this site, through Cloud Employee, from August 2024 to August 2026.",
            Highlights:
            [
                "Contributed full-stack development to visitor-facing features and page templates, including the saved-holiday wishlist and the resort and country content pages.",
                "Fixed defects and reviewed changes in a large shared codebase with many contributors.",
            ],
            HonestNote: "A team maintains this site and it has changed since my engagement ended in August 2026, so what you see today is not all my work.",
            Stack: ["C#", "ASP.NET Core", "Umbraco", "Vue.js"]),

        new LiveSite(
            Id: "santas-lapland",
            Name: "Santa's Lapland",
            Address: "santaslapland.com",
            Href: "https://www.santaslapland.com/",
            Summary: "The website of Santa's Lapland, which offers family Lapland holidays including flights, hotels and a private trip to see Santa.",
            Role: "One of the developers on this site, through Cloud Employee, from November 2025 to July 2026.",
            Highlights:
            [
                "Contributed web security and standards work: response headers, cookie attributes and subresource integrity.",
            ],
            HonestNote: "A team maintains this site and it has changed since my engagement ended in August 2026, so what you see today is not all my work.",
            Stack: ["C#", "ASP.NET Core", "Umbraco", "JavaScript"]),
    ];
}
