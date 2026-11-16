// Serves the generated site (web/.output/public) for local testing: `npm run serve`.
//
// It is a stand-in for GitHub Pages, so it behaves the same way: plain files, "/" is index.html, and an unknown
// address gets 404.html with a 404 status. It is deliberately small and strict:
//   - it listens on 127.0.0.1 only, so the site is not exposed to the local network;
//   - it answers GET and HEAD only;
//   - it refuses any path that would leave the site folder (path traversal), and dotfiles;
//   - it sends X-Content-Type-Options: nosniff, and no Server header.
// It adds no Content Security Policy of its own: the real policy is the meta tag in the page, which is what
// GitHub Pages will serve, so a violation here is a violation there.
import { createServer } from 'node:http'
import { readFile, stat } from 'node:fs/promises'
import { dirname, extname, join, resolve, sep } from 'node:path'
import { fileURLToPath } from 'node:url'

const root = resolve(dirname(fileURLToPath(import.meta.url)), '..', '.output', 'public')
const port = Number(process.env.PORT ?? 5099)

const types = {
  '.html': 'text/html; charset=utf-8',
  '.css': 'text/css; charset=utf-8',
  '.js': 'text/javascript; charset=utf-8',
  '.json': 'application/json',
  '.svg': 'image/svg+xml',
  '.jpg': 'image/jpeg',
  '.webp': 'image/webp',
  '.pdf': 'application/pdf',
  '.mp4': 'video/mp4',
  '.webm': 'video/webm',
  '.txt': 'text/plain; charset=utf-8',
  '.xml': 'application/xml',
}

async function send(response, status, file, method) {
  const body = await readFile(file)
  response.writeHead(status, {
    'Content-Type': types[extname(file).toLowerCase()] ?? 'application/octet-stream',
    'Content-Length': body.length,
    'X-Content-Type-Options': 'nosniff',
  })
  response.end(method === 'HEAD' ? undefined : body)
}

createServer(async (request, response) => {
  try {
    if (request.method !== 'GET' && request.method !== 'HEAD') {
      response.writeHead(405, { Allow: 'GET, HEAD' }).end()
      return
    }

    // STEP 1 of 3: turn the address into a path inside the site folder, and refuse anything that leaves it.
    const pathname = decodeURIComponent(new URL(request.url, 'http://localhost').pathname)
    const target = resolve(join(root, pathname === '/' ? 'index.html' : pathname))
    const isInside = target === root || target.startsWith(root + sep)
    const isDotfile = pathname.split('/').some((part) => part.startsWith('.'))
    if (!isInside || isDotfile) {
      response.writeHead(404).end()
      return
    }

    // STEP 2 of 3: serve the file if there is one.
    const info = await stat(target).catch(() => null)
    if (info?.isFile()) {
      await send(response, 200, target, request.method)
      return
    }

    // STEP 3 of 3: otherwise the not-found page, with a 404 status, as GitHub Pages does.
    await send(response, 404, join(root, '404.html'), request.method)
  } catch {
    response.writeHead(500).end()
  }
}).listen(port, '127.0.0.1', () => {
  console.log(`Serving ${root} at http://localhost:${port}/ (Ctrl+C to stop)`)
})
