# ADR-0004: Plain CSS with design tokens, dark royal purple by default

- **Status:** Accepted. Amended 2026-09-25 (royal purple identity, dark default).
- **Date:** 2026-09-24

## Context

The page must work on a phone, be readable, and stay accessible. Fonts and
styles must not require third-party requests.

On 2026-09-25 the owner asked for a royal purple theme in the spirit of the
robes of King David and King Solomon, and then clarified that it must be a
**dark theme by default**. That replaced the first version, which followed
each visitor's system light or dark setting.

## Decision

- One hand-written stylesheet (`src/Portfolio.Web/wwwroot/css/site.css`)
  using Cascading Style Sheets custom properties as design tokens (colors,
  spacing, type scale).
- **Palette:** deep Tyrian purple surfaces with gold accents (crown mark,
  rules, list markers, status dots), soft lavender text, and a serif display
  face for headings from the system font stack (no web fonts).
- **Dark is the default for every visitor, whatever their system setting.**
  The dark palette is defined on `:root`. An optional light palette is defined
  under `:root[data-theme="light"]` and applies only when the visitor picks it
  with the toggle. There is deliberately no `prefers-color-scheme` media query.
- The header, hero and footer are deep purple in both themes, so the identity
  never changes; the light theme only changes the reading surfaces.
- The visitor's manual choice is remembered in `localStorage` inside a
  try/catch. An inline script in `Views/Shared/_Layout.cshtml` applies a
  remembered choice before first paint to avoid a flash, and a small
  `wwwroot/js/theme.js` wires the toggle button (rendered hidden and revealed
  by the script, so it never shows as dead without scripting).
- **The stylesheet is tested.** `DesignTokenTests` parses `site.css` and checks
  that every text and background pair meets the Web Content Accessibility
  Guidelines contrast minimum in both themes, that the light block only
  overrides tokens that exist in the dark base, and that no system-setting
  media query switches the palette.
- System font stack only. No icon libraries, no external requests. The crown
  is an inline vector image.

## Consequences

- Zero third-party requests: faster, more private, nothing to break.
- Theme colors are changed in one block of tokens, and a change that breaks
  contrast fails the build.
- A visitor who prefers light must use the toggle once; the choice is then
  remembered on that device.
- No utility framework, so layout rules are written by hand. Accepted: the
  page is small.

## Alternatives rejected

- **Following the system setting (the first version):** contradicts the
  owner's decision that dark royal purple is the site's identity.
- **Tailwind or a component library:** more dependencies and generated
  class noise for a single small page.
- **Google Fonts:** an external request and a privacy footprint for a
  cosmetic gain.
