using Portfolio.Business.Models;

namespace Portfolio.Business.Content;

/// <summary>
/// Lookups over the content's lists, shared by the content builder and the tests so both resolve a key the same way.
/// </summary>
public static class ContentLookups
{
    /// <summary>Looks a technology logo up by its evidence key (case does not matter), or returns null when there is none.</summary>
    public static TechLogo? FindTechLogo(this IPortfolioContent content, string key) =>
        content.TechLogos.FirstOrDefault(l => string.Equals(l.Key, key, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Finds the icon for a star's key: the skills-map icons first (the extra logo and the generic glyphs), then the
    /// technology logos already drawn once for the page's background, which keep their brand colour.
    /// </summary>
    /// <returns>What to draw, or null when no icon has that key (a test makes sure every star has one).</returns>
    public static ResolvedIcon? ResolveSkillIcon(this IPortfolioContent content, string key)
    {
        // STEP 1 of 2: the skills-map icons.
        var own = content.SkillIcons.FirstOrDefault(i => string.Equals(i.Key, key, StringComparison.OrdinalIgnoreCase));
        if (own is not null)
        {
            return new ResolvedIcon(own.SymbolId, own.Fill, own.OnDark);
        }

        // STEP 2 of 2: the technology logos.
        var logo = content.FindTechLogo(key);
        return logo is null ? null : new ResolvedIcon(logo.SymbolId, logo.Hex, false);
    }

    /// <summary>
    /// Returns the icon shapes (inside a 24 by 24 view box) for a profile link label, or null when there is none.
    /// </summary>
    public static string? FindSocialIconMarkup(this IPortfolioContent content, string label)
    {
        var icon = content.SocialIcons.FirstOrDefault(i => string.Equals(i.Label.Trim(), label.Trim(), StringComparison.OrdinalIgnoreCase));
        if (icon is null)
        {
            return null;
        }

        return icon.Markup ?? content.FindTechLogo(icon.LogoKey ?? string.Empty)?.Markup;
    }
}
