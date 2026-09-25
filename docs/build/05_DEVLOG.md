# Devlog

Reverse-chronological. One entry per session or phase boundary: what was
asked, what changed and why, how it was verified, what is next. Full
narrative belongs here; `docs/build/04_TASKS.md` stays a short current
snapshot.

---

## 2026-09-26: New portrait, and GitHub and LinkedIn icons

**Asked:** change the portfolio image to the new photo (blue shirt, glasses),
and include the GitHub profile and LinkedIn.

**What changed:**
- **Portrait replaced.** The first photo he sent (the graduation portrait) was
  removed from the site. The new photo arrived saved on disk this time
  (1474 by 1069), so I cropped a 520 by 520 square centered on the face (head,
  glasses, hand and shoulders), resized it to 640 by 640 and saved it as a 50 KB
  JPEG, `wwwroot/images/randolf.jpg`. Nothing else in the image was altered.
  The link-preview image (email, LinkedIn, chat apps) uses the same file.
- **GitHub and LinkedIn** were already linked in the hero and the contact
  section; they now carry recognisable icons in both places. The GitHub icon is
  the public-domain Simple Icons shape; LinkedIn is not in that set, so its mark
  is a hand-drawn "in" square, flagged as an approximation in the notices file.
  The icons are decorative and follow the button colors and hover state.

**Verified:** 112 tests pass (new: both profiles are linked with icons in the
hero and the contact section, with safe new-tab links). In a real browser at 375
pixels the new portrait loads (640 by 640, shown at 120 pixels), the icons render
at 18 pixels in the button color, and there is no horizontal overflow. The
published output contains only the new photo file.

---

## 2026-09-25 (after midnight): Phone numbers, and the correct Santa's Lapland address

**Asked:** show his mobile and Viber number (+63 917 1022 203) and his WhatsApp
number (+63 920 909 3036) in the portfolio; then, correcting an earlier mistake
of his, the Santa's Lapland site is `santaslapland.com`, not `.co.uk`.

**What changed:**
- **Contact cards** for both numbers: "Mobile and Viber" is a tap-to-call link
  plus an "Open in Viber" link, and "WhatsApp" opens the official WhatsApp chat
  link in a new tab. Both are shown publicly at his request.
- **Santa's Lapland corrected** to `https://www.santaslapland.com/`. It loads
  (the `.co.uk` address did not from this machine), and its own public title and
  description now back the card's summary. The earlier note in the entry below
  that the site could not be loaded no longer applies. The allowed-domain test
  now lists the corrected host.

**Verified:** 111 tests pass, including that each number is a valid
Philippine mobile number and that every link (call, WhatsApp, Viber) carries
exactly the number shown.

**Privacy note given to him:** phone numbers on a public page can be collected
by automated scrapers and used for spam. He chose to publish them; they can be
removed by deleting the `Phones` entry in `Content/ProfileData.cs`.

---

## 2026-09-25 (late night): Deep royal gold and "Live sites I contributed to"

**Asked:** (1) make the yellow text a deep gold, "the royal gold of old"; (2)
confirmed that Inghams.co.uk and SantasLapland.co.uk may be included as long as
only publicly available information is used; (3) use a new photo as the
portfolio image.

**What changed:**
- **Gold:** every gold token in the dark palette moved from a bright lemon
  yellow (`#f2cd6b`, `#f0c75e`) to a deep old gold, `#c9a227`, which still
  reads clearly on the purple (lowest contrast 5.5 to 1 on the hero). The
  favicon matches. The contrast tests parse the stylesheet, so they checked
  every pair automatically.
- **Live sites section** between the stats strip and the projects, with a
  "Live sites" navigation link: two cards, each with a "Live site" badge, the
  public address, his plain-language part, a caveat, and technology chips.
  Only public information: the Inghams features named (wishlist, resort and
  country pages) were checked on the public site; Santa's Lapland could not be
  loaded from this machine, so its card claims nothing beyond its name and
  his role (the one response it gave redirected to an internal hosting
  address, which appears nowhere on the site, and a test keeps it that way).
- **Guard tests:** links only to the two public domains; no ticket numbers,
  internal tool or system names, repository names, hosting addresses or
  configuration terms; roles say "contributed", never "built" or "led"; every
  technology chip is backed by the experience or skills.

**Not done yet:** the new photo. It was shown in chat but never saved to disk,
so I could not read it; I asked him to save it and give me the path.

**Verified:** 109 tests pass; the exported page contains no hosting address;
in a real browser at 375 pixels the section renders with no overflow, both
links correct, and the new gold measured as `rgb(201, 162, 39)`.

---

## 2026-09-25 (night): Lanes that cannot overlap, page-wide background, real logos

**Asked:** the animated code columns overlapped each other; make the background
cover the whole page; replace the text technology tags (C#, ASP.NET and others)
with logos.

**What changed:**
- **Overlap fixed at the root.** Columns were positioned by percentages and
  crossed. They are now equal-width lanes of a CSS grid (8, 5 or 3 by screen
  width), so overlap cannot happen. Added an eighth lane (Docker and Jenkins).
- **Whole-page coverage.** The same lanes and logos exist in the hero
  (stronger) and fixed behind the whole page (quieter), and cards let a little
  through. A soft dark halo behind the hero copy keeps the paragraph readable.
- **Logos.** 12 technology logos as inline vectors from Simple Icons 16.32.0
  (CC0): .NET, Umbraco, Vue.js, React, Blazor, Docker, Jenkins, MongoDB, MySQL,
  GitHub, Next.js, plus a hand-drawn C# hexagon. Hero logos sit in light round
  badges; the page-wide ones are quiet silhouettes so text never sits on a bright
  shape. `docs/THIRD_PARTY_NOTICES.md` and a footer note record the source and
  the trademark position.
- **Limits, stated plainly.** The open icon set has no C#, SQL Server, Azure or
  ASP.NET Core logo (Microsoft asked for its logos to be removed). ASP.NET Core
  is represented by the .NET logo; SQL Server and Azure are not shown as logos.
  The C# mark is my own drawing, an approximation of the real one.

**Verified:** 103 tests pass (new: every backdrop technology has a logo and is
backed by the page; logos are inline vectors so the only image file is the
portrait; lanes are grid columns). In a real browser at 1280 pixels: 8 lanes of
158 pixels with 0 overlaps in both layers, 12 logos in each layer with 0
overlaps, 0 failed or third-party requests. At 375 pixels: 3 lanes, 5 logos, no
overlaps with the portrait or text, no horizontal overflow.

**Caught along the way:** on phones the hero is very tall, so logos placed by
percentage from the top landed on the portrait; top logos are now pinned into
the hero's top padding there.

---

## 2026-09-25 (evening): Portrait and a stronger animated code background

**Asked:** Add his graduation photo, and make the background like the
reference's animated code but with his own tech stack and moving code, "like a
gif background".

**What changed:**
- Portrait added (`wwwroot/images/randolf.webp`, 100 KB) as a gold-ringed
  circle beside the name, with alt text, a fixed size (no layout shift), no lazy
  loading (it is above the fold), and as the link-preview image for email and
  social sharing.
- **Animated background rebuilt** as scrolling columns of code (C# MVC, EF Core
  with MediatR, Razor, Vue.js, T-SQL, xUnit and React, Umbraco with a Content
  Security Policy nonce) that loop endlessly, plus 12 floating technology tags,
  plus a very faint fixed copy of the columns behind the whole page. Code
  lives in `Content/BackdropCode.cs`; the loop is seamless because each column
  holds its code twice and scrolls exactly half its height.
- New rule, enforced by a test: every floating tag must appear in the
  projects, roles or skills.

**Not done:** he also asked me to "correct and generate an image" by reshaping
facial features (hairline, ears, nose) in the graduation photo to match a
second photo. I cannot generate or reshape faces with the tools available, and
crude pixel edits would look wrong on a public page, so I told him plainly and
kept the photo unedited. The second photo was not saved to disk either. Options
given to him: use the unedited second photo instead (one-line change), or have
a photo editor or the photographer produce a corrected version.

**Verified:** 100 tests pass. In a real browser: the code columns measurably
scroll (transform changes over time), 12 tags and 7 page-wide columns render,
the portrait loads at 1254 by 1254 and displays at 160 pixels, zero failed or
third-party requests, no overflow at 375 pixels (two columns and five tags on
phones, at lower opacity so the text stays calm).

**Caught along the way:** the leftmost column drifted under the start of the
paragraph, so it now sits in the left margin and is hidden on tablet widths.

---

## 2026-09-25 (later still): "Developer identity" concept, inspired by a reference and made original

**Asked:** "Make the concept like RujeAlfon but not a copy. It should be unique
but use RujeAlfon's portfolio as a reference."

**How I used the reference:** I read the live page's structure and text (a
full-screen hero with faint code snippets, GSAP scroll animations, a photo and
animated skill percentage bars, project cards with role and date, a timeline,
a "Let's Work Together" contact section, navy and green). I took the *idea*
(a developer's identity expressed through code and motion) and nothing else:
none of its text, code, colors, photo, snippets or animation library.
ADR-0008 records what was borrowed as a concept and what is original.

**What changed:**
- **Hero:** two columns. The copy sits beside a `Developer.cs` card that
  renders the owner's profile as a C# record from the same data, over a faint
  backdrop of drifting C# and Razor fragments kept to the edges so the headline
  stays readable.
- **Stats strip** (9+ years, 3 industries, 5 employers, 660+ commits) replaces
  the reference's skill percentage bars, which cannot be verified.
- **Vanilla script, no library:** scroll reveals, an active-section highlight in
  the navigation, and a gold scroll-progress bar. Everything is an enhancement
  and is disabled by reduced motion.
- Contact became "Let's talk" with large link cards.

**Verified:** 94 tests pass, including new ones for the stats, the code card
(rendered from the profile, hidden from assistive technology), the decorative
backdrop, the reveal wiring, and contrast for the new code colors. In a real
browser: two-column hero at 1280 wide, single column at 375 with no overflow,
the progress bar scales 0 to 1, the right nav link is marked at each section,
and cards reveal as they scroll into view.

**Caught along the way:** a few backdrop fragments drifted behind the
headline and hurt readability, so they were moved to the edges and behind the
opaque card, and thinned to three on phones; a new reveal transition would
have overridden the cards' hover transitions, fixed with a zero-specificity
`:where()` selector.

---

## 2026-09-25 (later): Royal purple restyle, dark by default

**Asked:** "make the website look better, with styles", in royal purple like
King David's or King Solomon's color, then: "make it a dark theme", clarified
as "dark theme by default".

**What changed:**
- New visual identity in `site.css`: deep Tyrian purple with gold accents, a
  serif display face from the system font stack, a purple hero with a gold
  glow, faint brocade pattern, inline crown mark and gold rule, cards with
  depth and hover lift, a purple-to-gold stripe on featured projects, gold
  list markers and status dots, a vertical experience timeline with gold
  markers, pill chips, and a purple header and footer with gold hairlines.
- **Dark is the default for every visitor**, even when their system is set
  to light. The dark palette is the base `:root`; the light palette is an
  optional override applied only through the toggle. The
  `prefers-color-scheme` query was removed on purpose (ADR-0004 amended).
  `theme.js` no longer reads the system setting, and the toggle now says
  "Light" first.
- Favicon and browser toolbar color changed to purple and gold.

**Verified:**
- 77 tests pass. New `DesignTokenTests` parse the real stylesheet and check
  contrast for every text and background pair in both themes, that the light
  block only overrides tokens defined in the dark base, and that no
  system-setting media query exists. New render tests cover "dark by default".
- In a real browser with the **system set to light**, the page still renders
  dark purple (`rgb(18, 10, 36)`); a real click on the toggle switches to
  light and back, each choice is stored, and a reload keeps it. No overflow at
  375 pixels, no failed or third-party requests.

**Caught along the way:** the new contrast test flagged the bright gold used
for markers and underlines at 2.1 to 1 on the light background; the light
theme now uses a deeper antique gold (`#b07d00`). I also wrote one over-clever
test assertion and replaced it with a single clear one before it was ever
committed.

---

## 2026-09-25: Rebuilt in ASP.NET Core MVC on .NET 10 and published

**Asked:** Randolf asked to publish the site so the link can go in an email to
Ben. Mid-publish he asked which stack it used and whether it could use his
main stack. He chose "C#, ASP.NET MVC, with the latest .NET and C# version".
Nothing had been pushed yet, so the switch cost no public history.

**What changed:**
- Replaced the React and TypeScript front end with an **ASP.NET Core MVC**
  application on **.NET 10 (C# 14)**: `HomeController`, Razor layout, page and
  partial views, C# record content behind `IPortfolioContent`, dependency
  injection, an xUnit test project. New decision record ADR-0007 supersedes
  ADR-0002; ADR-0001, 0003, 0004 and 0005 were updated to match.
- GitHub Pages cannot run a server, so the app exports itself at build time:
  `dotnet run -- --export "$PWD/dist"` starts Kestrel on a free port, saves
  `/` as `index.html` and `/not-found` as `404.html`, copies `wwwroot`, and
  exits (`StaticExporter`). The deployed site is that static output. I did
  not silently narrow "MVC" to "static": the source is real MVC and the live
  site is its export, stated in ADR-0007 with the rejected alternatives.
- Kept the content, wording, styles, and documentation standard unchanged;
  the writing-rule test was ported to xUnit and extended with rendered-HTML
  and exporter tests.
- Rewrote the deploy workflow for .NET (setup, test, export, upload, deploy).
- Created the public repository and set Pages to the "GitHub Actions" source
  before the first push, so the first workflow run could deploy.

**Verified:**
- `dotnet build` (warnings as errors) and `dotnet test`: 18 tests pass
  (content rules, rendered HTML through the real MVC pipeline, exporter).
- The export produces `index.html` (about 30 KB), `404.html`, stylesheet,
  script, favicon, `robots.txt` and `.nojekyll`; a relative export path is
  rejected with a clear message.
- Served the exported files like Pages would and checked in a real browser at
  375 pixels wide: all five sections, 12 cards and roles, no failed or
  third-party requests, no horizontal overflow, and a real click on the theme
  toggle switches the theme, stores the choice, and updates the label.

**Caught along the way:** Razor rejects nested quotes inside tag-helper
attributes (moved those expressions into code blocks); `dotnet run` starts in
the project folder so a relative export path landed in the wrong place (now
rejected, absolute path used in CI); my throwaway test server compared
forward-slash and backslash paths and served the 404 page for everything
(fixed the script, not the site).

**Published:** pushed `main`; the GitHub Actions run passed (test, export,
deploy) and `https://randolfsegubre.github.io` returned 200 for the page and
every asset, with a real 404 page for unknown addresses. Checked in a browser
against the live address: five sections, 12 cards and roles, theme toggle
revealed, no failed or third-party requests.

**Next:** the owner reviews wording; then link the site from the GitHub
profile README. See `docs/build/04_TASKS.md`.

---

## 2026-09-24: Architectural Design Phase, scaffold and Phase 1 built locally

*(First build, in React and TypeScript, replaced on 2026-09-25 by ADR-0007.
Kept as history.)*

**Asked:** Randolf asked whether a GitHub Pages portfolio needs payment (it
does not), then to scaffold the website portfolio and "do our standard
process", meaning the Architectural Design Phase.

**What changed:**
- Created the repository at `D:\_DEV\Personal_Projects\randolfsegubre.github.io`
  (single-repo shape, like Ophir and Grace at Work: root `CLAUDE.md` pointer,
  operating docs under `docs/build/`).
- Produced the six Architectural Design Phase artifacts before application
  code: ADR-0001 to ADR-0006, `docs/build/01_CLAUDE.md`, `02_BUILD_PLAN.md`,
  `03_ARCHITECTURE_AND_PATTERNS_GUIDE.md`, this file and `04_TASKS.md`, and
  `docs/DEVELOPER_HANDBOOK.md`.
- Built Phase 1: typed content modules, five sections, theme toggle, a
  content integrity and writing-rule test, a render test, `WALKTHROUGH.md`
  and `docs/CONTENT_SOURCES.md`, all following the in-code documentation
  standard (TSDoc summaries, numbered STEP comments, a walkthrough guide).
- Chose Vite, React and TypeScript (ADR-0002) after weighing plain HTML,
  Astro, Next.js static export and Blazor WebAssembly; the last would load
  several megabytes before a recruiter sees anything.
- Wrote the deploy workflow but did not publish: creating the public
  repository and pushing are outward-facing and wait for approval.

**Decisions on content (ADR-0006):** the site names only public repositories,
states status and known gaps plainly, uses "Solo personal project" for role
wording, and leaves off a private repository's link, confidential client work
and an e-commerce demo whose front end was never connected.

**Verified:**
- `npm run typecheck`, `npm test` (13 tests) and `npm run build` pass; the
  production bundle is about 76 kilobytes of JavaScript compressed.
- Checked in a real browser: no console errors, zero third-party requests,
  one `h1`, all outbound links carry `noopener`, no horizontal overflow at 375
  pixels wide, and the theme toggle works with a real click and persists.
- Measured text contrast for every color pair in both themes: all pass the
  4.5 to 1 minimum (lowest measured 4.80 to 1, for the focus ring).

**Caught and fixed along the way:**
- The content test found four prose lines with bare abbreviations (JSON, API,
  JWT, and ASP stranded by an ordering bug in the allow-list); fixed the
  copy and reordered the allow-list longest first.
- A test query for the page banner also matched card `<header>` elements;
  tightened the test.
- At phone width the sticky header wrapped to three rows; restructured it to
  two rows and made it non-sticky on very short viewports.

**Next:** Randolf reviews the content; on approval, create the public
repository, set Pages to GitHub Actions and push (Phase 2). See
`docs/build/04_TASKS.md`.
