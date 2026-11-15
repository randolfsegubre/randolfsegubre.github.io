# ADR-0008: A "developer identity" design concept, inspired by a reference and made original

- **Status:** Accepted
- **Date:** 2026-09-25

## Context

The owner asked for a concept in the spirit of another developer's portfolio
(`rujealfon.github.io`) but unique to him: use it as a reference, not a copy.
The reference is a Vue.js developer's site with a navy and green palette. Its
concept, as observed on the live page, is:

- a full-screen hero with faint code snippets floating behind the name;
- animated scroll reveals (built with the GSAP animation library);
- an "about" area with a profile photo and animated percentage skill bars;
- featured project cards with a "Live" badge, role and date;
- an animated work-experience timeline;
- a "Let's Work Together" contact section.

Copying its text, code, colors, photo or animation code would be both
unoriginal and wrong. Its structure is a common portfolio pattern; its
execution belongs to its author.

## Decision

Keep the general idea (a developer's identity expressed through code and
motion) and give it an original execution:

| Reference idea | This site's original version |
|---|---|
| Faint code snippets floating in the hero | **C# and Razor** fragments from the owner's own stack, in purple and gold, kept to the edges and behind the card so the headline stays readable |
| (nothing comparable) | A **`Developer.cs` code card** that renders the owner's profile as a C# record from the same `Profile` data as the rest of the page, so it can never disagree with it |
| Animated percentage skill bars | **No percentage bars.** They cannot be verified, which breaks ADR-0006. A **stats strip** of checkable facts instead (years, industries, employers, commits) |
| GSAP scroll animations | About 100 lines of vanilla script: scroll reveals, an active-section highlight in the navigation, and a gold scroll-progress bar. No third-party library |
| Profile photo | The owner's own portrait in a gold ring beside the name, plus a crown mark (royal purple and gold, ADR-0004). Added 2026-09-25 at his request; the file is `wwwroot/images/randolf.webp` and the address is one setting (`Profile.PhotoUrl`), so swapping the photo is a one-line change |
| A few floating snippets | Amended 2026-09-25 at the owner's request for a stronger "moving code" background: **columns of real C#, Razor, Vue.js, T-SQL, xUnit and Umbraco code scrolling endlessly** (a looping animated background) in the hero, **floating technology tags**, and a very faint fixed copy of the columns behind the whole page so the motion continues as the visitor scrolls |
| Navy and green palette | Dark royal purple and gold, dark by default |
| "Let's Work Together" contact | "Let's talk" with large link cards for email, profiles and location |

Every motion is an enhancement: content is hidden for a reveal only when the
`js` class is present, reduced-motion visitors see everything at once, and the
whole page works with scripting off.

**Second amendment, 2026-09-25 (three requests):**

1. *Columns overlapped each other.* The first version positioned each column
   with a percentage, so neighbours crossed. Lanes now have no positions: they
   are equal-width columns of a CSS grid across the full width (8 on desktop, 5
   on tablets, 3 on phones), so overlap is impossible by construction. A test
   asserts the grid rule, and a browser measurement confirmed zero overlaps.
2. *Cover all of the page.* The same lanes and logos now exist twice: a stronger
   copy inside the hero and a quieter copy fixed behind the whole page, so the
   moving background is present in every section. Cards let a little of it show
   through, blurred so their text stays crisp. In the page-wide layer the logos
   are quiet silhouettes in the theme color rather than bright badges, so text
   never sits on a bright shape.
3. *Logos instead of text tags.* Real technology logos replace the text pills.
   They are embedded as inline vector markup from the Simple Icons set (CC0,
   version 16.32.0) so the site still makes no third-party requests; see
   `docs/THIRD_PARTY_NOTICES.md`. In the hero each logo sits in a light round
   badge so every brand color stays visible on purple. Not every technology has a
   logo in that set: there is none for C#, SQL Server, Azure or ASP.NET Core, so
   the C# mark is a hand-drawn hexagon (flagged as an approximation) and the
   others use the .NET logo or are omitted. Only technologies with a logo are
   shown, and a test requires every one to appear in the projects, roles or
   skills.

**Third amendment, 2026-11-02 (responsive pass):** a measured audit at 17 exact
screen widths (320 to 3440 pixels, including phones in landscape and the
owner's ultrawide monitor) found that the scattered floating hero logos
collided with the portrait, text or code card at almost every size, because
absolutely positioned decorations cannot stay clear of content whose height
changes at every breakpoint. Decision:

- **Hero logos became a scrolling logo rail** (`_LogoRail.cshtml`): one seamless
  strip in a band the hero reserves for it (extra top padding), so it cannot
  overlap content at any width. **Superseded the same day, see the fourth
  amendment:** the owner found the strip redundant and distracting.
- **Logos are drawn once** in a hidden sprite (`_LogoSprite.cshtml`) and referenced
  with `<use>`, so repeating them costs almost nothing. The C# mark's lettering
  uses a `--cutout` color variable so it works in both layers.
- The faint page layer keeps its scattered positions, which is safe because it
  sits behind the content at about 22 percent opacity, and its left edge is
  clamped so no logo is cut off by the screen edge.
- **Scale:** the content column grows from 60 rem to an 80 rem cap on large
  screens; background code scales up above 120 rem; phones use 3 lanes, no card
  blur (cost), and smaller logos.
- **Touch:** phones get a two-row header with a swipeable single-row navigation
  that keeps the current section's link in view; targets are 44 pixels on phones
  and on any touch device; anchor jumps are offset by the taller header.
- **Home-page navigation uses pure `#anchor` links.** Links written `/#anchor`
  dropped any query string and reloaded the page, which would happen to anyone
  who arrives from an email link carrying tracking parameters.

**Fourth amendment, 2026-11-02 (calmer background, no crown, resume):** the owner
found the logo strip "redundant and too distracting" (the same twelve logos
repeated six times, moving) and asked for logos only as scattered background,
and for the crown above his job title to go. Decision:

- **Removed** the scrolling logo rail, its extra hero padding, and the crown
  mark above the title. (The favicon keeps its small crown.)
- **Logos are quiet and scattered**, drawn once each: faint silhouettes (about
  13 to 20 percent opacity, no badges) with a very slow drift. They are split
  between two layers: the first `TechLogoData.HeroLogoCount` (three) sit in the
  hero's top padding, and the rest are fixed behind every other section. Tests
  require each technology to appear exactly once and every logo opacity to stay
  at or below 0.25.
- **Why the hero logos sit in its top padding:** an audit at 17 widths showed
  that percentage positions inside the hero landed on the portrait or text at
  some size, because the hero's height changes at every breakpoint. The fixed
  top padding never holds content, so those logos cannot collide at any width.
- **Résumé download:** a "Download resume (PDF)" button in the hero's actions
  and a Resume card in the contact section, serving
  `wwwroot/files/Randolf_Segubre_Resume.pdf`, an unedited copy of the owner's
  current final résumé. A test requires the file to exist and be a real PDF.

Rules for the animated background: it is purely decorative (hidden from
assistive technology, no pointer events); motion only uses `transform`, so it
stays smooth; it stops for visitors who prefer reduced motion; every floating
tag must appear in the projects, roles or skills (a test enforces this, so the
background cannot advertise a technology the page does not back up); and the
columns keep to the margins and behind the opaque code card so the headline
stays readable.

**Fifth amendment, 2026-11-14 (a hero of its own, "Home" in the header):** the
owner said the name in the header repeats the hero's name, and that the hero
banner and the page background "look redundant" (the same scrolling code sat in
both, by the original design of one continuous surface). He chose a "Home"
link in the header and asked for the hero background to be a unique video or
GIF suited to his background and skills. Decision:

- **The header's first link is labelled "Home"** (the conventional name for a
  link to the top of the site), not the owner's name. The hero's `<h1>` stays
  the one place the name is the page heading.
- **The hero background is a looping video made for this site**, not stock
  footage: his stack as a request path (browser, ASP.NET Core, Umbraco CMS,
  MongoDB, SQL Server, Docker, Jenkins) with gold packets, a small Content
  Security Policy shield and a slow scan line. **Content rule, added
  2026-11-14:** the video may show only technologies he has really worked with
  in his career. The first cut also had an invented "Edge" delivery box (easy to
  misread as the Microsoft browser), a "React" label (about six months
  commercially) and ".NET 10" (personal projects only); he asked for them to go
  and they were removed. It is drawn on a canvas
  by `tools/hero-video/scene.html` and encoded to WebM (about 250 KB) and MP4
  (about 390 KB) with a 15 KB poster; the boxes ring the edges so the text
  and the code card stay clear. Video was chosen over a Graphics Interchange Format (GIF) file: a GIF of this
  size would be several megabytes.
- **The hero's scrolling code lanes are removed.** The faint code-and-logo
  layer stays behind the rest of the page, so the hero is plainly a separate
  banner. The three hero logos stay.
- **Playback is polite.** No `autoplay` attribute, `preload="none"`, muted,
  no controls, hidden from assistive technology; `site.js` plays it only on
  screens wider than 42rem when the visitor has not asked for reduced motion or
  data saving, and pauses it while the hero is off screen. Under reduced
  motion, before it plays, and without scripting, the stylesheet shows a still
  frame. Phones hide the video (a tall, narrow hero would show a zoomed strip)
  and fetch neither the video nor the still.
- **Content Security Policy:** `media-src 'self'` is added (ADR-0009).
- **Known limit:** the page has continuous decorative motion (the page layer
  and, on wide screens, the video) but no pause button. Reduced motion is
  respected; a pause control would be needed to meet the Web Content Accessibility Guidelines (WCAG) success criterion 2.2.2 fully, and is
  recorded as an open item in the tasks.

**Sixth amendment, 2026-11-14:** the owner chose further parts of a new design
(RS monogram, greeting, margin code tags, letter-by-letter headings, a page-change
veil and a skills star map). They are recorded in
[ADR-0010](ADR-0010-skills-star-map-and-page-transitions.md); the background,
video and portrait described here are unchanged.

## Consequences

- The design has a recognisable idea that is the owner's own: his stack as the
  visual language.
- No third-party requests and no animation dependency to maintain.
- The stats and the code card are generated from data, so wording changes stay
  in `Content/ProfileData.cs`, and tests check them (percent signs are
  rejected, and the card must match the profile).
- The hero has more moving parts than a plain layout. Accepted: the backdrop is
  decorative, hidden from assistive technology, and switched off under reduced
  motion.

## Alternatives rejected

- **A close copy of the reference layout:** unoriginal and not the owner's
  request.
- **Skill percentage bars:** fashionable and unverifiable.
- **GSAP or another animation library:** a third-party dependency and network
  request for effects a small script can do.
