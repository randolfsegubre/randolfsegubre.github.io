// Asks the .NET backend to refresh the content database and write the site's content to data/content.json
// (ADR-0011, ADR-0012).
//
// In one run the backend (1) creates or updates the SQL Server database, (2) makes it say what the JSON files in
// /content say, and (3) reads the content back out of the database into data/content.json for Nuxt. Reading it back
// through the database, rather than straight from the files, means every build exercises the real storage path.
//
// `dotnet run` starts in the project folder, so the backend refuses relative paths; this script resolves absolute
// ones first. No web server is started.
//
// The connection string is never kept in the repository. On a Windows development machine the default below (the
// local SQL Server, Windows authentication, no password) is used; anywhere else, such as the deploy workflow,
// ConnectionStrings__Portfolio must be set in the environment.
import { spawnSync } from 'node:child_process'
import { mkdirSync } from 'node:fs'
import { dirname, resolve } from 'node:path'
import { fileURLToPath } from 'node:url'

const here = dirname(fileURLToPath(import.meta.url))
const output = resolve(here, '..', 'data', 'content.json')
const contentFolder = resolve(here, '..', '..', 'content')
const project = resolve(here, '..', '..', 'src', 'Portfolio.Api')

const localWindowsDefault = 'Server=localhost;Database=PortfolioSite;Integrated Security=true;TrustServerCertificate=true'
const connection = process.env.ConnectionStrings__Portfolio ?? (process.platform === 'win32' ? localWindowsDefault : undefined)

if (!connection) {
  console.error('Set ConnectionStrings__Portfolio to the SQL Server connection string (the content database).')
  process.exit(1)
}

mkdirSync(dirname(output), { recursive: true })

const result = spawnSync(
  'dotnet',
  ['run', '--project', project, '--no-launch-profile', '--', '--migrate', '--seed', contentFolder, '--export-content', output],
  { stdio: 'inherit', env: { ...process.env, ConnectionStrings__Portfolio: connection } },
)

if (result.status !== 0) {
  console.error('The .NET backend could not refresh the content database and write the content file.')
  console.error('Is SQL Server running and the .NET SDK installed? (see global.json and docs/DEVELOPER_HANDBOOK.md)')
  process.exit(result.status ?? 1)
}
