namespace Portfolio.Web.Content;

/// <summary>
/// Facts about the deployed site itself (not about the owner), kept in one place so the
/// page head, the sitemap and the tests can never disagree about the address.
/// </summary>
/// <remarks>
/// This is plain data in the "content lives in <c>Content/</c>" rule (ADR-0003). The
/// address is where GitHub Pages serves the site (ADR-0001).
/// </remarks>
public static class SiteInfo
{
    /// <summary>The public address of the site, with no trailing slash.</summary>
    public const string Url = "https://randolfsegubre.github.io";
}
