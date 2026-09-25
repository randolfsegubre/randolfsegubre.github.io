using Portfolio.Web.Models;

namespace Portfolio.Web.Content;

/// <summary>
/// Portfolio project cards, featured first (ADR-0003, ADR-0006).
/// </summary>
/// <remarks>
/// Every claim here has a source in <c>docs/CONTENT_SOURCES.md</c>. Statuses and
/// known gaps are stated plainly on purpose. Only public repositories are
/// linked; Galaxy Survivor is private, so it has no link.
/// </remarks>
public static class ProjectData
{
    public static IReadOnlyList<Project> Projects { get; } =
    [
        new Project(
            Id: "budgetph",
            Name: "BudgetPH",
            Tagline: "Personal and family finance manager for Filipino users and overseas workers",
            Summary: "A full-stack web application for tracking accounts, transactions, budgets, savings goals and loans, with Philippine pesos as the primary currency. The backend is a .NET 10 Clean Architecture solution and the front end is a React app, in one repository.",
            Role: "Solo personal project, backend and front end.",
            Status: ProjectStatus.InDevelopment,
            Featured: true,
            Highlights:
            [
                "Real JavaScript Object Notation Web Token (JWT) login with silent refresh through an axios interceptor, against an ASP.NET Core Application Programming Interface with an explicit cross-origin allow-list.",
                "Front end uses TanStack Query for server state and Zustand for client state, with a development proxy and a same-origin production build.",
                "Audited end to end in a real browser against a real SQL Server database: register, log in, create accounts, post transactions, and watch the dashboard update.",
                "That audit found and fixed three contract bugs by reproducing them first, including AutoMapper failing to build a record through its constructor.",
            ],
            HonestNote: "Automated test coverage is still small (one domain test and one application test).",
            Stack: ["C#", ".NET 10", "ASP.NET Core", "EF Core", "MediatR", "SQL Server", "React", "Vite", "TanStack Query", "Zustand"],
            Links: [new Link("Source on GitHub", "https://github.com/randolfsegubre/BudgetPH")]),

        new Project(
            Id: "ophir",
            Name: "Ophir Mineral Ventures website",
            Tagline: "Corporate website for a Philippines-based nickel and chromite ore exporter",
            Summary: "A freelance client project: a compliance-first company website that the owner can edit himself. Built on Umbraco 18 and .NET 10 with a bilingual English and Simplified Chinese content model.",
            Role: "Solo freelance project, from proposal to build.",
            Status: ProjectStatus.InDevelopment,
            Featured: true,
            Highlights:
            [
                "A hand-built contact form with anti-forgery tokens, a honeypot field and rate limiting, instead of a paid forms package.",
                "The 404 page is a real, editable content node so the client can rewrite it without a code change, while the 500 page is a deliberate hardcoded fallback for when the content system itself is what broke.",
                "Documentation first: build plan, architecture decision records and a session-by-session devlog, so any future session can pick the project up cold.",
                "Verified in a real browser in both languages, with backoffice login and both error pages confirmed against a seeded database.",
            ],
            HonestNote: "Hosting trial and go-live are still ahead, so the site is not public yet.",
            Stack: ["Umbraco 18", ".NET 10", "ASP.NET Core", "Razor", "xUnit"],
            Links: [new Link("Source on GitHub", "https://github.com/randolfsegubre/OphirMineralVentures")]),

        new Project(
            Id: "lakbay",
            Name: "Lakbay",
            Tagline: "A Philippines-first holiday-booking platform, built as a real multi-repository system",
            Summary: "A holiday-booking platform deliberately built as several cooperating services rather than one demo app: an Umbraco content system, a headless storefront, a booking service, and a search service kept in sync over messaging. The architecture is documented in a dedicated repository.",
            Role: "Solo personal project, architecture and build.",
            Status: ProjectStatus.InDevelopment,
            Featured: true,
            Highlights:
            [
                "A booking service using Command Query Responsibility Segregation, with an atomic availability decrement so two people cannot book the last room.",
                "A denormalised search service fed from the content system over Azure Service Bus.",
                "Eight repositories, one architecture repository with numbered decision records, and a documented local infrastructure setup.",
                "The early phases were verified running against real services on a local machine, not only by reading the code.",
            ],
            HonestNote: "Checkout and payment are not built yet, and the storefront does not call the booking service yet.",
            Stack: ["C#", ".NET", "Umbraco 18", "Next.js", "GraphQL", "Azure Service Bus", "MongoDB", "MediatR"],
            Links: [new Link("Architecture repository", "https://github.com/randolfsegubre/Lakbay.Docs")]),

        new Project(
            Id: "ai-tutor",
            Name: "AI Tutor",
            Tagline: "A local, offline study app for senior .NET assessment preparation",
            Summary: "Chat with a local language model, work through curated study topics, take mock exams and revisit past sessions, all running on the owner's own machine with no cloud model costs.",
            Role: "Solo personal project.",
            Status: ProjectStatus.Reference,
            Featured: false,
            Highlights:
            [
                "Blazor WebAssembly front end on an ASP.NET Core 10 Application Programming Interface in Clean Architecture layers, with Semantic Kernel talking to a local Ollama model.",
                "Built from scratch: sign-in with access and refresh tokens, and an encrypted vault for premium provider keys.",
            ],
            HonestNote: null,
            Stack: ["Blazor WebAssembly", "ASP.NET Core 10", "EF Core", "SQLite", "Semantic Kernel", "Ollama"],
            Links: [new Link("Source on GitHub", "https://github.com/randolfsegubre/ai-tutor")]),

        new Project(
            Id: "devhub",
            Name: "DevHub",
            Tagline: "A local-network dashboard for my development drive",
            Summary: "Scans the drive, sorts what it finds into projects, Windows apps and mobile apps, and lets me browse files, grab an installer, or install an Android app from a browser on the local network.",
            Role: "Solo personal project.",
            Status: ProjectStatus.Reference,
            Featured: false,
            Highlights:
            [
                "A from-scratch Android package metadata parser and a path-traversal guard on all file access.",
                "No third-party NuGet packages, and a documented session history including two real bugs caught by testing.",
            ],
            HonestNote: null,
            Stack: ["C#", ".NET 10", "Blazor", "Minimal API"],
            Links: [new Link("Source on GitHub", "https://github.com/randolfsegubre/DevHub")]),

        new Project(
            Id: "galaxy-survivor",
            Name: "Galaxy Survivor",
            Tagline: "A one-thumb endless roguelite space shooter for phones",
            Summary: "A mobile game project in Unity, specified documentation-first: design, architecture, security and monetisation ethics were written down before gameplay code, then built and checked on a real device.",
            Role: "Solo personal project, design and build.",
            Status: ProjectStatus.InDevelopment,
            Featured: false,
            Highlights: ["Shows range beyond web work: game loops, device builds and a store-review-aware plan."],
            HonestNote: "The repository is private, so there is no source link.",
            Stack: ["Unity", "C#"],
            Links: []),

        new Project(
            Id: "assessment-reviewer",
            Name: ".NET assessment reviewer",
            Tagline: "A senior .NET and full-stack reference with mock exams",
            Summary: "A self-contained study reference I wrote and maintain, with topic deep dives and mock exams whose examples point at real code in BudgetPH.",
            Role: "Author and maintainer.",
            Status: ProjectStatus.Reference,
            Featured: false,
            Highlights: ["Written as teaching material, so each topic explains the why and not only the syntax."],
            HonestNote: null,
            Stack: ["Markdown", "C#", ".NET"],
            Links: [new Link("Source on GitHub", "https://github.com/randolfsegubre/dotnet-assessment-reviewer")]),
    ];
}
