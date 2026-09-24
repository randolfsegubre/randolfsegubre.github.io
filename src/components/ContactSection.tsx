import { profile } from "../content/profile";
import { ExternalLink } from "./ExternalLink";
import { Section } from "./Section";

/**
 * How to reach the owner: email first, then profiles.
 *
 * Email is a plain `mailto:` link, not a form, because a form would need a
 * third-party service (ADR-0001).
 */
export function ContactSection() {
  return (
    <Section id="contact" title="Contact" intro={profile.availability}>
      <ul className="contact-list">
        <li>
          <span className="contact-label">Email</span> <a href={`mailto:${profile.email}`}>{profile.email}</a>
        </li>
        {profile.links.map((link) => (
          <li key={link.href}>
            <span className="contact-label">{link.label}</span> <ExternalLink href={link.href}>{link.href.replace("https://", "")}</ExternalLink>
          </li>
        ))}
        <li>
          <span className="contact-label">Location</span> {profile.location}
        </li>
      </ul>
    </Section>
  );
}
