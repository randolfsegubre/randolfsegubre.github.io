// The site's content, written by the .NET backend as data/content.json before the build (ADR-0011).
//
// Every claim on the page comes from this one document, and its sources are listed in
// docs/CONTENT_SOURCES.md. Components only draw it: none of them holds a claim or a piece of copy
// (ADR-0003). If the file is missing, run `npm run content` (or `npm run generate`, which does it first).
import data from '../../data/content.json'

export const content = data
