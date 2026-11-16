# Content sources

Where each claim on the site comes from (ADR-0006). Sources are things the
owner can produce or a reader can check. Re-verify before quoting a number as
current.

## Profile

| Claim | Source |
|---|---|
| Full-stack .NET developer, more than 9 years of professional C# and .NET | Employment history from June 2017 (first programming role) to now |
| Travel, healthcare and construction sectors | Airline (Scoot), healthcare payer (Centene, UnitedHealth Group), hospitality travel (Hotelplan UK), construction (F. Gurrea) |
| Metro Manila | His public profiles name different cities within it, so the metro area is used |
| Open to .NET and full-stack roles, remote preferred, open to hybrid | The owner's stated job-search preference |
| Email, GitHub, LinkedIn | The owner's own public profiles |
| Downloadable résumé (`web/public/files/Randolf_Segubre_Resume.pdf`) | An unedited copy of `Downloads/Corrected Resume/Final/Randolf_Segubre_Resume_2026-11-13.pdf`, the résumé he currently sends to employers (only the Professional Summary differs from the 2026-10-11 version: no "Senior" in the summary, and the personal AI setup wording). First published at his request on 2026-11-02; replaced with the 2026-11-13 version at his request the same day it was made. It is his own document and is more detailed (and in places stronger worded, for example "Led", "Independently built") than the site's cards; see the devlog for two lines to review |
| Hero background video (`web/public/media`) | Made for this site (`tools/hero-video`), not stock footage. It names only technologies the site already backs up and that the owner has really worked with in his career (Vue.js, JavaScript, ASP.NET Core Web API and MVC, Umbraco CMS, MongoDB, SQL Server, T-SQL, EF Core, Docker, Jenkins, Content Security Policy), each with a row elsewhere in this table. Removed on 2026-11-14 at his request because they were not his experience to claim: an invented "Edge" delivery box (also easy to misread as the Microsoft browser), "React" (about six months commercially) and ".NET 10" (personal projects only). Requested by the owner on 2026-11-14: "a unique .gif or video that is unique and suitable for my background and skills" |
| Header link "Home", now shown as the "RS" monogram (accessible name "RS, home") | The owner's choice on 2026-11-14, to avoid repeating the name the hero shows; changed to his initials the same day at his request (ADR-0010) |
| Hero greeting "Hi, I'm" above his name | The owner asked on 2026-11-14 for the page to introduce him as Randolf Segubre |
| Hero margin code tags (`@page "/"`, `<h1>`, `@model Developer`, `// remote preferred`) | Decoration only; the last repeats his stated availability (remote preferred) |
| Mobile and Viber number +63 917 1022 203, and WhatsApp number +63 920 909 3036 | Supplied by the owner in writing on 2026-09-25, to be shown publicly. The mobile number matches the one on his job applications |
| Stats strip: 9+ years, 3 industries (travel, healthcare, construction), 5 employers since 2017 | The employment history below: June 2017 to now, five employers (F. Gurrea Construction, BCS Technology International, Collabera, Accenture, Cloud Employee) |
| Stats strip: 660+ commits across eight repositories on one client platform | Git history review of his authored commits, 4 August 2026 (666 commits, 8 of 19 repositories). Re-verify before changing the figure |
| Hero code card: name, title, location, availability, signature stack (C#, ASP.NET Core, Umbraco, Vue.js, SQL Server) | Generated from the same profile data; each stack item appears in the experience and projects below |
| Backdrop code columns | Generic idioms written for the site (no proprietary code; they mirror this site's own source and the stacks named in the projects and roles) |
| Floating technology logos (C#, .NET, Umbraco, Vue.js, React, Blazor, Docker, Jenkins, MongoDB, MySQL, GitHub, Next.js) | Each technology must appear in a project stack, a role stack, or the skills, and have a logo (both enforced by tests). Logo artwork is from Simple Icons (CC0), except the C# mark, which is hand-drawn; see `docs/THIRD_PARTY_NOTICES.md` |
| Portrait | The owner's own photo, supplied by him on 2026-09-26 (replacing the graduation portrait he supplied on 2026-09-25). The file is a square head-and-shoulders crop, resized to 640 by 640 pixels and shown in a circle by the stylesheet; the image content is not otherwise edited |

## About me (ADR-0010)

| Claim | Source |
|---|---|
| Full-stack .NET developer based in Metro Manila; more than 9 years building and maintaining web platforms in C# and .NET for travel, healthcare and construction | The Profile rows above (employment history from June 2017; sectors; location) |
| His work began in 2017 with a desktop application in C# and Dapper | The F. Gurrea row below (first professional role, Dapper data access) |
| It grew into ASP.NET Core web platforms, an Umbraco content platform and the Vue.js front end on top of them | The Accenture, Hotelplan UK and BCS rows below and their role stacks in `content/experience.json` |

## Skills star map (ADR-0010)

The cloud shows **only technologies the owner has used himself**, at work or in his own
projects (45 spheres). This is deliberately smaller than the grouped skills list and the
project stacks: the owner checked his own record on 2026-11-15 and asked for nothing he has
not directly worked with. Left off on purpose, with the reason: Azure Service Bus (he has
never used a message broker), TanStack Query, Zustand and Vite (from BudgetPH, which was built
with AI help and which he did not work with), GraphQL and Semantic Kernel (the same), TypeScript
(he has none), SQLite, and everything that is a practice or technique rather than a stack (code
review, documentation-first delivery, legacy upgrades, production support triage, search
query debugging, content-type modelling, Models Builder, SOAP and REST integration, shared
instruction files, AI-assisted engineering as a heading). A test names these so they cannot
come back unnoticed. Every label is also a technology already in a role stack, a project stack
or the grouped skills list (a test enforces it). The one-line captions are limited to the facts
below, and a few are deliberately generic (marked "generic"). The owner is asked to review them.

Sources used to decide what he has used: his memory notes (profile, resume facts, interview
answers, the Azure Service Bus correction of 2026-10-29), the cross-machine memory repository
(`claude-dotfiles`), and his GitHub repositories.

| Star | Caption claim | Source |
|---|---|---|
| .NET, C# | More than 9 years of C# and .NET; the language he has worked in professionally since 2017 | Employment history from June 2017 (see Profile) |
| ASP.NET Core | Moving a codebase from ASP.NET Core 2.x to 6 at Accenture | Accenture row below (his account, confirmed directly) |
| Web API | Backend services for client platforms and his own projects | The projects section and the Hotelplan UK roles |
| ASP.NET MVC, HTML and CSS | The controller-and-view pattern of the ASP.NET applications in his earlier roles (it stopped being this portfolio's own pattern on 2026-11-16, ADR-0011); markup and styling | The Scoot and Centene role stacks and the grouped skills; this repository |
| Razor Pages, Blazor | Razor Pages at Accenture; Blazor WebAssembly in AI Tutor and Blazor in DevHub | The Accenture role stack; the AI Tutor and DevHub project stacks |
| Vue.js, JavaScript | Front-end work on top of the backend, in two roles; JavaScript behind it. JavaScript is also joined to React and AngularJS by a line, since both are written in it | The Hotelplan UK and BCS role stacks in `content/experience.json`; the grouped skills |
| Nuxt.js | "The framework built on Vue.js; this portfolio is generated with it." True since 2026-11-16: this site is built with Nuxt (ADR-0011), with the owner directing the work and AI help. Before that his memory held Nuxt only as a self-declared LinkedIn skill, so no earlier project or role is claimed | This repository (`web/`), ADR-0011 |
| React, Redux, Next.js | About six months of React and Redux in 2020 (his own correction of 2026-08-17); the Lakbay front end, which he describes as built by himself with Next.js, React and Redux Toolkit | His interview answers and the Collabera row below; the Lakbay project stack |
| MongoDB | Document storage in an earlier role and in the Lakbay project | The Centene role stack and the Lakbay project stack |
| JWT authentication | Access and refresh token sign-in in his own BudgetPH and AI Tutor projects | The BudgetPH and AI Tutor project rows |
| Umbraco, AngularJS | Content platform work, and custom backoffice property editors built in AngularJS for editors on two content platforms | The AngularJS property-editor row below |
| SCSS | Styling for the customer-facing sites in his latest role | The Hotelplan UK role stack |
| Security headers, Content Security Policy, Trusted Types | Content Security Policy hardening: he took the initiative on removing unsafe-inline and unsafe-eval, adding nonces, adopting Trusted Types | The Content Security Policy row below (worded "took the initiative on") |
| OWASP ZAP | Scanning sites for weaknesses, including missing security headers (generic beyond that) | LinkedIn skill (endorsed) and the Santa's Lapland security work |
| Docker, Jenkins | Delivery setup in his latest role | The Hotelplan UK role stack |
| Azure | Diagnosing a layered Azure hosting setup | The Layered Azure row below |
| Azure DevOps | Where some legacy projects were hosted at Accenture, formerly Team Foundation Server | His account (2026-08-10, memory); not checkable from a local repository; added to the grouped skills on 2026-11-15 |
| AWS | Hosting for the Scoot booking tools | The BCS and Scoot row below |
| EF Core, MySQL | Entity Framework Core Code First on MySQL for the Scoot work | The BCS and Scoot row below |
| SQL Server, T-SQL, Dapper | Relational data work across his roles; Dapper in his first professional role | The F. Gurrea row below; SQL Server and T-SQL as listed skills |
| xUnit | Unit tests written at Scoot toward a team target of at least 90 percent coverage (he corrected "achieved 90 percent" on 2026-11-06: it was the target, not a result) | The BCS and Scoot row below |
| MediatR, Clean Architecture | Command and query handling in his Lakbay booking service and AI Tutor; layered BudgetPH and AI Tutor. BudgetPH no longer uses MediatR | The Lakbay and AI Tutor project stacks; the project files |
| Unity | Galaxy Survivor, a mobile game he specified documentation-first | The Galaxy Survivor project row |
| Ollama | The local language model behind AI Tutor, on his own machine | The AI Tutor project row |
| .NET Framework, WPF, WinForms | The 4.6 to 4.8 upgrade and the WPF companion app at Collabera; the WinForms application at F. Gurrea | The Centene and F. Gurrea rows below (his account, confirmed directly) |
| Git and GitHub, GitHub Copilot, Claude Code, Model Context Protocol configuration | Version control with GitHub review and scanning; Copilot since 2022; Claude Code and the shared conventions; Model Context Protocol servers configured, not written | The Accenture row and the Model Context Protocol row below |

## Experience

| Claim | Source |
|---|---|
| Employers, clients, titles, dates | The owner's resume and LinkedIn experience section, which match |
| More than 660 commits across eight repositories (Hotelplan UK) | Git history review of his authored commits, 4 August 2026 (666 commits, 8 of 19 repositories) |
| Content Security Policy hardening (unsafe-inline and unsafe-eval removal, nonces, Trusted Types) | Commit and pull request history on the customer-facing sites. Worded "took the initiative on", not "led": the owner confirmed on 2026-10-09 it was his own initiative, not a formal lead role |
| Layered Azure hosting diagnosis | Ticket and pull request history on the same sites |
| Umbraco backoffice property editors in AngularJS | Git history: 25 commits in one content platform, 10 in the other, December 2024 to July 2026 |
| Model Context Protocol servers set up, shared instruction files | The owner's workspace configuration; he configured existing servers and did not author an MCP server. Worded as his own workflow, never team-wide: the owner corrected this on 2026-10-09, it is a personal setup |
| Accenture: code review, code-scanning remediation, pagination and consistency work, ASP.NET Core 2.x to 6 upgrade | The owner's account, confirmed directly; not checkable from a local repository |
| Centene: 834 processing, 500,000 record migration, .NET Framework 4.6 to 4.8, Windows Presentation Foundation maintenance, ServiceNow triage | The owner's account, confirmed directly; a teammate's public LinkedIn recommendation names the Member Additional Detail project across 19 health plans and his lead role |
| BCS and Scoot: SOAP integration, EF Core Code First models on MySQL implemented from his senior developer's design, xUnit tests toward a team target of at least 90 percent coverage | The owner's account, confirmed directly; corrected on 2026-11-06 (he implemented the models and did not design the schema; 90 percent was the target, not a result) |
| F. Gurrea: desktop certificate application with Dapper | The owner's account, confirmed directly |

## Live sites I contributed to

The owner confirmed on 2026-09-25 that naming these sites is fine, on the
condition that only publicly available information is shown, nothing private,
internal or confidential.

| Claim | Source |
|---|---|
| Inghams: website address, and that it describes itself as the ski, walking and Lapland holiday experts, with Hotelplan UK | The public site's own title and description, read on 2026-09-25 |
| Inghams: the saved-holiday wishlist and the resort and country pages exist | Visible on the public site on 2026-09-25 (a wishlist in the header, and country and resort pages under the ski holidays menu) |
| Inghams: he contributed from August 2024 to August 2026, as one of a team | His authored commits in the client's repositories behind the site, reviewed locally on 2026-09-25 (hundreds of commits, first in August 2024, last in August 2026) |
| Santa's Lapland: website address santaslapland.com, and that it offers family Lapland holidays including flights, hotels and a private trip to see Santa | The public site's own title and description, read on 2026-09-25 (the owner corrected the address from .co.uk to .com that day) |
| Santa's Lapland: he contributed from November 2025 to July 2026 on web security and standards work | His authored commits in the repositories behind the site (first November 2025, last July 2026) |
| "One of a team" and the caveat that the sites have changed since he left | The sites are large team codebases (thousands of commits from many authors), and his engagement ended on 30 August 2026 |

Never included on these cards, and blocked by a test: internal repository
names, ticket numbers, internal system or tool names, hosting addresses,
configuration terms, or screenshots of anything not public.

## Projects

| Claim | Source |
|---|---|
| BudgetPH features, stack, audit results, three fixed bugs, small test count | The project README, including its dated audit section, and a direct read of the front-end code |
| Ophir: stack, bilingual content, contact form protections, editable 404 and hardcoded 500, freelance client | The project README and the public repository description |
| Lakbay: multi-repository shape, decision records, atomic availability decrement, search sync over messaging | The public architecture repository and the owner's verified local runs. Not claimed: checkout, payment, or booking calls from the storefront, none of which are built |
| AI Tutor: stack, tokens, encrypted key vault | The project README and code |
| DevHub: parser, path-traversal guard, no third-party NuGet packages, two bugs found by testing | The project README and devlog |
| Galaxy Survivor: Unity, documentation-first | The project README. The repository is private, so no link |
| Assessment reviewer | The public repository |

## Deliberately left off the site

- Confidential or proprietary employer and client material of any kind.
- One personal e-commerce demo whose front end was never connected to its
  backend, so it would overstate what works.
- Skills without hands-on evidence.
