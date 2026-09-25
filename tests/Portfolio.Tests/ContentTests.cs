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
        var links = Content.Profile.Links.Concat(Content.Projects.SelectMany(p => p.Links)).ToList();

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
    public void Features_a_project_and_does_not_link_the_private_repository()
    {
        Assert.Contains(Content.Projects, p => p.Featured);

        var galaxy = Assert.Single(Content.Projects, p => p.Id == "galaxy-survivor");
        Assert.Empty(galaxy.Links);
    }
}
