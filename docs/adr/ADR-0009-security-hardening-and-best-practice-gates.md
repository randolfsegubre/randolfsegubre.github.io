# ADR-0009: Security hardening and best-practice gates

- **Status:** Accepted.
- **Date:** 2026-11-13

## Context

A review on 2026-11-13 compared the site with general ASP.NET Core and web
practice and found gaps, none of them in what the site says, all in how it is
built and shipped:

- the page had no Content Security Policy (CSP), on a site whose owner
  advertises CSP hardening work;
- the static exporter emptied its output folder with a recursive delete after
  checking only that the path was absolute, so a wrong argument could destroy
  real files;
- the deploy workflow gave every job permission to write to Pages, and pinned
  its actions to movable version tags;
- the page head lacked a canonical link, a sitemap and complete sharing tags;
- code comments still cited ADR-0002, the superseded React decision.

The owner's instruction was to fix all of it and to make following best
practice a standing part of the workflow, not a one-off cleanup.

## Decision

1. **CSP in a meta tag.** GitHub Pages cannot send response headers, so the
   policy rides in a `<meta http-equiv="Content-Security-Policy">` as the first
   element after the viewport tag. Scripts and styles may only come from this
   site (`'self'`); images also allow `data:`; everything else, including
   connections, forms, plugins and manifests, is `'none'`. The single
   allowance is `style-src-attr 'unsafe-inline'`, because per-element
   animation values are written as `style=""` attributes; script injection is
   the dangerous case and that stays strict.
2. **No inline script.** The pre-paint theme script moved from an inline
   block in `_Layout.cshtml` to `wwwroot/js/theme-init.js`, loaded in the head
   without `defer` so a remembered theme still applies before first paint.
3. **Limits of a meta policy, stated honestly.** `frame-ancestors`,
   `report-uri` and `sandbox` are ignored in a meta tag, so the site cannot
   forbid being framed. If the site ever moves to a host that sends headers
   (a custom domain behind a CDN), move the policy to a response header and
   add `frame-ancestors 'none'`.
4. **Exporter safety.** `StaticExporter.EnsureSafeToEmpty` refuses a drive
   root, the web root or any folder above it, and any non-empty folder that
   does not carry the `.nojekyll` marker a previous export leaves behind.
5. **Workflow least privilege.** Top-level permissions are empty; the build job
   gets `contents: read`; only the deploy job gets `pages: write` and
   `id-token: write`. Every third-party action is pinned to a full commit hash
   with its tag in a comment, and the SDK version comes from `global.json`.
6. **Search and sharing.** One `SiteInfo.Url` constant feeds the canonical
   link, `og:url` and the preview image address; `sitemap.xml` and `robots.txt`
   ship in `wwwroot`; the not-found page is `noindex`.
7. **Build strictness.** `AnalysisLevel` is `latest-recommended`, code style is
   enforced in the build, and `.editorconfig` records the shared settings.
8. **Standing gate.** `docs/build/06_BEST_PRACTICE_CHECKLIST.md` is the one
   checklist; the operating manual makes it a non-negotiable step before any
   change is called done, and the `best-practice-check` skill runs it.

**Amendment, 2026-11-14:** the hero video (ADR-0008) needs one more directive,
`media-src 'self'`, because `default-src 'none'` would otherwise block it. It
allows audio and video from the site itself only; no remote media is
permitted, and `HeadSecurityAndSeoTests` pins the directive.

## Alternatives considered

- **Hash-based inline script allowance** instead of moving the script out:
  rejected. The hash must be recomputed on every edit and the file is simpler
  and cacheable as an external script.
- **Refactor every `style=""` attribute into classes** to drop
  `style-src-attr 'unsafe-inline'`: rejected for now. Each value is a distinct
  number (animation duration, offset), so it would mean generating CSS per
  element for a risk that is low next to script injection.
- **Leave CSP out because Pages cannot send headers:** rejected. A meta policy
  still blocks injected scripts and third-party loads, which is what matters
  for a static page with no user input.

## Consequences

- Any new script must be an external file from this site. Inline script,
  inline event handlers (`onclick=` and similar), third-party scripts, fonts or
  images, and `eval` will be blocked by the browser and flagged by
  `HeadSecurityAndSeoTests`. A genuinely needed new origin requires a new ADR.
- Updating a pinned action means resolving its tag to a new hash and updating
  both together; the workflow header says so.
- Exports to an existing non-empty folder that is not a previous export now
  fail by design; use a new or empty folder.
