import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import App from "./App";
import { experience } from "./content/experience";
import { profile } from "./content/profile";
import { projects } from "./content/projects";

/**
 * Render tests: the page shows what the data says, and its accessibility and
 * safety details hold. Behavior over markup: these query by role and name the
 * way a screen reader user would.
 */
describe("App", () => {
  beforeEach(() => {
    document.documentElement.removeAttribute("data-theme");
    window.localStorage.clear();
  });

  it("has one h1 with the owner's name and the main landmarks", () => {
    render(<App />);
    expect(screen.getByRole("heading", { level: 1, name: profile.name })).toBeInTheDocument();
    // Card and role <header> elements can also match "banner" in jsdom, so check the first is the site header.
    expect(screen.getAllByRole("banner")[0]).toHaveClass("site-header");
    expect(screen.getByRole("main")).toBeInTheDocument();
    expect(screen.getByRole("contentinfo")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Skip to main content" })).toHaveAttribute("href", "#main");
  });

  it("renders a card for every project and a role for every job", () => {
    render(<App />);
    for (const project of projects) {
      expect(screen.getByRole("heading", { level: 3, name: project.name })).toBeInTheDocument();
    }
    for (const role of experience) {
      expect(screen.getByRole("heading", { level: 3, name: role.title, hidden: false })).toBeInTheDocument();
    }
  });

  it("shows a known gap only where the data names one", () => {
    render(<App />);
    const withGap = projects.filter((p) => p.honestNote).length;
    expect(screen.getAllByText("Known gap:")).toHaveLength(withGap);
  });

  it("opens every outbound link safely in a new tab", () => {
    render(<App />);
    const outbound = screen.getAllByRole("link").filter((a) => a.getAttribute("href")?.startsWith("https://"));
    expect(outbound.length).toBeGreaterThan(0);
    for (const link of outbound) {
      expect(link).toHaveAttribute("target", "_blank");
      expect(link.getAttribute("rel")).toContain("noopener");
    }
  });

  it("hides the resume button until a resume file is configured", () => {
    render(<App />);
    expect(screen.queryByRole("link", { name: /download resume/i })).not.toBeInTheDocument();
  });

  it("does not link the private project", () => {
    render(<App />);
    const card = screen.getByRole("article", { name: "Galaxy Survivor" });
    expect(within(card).queryAllByRole("link")).toHaveLength(0);
  });

  it("toggles the theme, sets the attribute and remembers the choice", async () => {
    const user = userEvent.setup();
    render(<App />);
    await user.click(screen.getByRole("button", { name: /switch to dark theme/i }));
    expect(document.documentElement).toHaveAttribute("data-theme", "dark");
    expect(window.localStorage.getItem("theme")).toBe("dark");
    await user.click(screen.getByRole("button", { name: /switch to light theme/i }));
    expect(document.documentElement).toHaveAttribute("data-theme", "light");
  });
});
