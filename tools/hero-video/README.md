# Hero video: how it is made

The looping background of the hero banner is a video I made for this site. It
is not stock footage and it is not a screen recording: `scene.html` draws it,
frame by frame, on an HTML canvas. This folder is the source, so the video can
be changed or rebuilt later instead of being an unexplained binary.

## What it shows

The path of a request through my own stack. Boxes for the Browser (Vue.js and
JavaScript), ASP.NET Core (Web API and MVC), Umbraco CMS, MongoDB, SQL Server
(T-SQL and EF Core), Docker and Jenkins; gold packets travel along the links and
each box glows when one arrives; a small Content Security Policy (CSP) shield
sits on the Browser to ASP.NET Core link and glows when a slow vertical scan
line passes it. Every technology shown is one named on the site (ADR-0006),
**and only things I have really worked with in my career**: the first version
also had an "Edge" box (a delivery layer I made up for the diagram, and easy to
misread as the Microsoft browser), a "React" label (my commercial React is about
six months) and ".NET 10" (a personal-project version only). They were removed on
2026-11-14. Before adding a box or label, check it against `docs/CONTENT_SOURCES.md`. The boxes sit around the edges of the frame, because on the page the
headline and paragraph cover the left-middle and the code card covers the
right-middle.

## Rebuild

From this folder, with Google Chrome installed:

```bash
npm install
npm run build
```

Set `CHROME_PATH` if Chrome is not in the usual Windows location. The build
renders 288 frames (12 seconds at 24 per second), encodes
`web/public/media/hero.webm` (VP9, about 250 KB) and `hero.mp4`
(H.264, about 390 KB), and saves `hero-poster.webp` (about 15 KB) from the
frame at 6 seconds. Nothing here runs in the website build or in the deploy
workflow; the three output files are committed. A rebuild gives an identical
poster and MP4; the WebM has the same size and picture but its bytes can differ,
because the VP9 encoder runs on several threads.

## Rules that keep it correct

- **It must loop with no jump.** Every motion is a function of the time `t`
  that repeats a whole number of times per loop (`LOOP` in `scene.html`, 12
  seconds). If you change `LOOP`, change `LOOP_SECONDS` in `build.js` too, and
  keep every speed an integer number of cycles per loop.
- **Keep it quiet.** The page puts a left-to-right shade over it so the
  headline stays readable (contrast is checked in `docs/build/06_BEST_PRACTICE_CHECKLIST.md`).
  Do not add bright, large or fast elements.
- **Keep it small.** A test (`Hero_video_is_decorative_muted_not_autoplaying_and_its_files_exist_and_stay_small`)
  fails if a file passes 1 MB.
- **Boxes stay away from the text.** Left and right edges are cropped by the
  page on wide screens (the video is scaled to cover the hero), so keep box
  centres between about x = 190 and x = 1090 in the 1280 by 720 frame.
- **No new origin.** The page allows media only from the site itself
  (`media-src 'self'`, ADR-0009).

## How the page uses it

`web/app/components/CodeBackdrop.vue` holds the `<video>`: muted, looping, `playsinline`,
`preload="none"`, no `autoplay` attribute and no controls, hidden from
assistive technology with the rest of the backdrop. `site.js` (step 4 of 4)
plays it only when the visitor has not asked for reduced motion or data saving
and the screen is wider than 42rem, and pauses it while the hero is off screen.
Otherwise, and until it plays, the stylesheet shows the poster frame as the
background. On phones the video and the poster are not loaded at all.
