// Asks the .NET backend to write the site's content to data/content.json (ADR-0011).
//
// `dotnet run` starts in the project folder, so the backend refuses a relative path; this script
// resolves the absolute path first, then runs the backend's export mode (no web server is started).
import { spawnSync } from 'node:child_process'
import { mkdirSync } from 'node:fs'
import { dirname, resolve } from 'node:path'
import { fileURLToPath } from 'node:url'

const here = dirname(fileURLToPath(import.meta.url))
const output = resolve(here, '..', 'data', 'content.json')
const project = resolve(here, '..', '..', 'src', 'Portfolio.Api')

mkdirSync(dirname(output), { recursive: true })

const result = spawnSync(
  'dotnet',
  ['run', '--project', project, '--no-launch-profile', '--', '--export-content', output],
  { stdio: 'inherit' },
)

if (result.status !== 0) {
  console.error('The .NET backend could not write the content file. Is the .NET SDK installed? (see global.json)')
  process.exit(result.status ?? 1)
}
