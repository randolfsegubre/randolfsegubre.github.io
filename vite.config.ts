import react from "@vitejs/plugin-react";
import { defineConfig } from "vitest/config";

/**
 * Build and test configuration.
 *
 * `base: "/"` is correct because this is a GitHub Pages *user* site
 * (repository `randolfsegubre.github.io`), served from the domain root. A
 * project site would need `/<repo>/` here (ADR-0001).
 */
export default defineConfig({
  base: "/",
  plugins: [react()],
  test: {
    environment: "jsdom",
    globals: true,
    setupFiles: ["./src/test/setup.ts"],
  },
});
