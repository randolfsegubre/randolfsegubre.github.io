using Portfolio.Business.Content;
using Portfolio.Business.Services;
using Portfolio.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Portfolio.Tests;

/// <summary>
/// Tests against a real SQL Server (ADR-0012): that what is stored comes back exactly as the data files say, and that
/// the schema itself refuses content that makes no sense.
/// </summary>
/// <remarks>
/// These are the tests that prove the database path, which every build also exercises. Everything else about the
/// content is tested without a database; here the database is the thing under test, so it is a real one (a throwaway
/// database per test, see <see cref="TestDatabase"/>).
/// </remarks>
public sealed class SqlContentTests
{
    private static readonly ContentSnapshot Content = TestContent.Snapshot;

    private static string Document(IPortfolioContent content) => SiteContentBuilder.ToJson(SiteContentBuilder.Build(content, 2026));

    private static Task SeedAsync(TestDatabase database) =>
        database.InScopeAsync(provider => provider.GetRequiredService<IContentMaintenance>().SeedFromFilesAsync(TestContent.Folder));

    private static Task<string> LoadedDocumentAsync(TestDatabase database) =>
        database.InScopeAsync(async provider => Document(await provider.GetRequiredService<IContentSource>().LoadAsync()));

    [Fact]
    public async Task Seeding_the_database_from_the_files_and_loading_it_gives_back_the_same_site()
    {
        await using var database = await TestDatabase.CreateAsync();

        await SeedAsync(database);

        // The document is exactly what gets published, so equal documents mean an identical page.
        Assert.Equal(Document(Content), await LoadedDocumentAsync(database));
    }

    [Fact]
    public async Task Seeding_twice_gives_the_same_database_so_it_is_safe_to_run_on_every_build()
    {
        await using var database = await TestDatabase.CreateAsync();

        await SeedAsync(database);
        var first = await LoadedDocumentAsync(database);
        await SeedAsync(database);

        Assert.Equal(first, await LoadedDocumentAsync(database));
        var rows = await database.InScopeAsync(provider => provider.GetRequiredService<IContentRepository>().ReadAsync());
        Assert.Equal(Content.Projects.Count, rows!.Projects.Count);
        Assert.Equal(Content.Universe.Stars.Count, rows.SkillStars.Count);
    }

    [Fact]
    public async Task An_empty_database_refuses_to_serve_and_says_how_to_seed_it()
    {
        await using var database = await TestDatabase.CreateAsync();

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => LoadedDocumentAsync(database));

        Assert.Contains("--seed", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Migrating_a_database_that_is_already_up_to_date_changes_nothing()
    {
        await using var database = await TestDatabase.CreateAsync();
        await SeedAsync(database);

        await database.InScopeAsync(provider => provider.GetRequiredService<IContentMaintenance>().MigrateAsync());

        Assert.Equal(Document(Content), await LoadedDocumentAsync(database));
    }

    /// <summary>
    /// Each case damages the content in one way the schema must refuse. The write is all or nothing, so after every
    /// refusal the previous content must still be there, untouched.
    /// </summary>
    [Theory]
    [InlineData("a project status that is not a status")]
    [InlineData("a second centre star")]
    [InlineData("a journey step for a star that does not exist")]
    [InlineData("a line from a star to itself")]
    [InlineData("a star visited twice on the journey")]
    [InlineData("a social icon with both a logo and its own drawing")]
    [InlineData("a social icon that points at a logo that does not exist")]
    [InlineData("a negative hero logo count")]
    [InlineData("a technology logo with a color that is not a color")]
    [InlineData("a code lane that takes no time")]
    [InlineData("a second row in a one-row table")]
    [InlineData("two projects with the same id")]
    [InlineData("a name longer than its column")]
    public async Task The_schema_refuses_content_that_makes_no_sense_and_keeps_the_old_content(string damage)
    {
        await using var database = await TestDatabase.CreateAsync();
        await SeedAsync(database);
        var before = await LoadedDocumentAsync(database);

        var rows = ContentMapper.ToRows(Content);
        Damage(rows, damage);

        await Assert.ThrowsAnyAsync<Exception>(() =>
            database.InScopeAsync(provider => provider.GetRequiredService<IContentRepository>().ReplaceAsync(rows)));

        Assert.Equal(before, await LoadedDocumentAsync(database));
    }

    private static void Damage(ContentRows rows, string damage)
    {
        switch (damage)
        {
            case "a project status that is not a status":
                rows.Projects[0].Status = "finished";
                break;
            case "a second centre star":
                rows.SkillStars[1].ParentId = null;
                break;
            case "a journey step for a star that does not exist":
                rows.JourneySteps.Add(new JourneyStepRow { SortOrder = 9999, StarId = "no-such-star" });
                break;
            case "a line from a star to itself":
                rows.SkillLinks.Add(new SkillLinkRow { SortOrder = 9999, FromId = "dotnet", ToId = "dotnet" });
                break;
            case "a star visited twice on the journey":
                rows.JourneySteps.Add(new JourneyStepRow { SortOrder = 9999, StarId = rows.JourneySteps[0].StarId });
                break;
            case "a social icon with both a logo and its own drawing":
                rows.SocialIcons[0].Markup = "<path d=\"M0 0\"/>";
                break;
            case "a social icon that points at a logo that does not exist":
                rows.SocialIcons[0].LogoKey = "No Such Logo";
                break;
            case "a negative hero logo count":
                rows.Site.HeroLogoCount = -1;
                break;
            case "a technology logo with a color that is not a color":
                rows.TechLogos[0].Hex = "red";
                break;
            case "a code lane that takes no time":
                rows.CodeStreams[0].Seconds = 0;
                break;
            case "a second row in a one-row table":
                rows.Site.Id = 2;
                break;
            case "two projects with the same id":
                rows.Projects.Add(new ProjectRow { Id = rows.Projects[0].Id, SortOrder = 9999, Status = "reference" });
                break;
            case "a name longer than its column":
                rows.Profile.Name = new string('x', 500);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(damage), damage, "No such damage is defined for this test.");
        }
    }
}
