using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Portfolio.Tests;

/// <summary>
/// Guards the responsive rules in the page and stylesheet (ADR-0004, ADR-0008).
/// </summary>
/// <remarks>
/// These cannot replace looking at real screen sizes in a browser (the devlog records those checks),
/// but they stop the rules that make the layout adapt from being quietly removed or contradicted.
/// </remarks>
public sealed partial class ResponsiveDesignTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [GeneratedRegex(@"@media \(max-width: 41\.99rem\) \{(?<body>(?:[^{}]|\{[^{}]*\})*)\}", RegexOptions.Singleline)]
    private static partial Regex PhoneMediaBlock();

    [GeneratedRegex(@"width:\s*\d{3,}px")]
    private static partial Regex FixedPixelWidth();

    private string Css() => File.ReadAllText(Path.Combine(factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath, "css", "site.css"));

    /// <summary>All the phone-width media blocks joined together (the stylesheet has several).</summary>
    private string PhoneRules() => string.Join("\n", PhoneMediaBlock().Matches(Css()).Select(m => m.Groups["body"].Value));

    [Fact]
    public async Task Viewport_meta_scales_to_the_device_and_never_blocks_zoom()
    {
        using var client = factory.CreateClient();
        var page = await new HtmlParser().ParseDocumentAsync(await client.GetStringAsync("/"));

        var content = page.QuerySelector("meta[name='viewport']")?.GetAttribute("content") ?? string.Empty;

        Assert.Contains("width=device-width", content);
        Assert.Contains("initial-scale=1", content);

        // Blocking pinch zoom is an accessibility failure for low-vision visitors.
        Assert.DoesNotContain("user-scalable=no", content);
        Assert.DoesNotContain("maximum-scale", content);
    }

    [Fact]
    public void Layout_uses_relative_units_not_fixed_pixel_widths()
    {
        // A fixed width of hundreds of pixels is what makes pages overflow on phones.
        Assert.Empty(FixedPixelWidth().Matches(Css()).Select(m => m.Value));
    }

    [Fact]
    public void Content_column_grows_on_large_screens_but_is_capped()
    {
        Assert.Contains("--content-width: clamp(60rem, 52vw, 80rem)", Css());
    }

    [Fact]
    public void Logo_rail_loop_distance_matches_the_number_of_repeated_sets()
    {
        // _LogoRail.cshtml repeats the set 6 times; the loop must move by exactly one sixth of the track.
        Assert.Contains("translate3d(calc(-100% / 6), 0, 0)", Css());
    }

    [Fact]
    public void Phones_get_a_swipeable_single_row_navigation_with_touch_sized_links()
    {
        var phone = PhoneRules();

        Assert.Contains("flex-wrap: nowrap", phone);
        Assert.Contains("overflow-x: auto", phone);
        Assert.Contains("min-height: 2.75rem", phone);
    }

    [Fact]
    public void Phones_offset_in_page_anchors_by_the_taller_header()
    {
        var match = Regex.Match(PhoneRules(), @"scroll-padding-top:\s*(?<rem>[\d.]+)rem");

        Assert.True(match.Success, "No scroll-padding-top in the phone rules");
        Assert.True(double.Parse(match.Groups["rem"].Value, System.Globalization.CultureInfo.InvariantCulture) >= 6.5, "Anchors would hide under the two-row phone header");
    }

    [Fact]
    public void Phones_drop_the_costly_card_blur_and_show_fewer_lanes()
    {
        var phone = PhoneRules();

        Assert.Contains("backdrop-filter: none", phone);
        Assert.Contains("--lanes: 3", phone);
    }

    [Fact]
    public void Faint_page_logos_are_clamped_so_the_right_edge_never_cuts_them_off()
    {
        Assert.Contains("left: min(var(--x), calc(100% - 3.4rem))", Css());
    }

    [Fact]
    public void Contact_and_card_links_have_touch_sized_padding()
    {
        Assert.Matches(@"\.contact-link,\s*\.contact-action,\s*\.live-address a,\s*\.card-links a \{[^}]*padding-block: 0\.6rem", Css());
    }

    [Fact]
    public void Every_animation_stops_for_visitors_who_prefer_reduced_motion()
    {
        var css = Css();

        Assert.Contains("@media (prefers-reduced-motion: reduce)", css);
        Assert.Contains("animation: none !important", css);
    }
}
