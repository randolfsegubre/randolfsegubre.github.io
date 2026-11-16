// Removes files Nuxt generates that the published site never loads (ADR-0011).
//
// With no Nuxt JavaScript in the browser (features.noScripts), the only thing left in .output/public/_nuxt is the
// stylesheets of Nuxt's own error screens (error-404 and error-500), which the site's pages never reference: the
// not-found page is an ordinary page that uses css/site.css. Shipping them would only add unused files to the
// published site.
//
// This is a guarded delete, not a blanket one: if any generated page does reference /_nuxt/, a later Nuxt change has
// started to use that folder, so the script stops with an error instead of deleting files the page needs.
import { existsSync, readdirSync, readFileSync, rmSync } from 'node:fs'
import { dirname, join, resolve } from 'node:path'
import { fileURLToPath } from 'node:url'

const here = dirname(fileURLToPath(import.meta.url))
const publicDir = resolve(here, '..', '.output', 'public')
const nuxtDir = join(publicDir, '_nuxt')

if (!existsSync(publicDir)) {
  console.error(`Nothing to prune: ${publicDir} does not exist. Run "nuxt generate" first.`)
  process.exit(1)
}

// STEP 1 of 2: find every generated page that mentions the folder.
const pages = readdirSync(publicDir).filter((name) => name.endsWith('.html'))
const referencing = pages.filter((name) => readFileSync(join(publicDir, name), 'utf8').includes('/_nuxt/'))
if (referencing.length > 0) {
  console.error(`Not pruning: ${referencing.join(', ')} reference /_nuxt/, so Nuxt now ships files the page needs.`)
  console.error('Review nuxt.config.js and ADR-0011 before publishing.')
  process.exit(1)
}

// STEP 2 of 2: nothing refers to it, so the folder is dead weight.
if (existsSync(nuxtDir)) {
  rmSync(nuxtDir, { recursive: true, force: true })
  console.log('Pruned unused _nuxt folder (Nuxt error-screen styles) from the output.')
}
