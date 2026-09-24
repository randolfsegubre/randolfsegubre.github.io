import { profile } from "../content/profile";
import { ExternalLink } from "./ExternalLink";

/**
 * Page footer: copyright and a pointer to this site's own source, which is
 * itself a small, public example of the owner's work.
 */
export function Footer() {
  return (
    <footer className="site-footer">
      <div className="container">
        <p>
          © {new Date().getFullYear()} {profile.name}. Built with React, Vite and TypeScript.{" "}
          <ExternalLink href="https://github.com/randolfsegubre/randolfsegubre.github.io">Source of this site</ExternalLink>
        </p>
      </div>
    </footer>
  );
}
