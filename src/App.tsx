import { ContactSection } from "./components/ContactSection";
import { ExperienceSection } from "./components/ExperienceSection";
import { Footer } from "./components/Footer";
import { Header } from "./components/Header";
import { Hero } from "./components/Hero";
import { ProjectsSection } from "./components/ProjectsSection";
import { SkillsSection } from "./components/SkillsSection";

/**
 * The whole page, top to bottom.
 *
 * This is the composition root (see `docs/build/03_ARCHITECTURE_AND_PATTERNS_GUIDE.md`):
 * it only decides the order of sections. Each section reads its own content
 * from `src/content`, so this file never holds a claim or a piece of copy.
 * The skip link is first in the tab order so keyboard visitors can jump past
 * the navigation straight to `#main`.
 */
export default function App() {
  return (
    <>
      <a className="skip-link" href="#main">
        Skip to main content
      </a>
      <Header />
      <main id="main">
        <Hero />
        <ProjectsSection />
        <ExperienceSection />
        <SkillsSection />
        <ContactSection />
      </main>
      <Footer />
    </>
  );
}
