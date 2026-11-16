namespace Portfolio.Business.Models;

/// <summary>
/// Everything the front end needs to draw the whole site, as one JSON document (ADR-0011).
/// </summary>
/// <remarks>
/// This is the contract between the .NET backend and the Nuxt front end. It holds the content records
/// as they are (the claims, with their sources in <c>docs/CONTENT_SOURCES.md</c>) plus the few values that
/// the old Razor views worked out while rendering, so the front end only draws and never decides:
/// the resolved icon for each skill sphere, the slots for the floating logos, the status wording of a
/// project card and the page titles. The front end is not trusted to hold a claim and the backend is not
/// asked to know about markup.
/// </remarks>
/// <param name="Meta">Page titles, descriptions and the absolute address used for sharing.</param>
/// <param name="Profile">Who he is, with contact details.</param>
/// <param name="ProfileLinks">His profile links with the icon each one shows (GitHub, LinkedIn), when there is one.</param>
/// <param name="LiveSites">Public sites he contributed to.</param>
/// <param name="Projects">Project cards, each with its status wording already chosen.</param>
/// <param name="Experience">Employment history, newest first.</param>
/// <param name="Skills">The full grouped skills list.</param>
/// <param name="Universe">The skills cloud: stars, lines and the walk order.</param>
/// <param name="HeroTags">The faint code tags in the hero's margins.</param>
/// <param name="Logos">Every technology logo, drawn once in a hidden sprite.</param>
/// <param name="SkillIcons">The skills cloud's own icons, drawn once in a second hidden sprite.</param>
/// <param name="HeroLogos">Where the floating logos inside the hero go.</param>
/// <param name="PageLogos">Where the floating logos fixed behind the rest of the page go.</param>
/// <param name="CodeStreams">The lanes of scrolling code behind the page.</param>
public sealed record SiteContent(
    SiteMeta Meta,
    Profile Profile,
    IReadOnlyList<ProfileLink> ProfileLinks,
    IReadOnlyList<LiveSite> LiveSites,
    IReadOnlyList<ProjectCard> Projects,
    IReadOnlyList<Role> Experience,
    IReadOnlyList<SkillGroup> Skills,
    UniverseView Universe,
    IReadOnlyList<string> HeroTags,
    IReadOnlyList<LogoSymbol> Logos,
    IReadOnlyList<LogoSymbol> SkillIcons,
    IReadOnlyList<LogoSlot> HeroLogos,
    IReadOnlyList<LogoSlot> PageLogos,
    IReadOnlyList<CodeLane> CodeStreams);

/// <summary>Titles, descriptions and addresses for the head of each page.</summary>
/// <param name="SiteUrl">The site's address with no trailing slash.</param>
/// <param name="HomeTitle">The title of the home page.</param>
/// <param name="NotFoundTitle">The title of the not-found page.</param>
/// <param name="Description">The search-result description.</param>
/// <param name="ShareDescription">The description link previews show.</param>
/// <param name="ShareImageUrl">The absolute address of the preview image, or null when there is no portrait.</param>
/// <param name="ShareImageAlt">The description of the preview image.</param>
/// <param name="Year">The year shown in the footer, fixed when the site is built.</param>
public sealed record SiteMeta(
    string SiteUrl,
    string HomeTitle,
    string NotFoundTitle,
    string Description,
    string ShareDescription,
    string? ShareImageUrl,
    string? ShareImageAlt,
    int Year);

/// <summary>A profile link with the icon it shows, when the site has one for it.</summary>
/// <param name="Label">The profile's name, for example "GitHub".</param>
/// <param name="Href">Where it goes.</param>
/// <param name="IconMarkup">The shapes inside a 24 by 24 view box, or null.</param>
public sealed record ProfileLink(string Label, string Href, string? IconMarkup);

/// <summary>A project with the wording and style hook of its status chosen on the server.</summary>
/// <param name="Project">The project.</param>
/// <param name="StatusLabel">The words on the status badge.</param>
/// <param name="StatusClass">The style hook of the badge.</param>
public sealed record ProjectCard(Project Project, string StatusLabel, string StatusClass);

/// <summary>The skills cloud as the front end needs it.</summary>
/// <param name="Intro">The line above the cloud.</param>
/// <param name="Journey">The order the walk visits the stars.</param>
/// <param name="Links">Every line between stars.</param>
/// <param name="Stars">Every star, with its place and the icon it draws.</param>
public sealed record UniverseView(
    string Intro,
    IReadOnlyList<string> Journey,
    IReadOnlyList<SkillLink> Links,
    IReadOnlyList<StarView> Stars);

/// <summary>One sphere of the cloud with its icon already resolved.</summary>
/// <param name="Id">Stable identifier, used by the script and the walk order.</param>
/// <param name="Label">The technology's name.</param>
/// <param name="Caption">One honest sentence about where he used it.</param>
/// <param name="Main">True for his main stack, which is drawn larger.</param>
/// <param name="Depth">Steps from the centre.</param>
/// <param name="Azimuth">Where it sits round the cloud, in degrees.</param>
/// <param name="Elevation">How far above or below the middle, in degrees.</param>
/// <param name="Radius">Distance from the centre in world units.</param>
/// <param name="IconSymbolId">The id of the sprite symbol it draws.</param>
/// <param name="IconFill">The brand colour of the icon, or null for a line glyph.</param>
/// <param name="OnDark">True when the sphere needs the dark variant.</param>
public sealed record StarView(
    string Id,
    string Label,
    string Caption,
    bool Main,
    int Depth,
    double Azimuth,
    double Elevation,
    double Radius,
    string IconSymbolId,
    string? IconFill,
    bool OnDark);

/// <summary>One drawing in a hidden SVG sprite.</summary>
/// <param name="Key">The technology name or icon key.</param>
/// <param name="SymbolId">The id the sprite gives it.</param>
/// <param name="Markup">The shapes inside a 24 by 24 view box. Trusted, static markup written by the site's author.</param>
public sealed record LogoSymbol(string Key, string SymbolId, string Markup);

/// <summary>Where one floating logo goes.</summary>
/// <param name="Key">The technology it shows.</param>
/// <param name="SymbolId">The sprite symbol to draw.</param>
/// <param name="Edge">True when it stays on phones, where only a few are shown.</param>
/// <param name="Style">The inline custom properties that place and time it.</param>
public sealed record LogoSlot(string Key, string SymbolId, bool Edge, string Style);

/// <summary>One lane of scrolling code.</summary>
/// <param name="Code">The code shown in the lane.</param>
/// <param name="Seconds">How long one loop takes.</param>
/// <param name="Reverse">True when the lane scrolls the other way.</param>
public sealed record CodeLane(string Code, int Seconds, bool Reverse);
