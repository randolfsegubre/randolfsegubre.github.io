import { profile } from "../content/profile";
import { ThemeToggle } from "./ThemeToggle";

/** Section anchors shown in the navigation, in page order. */
const NAV_ITEMS = [
  { href: "#projects", label: "Projects" },
  { href: "#experience", label: "Experience" },
  { href: "#skills", label: "Skills" },
  { href: "#contact", label: "Contact" },
];

/**
 * Sticky site header: the owner's name, anchor navigation and the theme toggle.
 *
 * Navigation uses plain in-page anchors (no router, ADR-0002), so it works
 * without any script and the browser handles focus and scrolling.
 */
export function Header() {
  return (
    <header className="site-header">
      <div className="container header-inner">
        <a className="brand" href="#top">
          {profile.name}
        </a>
        <nav aria-label="Primary">
          <ul className="nav-list">
            {NAV_ITEMS.map((item) => (
              <li key={item.href}>
                <a href={item.href}>{item.label}</a>
              </li>
            ))}
          </ul>
        </nav>
        <ThemeToggle />
      </div>
    </header>
  );
}
