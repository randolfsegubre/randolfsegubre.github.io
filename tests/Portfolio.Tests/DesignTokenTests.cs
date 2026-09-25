using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Portfolio.Tests;

/// <summary>
/// Guards the royal purple design tokens in <c>site.css</c> (ADR-0004).
/// </summary>
/// <remarks>
/// Accessibility is a requirement, so the stylesheet itself is tested: every text and
/// background pair must meet the Web Content Accessibility Guidelines (WCAG) minimum,
/// in both themes, and the two places that define the dark theme must never drift apart.
/// </remarks>
public sealed partial class DesignTokenTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>Minimum contrast for normal text (WCAG AA).</summary>
    private const double TextMinimum = 4.5;

    /// <summary>Minimum contrast for non-text elements such as focus rings (WCAG AA).</summary>
    private const double NonTextMinimum = 3.0;

    /// <summary>The base <c>:root</c> block: the dark palette, the default for every visitor.</summary>
    [GeneratedRegex(@"^:root\s*\{(?<body>[^}]*)\}", RegexOptions.Multiline)]
    private static partial Regex DarkBaseBlock();

    /// <summary>The optional light theme, applied only when the visitor picks it.</summary>
    [GeneratedRegex(@"^:root\[data-theme=""light""\]\s*\{(?<body>[^}]*)\}", RegexOptions.Multiline)]
    private static partial Regex LightOverrideBlock();

    [GeneratedRegex(@"--(?<name>[a-z0-9-]+):\s*(?<value>#[0-9a-fA-F]{6})\s*;")]
    private static partial Regex HexToken();

    private string Css() => File.ReadAllText(Path.Combine(factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath, "css", "site.css"));

    private static Dictionary<string, string> Tokens(Regex block, string css)
    {
        var match = block.Match(css);
        Assert.True(match.Success, "Expected block not found in site.css");
        return HexToken().Matches(match.Groups["body"].Value).ToDictionary(m => m.Groups["name"].Value, m => m.Groups["value"].Value, StringComparer.Ordinal);
    }

    /// <summary>The tokens a visitor actually sees in a theme: the dark base, overridden by the light block when light.</summary>
    private Dictionary<string, string> Theme(bool light)
    {
        var css = Css();
        var tokens = Tokens(DarkBaseBlock(), css);
        if (light)
        {
            foreach (var (name, value) in Tokens(LightOverrideBlock(), css))
            {
                tokens[name] = value;
            }
        }

        return tokens;
    }

    private static double Luminance(string hex)
    {
        double Channel(int start)
        {
            var c = int.Parse(hex.AsSpan(start, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture) / 255.0;
            return c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
        }

        return 0.2126 * Channel(1) + 0.7152 * Channel(3) + 0.0722 * Channel(5);
    }

    private static double Contrast(string foreground, string background)
    {
        var (a, b) = (Luminance(foreground), Luminance(background));
        var (lighter, darker) = a > b ? (a, b) : (b, a);
        return (lighter + 0.05) / (darker + 0.05);
    }

    /// <summary>Every text-on-background pair the design uses, by token name. Same-name pairs are checked per theme.</summary>
    public static TheoryData<string, string, double> Pairs => new()
    {
        { "text", "bg", TextMinimum },
        { "text", "surface", TextMinimum },
        { "text-muted", "bg", TextMinimum },
        { "text-muted", "surface", TextMinimum },
        { "text-muted", "surface-alt", TextMinimum },
        { "heading", "bg", TextMinimum },
        { "heading", "surface", TextMinimum },
        { "accent", "bg", TextMinimum },
        { "accent", "surface", TextMinimum },
        { "gold", "bg", TextMinimum },
        { "gold", "surface", TextMinimum },
        { "focus", "bg", NonTextMinimum },
        { "focus", "surface", NonTextMinimum },
        { "gold-bright", "bg", NonTextMinimum },
        { "chrome-text", "chrome-bg", TextMinimum },
        { "chrome-muted", "chrome-bg", TextMinimum },
        { "chrome-gold", "chrome-bg", TextMinimum },
        { "hero-text", "hero-from", TextMinimum },
        { "hero-text", "hero-via", TextMinimum },
        { "hero-text", "hero-to", TextMinimum },
        { "hero-muted", "hero-from", TextMinimum },
        { "hero-muted", "hero-via", TextMinimum },
        { "hero-muted", "hero-to", TextMinimum },
        { "hero-gold", "hero-from", TextMinimum },
        { "hero-gold", "hero-via", TextMinimum },
        { "hero-gold", "hero-to", TextMinimum },
        { "hero-button-text", "hero-gold", TextMinimum },
        { "chrome-gold", "hero-via", TextMinimum },
        { "code-text", "code-bg", TextMinimum },
        { "code-keyword", "code-bg", TextMinimum },
        { "code-type", "code-bg", TextMinimum },
        { "code-string", "code-bg", TextMinimum },
        { "code-comment", "code-bg", TextMinimum },
        { "code-operator", "code-bg", TextMinimum },
    };

    [Theory]
    [MemberData(nameof(Pairs))]
    public void Dark_default_theme_pair_meets_the_contrast_minimum(string foreground, string background, double minimum)
    {
        var tokens = Theme(light: false);

        var ratio = Contrast(tokens[foreground], tokens[background]);

        Assert.True(ratio >= minimum, $"dark: --{foreground} on --{background} is {ratio:F2}:1, needs {minimum}:1");
    }

    [Theory]
    [MemberData(nameof(Pairs))]
    public void Optional_light_theme_pair_meets_the_contrast_minimum(string foreground, string background, double minimum)
    {
        var tokens = Theme(light: true);

        var ratio = Contrast(tokens[foreground], tokens[background]);

        Assert.True(ratio >= minimum, $"light: --{foreground} on --{background} is {ratio:F2}:1, needs {minimum}:1");
    }

    [Fact]
    public void Light_theme_only_overrides_tokens_that_exist_in_the_dark_base()
    {
        var css = Css();

        var dark = Tokens(DarkBaseBlock(), css);
        var light = Tokens(LightOverrideBlock(), css);

        Assert.NotEmpty(light);
        Assert.All(light.Keys, name => Assert.True(dark.ContainsKey(name), $"--{name} is in the light theme but not in the dark base"));
    }

    [Fact]
    public void Dark_is_the_default_whatever_the_system_setting()
    {
        // No media query may switch the palette: a visitor whose system prefers light still gets dark.
        Assert.DoesNotContain("@media (prefers-color-scheme", Css(), StringComparison.Ordinal);
    }
}
