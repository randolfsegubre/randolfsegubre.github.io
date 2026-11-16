using Portfolio.Business.Content;

namespace Portfolio.Tests;

/// <summary>
/// Tests for reading the JSON data files (ADR-0012). The files are the source of truth, so reading them is strict:
/// a typo or a hole must stop the build, never pass quietly.
/// </summary>
public sealed class ContentFilesTests
{
    [Fact]
    public void Every_data_file_loads_and_the_content_is_not_empty()
    {
        var content = TestContent.Snapshot;

        Assert.NotEmpty(content.Projects);
        Assert.NotEmpty(content.LiveSites);
        Assert.NotEmpty(content.Experience);
        Assert.NotEmpty(content.Skills);
        Assert.NotEmpty(content.Universe.Stars);
        Assert.NotEmpty(content.TechLogos);
        Assert.NotEmpty(content.SkillIcons);
        Assert.NotEmpty(content.SocialIcons);
        Assert.NotEmpty(content.HeroTags);
        Assert.NotEmpty(content.CodeStreams);
        Assert.False(string.IsNullOrWhiteSpace(content.Site.Url));
    }

    [Fact]
    public async Task A_field_name_that_matches_nothing_is_an_error_naming_the_file()
    {
        using var folder = TempContentFolder.Copy();
        folder.Replace("projects.json", "\"tagline\"", "\"taglin\"");

        var error = await Assert.ThrowsAsync<InvalidDataException>(() => ContentFiles.LoadAsync(folder.Path));

        Assert.Contains("projects.json", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_null_where_a_value_is_required_is_an_error_naming_the_file()
    {
        using var folder = TempContentFolder.Copy();
        folder.Replace("profile.json", "\"email\": \"rsegubre@gmail.com\"", "\"email\": null");

        var error = await Assert.ThrowsAsync<InvalidDataException>(() => ContentFiles.LoadAsync(folder.Path));

        Assert.Contains("profile.json", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_file_that_is_not_valid_JSON_is_an_error_naming_the_file()
    {
        using var folder = TempContentFolder.Copy();
        folder.Replace("skills.json", "[", "[ oops");

        var error = await Assert.ThrowsAsync<InvalidDataException>(() => ContentFiles.LoadAsync(folder.Path));

        Assert.Contains("skills.json", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_missing_file_is_an_error_and_not_an_empty_section()
    {
        using var folder = TempContentFolder.Copy();
        File.Delete(Path.Combine(folder.Path, "experience.json"));

        var error = await Assert.ThrowsAsync<FileNotFoundException>(() => ContentFiles.LoadAsync(folder.Path));

        Assert.Contains("experience.json", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_broken_skills_tree_stops_the_load_with_the_layout_error()
    {
        using var folder = TempContentFolder.Copy();
        folder.Replace("skill-universe.json", "\"parentId\": \"dotnet\"", "\"parentId\": \"nobody\"");

        await Assert.ThrowsAsync<InvalidOperationException>(() => ContentFiles.LoadAsync(folder.Path));
    }
}
