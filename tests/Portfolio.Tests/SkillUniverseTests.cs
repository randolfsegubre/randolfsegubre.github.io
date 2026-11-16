using AngleSharp.Html.Parser;
using Portfolio.Business.Content;
using Portfolio.Business.Models;

namespace Portfolio.Tests;

/// <summary>
/// Guards the skills star map (ADR-0010): the layout geometry, the content rules and the rendered markup.
/// </summary>
/// <remarks>
/// The layout is a pure function of its data, so overlaps and bad trees are caught here without a browser.
/// What a person sees at real screen sizes is checked by looking (the devlog records it); these tests stop
/// the structure from quietly breaking in between.
/// </remarks>
public sealed class SkillUniverseTests(SiteFactory factory) : IClassFixture<SiteFactory>
{
    private static readonly ContentSnapshot Content = TestContent.Snapshot;

    private static UniverseMap Map => Content.Universe;

    [Fact]
    public void Has_exactly_one_centre_and_it_is_dot_net_at_the_middle_of_the_cloud()
    {
        var centre = Assert.Single(Map.Stars, s => s.Star.ParentId is null);

        Assert.Equal(".NET", centre.Star.Label);
        Assert.Equal(0, centre.Depth);
        Assert.Equal((0.0, 0.0, 0.0), (centre.Radius, centre.Elevation, centre.Azimuth));
    }

    [Fact]
    public void Every_star_has_a_unique_id_a_real_parent_and_a_depth_one_more_than_its_parent()
    {
        var byId = Map.Stars.ToDictionary(s => s.Star.Id);

        Assert.Equal(Map.Stars.Count, byId.Count);
        foreach (var placed in Map.Stars.Where(s => s.Star.ParentId is not null))
        {
            Assert.True(byId.ContainsKey(placed.Star.ParentId!), $"'{placed.Star.Id}' has an unknown parent");
            Assert.Equal(byId[placed.Star.ParentId!].Depth + 1, placed.Depth);
        }
    }

    [Fact]
    public void Draws_a_line_for_every_parent_link_and_every_cross_link_between_stars_that_exist()
    {
        var ids = Map.Stars.Select(s => s.Star.Id).ToHashSet();

        Assert.All(Map.Links, link =>
        {
            Assert.Contains(link.FromId, ids);
            Assert.Contains(link.ToId, ids);
            Assert.NotEqual(link.FromId, link.ToId);
        });
        Assert.Equal(Map.Links.Count, Map.Links.Select(l => (l.FromId, l.ToId)).Distinct().Count());
        Assert.True(Map.Links.Count >= Map.Stars.Count - 1, "Every star except the centre needs at least its parent link");
    }

    [Fact]
    public void The_journey_visits_every_star_exactly_once_starting_at_the_centre()
    {
        var ids = Map.Stars.Select(s => s.Star.Id).ToList();

        Assert.Equal(ids.Count, Map.Journey.Count);
        Assert.Equal(ids.Order(), Map.Journey.Order());
        Assert.Equal(Map.Stars.Single(s => s.Star.ParentId is null).Star.Id, Map.Journey[0]);
    }

    [Fact]
    public void Gives_every_star_a_label_that_the_roles_projects_or_skills_back_up()
    {
        // ADR-0006: the map may not advertise a technology the rest of the page does not show. The group headings
        // ("AI-assisted engineering", "Practice") are on the page too, so they may name a hub.
        var evidenced = Content.Projects.SelectMany(p => p.Stack)
            .Concat(Content.Experience.SelectMany(r => r.Stack))
            .Concat(Content.Skills.SelectMany(g => g.Items))
            .Concat(Content.Profile.SignatureStack)
            .Concat(Content.Skills.Select(g => g.Label))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var unbacked = Map.Stars.Select(s => s.Star.Label).Where(label => !evidenced.Contains(label)).ToList();

        Assert.Empty(unbacked);
        Assert.Equal(Map.Stars.Count, Map.Stars.Select(s => s.Star.Label).Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Fact]
    public void Shows_only_technologies_the_owner_has_used_himself_and_no_practices()
    {
        // Rule confirmed by the owner on 2026-11-15, after he checked his own record: the cloud is a tech stack he has
        // used himself, at work or in his own projects. These were on an earlier version and are not his experience:
        // Azure Service Bus (he has never used a message broker), TanStack Query, Zustand and Vite (from a project built
        // with AI help), GraphQL, Semantic Kernel, TypeScript (he has none), and the practices and techniques that
        // are not a stack at all. If one of them is ever really used, remove it here in the same change that adds the sphere.
        var notHisExperience = new[]
        {
            "Azure Service Bus", "TanStack Query", "Zustand", "Vite", "GraphQL", "Semantic Kernel", "TypeScript", "SQLite",
            "Figma handoff", "Markdown", "Practice", "Code review", "Legacy upgrades", "Production support triage",
            "Documentation-first delivery", "AI-assisted engineering", "Shared instruction files", "Content-type modelling",
            "Models Builder", "Custom backoffice property editors", "Code scanning remediation", "Search engine query debugging",
            "SOAP and REST integration",
        };

        var onMap = Map.Stars.Select(s => s.Star.Label).ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.All(notHisExperience, label => Assert.DoesNotContain(label, onMap));
    }

    [Fact]
    public async Task Draws_the_main_stack_larger_and_keeps_it_in_step_with_the_profiles_signature_stack()
    {
        // Owner's request, 2026-11-15: his main stack reads first, everything else is a smaller sphere. The signature
        // stack on the profile is the one definition of "main", so the two lists must never drift apart.
        var main = Map.Stars.Where(s => s.Star.Main).Select(s => s.Star.Label).Order().ToList();
        Assert.Equal(Content.Profile.SignatureStack.Order(), main);

        using var client = factory.CreateClient();
        var page = await new HtmlParser().ParseDocumentAsync(await client.GetStringAsync("/"));

        var marked = page.QuerySelectorAll("li.star.star-main").Select(s => s.GetAttribute("data-star")!).Order().ToList();
        Assert.Equal(Map.Stars.Where(s => s.Star.Main).Select(s => s.Star.Id).Order(), marked);
    }

    [Fact]
    public void Gives_every_star_an_icon_that_exists_and_every_icon_a_unique_valid_id()
    {
        Assert.All(Map.Stars, star => Assert.True(Content.ResolveSkillIcon(star.Star.Icon) is not null, $"{star.Star.Id}: no icon '{star.Star.Icon}'"));

        Assert.Equal(Content.SkillIcons.Count, Content.SkillIcons.Select(i => i.Key).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.All(Content.SkillIcons, icon =>
        {
            Assert.Matches("^skillicon-[a-z0-9-]+$", icon.SymbolId);
            Assert.False(string.IsNullOrWhiteSpace(icon.Markup));
            Assert.DoesNotContain("<script", icon.Markup, StringComparison.OrdinalIgnoreCase);
            if (icon.Fill is not null)
            {
                Assert.Matches("^#[0-9A-Fa-f]{6}$", icon.Fill);
            }
        });

        // Nothing is defined without being used: dead icons would drift.
        var used = Map.Stars.Select(s => s.Star.Icon).ToHashSet(StringComparer.OrdinalIgnoreCase);
        Assert.All(Content.SkillIcons, icon => Assert.Contains(icon.Key, used));
    }

    [Fact]
    public async Task Draws_each_star_as_a_sphere_with_its_logo_and_its_name_underneath()
    {
        using var client = factory.CreateClient();
        var page = await new HtmlParser().ParseDocumentAsync(await client.GetStringAsync("/"));

        foreach (var placed in Map.Stars)
        {
            var star = page.QuerySelector($"li.star[data-star='{placed.Star.Id}']")!;
            var icon = Content.ResolveSkillIcon(placed.Star.Icon)!;

            Assert.NotNull(star.QuerySelector(".orb"));
            Assert.Equal("true", star.QuerySelector(".orb")!.GetAttribute("aria-hidden"));
            Assert.Equal("#" + icon.SymbolId, star.QuerySelector(".orb-icon use")!.GetAttribute("href"));
            Assert.Equal(placed.Star.Label, star.QuerySelector(".star-name")!.TextContent.Trim());

            // Every symbol a star points at is really defined on the page (the page sprite or the skills sprite).
            Assert.NotNull(page.GetElementById(icon.SymbolId));
        }
    }

    [Fact]
    public void Gives_every_star_a_one_sentence_caption_without_percentages()
    {
        Assert.All(Map.Stars, star =>
        {
            Assert.False(string.IsNullOrWhiteSpace(star.Star.Caption), $"{star.Star.Id} has no caption");
            Assert.EndsWith(".", star.Star.Caption);
            Assert.DoesNotContain('%', star.Star.Caption);
        });
    }

    [Fact]
    public void The_layout_is_the_same_every_time_so_the_exported_page_never_changes_by_itself()
    {
        var again = SkillUniverseLayout.Build(Map.Intro, Map.Stars.Select(p => p.Star).ToList(), [new SkillLink("csharp", "aspnet-core"), new SkillLink("csharp", "ef-core")], Map.Journey);

        Assert.Equal(Map.Stars.Select(s => (s.Star.Id, s.Azimuth, s.Elevation, s.Radius)), again.Stars.Select(s => (s.Star.Id, s.Azimuth, s.Elevation, s.Radius)));
    }

    [Fact]
    public void The_layout_refuses_a_broken_tree_instead_of_drawing_a_wrong_map()
    {
        var centre = new SkillStar("a", "A", null, "Caption.", "check");

        Assert.Throws<InvalidOperationException>(() => SkillUniverseLayout.Build("x", [centre, new SkillStar("b", "B", null, "Caption.", "check")], [], ["a", "b"]));
        Assert.Throws<InvalidOperationException>(() => SkillUniverseLayout.Build("x", [centre, new SkillStar("b", "B", "missing", "Caption.", "check")], [], ["a", "b"]));

        // One level deeper than the layout accepts.
        var chain = new List<SkillStar> { centre };
        for (var i = 1; i <= SkillUniverseLayout.MaxDepth + 1; i++)
        {
            chain.Add(new SkillStar($"s{i}", $"S{i}", i == 1 ? "a" : $"s{i - 1}", "Caption.", "check"));
        }

        Assert.Throws<InvalidOperationException>(() => SkillUniverseLayout.Build("x", chain, [], chain.Select(s => s.Id).ToList()));
    }

    [Fact]
    public async Task Renders_every_star_caption_and_line_in_the_page_without_needing_script()
    {
        using var client = factory.CreateClient();
        var page = await new HtmlParser().ParseDocumentAsync(await client.GetStringAsync("/"));

        var section = page.QuerySelector("section#about");
        Assert.NotNull(section);

        // Every star is in the markup, in place, as a list item.
        var stars = section.QuerySelectorAll("li.star[data-star]");
        Assert.Equal(Map.Stars.Select(s => s.Star.Id).Order(), stars.Select(s => s.GetAttribute("data-star")!).Order());

        // Every caption is in the markup too, so a visitor without script can read the whole map.
        var captions = section.QuerySelectorAll("li.universe-caption[data-caption]");
        Assert.Equal(Map.Stars.Count, captions.Length);

        // Every link is drawn, and the walk order travels with the page for the script to read.
        Assert.Equal(Map.Links.Count, section.QuerySelectorAll("line.universe-link").Length);
        Assert.Equal(string.Join(',', Map.Journey), section.QuerySelector("[data-universe]")!.GetAttribute("data-journey"));

        // The walk button ships hidden: without script it would do nothing, so script reveals it.
        Assert.True(section.QuerySelector("[data-universe-walk]")!.HasAttribute("hidden"));
    }

    [Fact]
    public async Task Makes_the_map_a_keyboard_reachable_region_and_hides_the_decorative_drawing_from_assistive_technology()
    {
        using var client = factory.CreateClient();
        var page = await new HtmlParser().ParseDocumentAsync(await client.GetStringAsync("/"));

        var scroller = page.QuerySelector(".universe-scroll")!;
        Assert.Equal("region", scroller.GetAttribute("role"));
        Assert.Equal("0", scroller.GetAttribute("tabindex"));
        Assert.False(string.IsNullOrWhiteSpace(scroller.GetAttribute("aria-label")));
        Assert.Equal("true", page.QuerySelector("svg.universe-lines")!.GetAttribute("aria-hidden"));
    }

    /// <summary>A star's place in the 3D cloud as x, y, z in world units.</summary>
    private static (double X, double Y, double Z) Point(PlacedStar star)
    {
        var azimuth = star.Azimuth * Math.PI / 180;
        var elevation = star.Elevation * Math.PI / 180;
        return (
            star.Radius * Math.Cos(elevation) * Math.Cos(azimuth),
            star.Radius * Math.Sin(elevation),
            star.Radius * Math.Cos(elevation) * Math.Sin(azimuth));
    }

    [Fact]
    public void Spreads_the_stars_evenly_over_three_spheres_so_no_two_crowd_each_other_however_it_is_turned()
    {
        var centre = Map.Stars.Single(s => s.Depth == 0);
        Assert.Equal((0.0, 0.0, 0.0), (centre.Radius, centre.Elevation, centre.Azimuth));

        foreach (var star in Map.Stars.Where(s => s.Depth > 0))
        {
            Assert.Equal(SkillUniverseLayout.RadiusFor(star.Depth), star.Radius);
            Assert.InRange(star.Azimuth, 0, 360);

            // Clear of the poles, where a spiral bunches up.
            Assert.InRange(star.Elevation, -59, 59);
        }

        // Three shells, each used by exactly the depths it should hold, so nothing sits half way between.
        Assert.Equal(
            [SkillUniverseLayout.InnerRadius, SkillUniverseLayout.MiddleRadius, SkillUniverseLayout.OuterRadius],
            Map.Stars.Where(s => s.Depth > 0).Select(s => s.Radius).Distinct().Order());

        // On each sphere, the two closest stars are still a good fraction of the radius apart.
        foreach (var onSphere in Map.Stars.Where(s => s.Depth > 0).GroupBy(s => s.Radius))
        {
            var points = onSphere.Select(Point).ToList();
            var closest = double.MaxValue;
            for (var i = 0; i < points.Count; i++)
            {
                for (var j = i + 1; j < points.Count; j++)
                {
                    var (dx, dy, dz) = (points[i].X - points[j].X, points[i].Y - points[j].Y, points[i].Z - points[j].Z);
                    closest = Math.Min(closest, Math.Sqrt(dx * dx + dy * dy + dz * dz));
                }
            }

            Assert.True(closest >= onSphere.Key * 0.45, $"On the sphere of radius {onSphere.Key} two stars are only {closest:0} apart");
        }
    }

    [Fact]
    public async Task Hands_the_3D_view_each_stars_depth_and_place_on_the_sphere()
    {
        using var client = factory.CreateClient();
        var page = await new HtmlParser().ParseDocumentAsync(await client.GetStringAsync("/"));

        var universe = page.QuerySelector("[data-universe]")!;
        var number = (string name, AngleSharp.Dom.IElement star) =>
            double.Parse(star.GetAttribute(name)!, System.Globalization.CultureInfo.InvariantCulture);

        foreach (var placed in Map.Stars)
        {
            var star = universe.QuerySelector($"li.star[data-star='{placed.Star.Id}']")!;
            Assert.Equal(placed.Depth.ToString(System.Globalization.CultureInfo.InvariantCulture), star.GetAttribute("data-depth"));
            Assert.Equal(placed.Azimuth, number("data-azimuth", star), 1);
            Assert.Equal(placed.Elevation, number("data-elevation", star), 1);
            Assert.Equal(placed.Radius, number("data-radius", star), 1);
        }

        // The turn and pause controls ship hidden: without script they would do nothing.
        Assert.All(universe.QuerySelectorAll("[data-universe-spin], [data-universe-turn]"), button => Assert.True(button.HasAttribute("hidden")));
    }

    [Fact]
    public async Task Puts_About_me_first_with_his_paragraphs_and_the_skills_inside_the_same_section()
    {
        using var client = factory.CreateClient();
        var page = await new HtmlParser().ParseDocumentAsync(await client.GetStringAsync("/"));

        var section = page.QuerySelector("section#about")!;
        Assert.Equal("About Me", section.QuerySelector("h2")!.TextContent.Trim());
        Assert.Equal(Content.Profile.About, section.QuerySelectorAll(".about-copy p").Select(p => p.TextContent.Trim()));
        Assert.NotNull(section.QuerySelector("[data-universe]"));

        // There is no separate Skills section any more, and the navigation says About instead of Skills.
        Assert.Null(page.QuerySelector("section#skills"));
        var links = page.QuerySelectorAll(".nav-list a").Select(a => a.TextContent.Trim()).ToList();
        Assert.Equal("About Me", links[0]);
        Assert.DoesNotContain("Skills", links);

        // Every link in the navigation lands on a section that exists.
        Assert.All(page.QuerySelectorAll(".nav-list a"), link => Assert.NotNull(page.GetElementById(link.GetAttribute("href")!.TrimStart('#'))));
    }

    [Fact]
    public async Task Keeps_the_full_grouped_skills_list_under_the_map_so_no_skill_is_lost()
    {
        using var client = factory.CreateClient();
        var page = await new HtmlParser().ParseDocumentAsync(await client.GetStringAsync("/"));

        var details = page.QuerySelector("details.skills-all");
        Assert.NotNull(details);

        var chips = details.QuerySelectorAll(".tag").Select(t => t.TextContent.Trim()).ToList();
        Assert.Equal(Content.Skills.SelectMany(g => g.Items).Order(), chips.Order());
    }

    [Fact]
    public async Task Writes_every_number_with_a_dot_decimal_whatever_the_machines_culture()
    {
        // A machine set to a comma-decimal culture would otherwise write "12,5" into the data attributes the script reads.
        var original = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
            using var client = factory.CreateClient();
            var html = await client.GetStringAsync("/");

            Assert.DoesNotMatch(@"data-(azimuth|elevation|radius)=""-?\d+,\d+""", html);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = original;
        }
    }
}
