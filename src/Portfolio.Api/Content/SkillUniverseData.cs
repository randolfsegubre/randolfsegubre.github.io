using Portfolio.Api.Models;

namespace Portfolio.Api.Content;

/// <summary>
/// The skills sphere cloud: what hangs from what, and one honest sentence per technology (ADR-0010).
/// </summary>
/// <remarks>
/// .NET is the centre and everything else connects back to it: the web framework and the pages and
/// services it serves, the data access, the delivery tools, the language and its tests, the older
/// desktop and Framework work and the developer tools.
/// <para>
/// <b>Only technologies the owner has used himself.</b> Every sphere is a technology or tool he has
/// worked with directly, at work or in his own projects. It is deliberately not the whole of the
/// grouped skills list or every entry of every project stack: a technology that only appears in a
/// project the owner built with AI help and never worked with himself (TanStack Query, Azure Service
/// Bus, TypeScript and similar) has no sphere, and neither do practices (code review, documentation)
/// or techniques, which are not a stack. The owner confirmed this rule on 2026-11-15 after checking
/// his own record; a test names the exclusions so they cannot creep back in.
/// </para>
/// The five stars marked <c>Main</c> are the owner's main stack (the profile's signature stack) and are drawn as larger
/// spheres; every other technology is drawn smaller, so the main stack reads first (owner's request, 2026-11-15).
/// Each star label must appear in a role, project or the grouped skills list, and each caption must be
/// backed by an entry in <c>docs/CONTENT_SOURCES.md</c> (ADR-0006); tests check the first, the second is a
/// review rule. Stars are listed branch by branch, because the layout spreads each sphere in this order and
/// keeps a branch together. Captions are prose, so they follow the writing rules: no em dashes,
/// abbreviations in full words.
/// </remarks>
public static class SkillUniverseData
{
    private const string Intro = "Everything here connects back to .NET, and everything here is something I have used myself, at work or in my own projects. Choose a sphere to see where, drag the cloud to turn it, or walk the journey from the centre outward.";

    private static readonly IReadOnlyList<SkillStar> Stars =
    [
        new SkillStar("dotnet", ".NET", null, "The centre of the map. More than 9 years of C# and .NET, and everything else here connects back to it.", ".NET 10"),

        // Branch 1: the web framework, the services, pages and content platform it serves, and the front end on top.
        new SkillStar("aspnet-core", "ASP.NET Core", "dotnet", "Web platforms and APIs, including moving a codebase from ASP.NET Core 2.x to 6 at Accenture.", "globe", Main: true),
        new SkillStar("web-api", "Web API", "aspnet-core", "Backend services for client platforms and for my own projects.", "braces"),
        new SkillStar("vuejs", "Vue.js", "web-api", "Front-end work on top of the backend, in two of my roles.", "Vue.js", Main: true),
        new SkillStar("javascript", "JavaScript", "vuejs", "Behind the Vue.js and Umbraco front-end work.", "javascript"),
        new SkillStar("nuxtjs", "Nuxt.js", "vuejs", "The framework built on Vue.js; this portfolio is generated with it.", "nuxt"),
        new SkillStar("react", "React", "web-api", "About six months of React and Redux work in 2020, and the front end of my Lakbay project.", "React"),
        new SkillStar("redux", "Redux", "react", "State handling for the React work in 2020, and Redux Toolkit in my Lakbay project.", "redux"),
        new SkillStar("nextjs", "Next.js", "react", "The headless front end of my Lakbay project.", "Next.js"),
        new SkillStar("mongodb", "MongoDB", "web-api", "Document storage, in an earlier role and in the Lakbay project.", "MongoDB"),
        new SkillStar("jwt", "JWT authentication", "web-api", "Sign-in with access and refresh tokens, in my own BudgetPH and AI Tutor projects.", "jwt"),
        new SkillStar("aspnet-mvc", "ASP.NET MVC", "aspnet-core", "Controllers and views, the pattern of the ASP.NET web applications in my earlier roles.", "layout"),
        new SkillStar("html-css", "HTML and CSS", "aspnet-mvc", "The markup and styling behind the pages I build, this portfolio included.", "html"),
        new SkillStar("scss", "SCSS", "html-css", "Styling for the customer-facing sites in my latest role.", "sass"),
        new SkillStar("razor-pages", "Razor Pages", "aspnet-core", "Page-based web applications at Accenture.", "code"),
        new SkillStar("blazor", "Blazor", "aspnet-core", "The Blazor WebAssembly front end of AI Tutor, and the interface of DevHub.", "Blazor"),
        new SkillStar("ollama", "Ollama", "blazor", "The local language model behind my AI Tutor project, running on my own machine.", "ollama"),
        new SkillStar("umbraco", "Umbraco", "aspnet-core", "Content platform work on the customer-facing sites, including editing tools for the editors.", "Umbraco", Main: true),
        new SkillStar("angularjs", "AngularJS", "umbraco", "The framework I built Umbraco backoffice property editors in, for editors on two content platforms.", "angularjs"),
        new SkillStar("security-headers", "Security headers", "aspnet-core", "Content Security Policy hardening: I took the initiative on removing unsafe-inline and unsafe-eval on the customer-facing sites.", "shield"),
        new SkillStar("csp", "Content Security Policy", "security-headers", "Removing unsafe-inline and unsafe-eval and adding per-request nonces on two customer-facing sites.", "lock"),
        new SkillStar("trusted-types", "Trusted Types", "security-headers", "Adopted as part of the same Content Security Policy hardening.", "shield"),
        new SkillStar("owasp-zap", "OWASP ZAP", "security-headers", "Security scans of sites, including checks for missing security headers.", "owasp"),

        // Branch 2: delivery and hosting.
        new SkillStar("docker", "Docker", "dotnet", "Containers, part of the delivery setup in my latest role.", "Docker"),
        new SkillStar("jenkins", "Jenkins", "docker", "Build and deploy pipelines in my latest role.", "Jenkins"),
        new SkillStar("azure", "Azure", "docker", "Diagnosing a layered Azure hosting setup to find which layer produced a response header.", "cloud"),
        new SkillStar("azure-devops", "Azure DevOps", "azure", "Where some legacy projects were hosted at Accenture, formerly Team Foundation Server.", "cloud"),
        new SkillStar("aws", "AWS", "docker", "Hosting for the airline booking tools at Scoot, on Amazon Web Services.", "cloud"),

        // Branch 3: data access and databases.
        new SkillStar("ef-core", "EF Core", "dotnet", "Entity Framework Core with Code First, on MySQL, for the Scoot work.", "table"),
        new SkillStar("sql-server", "SQL Server", "ef-core", "Relational data work across my roles, from my first desktop application onward.", "database", Main: true),
        new SkillStar("t-sql", "T-SQL", "sql-server", "The query language I use with SQL Server.", "query"),
        new SkillStar("dapper", "Dapper", "sql-server", "Data access in my first professional role, a desktop certificate application at F. Gurrea.", "bolt"),
        new SkillStar("mysql", "MySQL", "ef-core", "The database behind the Code First work for Scoot.", "MySQL"),

        // Branch 4: the language, its tests and the libraries built on it.
        new SkillStar("csharp", "C#", "dotnet", "The language I have worked in professionally since 2017.", "C#", Main: true),
        new SkillStar("xunit", "xUnit", "csharp", "Unit tests, written at Scoot toward a team target of at least 90 percent coverage.", "check"),
        new SkillStar("mediatr", "MediatR", "csharp", "Command and query handling in my Lakbay booking service and in AI Tutor.", "flow"),
        new SkillStar("clean-architecture", "Clean Architecture", "csharp", "The layered structure of BudgetPH and AI Tutor.", "layers"),
        new SkillStar("unity", "Unity", "csharp", "Galaxy Survivor, a mobile game I specified documentation-first.", "unity"),

        // Branch 5: the older desktop and Framework work.
        new SkillStar("dotnet-framework", ".NET Framework", "dotnet", "Legacy platform work, including the upgrade from version 4.6 to 4.8 at Collabera.", "history"),
        new SkillStar("wpf", "WPF", "dotnet-framework", "Maintained a legacy Windows Presentation Foundation desktop companion app at Collabera.", "window"),
        new SkillStar("winforms", "WinForms", "dotnet-framework", "The Windows desktop application of my first professional role, at F. Gurrea.", "window"),

        // Branch 6: developer tools.
        new SkillStar("git", "Git and GitHub", "dotnet", "Version control for all my work, with GitHub for review and code scanning.", "GitHub"),
        new SkillStar("copilot", "GitHub Copilot", "git", "In use since 2022, when Accenture began encouraging its adoption.", "copilot"),
        new SkillStar("claude-code", "Claude Code", "git", "Part of my own workflow, following the same shared conventions as Copilot.", "claude-code"),
        new SkillStar("mcp", "Model Context Protocol configuration", "claude-code", "Model Context Protocol servers set up for my own workflow; I configured existing ones and did not write one.", "mcp"),
    ];

    /// <summary>Lines between related technologies that are not parent and child: C# is what ASP.NET Core and EF Core are written in, and JavaScript is what the front-end frameworks are written in.</summary>
    private static readonly IReadOnlyList<SkillLink> CrossLinks =
    [
        new SkillLink("csharp", "aspnet-core"),
        new SkillLink("csharp", "ef-core"),
        // JavaScript sits under Vue.js in the tree, but it is also what React and AngularJS are written in.
        new SkillLink("javascript", "react"),
        new SkillLink("javascript", "angularjs"),
    ];

    /// <summary>The order the walk visits the stars, roughly the order the work grew in.</summary>
    private static readonly IReadOnlyList<string> Journey =
    [
        "dotnet", "csharp", "winforms", "dapper", "sql-server", "t-sql", "aspnet-core", "aspnet-mvc", "html-css", "scss",
        "web-api", "ef-core", "mysql", "xunit", "vuejs", "javascript", "nuxtjs", "react", "redux", "mongodb", "dotnet-framework",
        "wpf", "razor-pages", "git", "azure-devops", "umbraco", "angularjs", "security-headers", "csp", "trusted-types",
        "owasp-zap", "jwt", "docker", "jenkins", "azure", "aws", "blazor", "ollama", "mediatr", "clean-architecture",
        "nextjs", "unity", "copilot", "claude-code", "mcp",
    ];

    /// <summary>The laid-out map, computed once. The data is immutable, so one shared instance is safe.</summary>
    public static UniverseMap Map { get; } = SkillUniverseLayout.Build(Intro, Stars, CrossLinks, Journey);
}
