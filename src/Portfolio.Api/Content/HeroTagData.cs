namespace Portfolio.Api.Content;

/// <summary>
/// The faint code tags in the hero's side margins (ADR-0010).
/// </summary>
/// <remarks>
/// They are decoration: Razor and C# fragments that make the page look like it is showing its own
/// source. They make no claim about the owner beyond what the page already says, which is why the
/// last one repeats his stated availability rather than a technology or a version. The stylesheet
/// positions them by their order (<c>hero-tag-1</c> to <c>hero-tag-6</c>), so keep exactly six.
/// </remarks>
public static class HeroTagData
{
    /// <summary>How many tags the stylesheet has positions for.</summary>
    public const int Count = 6;

    public static IReadOnlyList<string> Tags { get; } =
    [
        "@page \"/\"",
        "<h1>",
        "</h1>",
        "<p>",
        "@model Developer",
        "// remote preferred",
    ];
}
