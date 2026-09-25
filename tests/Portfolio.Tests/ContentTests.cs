using System.Text.RegularExpressions;
using Portfolio.Web.Content;

namespace Portfolio.Tests;

/// <summary>
/// Content integrity and writing-rule tests (ADR-0003, ADR-0006).
/// </summary>
/// <remarks>
/// These turn the owner's writing rules into a failing build instead of something a
/// reviewer has to remember. They only read data, so they run in milliseconds and
/// need no rendering.
/// </remarks>
public sealed partial class ContentTests
{
    /// <summary>Product names that legitimately contain capital-letter runs in prose. Longest first: ".NET" would otherwise strand "ASP".</summary>
    private static readonly string[] AllowedCapitals = ["ASP.NET", ".NET", "SQL Server", "UK"];

    private static readonly PortfolioContent Content = new();

    [GeneratedRegex(@"\([A-Z]{2,}\)")]
    private static partial Regex ParenthesisedShortForm();

    [GeneratedRegex(@"\b[A-Z]{2,}\b")]
    private static partial Regex CapitalRun();

    /// <summary>Every prose string on the site: text a visitor reads as a sentence.</summary>
    private static List<(string Where, string Text)> ProseStrings()
    {
        var prose = new List<(string, string)>
        {
            ("profile.headline", Content.Profile.Headline),
            ("profile.availability", Content.Profile.Availability),
        };

        prose.AddRange(Content.Profile.Stats.Select((stat, i) => ($"profile.stats[{i}].label", stat.Label)));

        foreach (var project in Content.Projects)
        {
            prose.Add(($"{project.Id}.tagline", project.Tagline));
            prose.Add(($"{project.Id}.summary", project.Summary));
            prose.Add(($"{project.Id}.role", project.Role));
            if (project.HonestNote is not null)
            {
                prose.Add(($"{project.Id}.honestNote", project.HonestNote));
            }

            prose.AddRange(project.Highlights.Select((text, i) => ($"{project.Id}.highlights[{i}]", text)));
        }

        foreach (var role in Content.Experience)
        {
            prose.AddRange(role.Highlights.Select((text, i) => ($"{role.Id}.highlights[{i}]", text)));
        }

        foreach (var site in Content.LiveSites)
        {
            prose.Add(($"{site.Id}.summary", site.Summary));
            prose.Add(($"{site.Id}.role", site.Role));
            prose.Add(($"{site.Id}.honestNote", site.HonestNote));
            prose.AddRange(site.Highlights.Select((text, i) => ($"{site.Id}.highlights[{i}]", text)));
        }

        return prose;
    }

    /// <summary>Every string in the content, prose and chips alike.</summary>
    private static IEnumerable<string> AllStrings() =>
        ProseStrings().Select(p => p.Text)
            .Concat([Content.Profile.Name, Content.Profile.Title, Content.Profile.Location, Content.Profile.Email])
            .Concat(Content.Profile.SignatureStack)
            .Concat(Content.Profile.Stats.Select(s => s.Value))
            .Concat(Content.Projects.SelectMany(p => p.Stack.Prepend(p.Name)))
            .Concat(Content.Experience.SelectMany(r => r.Stack.Concat([r.Company, r.Title, r.Period]).Concat(r.Client is null ? [] : [r.Client])))
            .Concat(Content.Skills.SelectMany(g => g.Items.Prepend(g.Label)));

    [Fact]
    public void Contains_no_em_dashes_anywhere()
    {
        var offenders = AllStrings().Where(s => s.Contains('—')).ToList();

        Assert.Empty(offenders);
    }

    [Fact]
    public void Writes_abbreviations_out_in_full_words_in_prose()
    {
        var offenders = new List<string>();

        foreach (var (where, text) in ProseStrings())
        {
            // STEP 1 of 3: drop allowed product names that contain capital runs.
            var cleaned = AllowedCapitals.Aggregate(text, (current, name) => current.Replace(name, " "));

            // STEP 2 of 3: drop a short form that directly follows its full words, for example "(JWT)".
            cleaned = ParenthesisedShortForm().Replace(cleaned, " ");

            // STEP 3 of 3: any remaining run of two or more capitals is a bare abbreviation.
            var bare = CapitalRun().Matches(cleaned).Select(m => m.Value).ToList();
            if (bare.Count > 0)
            {
                offenders.Add($"{where}: {string.Join(", ", bare)}");
            }
        }

        Assert.Empty(offenders);
    }

    [Fact]
    public void Uses_https_for_every_outbound_link()
    {
        var links = Content.Profile.Links
            .Concat(Content.Projects.SelectMany(p => p.Links))
            .Concat(Content.LiveSites.Select(s => new Portfolio.Web.Models.Link(s.Address, s.Href)))
            .ToList();

        Assert.NotEmpty(links);
        Assert.All(links, link =>
        {
            Assert.StartsWith("https://", link.Href);
            Assert.False(string.IsNullOrWhiteSpace(link.Label));
        });
    }

    [Fact]
    public void Gives_every_project_role_and_skill_group_a_unique_id()
    {
        var ids = Content.Projects.Select(p => p.Id)
            .Concat(Content.LiveSites.Select(s => s.Id))
            .Concat(Content.Experience.Select(r => r.Id))
            .Concat(Content.Skills.Select(g => g.Id))
            .ToList();

        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact]
    public void Has_no_empty_required_fields()
    {
        foreach (var project in Content.Projects)
        {
            Assert.All([project.Name, project.Tagline, project.Summary, project.Role], value => Assert.False(string.IsNullOrWhiteSpace(value), project.Id));
            Assert.NotEmpty(project.Highlights);
            Assert.NotEmpty(project.Stack);
        }

        Assert.All(Content.Experience, role => Assert.NotEmpty(role.Highlights));
    }

    /// <summary>The only web addresses the live-sites cards may link to: the sites' own public domains.</summary>
    private static readonly string[] AllowedLiveHosts = ["www.inghams.co.uk", "www.santaslapland.co.uk"];

    [Fact]
    public void Live_site_cards_link_only_to_the_sites_own_public_domains()
    {
        Assert.NotEmpty(Content.LiveSites);
        Assert.All(Content.LiveSites, site =>
        {
            var uri = new Uri(site.Href);
            Assert.Equal("https", uri.Scheme);
            Assert.Contains(uri.Host, AllowedLiveHosts);
            Assert.Contains(site.Address, uri.Host);
        });
    }

    [Fact]
    public void Live_site_cards_contain_no_internal_names_ticket_numbers_or_hosting_addresses()
    {
        // The owner allowed these sites on the condition that only public information is shown (2026-09-25).
        // These patterns are things that exist only inside the client's estate, never on the public site.
        string[] forbidden =
        [
            @"\b[A-Z]{2,5}-\d{2,5}\b",              // ticket numbers such as HPL-1234
            @"(?i)azurewebsites|\.azure\.|blob\.core", // hosting addresses
            @"(?i)\b(jira|confluence|jenkins|sonar)\b", // internal tools
            @"(?i)\b(pcms|ecms|pim)\b",              // internal systems
            @"(?i)santa-web|santa-static|Hotelplan\.\w+|Inghams\.\w+V2|Prototype\.V2", // repository names
            @"(?i)(password|secret|token|connection string|nonce)", // sensitive configuration terms
        ];

        var texts = Content.LiveSites.SelectMany(s => new[] { s.Name, s.Address, s.Href, s.Summary, s.Role, s.HonestNote }.Concat(s.Highlights).Concat(s.Stack)).ToList();

        var hits = new List<string>();
        foreach (var pattern in forbidden)
        {
            hits.AddRange(texts.Where(t => Regex.IsMatch(t, pattern)).Select(t => $"{pattern} matched: {t}"));
        }

        Assert.Empty(hits);
    }

    [Fact]
    public void Live_site_cards_state_the_role_plainly_and_name_a_caveat()
    {
        Assert.All(Content.LiveSites, site =>
        {
            Assert.DoesNotMatch(@"(?i)\b(built|owned|created|architected|led)\b", site.Role + " " + string.Join(" ", site.Highlights));
            Assert.False(string.IsNullOrWhiteSpace(site.HonestNote));
            Assert.NotEmpty(site.Highlights);
        });
    }

    [Fact]
    public void Live_site_technologies_are_backed_by_the_roles_or_skills()
    {
        var evidenced = Content.Experience.SelectMany(r => r.Stack)
            .Concat(Content.Skills.SelectMany(g => g.Items))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var unbacked = Content.LiveSites.SelectMany(s => s.Stack).Where(tech => !evidenced.Contains(tech)).ToList();

        Assert.Empty(unbacked);
    }

    [Fact]
    public void Stats_are_present_and_complete_and_never_percentages()
    {
        Assert.NotEmpty(Content.Profile.Stats);
        Assert.All(Content.Profile.Stats, stat =>
        {
            Assert.False(string.IsNullOrWhiteSpace(stat.Value));
            Assert.False(string.IsNullOrWhiteSpace(stat.Label));

            // Skill percentages cannot be verified, so the site never shows them (ADR-0006, ADR-0008).
            Assert.DoesNotContain('%', stat.Value);
        });
        Assert.NotEmpty(Content.Profile.SignatureStack);
    }

    [Fact]
    public void Every_floating_backdrop_tag_is_backed_up_by_the_projects_roles_or_skills()
    {
        // ADR-0006: the animated background may not advertise a technology the rest of the page does not show.
        var evidenced = Content.Projects.SelectMany(p => p.Stack)
            .Concat(Content.Experience.SelectMany(r => r.Stack))
            .Concat(Content.Skills.SelectMany(g => g.Items))
            .Concat(Content.Profile.SignatureStack)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var unbacked = Content.Profile.BackdropTags.Where(tag => !evidenced.Contains(tag)).ToList();

        Assert.Empty(unbacked);
        Assert.Equal(Content.Profile.BackdropTags.Count, Content.Profile.BackdropTags.Distinct().Count());
    }

    [Fact]
    public void Every_backdrop_tech_has_a_logo_with_a_valid_brand_color_and_shapes()
    {
        Assert.All(Content.Profile.BackdropTags, tech =>
        {
            var logo = TechLogoData.Find(tech);
            Assert.True(logo is not null, $"No logo for '{tech}'");
            Assert.Matches("^#[0-9A-Fa-f]{6}$", logo.Hex);
            Assert.Contains("<path", logo.Markup);
        });

        // Logo keys are unique, and none is defined without being used (dead data would drift).
        Assert.Equal(TechLogoData.Logos.Count, TechLogoData.Logos.Select(l => l.Key).Distinct().Count());
        Assert.All(TechLogoData.Logos, logo => Assert.Contains(logo.Key, Content.Profile.BackdropTags));
    }

    [Fact]
    public void Backdrop_code_lanes_each_have_code_and_a_sensible_speed()
    {
        // Eight lanes fill a wide screen edge to edge; the stylesheet shows fewer on tablets and phones.
        Assert.Equal(8, BackdropCode.Streams.Count);
        Assert.All(BackdropCode.Streams, stream =>
        {
            Assert.False(string.IsNullOrWhiteSpace(stream.Code));
            Assert.InRange(stream.Seconds, 30, 180);
        });
    }

    [Fact]
    public void Portrait_has_alt_text_and_is_a_site_relative_path()
    {
        Assert.NotNull(Content.Profile.PhotoUrl);
        Assert.StartsWith("/", Content.Profile.PhotoUrl);
        Assert.False(string.IsNullOrWhiteSpace(Content.Profile.PhotoAlt));
    }

    [Fact]
    public void Features_a_project_and_does_not_link_the_private_repository()
    {
        Assert.Contains(Content.Projects, p => p.Featured);

        var galaxy = Assert.Single(Content.Projects, p => p.Id == "galaxy-survivor");
        Assert.Empty(galaxy.Links);
    }
}
