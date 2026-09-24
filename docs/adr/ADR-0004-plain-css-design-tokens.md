# ADR-0004: Plain CSS with design tokens, light and dark themes

- **Status:** Accepted
- **Date:** 2026-09-24

## Context

The design goal is clear, restrained and readable, not flashy. The page must
work on a phone, respect the visitor's light or dark preference, and stay
accessible. Fonts and styles must not require third-party requests.

## Decision

- One hand-written stylesheet (`src/styles.css`) using Cascading Style Sheets
  custom properties as design tokens (colors, spacing, type scale).
- The light palette is defined on `:root`. Dark values are redefined under
  `prefers-color-scheme: dark` (unless the visitor chose light) and again
  under `:root[data-theme="dark"]`, so the manual toggle wins in both
  directions.
- The visitor's manual choice is remembered in `localStorage` inside a
  try/catch, and an inline script in `index.html` applies it before first
  paint to avoid a flash of the wrong theme.
- System font stack only. No web fonts, no icon libraries, no external
  requests.

## Consequences

- Zero third-party requests: faster, more private, nothing to break.
- Theme colors are changed in one block of tokens.
- No utility framework, so layout rules are written by hand. Accepted: the
  page is small.

## Alternatives rejected

- **Tailwind or a component library:** more dependencies and generated
  class noise for a single small page.
- **Google Fonts:** an external request and a privacy footprint for a
  cosmetic gain.
