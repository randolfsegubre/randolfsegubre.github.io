// Removes the inline "speculationrules" script that Nuxt adds when its browser JavaScript is switched off
// (features.noScripts in nuxt.config.js). The hint is declarative, but an inline script is exactly what the
// page's Content Security Policy forbids (script-src 'self', ADR-0009), and it would hint at prefetching the
// not-found page. The page needs neither, so the tag is dropped from the generated HTML.
export default defineNitroPlugin((nitroApp) => {
  nitroApp.hooks.hook('render:html', (html) => {
    html.head = html.head.map((chunk) => chunk.replace(/<script type="speculationrules">[\s\S]*?<\/script>/g, ''))
  })
})
