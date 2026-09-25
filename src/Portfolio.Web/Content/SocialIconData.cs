namespace Portfolio.Web.Content;

/// <summary>
/// Small monochrome icons shown beside the owner's profile links (GitHub and LinkedIn).
/// </summary>
/// <remarks>
/// The GitHub icon is the same public-domain Simple Icons shape used in <see cref="TechLogoData"/>
/// (see docs/THIRD_PARTY_NOTICES.md). Simple Icons does not include the LinkedIn logo, so the
/// LinkedIn mark here is a simple rounded square with the letters "in" cut out, drawn for this
/// site: a hand-drawn approximation, not the official artwork. Both are drawn in the current text
/// color, so they follow the button's color and hover state. Decorative: the link text still
/// names the destination.
/// </remarks>
public static class SocialIconData
{
    /// <summary>
    /// Returns the icon shapes (inside a 24 by 24 view box) for a profile link label, or null when there is none.
    /// </summary>
    public static string? Find(string label) => label.Trim().ToLowerInvariant() switch
    {
        "github" => TechLogoData.Find("GitHub")?.Markup,
        "linkedin" => LinkedInMark,
        _ => null,
    };

    /// <summary>A rounded square with "in" cut out (even-odd fill), drawn for this site.</summary>
    private const string LinkedInMark =
        """<path fill-rule="evenodd" d="M4 2h16a2 2 0 0 1 2 2v16a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2zM7 5.7a1.5 1.5 0 1 0 0 3 1.5 1.5 0 0 0 0-3zM5.6 10h2.8v8.5H5.6zM10.4 10h2.7v1.2c.4-.8 1.4-1.4 2.7-1.4 2.6 0 3.2 1.7 3.2 3.9v4.8h-2.8v-4.3c0-1-.1-2-1.4-2s-1.6 1-1.6 2v4.3h-2.8z"/>""";
}
