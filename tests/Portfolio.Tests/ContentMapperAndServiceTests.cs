using Portfolio.Business.Content;
using Portfolio.Business.Services;
using Portfolio.Data;

namespace Portfolio.Tests;

/// <summary>
/// Tests for the business layer's translation between rows and content (ADR-0013), and for the service that uses it.
/// No database: the data layer is replaced by an in-memory repository, which is what the repository interface is for.
/// </summary>
public sealed class ContentMapperAndServiceTests
{
    private static readonly ContentSnapshot Content = TestContent.Snapshot;

    private static string Document(IPortfolioContent content) => SiteContentBuilder.ToJson(SiteContentBuilder.Build(content, 2026));

    [Fact]
    public void A_trip_to_rows_and_back_loses_nothing()
    {
        var back = ContentMapper.ToSnapshot(ContentMapper.ToRows(Content));

        // The document is what gets published, so equal documents mean the page would be identical.
        Assert.Equal(Document(Content), Document(back));
    }

    [Fact]
    public void Every_list_counts_its_positions_from_one_without_gaps()
    {
        var rows = ContentMapper.ToRows(Content);

        static void Consecutive(IEnumerable<int> orders) => Assert.Equal(Enumerable.Range(1, orders.Count()), orders);

        Consecutive(rows.ProfileLinks.Select(r => r.SortOrder));
        Consecutive(rows.ContactChannels.Select(r => r.SortOrder));
        Consecutive(rows.Stats.Select(r => r.SortOrder));
        Consecutive(rows.Projects.Select(r => r.SortOrder));
        Consecutive(rows.LiveSites.Select(r => r.SortOrder));
        Consecutive(rows.Roles.Select(r => r.SortOrder));
        Consecutive(rows.SkillGroups.Select(r => r.SortOrder));
        Consecutive(rows.SkillStars.Select(r => r.SortOrder));
        Consecutive(rows.SkillLinks.Select(r => r.SortOrder));
        Consecutive(rows.JourneySteps.Select(r => r.SortOrder));
        Consecutive(rows.HeroTags.Select(r => r.SortOrder));
        Consecutive(rows.CodeStreams.Select(r => r.SortOrder));
        Consecutive(rows.TechLogos.Select(r => r.SortOrder));
        Consecutive(rows.SkillIcons.Select(r => r.SortOrder));
        Consecutive(rows.SocialIcons.Select(r => r.SortOrder));
        Assert.All(rows.Projects, project => Consecutive(project.Links.Select(l => l.SortOrder)));
    }

    [Fact]
    public void Only_the_cross_links_are_stored_because_the_lines_to_parents_follow_from_the_tree()
    {
        var rows = ContentMapper.ToRows(Content);

        var parentLines = Content.Universe.Stars.Count(p => p.Star.ParentId is not null);
        Assert.Equal(Content.Universe.Links.Count - parentLines, rows.SkillLinks.Count);
        Assert.All(rows.SkillLinks, link =>
            Assert.DoesNotContain(rows.SkillStars, s => s.ParentId == link.FromId && s.Id == link.ToId));
    }

    [Fact]
    public void A_status_the_database_does_not_know_is_an_error_and_not_a_guess()
    {
        var rows = ContentMapper.ToRows(Content);
        rows.Projects[0].Status = "finished";

        Assert.Throws<InvalidOperationException>(() => ContentMapper.ToSnapshot(rows));
    }

    [Fact]
    public async Task Loading_from_a_database_that_was_never_seeded_says_how_to_seed_it()
    {
        var service = new ContentService(new InMemoryContentRepository());

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => service.LoadAsync());

        Assert.Contains("--seed", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Seeding_from_the_files_stores_what_the_files_say_and_loading_returns_it()
    {
        var repository = new InMemoryContentRepository();
        var service = new ContentService(repository);

        await service.SeedFromFilesAsync(TestContent.Folder);
        var loaded = await service.LoadAsync();

        Assert.Equal(1, repository.ReplaceCount);
        Assert.Equal(Document(Content), Document(loaded));
    }

    [Fact]
    public async Task A_mistake_in_the_files_is_found_before_the_database_is_touched()
    {
        var repository = new InMemoryContentRepository();
        var service = new ContentService(repository);
        using var folder = TempContentFolder.Copy();
        folder.Replace("profile.json", "\"name\": \"Randolf Segubre\"", "\"name\": null");

        await Assert.ThrowsAsync<InvalidDataException>(() => service.SeedFromFilesAsync(folder.Path));

        Assert.Equal(0, repository.ReplaceCount);
    }

    [Fact]
    public async Task Migrating_is_passed_down_to_the_data_layer()
    {
        var repository = new InMemoryContentRepository();

        await new ContentService(repository).MigrateAsync();

        Assert.True(repository.Migrated);
    }
}

/// <summary>A repository that keeps its rows in memory, so the business layer can be tested without a database.</summary>
internal sealed class InMemoryContentRepository : IContentRepository
{
    public ContentRows? Rows { get; private set; }

    public int ReplaceCount { get; private set; }

    public bool Migrated { get; private set; }

    public Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        Migrated = true;
        return Task.CompletedTask;
    }

    public Task<ContentRows?> ReadAsync(CancellationToken cancellationToken = default) => Task.FromResult(Rows);

    public Task ReplaceAsync(ContentRows rows, CancellationToken cancellationToken = default)
    {
        Rows = rows;
        ReplaceCount++;
        return Task.CompletedTask;
    }
}

/// <summary>A throwaway copy of the content folder that a test can damage, and that is deleted afterwards.</summary>
internal sealed class TempContentFolder : IDisposable
{
    private TempContentFolder(string path) => Path = path;

    public string Path { get; }

    public static TempContentFolder Copy()
    {
        var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "portfolio-content-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        foreach (var file in Directory.EnumerateFiles(TestContent.Folder, "*.json"))
        {
            File.Copy(file, System.IO.Path.Combine(path, System.IO.Path.GetFileName(file)));
        }

        return new TempContentFolder(path);
    }

    /// <summary>Replaces the first occurrence of some text in one of the copied files, and fails if the text is not there.</summary>
    public void Replace(string file, string from, string to)
    {
        var full = System.IO.Path.Combine(Path, file);
        var text = File.ReadAllText(full);
        var index = text.IndexOf(from, StringComparison.Ordinal);
        Assert.True(index >= 0, $"'{from}' is not in {file}, so the test cannot damage it.");
        File.WriteAllText(full, text.Remove(index, from.Length).Insert(index, to));
    }

    public void Dispose() => Directory.Delete(Path, recursive: true);
}
