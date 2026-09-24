import { profile } from "../content/profile";
import { ExternalLink } from "./ExternalLink";

/**
 * The first screen: name, title, the pitch, availability and the main
 * calls to action.
 *
 * It holds the page's only `<h1>`. The resume button renders only when
 * `profile.resumeUrl` is set, so the site never shows a dead download link
 * (the resume is added once the owner confirms the final file).
 */
export function Hero() {
  return (
    <section id="top" className="hero" aria-labelledby="hero-heading">
      <div className="container">
        <p className="eyebrow">{profile.title}</p>
        <h1 id="hero-heading" className="hero-title">
          {profile.name}
        </h1>
        <p className="hero-lead">{profile.headline}</p>
        <p className="hero-meta">
          <span>{profile.location}</span>
          <span aria-hidden="true"> · </span>
          <span>{profile.availability}</span>
        </p>
        <div className="hero-actions">
          <a className="button button-primary" href="#projects">
            See selected projects
          </a>
          <a className="button" href={`mailto:${profile.email}`}>
            Email me
          </a>
          {profile.resumeUrl ? (
            <a className="button" href={profile.resumeUrl} download>
              Download resume
            </a>
          ) : null}
          {profile.links.map((link) => (
            <ExternalLink key={link.href} href={link.href} className="button">
              {link.label}
            </ExternalLink>
          ))}
        </div>
      </div>
    </section>
  );
}
