import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App";
import "./styles.css";

/**
 * Browser entry point: mounts the React tree into `#root` in `index.html`.
 *
 * Throwing when the root is missing is deliberate: a silent blank page would
 * be much harder to diagnose than a clear error in the console.
 */
const rootElement = document.getElementById("root");
if (!rootElement) {
  throw new Error("Missing #root element in index.html");
}

createRoot(rootElement).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
