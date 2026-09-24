import type { ReactNode } from "react";

interface ExternalLinkProps {
  href: string;
  children: ReactNode;
  className?: string;
}

/**
 * A link that leaves the site.
 *
 * Every outbound link goes through this component so the safety and
 * accessibility details are written once: `rel="noopener noreferrer"` stops
 * the new tab controlling this one, and the visually hidden text tells screen
 * reader users the link opens a new tab.
 */
export function ExternalLink({ href, children, className }: ExternalLinkProps) {
  return (
    <a href={href} target="_blank" rel="noopener noreferrer" className={className}>
      {children}
      <span className="visually-hidden"> (opens in a new tab)</span>
    </a>
  );
}
