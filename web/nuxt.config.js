// Nuxt configuration for the portfolio (ADR-0011).
//
// The site is generated to plain static files for GitHub Pages (ADR-0001). Nuxt is used as a build-time
// template engine: it renders the Vue components to HTML, and the page's behaviour stays in the two small
// scripts in public/js that the page has always used. Nuxt's own browser bundle is switched off
// (features.noScripts), so no framework JavaScript is shipped, the Content Security Policy (script-src 'self')
// is unchanged, and the output page behaves exactly as before.
import { createHash } from 'node:crypto'
import { readFileSync } from 'node:fs'

// Cache-busting query for the files in public/: the hash changes when the file does (what ASP.NET's
// asp-append-version did), so a visitor never keeps an old stylesheet or script.
const version = (file) =>
  createHash('sha256').update(readFileSync(new URL(`./public/${file}`, import.meta.url))).digest('hex').slice(0, 12)

// Content Security Policy (ADR-0009). GitHub Pages cannot send headers, so the policy rides in a meta tag.
// Scripts and styles may only come from this site; the one allowance is style="" attributes, which carry
// per-element animation values. frame-ancestors and similar header-only directives cannot be set here.
const csp =
  "default-src 'none'; script-src 'self'; style-src 'self'; style-src-attr 'unsafe-inline'; img-src 'self' data:; " +
  "media-src 'self'; font-src 'self'; connect-src 'none'; manifest-src 'none'; object-src 'none'; base-uri 'self'; form-action 'none'"

export default defineNuxtConfig({
  compatibilityDate: '2026-11-01',
  devtools: { enabled: false },
  telemetry: false,
  ssr: true,

  // Static output only: no Nuxt JavaScript in the browser (see the note at the top).
  features: { noScripts: true, inlineStyles: false },
  experimental: {
    // No _payload.json files: with no Nuxt JavaScript in the browser there is nothing to read them.
    payloadExtraction: false,
    // The Speculation Rules script Nuxt adds for cross-origin prefetch is inline, which the Content Security Policy below blocks.
    crossOriginPrefetch: false,
    // No _nuxt/builds manifest: it only exists so Nuxt's browser code can notice a newer deploy, and none is shipped.
    appManifest: false,
  },

  hooks: {
    // No preload or prefetch links: there is no Nuxt JavaScript to load, and its own error-page styles are never used.
    'build:manifest': (manifest) => {
      for (const entry of Object.values(manifest)) {
        entry.prefetch = false
        entry.preload = false
      }
    },
  },

  // Pages written as plain files: /index.html and /404.html (what GitHub Pages serves for unknown addresses).
  // Nuxt also prerenders an empty single-page-app shell to /200.html and /404.html. GitHub Pages has no use for
  // 200.html, and the 404.html shell raced with the real not-found page for the same file (whichever finished last
  // won, so a build could publish a blank 404 page), so both are ignored here and only the two routes above are written.
  nitro: {
    prerender: { routes: ['/', '/404'], ignore: ['/200.html', '/404.html'], crawlLinks: false, autoSubfolderIndex: false },
  },

  app: {
    head: {
      htmlAttrs: { lang: 'en' },
      meta: [
        // A priority before the defaults puts the policy first in <head>: a meta policy only governs what comes after it, so it must
        // come before every script and stylesheet (a test in HeadSecurityAndSeoTests enforces the order).
        { 'http-equiv': 'Content-Security-Policy', content: csp, tagPriority: -90 },
        { name: 'referrer', content: 'strict-origin-when-cross-origin' },
        { name: 'theme-color', content: '#1a0b3b' },
      ],
      link: [
        { rel: 'icon', type: 'image/svg+xml', href: '/favicon.svg' },
        { rel: 'stylesheet', href: `/css/site.css?v=${version('css/site.css')}` },
      ],
      script: [
        // Applies the saved theme before the first paint, so the page never flashes the wrong one.
        { src: `/js/theme-init.js?v=${version('js/theme-init.js')}`, tagPosition: 'head' },
        { src: `/js/theme.js?v=${version('js/theme.js')}`, defer: true, tagPosition: 'bodyClose' },
        { src: `/js/site.js?v=${version('js/site.js')}`, defer: true, tagPosition: 'bodyClose' },
      ],
    },
  },
})
