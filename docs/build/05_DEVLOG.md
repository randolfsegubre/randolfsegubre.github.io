# Devlog

Reverse-chronological. One entry per session or phase boundary: what was
asked, what changed and why, how it was verified, what is next. Full
narrative belongs here; `docs/build/04_TASKS.md` stays a short current
snapshot.

---

## 2026-11-17: content in JSON files and SQL Server, and a three-layer backend (branch `sql-content`)

**Asked:** the data is all hard-coded, which is not best practice: put it in a file type made for data and in a Microsoft
SQL Server database (not SQLite), after cleaning up and deploying. Then, mid-work: use a three-layer architecture. Then, as
questions: is three-layer also N-tier, and what would the projects look like if it were N-tier.

**Chosen with the owner:** JSON files in the repository are the source of truth; a seed step loads them into SQL Server;
the build starts its own SQL Server container, because GitHub's build server cannot reach a laptop. Production stays
static. (The alternative offered was the local database as the source of truth with a committed snapshot; not chosen.)

**What was built (ADR-0012, ADR-0013):**
- `content/` (12 JSON files plus a README carrying the rationale that used to sit in C# comments). They were produced by
  a throwaway program that read the old classes, so nothing was retyped. The 13 hard-coded classes were deleted.
- `Portfolio.Data`: row classes, `PortfolioDbContext` (schema `content`, one table per kind of thing, one-row tables
  enforced by a check constraint, foreign keys for the skills tree, journey and icons, a filtered unique index for a
  single centre star, status, color and length rules), the first migration, `ContentRepository` (read all, replace all in
  one transaction), and a design-time factory.
- `Portfolio.Business`: the models, the strict JSON reader (`ContentFiles`), `ContentMapper` (rows to records and back),
  `ContentService` (`IContentSource` and `IContentMaintenance`), the builder, the skills-cloud layout.
- `Portfolio.Api`: `Program.cs` with `--migrate`, `--seed`, `--export-content`; content is read once at start-up.
- `LayeringTests` check the compiled assemblies: presentation to business only, business to data, data to nothing.

**Proof:** the document built through SQL Server equals the one the C# classes produced, except line endings inside the
decorative code lanes (the C# raw strings had Windows endings, so Windows and Linux builds used to differ; the data files
use LF). The generated site is identical to the deployed one, file by file.

**Tests:** 179 became 197 without a database (layering, mapper, service, strict file reading) plus 17 against a real SQL
Server (round trip, idempotent seeding, empty database, and 13 ways the schema must refuse nonsense, each leaving the old
content intact). Databases are throwaway with random names; none were left behind.

**Found and fixed on the way:** (1) EF Core 11 targets net11 and does not install on net10: pinned the 10.0.15 line.
(2) A deadlock in my own test helper: a static initializer waited on a worker thread that read another static of the same
class. Found because the run hung; fixed by passing the folder as an argument, and `--blame-hang-timeout` is now in the
skill. (3) A pipeline that cut the build command off early looked like a database failure; it was not.

**Not verified:** the workflow's SQL Server container step has never run (Docker is not running locally; the SQL Server
2025 image has no `sqlcmd`, so readiness waits for the server's own "ready" log line). A manual run of the workflow from
the branch builds and tests but cannot deploy (`deploy` is limited to `main`). **Nothing from this change is pushed.**

---

## 2026-11-16 (clean-up before the database): dead code, a blank-404 race, unused output files

**Asked:** clean up unnecessary files and code without breaking logic, then push and deploy, then move the content into
a Microsoft SQL Server database.

**Method:** saved the generated site as a baseline first, so the effect of every change could be proven. Machine checks:
strict .NET analyzers (no unused usings, members or parameters), a script comparing every stylesheet class with the pages
and scripts (151 of 151 used), a script for unused names in the scripts and unused Vue components (19 of 19 used), and a
script for record fields nothing reads (127 of 127 read). The code was already tight, so the code clean-up is small.

**Found and fixed (the important one):** the first regenerate after the clean-up failed four not-found tests.
Nuxt prerenders its own empty fallback to `/404.html` as well as my real `/404` page, both to `404.html`; whichever finished
last won, so a build could publish a blank 404 page. Earlier green runs were luck. Fixed with `nitro.prerender.ignore`,
verified over three consecutive builds, and guarded by a new test. Also stopped publishing `200.html`, the `_nuxt/builds`
manifest and `_nuxt/error-404|500.css` (unused), via config plus a guarded `scripts/prune-output.mjs`.

**Small clean-ups:** an unused variable in `site.js`; a stale "controllers" comment on `IPortfolioContent`; three
culture-sensitive calls in the tests made explicit (`CA1310`, `CA1305`).

**Verified:** build with warnings as errors, 179 tests (one new), and a file-by-file comparison with the baseline: the only
differences are the five unneeded files gone and the one dead variable.

---

## 2026-11-16 (page changes): "More projects" carousel, mouse dragging, theme switching hidden

**Asked:** make "More projects" a carousel, draggable like the one on the Inghams site; hide theme switching for now;
and opinions on an expandable (accordion) Experience and a better Let's talk section (answered in chat, not built yet).

**Carousel:** `ProjectsSection.vue` now puts the compact cards in a scroll-snap strip (`.carousel-track` in `site.css`),
which scrolls with no script at all (touch, wheel, scrollbar, and the arrow keys because the strip is focusable and
labelled as a region). `site.js` step 8 reveals previous and next buttons (drawn hidden, like the theme button) only when
the strip overflows, keeps a "Showing 1 to 2 of 4" count, uses `aria-disabled` so keyboard focus survives the ends, and
adds mouse dragging: the strip follows the pointer with snapping paused, then glides to the card the flick was heading
for; a drag swallows the click that ends it so a link under the pointer is not followed. Reduced motion jumps instead
of gliding. No Content Security Policy change (an external script, no inline code).

**Theme switching hidden:** `TOGGLE_ENABLED = false` at the top of `web/public/js/theme.js` removes the button and clears a
saved "light" choice (otherwise a returning visitor would be stuck in light with no way back). Light styles, markup and
tests are untouched; set the flag to `true` to restore it.

**Verified:** 178 tests, a real browser (next, previous, ends, count, simulated mouse drag and flick, plain link click
still works, console empty, no sideways page scroll at 390 px wide). **Not verified:** a real mouse (the drag was
simulated with pointer events), Safari. Nothing committed.

---

## 2026-11-16 (cleanup): leftovers of the rewrite removed

**Asked:** remove the folders and files that are not necessary.

**Removed (all untracked build leftovers, nothing the repository tracks):** `src/Portfolio.Web/` (only an old `obj`
folder was left), the root `dist/` (the old MVC export, referenced by nothing now), and the `web/dist` junction (a link
to `web/.output/public`; only the link went, the output stayed). **Kept on purpose:** `web/.output` (the running local
site), `web/.nuxt`, `web/data`, the `bin`/`obj` folders and `node_modules` (regenerable, but removing them only costs
a rebuild), and every tracked asset (each one is still referenced: the hero video in both formats, photo, favicon,
resume, robots, sitemap). **Stale text fixed:** `03_ARCHITECTURE_AND_PATTERNS_GUIDE.md` rewritten for the API plus Nuxt
shape (it still described Razor views and `StaticExporter`), and old `Portfolio.Web`, `wwwroot` and "live MVC server"
paths in `02_BUILD_PLAN.md` and `04_TASKS.md`. **Verified:** build with warnings as errors, 178 tests pass, site still
answers 200 on port 5099.

---

## 2026-11-16 (later): a .NET API feeding a Nuxt front end, on the branch `nuxt-rewrite`

**Asked:** can a portfolio use Vue.js with .NET as the API, and would it work on github.io; then: sketch the
Nuxt plus .NET version, code it, follow best practice, keep the output page the same, and first commit, push
and deploy the working version so it can be reverted. Mid-work: rename the project to match its new job, update
folders and files accordingly, never bypass or turn off security, and leave localhost running.

**Safety net first:** six logical commits of the redesign, pushed to `main`, deployed (run green, live page
checked: 45 spheres, About Me), and tagged `pre-nuxt-rewrite`. The rewrite is on branch `nuxt-rewrite`, because a push
to `main` deploys.

**What I built (ADR-0011):**
- `src/Portfolio.Web` became `src/Portfolio.Api` (folder, project, namespace, solution, tests, workflow, scripts,
  living docs). Visual Studio held the old folder open, so the files were moved one by one.
- The API: `SiteContentBuilder` builds one JSON document, served at `GET /api/content` and written by
  `--export-content`; both use the same builder and serializer. The Razor views, controller and static exporter are gone.
- `web/`: Nuxt 4.6.4 (Vue 3.5), 19 components mirroring the views, `features.noScripts` so no Nuxt JavaScript ships.
  The stylesheet and the three page scripts are the same files, moved to `web/public`.
- Tests: `SiteFactory` serves `web/.output/public`, so the existing 173 tests now check the real output (proof of
  sameness), plus `ContentApiTests` and `SiteOutputTests` (178 in all).
- Parity check: a structural comparison of the old exported page and the new one (every element, attribute and text)
  found only the footer sentence, a trailing semicolon in an inline style, and the decorative code text that carries the
  renamed namespace.

**Security, as asked (nothing bypassed or turned off):**
- The existing security test failed on the first run: Nuxt put the Content Security Policy after the theme script and the
  stylesheet, and a meta policy only covers what follows it. Fixed by the tag priority, not by editing the test.
- Nuxt added an inline speculation-rules script the policy blocks. Removed with a Nitro plugin; the policy is unchanged.
- `npm audit` found 10 advisories; updating Nuxt to 4.6.4 and a `minimatch` override left one low finding
  (`deepmerge`, no patched release exists), recorded as accepted in ADR-0011. npm install scripts are left unapproved.
- The API got security headers, no cross-origin policy, no Server header, HTTPS and HSTS outside development; the local
  static server is localhost-only, GET and HEAD only, with path-traversal and dotfile refusal. All tested.
- The deploy workflow pins `setup-node` to a commit hash, installs with `npm ci`, and fails on a .NET vulnerability or a
  moderate or worse npm one.

**Two page captions corrected:** the ASP.NET MVC sphere no longer says the portfolio is built on it, and the Nuxt.js sphere
now says the portfolio is generated with it (true from today).

**Verified:** strict build, 178 tests, real browser on the generated output: console empty, 45 spheres, 3D cloud, theme toggle,
policy first in head. **Not verified:** the new workflow on GitHub (it has not run), a real touch phone, Safari.
**Nothing from the rewrite is committed, merged or deployed.**

---

## 2026-11-16: JavaScript joined to React and AngularJS, Nuxt.js added

**Asked:** "Nuxt.js is not on the map. Also, why JavaScript is not connect to react and angular like
vue.js." Both are fair: JavaScript had a single parent link (Vue.js) because the map is a tree, and
I had held Nuxt back after asking whether he had used it (he repeated the request instead).

**What I did:** two cross links from JavaScript to React and to AngularJS (a tree has one parent, so
the other relationships are cross links); Nuxt.js as a sphere under Vue.js with its CC0 logo, a
generic caption and a row in `CONTENT_SOURCES.md` saying it is on his instruction and that memory
holds it only as a LinkedIn skill; Nuxt.js added to the grouped skills list. Verified in a real
browser: choosing JavaScript lights Vue.js, React and AngularJS, Nuxt.js shows in its green, nothing
past the edge, console empty. **Note for him:** an interviewer may ask about Nuxt, and nothing in his
record backs a project answer.

---

## 2026-11-15 (afternoon): the main stack drawn larger

**Asked:** "make the other stack that not my main on a smaller circle." I read it as smaller
spheres for everything outside his main stack (an alternative reading, a smaller orbit, would put
them closer to .NET, which is the opposite of what the cloud needs).

**What I did:** `SkillStar.Main` on the five signature-stack technologies (C#, ASP.NET Core, Umbraco,
Vue.js, SQL Server), the `star-main` class in the markup, and separate sizes in `site.js` for the
centre, the main stack and the rest. A test ties the flag to `Profile.SignatureStack` so there is one
definition of "main". Measured in a real browser: .NET 102 pixels, main 79, others 41 to 54, nothing
past the edge, console empty. **Not re-checked:** the phone view (the spheres only got smaller).
**Open:** whether other technologies (for example JavaScript, Web API or EF Core) should also count as
main; today only the signature stack does.

---

## 2026-11-15 (midday): only what he has used, a lighter cloud, About Me, a crowned monogram

**Asked:** change About to About Me in the header; give RS a crown with a cross engraved like
a hole in its front; the 3D graph is too cramped, so remove the nodes that are not tech stack and
the stack he has not experienced (Azure Service Bus, TanStack Query named), keeping only what he
used directly, personal projects included; check memory, claude-dotfiles and GitHub first.

**What I checked:** his memory (profile, resume facts, the Aluvii and 77GSI notes: "he has NOT
used Azure Service Bus", the 2026-11-02 note "he has no TypeScript experience", the Q8 line
"budgeting project with AI help uses TanStack Query"), the dotfiles memory (same files) and
`gh repo list`. Lesson from my own last turn: I had treated "appears in a project stack" as
"he has experience", which is the exact mistake his memory already warns about (a package in a
project is not experience). The rule now is his own: only what he has used himself.

**What I did (ADR-0010 fifth amendment):**
- 68 spheres became 44; the denylist test keeps the removed ones out. Practices and techniques
  are not spheres. Copilot and Claude Code hang from Git and GitHub. Azure DevOps added (and to the
  grouped list; TypeScript left it).
- Found two wrong facts while checking: the BudgetPH card still listed MediatR (dropped
  2026-10-01), and the Scoot bullet said "designed" and "achieved 90 percent" (he corrected both
  on 2026-11-06). Fixed both; also reworded the TanStack Query, Zustand and Azure Service Bus sentences.
- "About Me" in the navigation and the heading; the crown is one even-odd path above RS.
- Spheres enlarged a little now there is room; the phone zoom lowered to 0.62 because a full turn
  pushed names up to 7 pixels past the edge at 0.66.
- The checklist line I added this morning ("every skill on the map") was wrong and is replaced.

**Verified:** strict build clean, 172 tests passing, real browser at 1440 and 390 pixels, console
empty, nothing past the edge over a full turn on a phone, no sideways scrolling, crown inspected
zoomed in (the cross shows the header through it). **Not verified:** a real touch phone, Safari,
the live site. **Open:** Nuxt.js (asked for; memory says it is only a LinkedIn skill, so waiting for his confirmation).

**Follow-up, same day (his yes):** removed the GraphQL (Lakbay), Vite (BudgetPH), Semantic Kernel
and SQLite (AI Tutor) tags from the project cards, reworded the AI Tutor highlight to "a local Ollama
model behind it", and dropped SQLite from the grouped skills list so the page does not contradict
itself. He also asked for Nuxt.js. His memory says Nuxt is only a self-declared LinkedIn skill with
no project or role behind it (levate note: "Nuxt only a LinkedIn skill"), which is the exact case his
rule excludes, so I asked him rather than add it.

---

## 2026-11-15 (morning): every skill on the map, a real AngularJS logo, documentation as a gate

**Asked:** "I am not sure if updating documentations accordingly is in our workflow but
please sure that it is. Also, Angularjs, is that its logo? And all of my skills and stack
are not in there too."

**What I found:**
- *Documentation:* the checklist already required `WALKTHROUGH.md`, `04_TASKS.md` and
  the devlog, but said nothing about ADR amendments, the notices file for new assets or
  timing, so docs could lag. Made explicit in `06_BEST_PRACTICE_CHECKLIST.md` section 6
  and the operating manual: done means in the same turn, and an amendment must also fix
  the earlier sentences it makes false (I fixed three in ADR-0010 this time).
- *AngularJS:* it was a generic "sliders" glyph, not a logo. The CC0 icon set has no
  AngularJS mark (only the newer Angular, now black), so the sphere draws that shield in
  AngularJS red and the notices file says it is an approximation.
- *Skills:* the map had 19 spheres; the grouped list and the role and project stacks have
  many more. Now 68, one for every skill and stack entry, enforced by a new test.

**What I did (ADR-0010 fourth amendment):**
- Three shells (radius 260, 430, 600) because two were too crowded; the flat no-script map
  was removed (it cannot hold 68 stars) and the caption list is the fallback.
- Names of far spheres fade out first, long names wrap, the cloud is a larger square under
  the About text, the walk is 1.8 seconds a step.
- 18 more Simple Icons logos and 17 more glyphs; captions written only from facts already
  in `CONTENT_SOURCES.md`, with the thin ones marked "generic" there.
- Found while testing: a hash-only navigation does not reload, so my first look at the
  browser showed the old 19-star page; reloaded with a query string.

**Verified:** strict build clean, 171 tests passing, real browser at 1440 and 390 pixels,
console empty, no sphere past the edge and no sideways scrolling, choosing AngularJS lit
its connections and showed its caption. **Not verified:** a real touch phone, Safari, the
live site. **Known:** dense on a phone; the owner may want fewer, grouped spheres.

---

## 2026-11-15 (small hours): bigger spheres, an even arrangement, the page made to match

**Asked:** "Make the spheres bigger. Make the whole design of the page appropriate.
Fix the arrangement. And update the documentation accordingly."

**What I did (ADR-0010 third amendment):**
- *Arrangement:* the cause of the clumping was the layout, not the spheres. Rings per
  tree level plus a random-looking height spread let spheres line up along the view
  axis and pile up behind the centre, and bigger spheres made it obvious. The layout now
  puts the four first-step stars on an inner sphere and the other fourteen on an outer
  one, each spread with a golden-angle spiral (`SkillUniverseLayout.SpreadOverSphere`).
  New test: the nearest two stars on each sphere are at least half a radius apart.
- *Size:* the centre about 107 px and the rest about 51 to 70 px on a wide screen, in a
  700 px square cloud; names have a 10 px floor on phones.
- *Page design:* "About me" is now a hero-purple band (both themes) with the cloud
  directly on it and a golden glow behind .NET, wider than the usual column; the grouped
  skills list is a plain section underneath. The page background still shows faintly
  through the band. The sun's repeated ".NET" label is visually hidden.

**Verified:** `dotnet test` 172 passing (the orbit tests became sphere-spread tests;
the details selector no longer assumes the list is inside the About section);
warnings-as-errors build clean. In a real browser: desktop and 390 px, the cloud evenly
spread at several rotations, far spheres fading back, no page overflow. **Not verified:**
a real touch phone, Safari, and the live site. **Known and accepted:** two spheres that
line up along the view axis can overlap for a moment as the cloud turns; the farther one
is faint then, and it clears as the cloud rotates.

---

## 2026-11-14 (late night): the skills become a cloud of logo spheres

**Asked:** after looking at `jack.gif` again: "notice the stack design, the
arrangement; I want mine like that, with .NET at the centre", then "use spheres
instead of pills, use their logos with a small name under the logo, and expand
them", and "what is this design called, is it a Hoberman sphere?".

**Answer to the naming question (also in ADR-0010):** the reference is a 3D tag
cloud (sphere or globe cloud). Ours is a 3D radial node graph (radial tree or
constellation map). It is not a Hoberman sphere (a mechanical expanding toy); the
"solar system" wording I used earlier was loose.

**What I did:** each star is now a glossy sphere with its logo and name underneath
(`SkillIconData`, a second hidden sprite `_SkillIcons`, `SkillStar.Icon`);
the cloud is square and sits beside the About text on wide screens; shells are
wider and the camera looks down at about 37 degrees so the spheres spread around
the centre; connection lines appear only for the chosen sphere; touch devices can
swipe it because nothing needs sideways scrolling any more. ASP.NET Core got a
globe glyph so only the sun carries the .NET logo.

**Found on the way:** with a flat camera the spheres lined up behind the centre
and overlapped it (fixed by raising the pitch and spreading the first ring); the
flat fallback's outer chain overlapped (the overlap test caught it; rings opened
up); the existing sprite test counted every `use` element and broke when a second
sprite appeared (now scoped to logo symbols).

**Verified:** `dotnet test` 172 passing (new: every star has a resolvable icon,
unique valid icon ids, no dead icons, each star rendered as sphere plus logo plus
name with its symbol defined). In a real browser: desktop and 390 pixel width, no
page overflow, spheres readable. **Not verified:** a real touch phone, Safari, and
the live site. **Open:** he may want the spheres larger still (he said "expand");
the size table is in `site.js` (`ORB_SIZES`).

---

## 2026-11-14 (night): About me, and the skills become a 3D solar system

**Asked:** "Skills should have been grouped with About me. Make the nodes
floating, like a multiverse. They should appear 3D and rotatable like a solar
system." (Local only; test thoroughly before any deploy.)

**What I did (ADR-0010 amendment):** a new `_AboutSection` replaces the Skills
section: `Profile.About` (two sourced paragraphs), then the map, then the
grouped list. Navigation now starts with "About". The map is drawn flat on the
server (still the no-script fallback), and `site.js` step 5 turns it into a 3D
scene with no library: orbit radii from `SkillUniverseLayout.OrbitRadii`,
direction from each star's flat angle, a fixed height spread, a small
projection, floating, dust, slow spin, drag and arrow-key turning, Turn and
Pause buttons, click-to-front and a walk that turns each star forward.

**Decisions worth remembering:**
- No three-dimensional library: it would be a third-party script and break the
  Content Security Policy and the "no script libraries" rule, and the scene is
  only about twenty points.
- Pause freezes everything (spin and floating), not only the rotation.
- Touch screens keep native scrolling, so dragging to rotate is mouse and pen
  only; touch uses the Turn buttons and a tap on a star.
- The first version of the click handling could swallow the next click after a
  drag that produced no click event; fixed by clearing the flag after the
  release. The centre star was dimmed along with the others; it now never fades.

**Verified:** `dotnet test` 170 passing (new: orbit radii and directions,
rendered depth and angle attributes, About me content and navigation, controls
ship hidden). In a real browser: the system spins and floats, drag turns and
tilts it, click turns a star to the front, Pause stops it, phone width has no
page overflow, reduced motion moves nothing and hides Pause while Turn still
works instantly, and the console is empty. **Not verified:** a real touch phone,
Safari, frame rate on a slow machine, and the live site.

---

## 2026-11-14 (evening): the new design, built locally

**Asked:** "Is there a better, more modern, unique design?", then (after a
comparison page of four concepts and a motion prototype he liked) "code it ...
apply the new design now and let's see how it looks. Don't forget the
background we have", plus "consider my portfolio picture", "as always, observe
best practice", and "keep it local ... thoroughly test it before we deploy".

**What I did (ADR-0010):**
- Skills became a star map with .NET at the centre. Data in
  `SkillUniverseData`, positions from the pure function `SkillUniverseLayout`
  (radial tree, subtree slices by leaf count), drawn server-side so it works
  without script; `site.js` adds hover, focus and tap highlighting, a caption
  live region and a "Walk my journey" button. The grouped list stays in a
  disclosure. "Web API" added to the grouped skills so every star is backed.
- Header monogram is now "RS" (link to the top, accessible name "RS, home");
  the hero got a "Hi, I'm" greeting above his name (`Profile.Greeting`) and the
  title moved below the name; faint code tags in the hero margins at 75 rem and
  wider; letter-by-letter headings; a translucent page-change veil with an off
  switch in the footer. Portrait, background layers and the hero video are
  unchanged.

**Found and fixed on the way:** the first layout put long chains where the
rings are shortest and two labels overlapped (the overlap test caught it);
chains now point upward and the rings are wider. The "Walk" button wrapped
onto two lines on a phone (now no-wrap) and had page code showing through it
(now a solid background). The monogram was only 31 pixels wide on touch
screens (now a 44 pixel target).

**Verified:** `dotnet test` 167 passing (new tests: layout geometry and
overlaps, tree errors, journey, labels backed, captions, rendered markup,
culture-independent numbers, monogram, greeting, margin tags, footer switch,
phone scroll rules). In a real browser: 1440 and 390 pixel widths, light and
dark, no horizontal page scroll, layout shift 0 on the phone, and with
reduced motion no letters split, no veil, the switch hidden and no running
animations. **Not verified:** real touch devices, Safari, the live site, and
a full Lighthouse run.

**Next:** he reviews the look and the star-map captions; then deploy only on
his explicit yes. Optional later: the left navigation rail from the prototype.

---

## 2026-11-14 (after the push): the video shows only what is really his

**Asked:** he saw "Edge" in the new banner and said "I don't usually use Edge
... Only appropriate to me, my skills and experience." The video was made in the
job-search chat, not this one.

**What I did:** audited every box and label in `tools/hero-video/scene.html`
against his verified experience (profile and résumé facts). Removed the "Edge"
box (an invented delivery layer, and easy to read as the Microsoft browser) and
rejoined Browser to ASP.NET Core with the CSP shield on that link. Also changed
two labels that overstated: "Vue.js · React" to "Vue.js · JavaScript" (his
commercial React is about six months) and "Web API · .NET 10" to "Web API · MVC"
(.NET 10 is his personal projects only). Kept the rest: Umbraco CMS, MongoDB
(Centene), SQL Server and EF Core, Docker, Jenkins, and the Content Security
Policy work. Rebuilt with `npm run build` in `tools/hero-video`: 244 KB WebM,
392 KB MP4, 14 KB poster, all under the 1 MB limit. Documented the rule (only
what he has really worked with) in the tool README, ADR-0008 and CONTENT_SOURCES.

**Lighthouse on the live site, run before this change:** performance 100,
accessibility 100, best practices 100, SEO 92 (the known robots.txt audit
blocked by `connect-src 'none'`); LCP 1.3 s, CLS 0, 42 KiB transferred.

---

## 2026-11-14 (local test): the "Home" link and the hero video, in a real browser

**Asked:** "Let's test locally the recent changes." Ran the best-practice-check
skill (ADR-0009) against the uncommitted work, on a local server at port 5099.

**Passed:** `dotnet build -warnaserror` clean; 147 tests (148 after the fix
below); no vulnerable packages; no stale ADR-0002 references; the page sends the
Content Security Policy with `media-src 'self'`; the console had zero messages
across the whole session.
- **Desktop 1440:** the video starts by itself, plays (checked at 7.6 s), only
  the 240 KB WebM and the 15 KB poster are fetched, WebM is chosen, 1280 by 720,
  no error. The hero shows his stack as a request path with travelling packets;
  the text and code card stay readable.
- **Tablet 820:** video plays behind the content, no sideways scroll.
- **Phone 375:** video hidden and never fetched (no `/media/` requests), no
  sideways scroll, header 93 px, tap targets at least 40 px.
- **Reduced motion:** video paused and not fetched; only the still poster loads.
- **Theme toggle:** switches, stores the choice and survives a reload; the
  "Home" link is `#top`, keeps the query string, and scrolls to the top.

**Bug found and fixed:** under reduced motion the browser still listed two
running animations, both the pulsing dot on the "Live site" badge. The
reduced-motion rule used `*`, which does not match `::before`, and that dot is a
`::before`. The rule now also lists `*::before` and `*::after`; a new test guards
it; the browser then reported zero running animations.

**Tool notes:** the in-app browser pane was hidden, so its automatic video play
could not fire (it never reports the hero as visible). I confirmed playback
separately with a second real browser (Playwright) rather than guess. A tablet
screenshot looked scrolled; scroll position stayed at 0 for three seconds, so it
was a screenshot artifact.

**Not verified:** Safari playback of the MP4 fallback (no Safari here), and the
live site (nothing is pushed). Stopped only the server I started, by its PID.

---

## 2026-11-14 (later): "Home" in the header and a hero video of his own

**Asked:** "Is it not redundant if my name is on the site-header. Hero banner
background should be different from the page background. They look
redundant." I looked at the page first (the name appeared in the header, the
hero heading and the code card; the same scrolling code sat in the hero and
behind the page) and asked how to fix both. His answers: the header link should
say "Home", "what should be the standard name ... as what its functionality
suggests"; and "Hero should be a unique .gif or video that is unique and
suitable for my background and skills".

**What changed:** (1) The header's first link reads "Home". (2) New hero video,
made for this site: a request travelling through his stack (Browser, Edge,
ASP.NET Core, Umbraco CMS, MongoDB, SQL Server, Docker, Jenkins, with a Content
Security Policy shield and a scan line), drawn on a canvas and encoded to WebM
(246 KB) and MP4 (387 KB) plus a 15 KB poster. The boxes ring the edges of the
frame so the headline and the code card stay clear; a left-to-right shade
keeps text readable. The hero's own scrolling code lanes are gone; the faint
code and logo layer stays behind the rest of the page. (3) `site.js` step 4
plays the video only on wide screens when motion and data saving allow, and
pauses it off screen; otherwise a CSS background shows a still frame; phones
fetch neither. (4) `media-src 'self'` in the policy. (5) The source and a
rebuild script are in `tools/hero-video` (verified: a rebuild reproduces the
poster and MP4 byte for byte). Chosen over a GIF because a GIF of this size is
several megabytes.

**Verified:** build clean with warnings as errors; 147 tests pass (two new, two
updated); static export includes `media/`; real browser at 1440 and 375 wide:
video plays (currentTime advances, no console errors, no policy violations),
layout and text unchanged, phone shows the plain gradient with no media
requests; Lighthouse on a local build: performance 100, accessibility 100, best
practices 100 on mobile and desktop, layout shift 0, mobile fetches no media,
desktop fetches the poster and the WebM (about 262 KB).
**Not verified:** the live site (not pushed), and Safari playback of the MP4
(only Chrome was available; the MP4 is encoded as H.264 High, yuv420p, with
the index at the front, which is what Safari needs).

**Open:** the owner has not yet seen it; wording and look are his call. A pause
control for decorative motion is recorded in the tasks.

---

## 2026-11-14: Portrait converted to WebP

**Asked:** "Yes, convert the portrait to WebP" (the one Lighthouse image finding
left from 2026-11-13: a 51 KB, 640 px JPEG shown at 120 to 160 px).

**What changed:** `wwwroot/images/randolf.webp` (320 by 320, quality 85, 8,746
bytes, made with the `sharp` library from the existing JPEG; side-by-side check
showed no visible difference). The hero now renders `<picture>` with a WebP
`<source>` and the existing `<img>` as fallback (`Profile.PhotoWebpUrl`, new
optional field). **The JPEG stays** and is still the Open Graph image, because
LinkedIn and some other sharing crawlers do not reliably show WebP previews;
the 320 px size is twice the largest display size, so it stays sharp on dense
screens.

**Verified:** build clean with warnings as errors; 145 tests pass (one new:
`<picture>` markup, real RIFF/WEBP file, under half the JPEG's size); in a real
browser the WebP is the current source (320 by 320, shown at 120) and the layout
is unchanged; Lighthouse on a local build lists only `randolf.webp` as fetched
(8,960 bytes on the wire) and both "properly size images" and "modern image
formats" now pass, CLS 0.
**Not verified:** the live site after the push (see the run result below).

---

## 2026-11-13 (final): Lighthouse pass and phone-width check

**Asked:** "Yes, run the Lighthouse pass and phone-width check" (the two items
the best-practice review had left unverified).

**Lighthouse 12.8.2 on the live site, before fixes:** mobile 99 performance, 100
accessibility, 96 best practices, 92 SEO; desktop 99 / 100 / 100 / 92. Largest
Contentful Paint 1.5 s mobile and 0.3 s desktop; mobile Total Blocking Time 90 ms.
Four findings were real or explainable:

1. **Layout shift, desktop 0.064 (repeatable, same value in four runs).** The
   trace showed the header growing 55 to 59 px and the navigation moving 89 px
   left: the theme toggle ships `hidden` and `theme.js` reveals it after first
   paint. Fixed in `site.css`: with the `js` class present the hidden toggle
   keeps its space (`visibility: hidden`), without scripting it stays out of the
   layout. Measured after the fix on a local build: CLS 0 on mobile and desktop.
2. **Illegible text, mobile.** The decorative code lanes were 0.74rem (11.84 px),
   under the 12 px legibility line, and made up most of the page's text. Now
   0.75rem; the audit passes.
3. **"robots.txt is not valid", SEO 92.** The file is fine (200, `text/plain`).
   The audit fetches it from inside the page and the policy's
   `connect-src 'none'` blocks that fetch; an experiment with `connect-src 'self'`
   gave SEO 100. Kept `'none'` (ADR-0009) and left this as an owner decision:
   see the open item below.
4. **Not fixable here, GitHub Pages:** short cache lifetime (10 minutes) on
   static files, and no way to set headers. The portrait (51 KB JPEG, shown at
   120 px) could be a smaller WebP, about 30 KB saved; deferred, low value.

**Phone width (375 and 320 px, real browser):** no horizontal page scroll (the
only overflowing element is the swipeable navigation row, by design), no tap
target under 24 px, smallest visible text 12 px, header 93 px, the theme toggle
visible, hero, experience and contact read cleanly. One bug found and fixed:
the navigation kept the last section highlighted after scrolling back up to the
hero, because `site.js` ignored sections leaving the viewport band. It now clears
the leaving section's link; checked by scrolling through every section and back
to the top (the highlight follows, and is empty at the top).

**Verified:** build with warnings as errors clean; 144 tests pass (one new);
all measurements above taken in real Chrome (Lighthouse) and a real browser
(phone width) against a local server that I stopped afterwards.
**Not verified at the time of writing:** the live site after these fixes (see
the push entry below the commit for the live re-measurement).

**Decided (owner: "as long as we follow best practice", then "you can push"):**
keep `connect-src 'none'`. The alternative, `'self'`, would raise the SEO audit
to 100 and loses almost nothing (it only allows requests to the site's own
address), but `'none'` is the stricter policy ADR-0009 specifies, the page makes
no requests, and the 92 is a tool artifact (the file is valid). A real crawler is
unaffected. Revisit only if a Lighthouse score ever matters more than the
stricter policy.

---

## 2026-11-13 (last): Workflow actions moved off Node 20

**Asked:** "Yes, bump the actions and re-pin."

**What changed:** `deploy.yml` now pins `actions/checkout` v7.0.1, `actions/setup-dotnet`
v6.1.0, `actions/upload-pages-artifact` v5.0.0 and `actions/deploy-pages`
v5.0.1, each to its full commit hash (resolved with `gh api
repos/<owner>/<action>/git/ref/tags/<tag>`; all four are lightweight tags, so
the hash is the commit). The three that run JavaScript declare `node24`;
`upload-pages-artifact` is composite and pins its own `upload-artifact` v7.0.0
by hash. Since version 4 that action skips dotfiles by default, so
`include-hidden-files: true` keeps the `.nojekyll` marker the exporter writes
in the published artifact.

**Verified:** see the run result in the commit that follows this entry.

---

## 2026-11-13 (later): Résumé download replaced

**Asked:** "Yes, swap in the new résumé." The corrected résumé (no "Senior" in
the Professional Summary, personal AI setup wording) was made the same day.

**What changed:** `wwwroot/files/Randolf_Segubre_Resume.pdf` is now a byte for
byte copy of `Randolf_Segubre_Resume_2026-11-13.pdf` (SHA-256 starts D333190E,
357,247 bytes). The file name is unchanged, so the hero button, the contact
card and any shared link keep working; no code change. `CONTENT_SOURCES.md`
and `04_TASKS.md` updated.

**Verified:** hash of the copy equals the source; the file starts with `%PDF-`;
the first page text reads "Full-stack .NET developer with 9+ years"; the
checklist's build, test and vulnerability checks were rerun (see the commit).
The résumé's real job titles (Cloud Employee "Senior Full-Stack .NET
Developer", Accenture "Package App Senior Analyst") stay as they are true.

---

## 2026-11-13: Title wording, best-practice review and fixes (ADR-0009)

**Asked:** (1) remove "senior" from the portfolio title, because he is about
75 percent back end and 25 percent front end and is not yet a full senior; the
Cloud Employee and Accenture job titles stay, they are true. (2) "I preferred
remote but open for hybrid." (3) "Use 9 years instead of nine." (4) "Are we
following best practice in our code there?", then "fix everything that is not
right ... it should also be part of our workflow and skills."

**What changed:** the profile title is now "Full-Stack .NET Developer"; the
availability line reads "Open to .NET and full-stack roles. Remote preferred,
open to hybrid."; the page and link-preview descriptions no longer say
"senior"; "nine" became "9" in the headline, the experience intro and the
sources table. The review found and fixed (see ADR-0009): no Content Security
Policy (now a meta policy, and the inline theme script moved to
`theme-init.js`); an exporter that could recursively delete any absolute
folder (now `EnsureSafeToEmpty`); workflow permissions granted to every job and
actions pinned to movable tags (now per job, pinned to commit hashes); no
canonical link, sitemap or complete sharing tags (added, one `SiteInfo.Url`);
stale ADR-0002 citations in four comments (now ADR-0007); no `global.json` or
`.editorconfig` (added; analyzers raised to `latest-recommended` with code
style enforced in the build). Test packages were updated (AngleSharp 1.10.2,
coverlet 10.1.0, Test.Sdk 18.12.0, runner 4.0.1); `Mvc.Testing` stays on 10.x
because 11.x targets .NET 11.

**Standing gate:** `docs/build/06_BEST_PRACTICE_CHECKLIST.md` is the one
checklist, the operating manual makes it a non-negotiable step, and the
`best-practice-check` skill (`.claude/skills/`) runs it.

**Verified:** `dotnet build -warnaserror` clean; 143 tests pass (130 before,
13 new for the policy, the sharing tags and the exporter guard; two existing
tests were updated because their assumptions changed on purpose); no vulnerable
packages. In a real browser on a local server the page renders, the console is
empty, a `securitypolicyviolation` listener caught nothing, the theme toggle
works and survives a reload, scroll progress and navigation highlighting work.
**Not verified:** a Lighthouse pass, a phone-width re-check after the head
change, and the GitHub Actions run itself (nothing is pushed yet).

**Open:** nothing is committed or pushed; he reviews first. The résumé download
still serves the 2026-10-11 file; the corrected 2026-11-13 résumé exists and
can replace it. `style-src-attr 'unsafe-inline'` remains because per-element
animation values are inline style attributes (ADR-0009).

---

## 2026-11-02 (later): Calmer logos, no crown, résumé download

**Asked:** the logos "look redundant and too distracting; they should just be
scattered in the background"; remove the crown above the position; "don't
forget to consider responsive"; add a link to download a copy of his résumé.

**What changed:** see the fourth amendment in ADR-0008. The scrolling logo strip,
its extra hero padding and the crown are gone. Logos are faint, each shown once,
three in the hero's top padding and the rest behind the other sections. A
"Download resume (PDF)" button joined the hero actions and a Resume card joined
the contact section.

**Résumé:** published the unedited `Corrected Resume/Final/Randolf_Segubre_Resume.pdf`
(`resume_facts.md` and my notes name it as the current one). I read all of it
first. Two lines I flagged to him, not edited: the summary's "9+ years" next to
"full-stack" (his 2026-10-29 rule is about 5 years full-stack, 9 for C# and .NET
only) and the Collabera entry's React.js and Redux claim (the React work was the
2020 bench project, not Centene). The file also describes internal work
(a document-merge tool, internal write-ups) in more detail than the site's cards,
and uses stronger verbs ("Led", "Independently built") than the site.

**Responsive re-check (same 17 exact widths, 320 to 3440):** no horizontal
scroll, no overflow, no code card overflow, no crown or strip, no logo touching
the portrait or any text, none clipped, the résumé button 225 by 44 pixels at
every size, and no tap target under 40 pixels at phone widths. Checked by eye on
a phone and a laptop render. An earlier version of this change put hero logos at
percentage positions; the audit caught them on the portrait and on words at
several sizes, which is why they now sit in the fixed top padding.

**Tests:** 130 pass (new: each logo appears once, no crown or strip, logo opacity
cap, the résumé is a real PDF offered in the hero and the contact section).

---

## 2026-11-02: Responsive pass, including the animated background

**Asked:** make sure the portfolio is responsive, including the background, and
looks right at every size.

**How I checked:** the in-app browser resets its emulated size whenever the pane
changes, so I tested each size inside an iframe of exactly that width (plus 15
pixels for the scrollbar) and measured instead of eyeballing. A script checked,
at 17 sizes from 320 to 3440 pixels (phones, phones in landscape, tablets,
laptops, desktop, ultrawide): page-level horizontal scroll, any element past
the screen edge, code card overflow, header height and wrapping, grid column
counts, tap target sizes, and whether any hero logo overlapped the portrait,
the text, the code card, another logo, or the screen edge.

**What the audit found (before):**
- The scattered floating logos collided with content at nearly every size,
  including the owner's own desktop: at 1280 pixels the .NET logo sat on the
  portrait and MySQL and GitHub on the text; on tablets five logos sat on the
  code card; on phones two overlapped the card and one was cut off.
- Nav links were 24 pixels tall (the touch guideline is about 44), and the phone
  header took 128 pixels in three rows.
- On a 3440 pixel ultrawide the content stayed a 60 rem column and the
  background code looked tiny.
- A latent bug: nav links were `/#anchor`, which drops a query string and
  reloads the page for anyone arriving from a tracked email link.

**What changed:** see ADR-0008, third amendment. In short: a scrolling logo
rail in a reserved band replaced the scattered hero logos; logos are drawn once
in a hidden sprite; a swipeable two-row phone header with 44 pixel targets and
corrected anchor offsets; wider content and larger code on big screens; no card
blur on phones; pure `#anchor` navigation on the home page.

**Verified:** 126 tests pass, including new guards for the responsive rules.
Re-running the same audit after the changes: **17 of 17 sizes with no
horizontal scroll, no overflow, no collisions and no code card overflow**; 0 small
tap targets at every phone width. On a phone address carrying
`?utm_source=email`, all four nav jumps stayed on the page and kept the query
string, each section heading landed 60 pixels below the header, and the nav row
slid to keep the current link fully in view. Tablet, phone and ultrawide
renders checked by eye.

**Known and accepted:** at 667 pixels and wider the compact mouse-sized nav
links measure 27 pixels in a desktop browser; on a touch device the
`pointer: coarse` rule raises them to 44 pixels, which a desktop browser cannot
show. I could not test on physical phones.

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
