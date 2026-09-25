# Content sources

Where each claim on the site comes from (ADR-0006). Sources are things the
owner can produce or a reader can check. Re-verify before quoting a number as
current.

## Profile

| Claim | Source |
|---|---|
| Senior full-stack .NET developer, more than nine years of professional C# and .NET | Employment history from June 2017 (first programming role) to now |
| Travel, healthcare and construction sectors | Airline (Scoot), healthcare payer (Centene, UnitedHealth Group), hospitality travel (Hotelplan UK), construction (F. Gurrea) |
| Metro Manila | His public profiles name different cities within it, so the metro area is used |
| Open to senior full-stack .NET roles, remote preferred | The owner's stated job-search preference |
| Email, GitHub, LinkedIn | The owner's own public profiles |
| Stats strip: 9+ years, 3 industries (travel, healthcare, construction), 5 employers since 2017 | The employment history below: June 2017 to now, five employers (F. Gurrea Construction, BCS Technology International, Collabera, Accenture, Cloud Employee) |
| Stats strip: 660+ commits across eight repositories on one client platform | Git history review of his authored commits, 4 August 2026 (666 commits, 8 of 19 repositories). Re-verify before changing the figure |
| Hero code card: name, title, location, availability, signature stack (C#, ASP.NET Core, Umbraco, Vue.js, SQL Server) | Generated from the same profile data; each stack item appears in the experience and projects below |
| Backdrop code columns | Generic idioms written for the site (no proprietary code; they mirror this site's own source and the stacks named in the projects and roles) |
| Floating technology logos (C#, .NET, Umbraco, Vue.js, React, Blazor, Docker, Jenkins, MongoDB, MySQL, GitHub, Next.js) | Each technology must appear in a project stack, a role stack, or the skills, and have a logo (both enforced by tests). Logo artwork is from Simple Icons (CC0), except the C# mark, which is hand-drawn; see `docs/THIRD_PARTY_NOTICES.md` |
| Portrait | The owner's own photo, supplied by him on 2026-09-25 for the site. It is his graduation portrait, cropped to a circle by the stylesheet; the file is not otherwise edited |

## Experience

| Claim | Source |
|---|---|
| Employers, clients, titles, dates | The owner's resume and LinkedIn experience section, which match |
| More than 660 commits across eight repositories (Hotelplan UK) | Git history review of his authored commits, 4 August 2026 (666 commits, 8 of 19 repositories) |
| Content Security Policy hardening (unsafe-inline and unsafe-eval removal, nonces, Trusted Types) | Commit and pull request history on the customer-facing sites |
| Layered Azure hosting diagnosis | Ticket and pull request history on the same sites |
| Umbraco backoffice property editors in AngularJS | Git history: 25 commits in one content platform, 10 in the other, December 2024 to July 2026 |
| Model Context Protocol servers configured, shared instruction files | The owner's workspace configuration; he configured existing servers and did not author an MCP server, so the site says "configured" |
| Accenture: code review, code-scanning remediation, pagination and consistency work, ASP.NET Core 2.x to 6 upgrade | The owner's account, confirmed directly; not checkable from a local repository |
| Centene: 834 processing, 500,000 record migration, .NET Framework 4.6 to 4.8, Windows Presentation Foundation maintenance, ServiceNow triage | The owner's account, confirmed directly; a teammate's public LinkedIn recommendation names the Member Additional Detail project across 19 health plans and his lead role |
| BCS and Scoot: SOAP integration, EF Core Code First on MySQL, 90 percent xUnit coverage | The owner's account, confirmed directly |
| F. Gurrea: desktop certificate application with Dapper | The owner's account, confirmed directly |

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
