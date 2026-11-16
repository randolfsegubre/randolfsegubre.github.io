# Third-party notices

This site makes no third-party requests when a visitor opens it. The only
third-party material is embedded in the source, listed here.

## Technology logos

- **What:** eleven technology logos (.NET, Umbraco, Vue.js, React, Blazor,
  Docker, Jenkins, MongoDB, MySQL, GitHub, Next.js), embedded as inline vector
  markup in `src/Portfolio.Api/Content/TechLogoData.cs`.
- **Source:** the Simple Icons package, version 16.32.0
  (<https://simpleicons.org>), released under the CC0 1.0 Universal
  public-domain dedication.
- **Trademarks:** the logos are trademarks of their respective owners. They are
  shown only to indicate which technologies the site owner has used, and do not
  imply endorsement.
- **The C# mark is not from that set.** Microsoft's C# logo is not part of the
  public-domain icon set, so the C# mark in `TechLogoData.cs` is a simple
  purple hexagon with the letters "C#", drawn for this site. It is a hand-drawn
  approximation, not the official artwork, and can be replaced with the
  official logo by editing that one entry.
- **How the file was made:** generated from the package by a small script that
  copies each icon's shape and brand color. To add a logo, add its technology
  name to `Profile.BackdropTags` (it must also appear in a project or role stack)
  and add an entry to `TechLogoData`.

## Skills map icons

- **Logos:** JavaScript, HTML5, Sass, OWASP, JSON Web Tokens, GitHub Copilot, Claude Code,
  Model Context Protocol, Redux, Unity and Ollama come from the same Simple Icons
  package, version 16.32.0, released under the CC0 1.0 public-domain dedication, embedded in
  `src/Portfolio.Api/Content/SkillIconData.cs`. The C#, .NET, Umbraco, Vue.js, React, Blazor,
  Docker, Jenkins, MongoDB, MySQL, GitHub and Next.js spheres reuse the logos listed above.
  Trademarks belong to their owners; the logos only show which technologies the owner has
  used. (An earlier version also carried TypeScript, Figma, SQLite, React Query, Vite,
  GraphQL and Markdown; those spheres were removed on 2026-11-15, see ADR-0010.)
- **AngularJS is an approximation.** The Simple Icons set has no AngularJS mark (its
  Angular mark is the newer framework's, now black), so the AngularJS sphere draws
  the Angular shield shape from that set in AngularJS red (#E23237). It is not the
  official artwork and can be replaced by editing the `angularjs` entry.
- **Generic glyphs:** the database cylinder, table, bolt, check mark, braces, layers,
  shield, prompt, globe, history, window, layout, flow, code, cloud and lock in
  `SkillIconData.cs` are simple line drawings made for this site. They are not any
  company's artwork. They stand in where the public-domain icon set has no logo for a
  technology (SQL Server, Entity Framework Core, Dapper, xUnit, Azure and AWS among
  them), because a generic symbol is more honest than a lookalike of a trademarked mark.
- **The crown on the RS monogram** in the header (`SiteHeader.vue`) is a simple shape drawn
  for this site, with a cross cut out of it. It is not copied from any logo.

## Build tools (not shipped)

- **Nuxt, Vue.js and their dependencies** are used at build time only to generate the static pages
  (ADR-0011). They are open-source packages under the MIT licence and similar permissive licences, installed from
  `web/package-lock.json`. No Nuxt or Vue.js code is sent to visitors: the published page loads only the site's own
  three scripts.

## Profile link icons

- **GitHub** uses the same public-domain Simple Icons shape as the technology
  logos above.
- **LinkedIn** is not in that icon set, so its mark in
  `src/Portfolio.Api/Content/SocialIconData.cs` is a rounded square with the
  letters "in" cut out, drawn for this site. It is a hand-drawn approximation,
  not the official artwork, and can be replaced by editing that one entry.
- Both icons are drawn in the surrounding text color and are decorative; the
  link text names the destination. Trademarks belong to their owners.

## Portrait

The portrait `web/public/images/randolf.jpg` is the site
owner's own photograph, supplied by him. It is a square crop (head and
shoulders) of the photo he supplied on 2026-09-26, resized to 640 by 640
pixels; the image content is otherwise unedited. `randolf.webp` beside it is
the same picture resized to 320 by 320 pixels and saved as WebP (quality 85,
about 9 KB instead of 51 KB) for the page itself; the JPEG stays as the
fallback and as the link-preview image.

## Hero video

`web/public/media/` holds the hero background video (`hero.webm`,
`hero.mp4`) and its still frame (`hero-poster.webp`). They are the site owner's
own work, drawn by `tools/hero-video/scene.html` with no stock footage and no
third-party artwork. The files are built with two development-only tools that
never ship with the site: puppeteer-core (Apache License 2.0), which drives
the owner's installed Chrome to draw the frames, and ffmpeg-static (a build of
FFmpeg, which is licensed under the GNU General Public License in that build),
which encodes them. The encoded files are the owner's output and carry no
licence obligation from the encoder.
