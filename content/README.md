# Content data files

Everything the site says or draws lives here, as JSON data. These files are the source of truth (ADR-0012). The
build loads them into SQL Server and the site is generated from what the database returns, so a change to a file
changes the site the next time it is built. No claim is written in code any more.

## Changing something

1. Edit the file. The format is plain JSON, so any editor will check the syntax as you type.
2. Update `docs/CONTENT_SOURCES.md` with where a new or changed claim comes from. Every claim needs a source (ADR-0006).
3. From the `web` folder run `npm run generate`. It refreshes the database from these files and rebuilds the site.
4. From the repository root run `dotnet test Portfolio.slnx`. It enforces the writing rules on these exact words.

Reading is strict on purpose. A field name that matches nothing (a typo such as `hightlights`), a missing required
value, a `null` where the model needs a value, or an invalid JSON file stops the build with the file's name. It never
passes quietly. JSON cannot carry comments, so the reasons behind each file are written below.

## The files

| File | What it holds | Notes |
|---|---|---|
| `site.json` | The site's address, its two descriptions and the number of hero logos | `url` has no trailing slash. `heroLogoCount` is how many of the profile's `backdropTags` float inside the hero; the rest go in the page layer. |
| `profile.json` | The owner: name, title, location, headline, contact details, stats, About paragraphs | See "Profile" below. |
| `live-sites.json` | Public websites the owner contributed to | See "Live sites" below. |
| `projects.json` | Project cards, featured first | `status` is `inDevelopment` or `reference`. `honestNote` is a known gap, stated plainly, or absent. |
| `experience.json` | Employment history, newest first | Employer and end-client names are public on his own profiles. |
| `skills.json` | The full grouped skills list | Anything listed appears in a project or role, or on his public profiles. |
| `skill-universe.json` | The skills cloud: intro, stars, extra lines, the walk order | See "Skills cloud" below. Positions are not stored; they are worked out from this. |
| `hero-tags.json` | The faint code tags in the hero's margins | Exactly six: the stylesheet positions them by order (`hero-tag-1` to `hero-tag-6`). Decoration that makes no claim; the last one repeats his stated availability. |
| `code-streams.json` | The lanes of scrolling code in the page background | Decorative and hidden from assistive technology, so it is not prose and the writing rules do not cover it. Generic idioms from his own stack, nothing proprietary. Slow on purpose. Lanes have no positions: the stylesheet lays them out in equal columns. Each lane is drawn twice in a row so the scroll can loop. |
| `tech-logos.json` | Technology logos as vector markup | See "Drawings" below. |
| `skill-icons.json` | Icons for the skills spheres | See "Drawings" below. |
| `social-icons.json` | Icons beside the GitHub and LinkedIn links | See "Drawings" below. |

## Rules that come with the content

**Profile.** The location is deliberately the metro area, not a city, because his public profiles name different cities
within it. The phone numbers were supplied by the owner on 2026-09-25 and are shown publicly at his request.

**Live sites.** The owner confirmed on 2026-09-25 that naming these sites is fine, on the condition that only publicly
available information is shown and nothing private, internal or confidential. So: names and public addresses only,
plus his plain-language part. No repository names, ticket numbers, hosting addresses, screenshots of internal tools
or configuration detail. A test enforces this. The Inghams features named (the wishlist, the resort and country pages)
and the Santa's Lapland description were checked against each public site's own text on 2026-09-25.

**Skills cloud.** .NET is the centre and everything else connects back to it. Only technologies the owner has used
himself have a sphere, at work or in his own projects. It is deliberately not the whole grouped skills list or every
entry of every project stack: a technology that only appears in a project built with AI help and never worked with
himself (TanStack Query, Azure Service Bus, TypeScript and similar) has no sphere, and neither do practices or
techniques, which are not a stack. The owner confirmed this rule on 2026-11-15, and a test names the exclusions so
they cannot creep back in. The five stars marked `main` are his main stack (the profile's `signatureStack`) and are
drawn larger. Stars are listed branch by branch, because the layout spreads each sphere in this order and keeps a
branch together. Captions are prose, so they follow the writing rules: no em dashes, abbreviations in full words.
`crossLinks` are lines between related technologies that are not parent and child. `journey` is the order the walk
visits the stars, roughly the order the work grew in, and lists every star exactly once.

**Drawings.** The markup is trusted, static vector shapes inside a 24 by 24 view box, written or generated by the
site's author and never user input. Most logos come from the Simple Icons package version 16.32.0, released under
the CC0 1.0 public-domain dedication (see `docs/THIRD_PARTY_NOTICES.md`), embedded so the site makes no third-party
requests. Trademarks belong to their owners and the logos only show which technologies he has used. Three marks are
drawn for this site, not copied: the C# hexagon (Microsoft's logo is not in the set), the AngularJS mark (the Angular
shield shape in AngularJS red, an approximation) and the LinkedIn rounded square with "in" cut out. Several
technologies here (SQL Server, Entity Framework Core, Dapper, Azure, AWS) have no logo in the public-domain set, so
they use generic line glyphs, which is more honest than a lookalike of a trademarked mark. A star's `icon` is looked up
in `skill-icons.json` first, then among the logos in `tech-logos.json`. A `tech-logos.json` `key` is the technology name
exactly as it appears in a project or role stack, so a test can prove the page backs it up. In `social-icons.json`,
GitHub reuses its logo from `tech-logos.json` (`logoKey`) and LinkedIn carries its own `markup`; exactly one of the
two is set, and the database enforces that.

## Line endings

All files use LF line endings, so the site is built the same way on every machine. A line break inside a text value is
written `\n`.
