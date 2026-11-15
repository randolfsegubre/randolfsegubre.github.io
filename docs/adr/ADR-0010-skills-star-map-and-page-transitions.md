# ADR-0010: Skills as a star map, the RS monogram, and page-change motion

- **Status:** Accepted (built and tested locally on 2026-11-14; not yet deployed).
- **Date:** 2026-11-14

## Context

The owner asked for a "better, more modern, unique" design. After comparing
four concepts and one motion prototype he picked the parts he liked and
asked for them to be built into the real site and tried locally before any
deployment:

- the monogram reads **RS** and is a clickable link back to the top;
- the page introduces him by full name: "Hi, I'm Randolf Segubre";
- faint code tags in the hero's margins, as if the page showed its own source
  (the idea he liked in another developer's portfolio, redrawn here; nothing
  of that site's code, wording or look was copied, ADR-0008);
- headings that settle in letter by letter;
- a short fade to the monogram and a thin loading line when a link jumps to
  another section;
- Skills drawn as a **universe**: .NET at the centre and every other
  technology connected outward along his tech journey.

The existing background (the page-wide code layer and logos, the hero video)
and the portrait stay as they are.

## Decision

1. **Skills star map.** Every star is data (`Content/SkillUniverseData.cs`: a
   tree plus a few cross links and a walk order). `SkillUniverseLayout` turns
   the tree into fixed positions: the centre in the middle, each level on a
   larger ellipse, each subtree given a slice of the circle as wide as the
   number of leaves beneath it. The page is complete without script (stars
   drawn in place, every caption listed). With script, a star becomes a button
   that lights its links and shows its caption in a live region, and a "Walk
   my journey" button steps through the stars in order. The map is never
   narrower than 56 rem; on a phone its wrapper scrolls sideways (a labelled,
   keyboard-reachable region) instead of shrinking the labels. The full
   grouped skills list stays underneath in an "All skills, grouped" disclosure
   so no skill is lost. Captions are checkable one-liners, each backed by
   `docs/CONTENT_SOURCES.md`; there are no percentage bars (ADR-0006).
2. **RS monogram.** The header's first link shows "RS" (gold with a violet
   offset) and goes to `#top` (to `/#top` on the not-found page). Its
   accessible name starts with the visible letters ("RS, home") so voice
   control works (Web Content Accessibility Guidelines success criterion
   2.5.3, label in name); the owner's full name stays in the hero's single
   `<h1>`, which is still exactly his name. The greeting "Hi, I'm" is data
   (`Profile.Greeting`) in its own paragraph above it.
3. **Letter-by-letter headings** (hero name and section titles). Script splits
   a heading into words (kept whole so it never breaks mid-word) and letters
   and keeps the full text as the accessible name. Not done for visitors who
   prefer reduced motion.
4. **Page-change veil.** Following an in-page link fades the page to a
   translucent veil with the RS mark and a loading line for under a second,
   jumps while covered, updates the address and moves keyboard focus to the
   target. Never for reduced motion or data saving; a footer switch ("Page
   transitions: on/off", remembered in local storage, best effort) turns it
   off; skip links, modifier-key clicks and links to a place already on
   screen are left alone. The veil is translucent so the background still
   shows through.
5. **Margin tags** are decoration (`Content/HeroTagData.cs`, six fixed
   positions): `aria-hidden`, no pointer events, shown only at 75 rem and
   wider where the margins are empty.

No new dependency, no script library, no Content Security Policy change:
positions use `style` attributes (already allowed by `style-src-attr`) and
all behaviour is in `site.js` (steps 5 to 7).

## Amendment, 2026-11-14 (later the same day): About me, and a 3D solar system

The owner reviewed the first local build and asked for three changes:

1. **Skills belong with About me.** The page has no separate Skills section
   any more. A new **About me** section (`#about`, first after the stats
   strip, "About" first in the navigation) holds two paragraphs of his own
   (`Profile.About`, sourced in `docs/CONTENT_SOURCES.md`) and, under a
   "Skills" sub-heading, the map and the grouped list.
2. **Floating, multiverse-like nodes.** Every star bobs gently on its own
   rhythm, and faint dust drifts around the system.
3. **3D and rotatable like a solar system.** Done with about 300 lines of plain
   script and no library (a three-dimensional graphics library would be a
   third-party script, which the project rules and the Content Security Policy
   forbid without a new decision):
   - each star sits on a sphere around .NET (first written as per-level orbits,
     replaced by two evenly spread spheres in the third amendment below: the
     server gives each star `data-azimuth`, `data-elevation` and `data-radius`);
   - a small projection (turn about the vertical axis, tip toward the viewer,
     gentle perspective) places the stars; nearer stars are a little
     larger and brighter, farther ones smaller and fainter, and the centre never
     fades;
   - the system turns slowly by itself; a mouse or pen drags it, the arrow keys
     turn it, and Turn left, Turn right and Pause rotation buttons are shown;
     choosing a star (click, Enter, tab to it) or walking the journey turns that
     star to the front;
   - it pauses while the pointer is over the map or a star has keyboard focus,
     stops when the map is off screen or the tab is hidden, and **Pause rotation
     freezes all of the motion**, which also answers the pause requirement
     (Web Content Accessibility Guidelines 2.2.2) for this part of the page;
   - **reduced motion:** nothing moves by itself (no spin, no floating, no
     gliding), the camera jumps, and the visitor can still turn the map by hand;
   - **touch screens** keep their own scrolling (the map scrolls sideways) and
     use the Turn buttons or a tap on a star, so two gestures never fight;
   - without script the caption list remains (the first version also kept a flat map;
     the fourth amendment below removed it).

## Second amendment, 2026-11-14 (night): a sphere cloud, as in the reference

The owner compared the first 3D version with the animation he liked ("jack.gif")
and asked for that arrangement with .NET at the centre, and for the skills to be
**spheres with their logos and a small name underneath**, larger than before. The
reference is a **3D tag cloud** (a globe or sphere cloud): plain words spread over
a rotating sphere, near words large and bright, far words small and faint, with no
boxes or lines. What we build is a **3D radial node graph** (a radial tree, or
constellation map): .NET is the hub and every other technology hangs from the one it
grew out of. It is not a Hoberman sphere, which is a mechanical expanding toy, and
the earlier "solar system" wording was loose, because nothing here orbits like a
planet.

- **Spheres.** Each star is a glossy pearl-white sphere (a radial gradient with a
  shine) holding its logo in the brand colour, with its name below. A logo that
  needs a dark ground (the yellow JavaScript mark) gets a dark sphere. .NET is a
  larger golden sphere with its logo in the page's deep purple.
- **Logos where they exist, plain glyphs where they do not.** Technology logos
  already on the page (.NET, C#, Umbraco, Vue.js, Docker, Jenkins, MongoDB, MySQL)
  are reused from the shared sprite; the JavaScript logo is added from the same
  CC0 icon set. Several technologies have no logo in the public-domain set (SQL
  Server, Entity Framework Core, Dapper, xUnit, AngularJS and the Web API, MVC and
  security-header ideas), so they get generic line glyphs drawn for this site (a
  database cylinder, a table, a bolt, a check mark, sliders, braces, layers, a
  shield, a prompt, a globe). A lookalike of a trademarked mark would be less
  honest than a generic symbol. See `SkillIconData.cs`.
- **Cloud arrangement.** The cloud is square and sat beside the About text on wide
  screens (the fourth amendment moved it under the text). Words are spread over shells around the centre
  with a fixed scatter above and below the middle, so it reads as a ball. Near
  spheres are larger and brighter, far ones smaller and fainter, and connection
  lines are drawn only for the chosen sphere (the cloud itself stays clean).
- **Fits every screen.** Because the square cloud fits its box, a phone no longer
  needs sideways scrolling, so a finger can drag or swipe it (a vertical swipe still
  scrolls the page). The caption list is the no-script fallback (the flat scrolling map
  this originally named was removed in the fourth amendment).

## Third amendment, 2026-11-14 (late night): bigger spheres, an even arrangement, a purple band

The owner asked for bigger spheres, a fixed arrangement and a page design that fits
the new look. The earlier layout (a ring per level of the tree, a fixed height spread)
let spheres line up behind the centre and overlap it, which got worse as the spheres grew.

- **Even arrangement.** The server now places the stars on **two spheres** around .NET
  with a golden-angle spiral: the four stars one step out on an inner sphere
  (radius 200) and the other fourteen on an outer sphere (radius 400; the fourth
  amendment grew this to three spheres for 68 stars). Evenly spaced
  heights plus a turn by the golden angle at each step spread points evenly, so no two
  stars crowd each other however the cloud is turned. Stars are taken in tree order, so
  a branch lands in one band of latitude, and the connection lines (shown for the chosen
  sphere) do the work of relating them. A test measures the nearest pair on each sphere.
  This replaced the per-level orbit radii and the `data-angle` and `data-orbits`
  attributes: each star now carries `data-azimuth`, `data-elevation` and `data-radius`
  (`PlacedStar.Azimuth`, `Elevation`, `Radius`), and the script only projects them.
- **Bigger spheres.** The centre is about 107 pixels across and the others between
  about 51 and 70 on a wide screen (sizes scale with the cloud's box, names never drop
  below 10 pixels). The cloud was a 700-pixel square on desktop (now up to 58rem, see the
  fourth amendment), so spheres were larger *and* had room.
- **A purple band.** "About me" is now a band in the hero's purple (both themes, like the
  header, hero and footer), slightly see-through so the page's faint background still
  shows, wider than the ordinary column, with a soft golden glow behind .NET. The cloud
  sits straight on the band instead of in a boxed panel. The plain grouped skills list
  moved to its own ordinary section below it, because its cards use the reading
  surface colours.
- **The sun's name** is visually hidden (the logo already says .NET) but still the
  accessible name.

## Fourth amendment, 2026-11-15: every skill, a real AngularJS logo, a larger cloud

The owner asked whether the AngularJS sphere carried its logo (it did not: it had a
generic "sliders" glyph) and pointed out that most of his skills and stack were missing
from the map (it had 19 of them).

- **Every skill and every stack entry has a sphere** (superseded by the fifth amendment, which keeps only what he has used). 68 in all: each item of the grouped
  skills list and each technology in a role or project stack, with the few same-thing
  pairs mapped on purpose (see `CONTENT_SOURCES.md`). The tree grew new branches (older
  desktop and Framework work, version control, AI-assisted engineering and "Practice" as
  hubs under .NET) and new leaves under the old ones. Two tests enforce it: every skill
  and stack entry is on the map, and every label is evidenced on the page.
- **Three shells.** With 68 stars two spheres were too crowded, so stars one step out
  sit at radius 260, two steps out at 430 and further out at 600 (`RadiusFor`). The
  nearest-pair test now checks each shell at 0.45 of its radius.
- **No flat fallback.** A flat map of 68 stars cannot avoid overlaps, so it was removed:
  `PlacedStar` lost its `X` and `Y`, `UniverseMap` lost `Width`, `Height` and
  `Rings`, and the flat label-overlap tests went with them. Without script the cloud is
  hidden and the caption list (already complete) carries the content, which suits
  search engines and screen readers equally well.
- **A bigger layout.** The cloud is a square up to 58rem wide, centred under the About
  text (which now sits in two columns above it), with names that wrap under their sphere
  (long ones such as "Custom backoffice property editors") and **the names of far
  spheres fading out first**, so the near half stays readable. The walk takes 1.8
  seconds a step (it was 2.6) because it now visits 68 stars.
- **Logos.** Eighteen more Simple Icons logos (CC0) and seventeen more generic glyphs.
  AngularJS has no mark in that set, so its sphere uses the Angular shield shape in
  AngularJS red, disclosed as an approximation in `THIRD_PARTY_NOTICES.md`.
- **Known trade-off.** 68 spheres is dense, most of all on a phone (about 327 pixels
  wide), where the cloud is a small, crowded ball. It still fits with no sideways
  scrolling, and choosing a sphere brings it to the front with its caption, but it reads
  better on a tablet or desktop. The owner can ask for fewer, grouped spheres.

## Fifth amendment, 2026-11-15 (later): only what he has used, a less crowded cloud, a crowned monogram

The owner said the 68-sphere cloud was too cramped, that some spheres were not tech stack, and
that some technologies on it he had never used: Azure Service Bus and TanStack Query were
named. He asked that only technologies he has directly used appear, in work or in his own
projects, and that memory, the dotfiles repository and GitHub be checked first.

- **Rule.** The cloud is a tech stack he has used himself, not a copy of the skills list and
  the project stacks. Checked against his memory notes (profile, resume facts, interview
  answers, the 2026-10-29 Azure Service Bus correction), `claude-dotfiles` and his repositories.
  Where a package only appears in a project built with AI help (TanStack Query, Zustand, Vite
  in BudgetPH; GraphQL, Semantic Kernel) it is not counted as his experience, even though it
  is in a personal project. A test (`Shows_only_technologies_the_owner_has_used_himself...`)
  lists the removed spheres so they cannot return unnoticed.
- **68 spheres became 44.** Removed: Azure Service Bus, TanStack Query, Zustand, Vite, GraphQL,
  Semantic Kernel, TypeScript, SQLite, and every sphere that was a practice or technique
  (Practice, Code review, Legacy upgrades, Production support triage, Documentation-first
  delivery, Markdown, Figma handoff, Shared instruction files, AI-assisted engineering,
  Content-type modelling, Models Builder, Custom backoffice property editors, Code scanning
  remediation, Search engine query debugging, SOAP and REST integration, Razor and Minimal API).
  Added: Azure DevOps (where some legacy Accenture projects lived, formerly
  Team Foundation Server). GitHub Copilot and Claude Code now hang from Git and GitHub.
- **Less crowded.** With fewer spheres the cloud breathes again, so the spheres could grow
  (centre about 100 pixels, the rest about 51 to 76 on a wide screen). Names of far spheres still
  fade; the three shells stay (radius 260, 430 and 600).
- **Fixes made on the way, from his own corrections in memory.** The BudgetPH stack chip said
  MediatR, which that project dropped on 2026-10-01 (removed). The Scoot bullet said he
  "designed" the data models and "achieved" 90 percent coverage; he implemented them from his
  senior developer's design and 90 percent was a team target (corrected 2026-11-06). TanStack
  Query, Zustand and Azure Service Bus were removed from the project cards and their
  highlights reworded; TypeScript was removed from the grouped skills list (he has none) and
  Azure DevOps added to it. The owner then confirmed the same removal from the project cards
  (same day): GraphQL (Lakbay), Vite (BudgetPH), Semantic Kernel and SQLite (AI Tutor) are
  gone from the stack tags, the AI Tutor highlight now says "a local Ollama model behind it",
  and SQLite also left the grouped skills list. Projects themselves all stay.
- **Header.** The first navigation link and the section heading now read "About Me". The RS
  monogram carries a small gold crown above the letters with a cross cut out of its front:
  one even-odd path, so the cross is a real hole and the dark header shows through it. It is
  decoration (hidden from assistive technology; the link is still "RS, home"). This is a
  different mark from the hero crown ADR-0008 removed. It adds about 1.1 rem to the header's
  height at the top of the page; checked on desktop and phone width.

## Sixth amendment, 2026-11-15 (afternoon): the main stack drawn larger

The owner asked for the technologies that are not his main stack to be drawn on a smaller circle.
Read as: smaller spheres, so the main stack reads first.

- **Main** is the profile's signature stack, the one place his main stack is already defined:
  C#, ASP.NET Core, Umbraco, Vue.js and SQL Server. `SkillStar` has a `Main` flag set on those five;
  a test keeps the flag in step with `Profile.SignatureStack` and checks the page marks the same
  five (`star-main`). Changing his main stack therefore means changing the signature stack, and the
  flags with it.
- **Sizes** (wide screen): .NET about 102 pixels, the main stack about 79, everything else about 41 to
  54 (smaller the further out). The script reads the `star-main` class; the server decides who is main.
- The three shells, the names that fade and the phone fit are unchanged; smaller spheres only add
  room, so the phone check was not repeated.

## Seventh amendment, 2026-11-16: JavaScript joined to its frameworks, and Nuxt.js

- **Cross links.** JavaScript hung only from Vue.js, so choosing it lit one framework. React and
  AngularJS are written in JavaScript too, so two cross links (`javascript` to `react` and to
  `angularjs`) join them. The tree keeps one parent per sphere (JavaScript stays under Vue.js, which
  decides its shell and place); extra relationships are cross links, as C# already has with ASP.NET
  Core and EF Core. Choosing JavaScript now lights Vue.js, React and AngularJS.
- **Nuxt.js** is a sphere under Vue.js (a framework built on Vue.js, as Next.js is on React). The owner
  asked for it twice. His memory has it only as a LinkedIn skill, which the fifth amendment would
  exclude, so the caption is deliberately generic and `CONTENT_SOURCES.md` records that it is added on his
  instruction. The official Nuxt mark (CC0, Simple Icons) is used. Nuxt.js is also in the grouped skills
  list, so the evidence test passes.

## Consequences

- The skills section is far more distinctive and the owner's stack reads as a
  journey, but it costs a larger section; the grouped list is folded away by
  default.
- The layout is a pure function, so tests catch a crowded sphere, a broken
  tree, a sphere that should not be there and a journey that skips a star before a browser
  is opened. Real devices are still needed for touch behaviour (not verified
  here).
- The page-change veil is the first behaviour that intercepts link clicks, so
  it has an off switch and several bail-outs; the plain behaviour (smooth
  scroll) remains the fallback.
- Number formatting in the view uses the invariant culture so a machine with
  a comma decimal can never write `12,5%` into an attribute (a test sets
  German culture to prove it).

## Alternatives rejected

- **A fixed left navigation rail** (as in the prototype). A real change to the
  responsive layout audited at 17 widths; kept as an optional later step.
- **A word cloud in the hero or About.** The page already has scattered
  background logos, and the owner found a repeat of them redundant (ADR-0008,
  fourth amendment).
- **Positioning stars with script.** The page would be empty without script
  and the tests could not check it.
- **A graph library.** A third-party dependency for twenty labelled points.
