using System.Text.Json;
using System.Text.Json.Serialization;
using Portfolio.Business.Models;

namespace Portfolio.Business.Content;

/// <summary>
/// Reads the site's content from the JSON data files under <c>content/</c> (ADR-0012).
/// </summary>
/// <remarks>
/// The files are the content's source of truth: reviewable, versioned, and the place every claim is edited. They
/// are loaded into SQL Server by the seeder, and the tests also read them directly, so the writing rules are
/// checked on the same words the database will hold. Reading is strict on purpose: a field name that does not match
/// (a typo such as "hightlights") is an error, not something silently ignored, and a null where the model needs a
/// value is an error too.
/// </remarks>
public static class ContentFiles
{
    /// <summary>The file names, one per kind of content. See <c>content/README.md</c>.</summary>
    private static class Names
    {
        public const string Site = "site.json";
        public const string Profile = "profile.json";
        public const string LiveSites = "live-sites.json";
        public const string Projects = "projects.json";
        public const string Experience = "experience.json";
        public const string Skills = "skills.json";
        public const string SkillUniverse = "skill-universe.json";
        public const string HeroTags = "hero-tags.json";
        public const string CodeStreams = "code-streams.json";
        public const string TechLogos = "tech-logos.json";
        public const string SkillIcons = "skill-icons.json";
        public const string SocialIcons = "social-icons.json";
    }

    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        RespectNullableAnnotations = true,
        AllowTrailingCommas = false,
        ReadCommentHandling = JsonCommentHandling.Disallow,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    /// <summary>
    /// Loads every file in the folder into one snapshot, laying out the skills map as it goes.
    /// </summary>
    /// <param name="folder">The folder that holds the JSON files.</param>
    /// <param name="cancellationToken">Cancels the reading.</param>
    /// <exception cref="FileNotFoundException">A file is missing.</exception>
    /// <exception cref="InvalidDataException">A file is not valid for its model, naming the file.</exception>
    public static async Task<ContentSnapshot> LoadAsync(string folder, CancellationToken cancellationToken = default)
    {
        var universe = await ReadAsync<UniverseSource>(folder, Names.SkillUniverse, cancellationToken);

        return new ContentSnapshot(
            Site: await ReadAsync<SiteSettings>(folder, Names.Site, cancellationToken),
            Profile: await ReadAsync<Profile>(folder, Names.Profile, cancellationToken),
            LiveSites: await ReadAsync<List<LiveSite>>(folder, Names.LiveSites, cancellationToken),
            Projects: await ReadAsync<List<Project>>(folder, Names.Projects, cancellationToken),
            Experience: await ReadAsync<List<Role>>(folder, Names.Experience, cancellationToken),
            Skills: await ReadAsync<List<SkillGroup>>(folder, Names.Skills, cancellationToken),
            Universe: SkillUniverseLayout.Build(universe.Intro, universe.Stars, universe.CrossLinks, universe.Journey),
            HeroTags: await ReadAsync<List<string>>(folder, Names.HeroTags, cancellationToken),
            CodeStreams: await ReadAsync<List<CodeStream>>(folder, Names.CodeStreams, cancellationToken),
            TechLogos: await ReadAsync<List<TechLogo>>(folder, Names.TechLogos, cancellationToken),
            SkillIcons: await ReadAsync<List<SkillIcon>>(folder, Names.SkillIcons, cancellationToken),
            SocialIcons: await ReadAsync<List<SocialIcon>>(folder, Names.SocialIcons, cancellationToken));
    }

    /// <summary>
    /// Reads and checks one file.
    /// </summary>
    private static async Task<T> ReadAsync<T>(string folder, string fileName, CancellationToken cancellationToken)
    {
        var path = Path.Combine(folder, fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The content file '{fileName}' is missing from '{folder}'.", path);
        }

        try
        {
            var json = await File.ReadAllTextAsync(path, cancellationToken);
            return JsonSerializer.Deserialize<T>(json, Options)
                ?? throw new InvalidDataException($"The content file '{fileName}' is empty or null.");
        }
        catch (JsonException error)
        {
            throw new InvalidDataException($"The content file '{fileName}' is not valid: {error.Message}", error);
        }
    }
}
