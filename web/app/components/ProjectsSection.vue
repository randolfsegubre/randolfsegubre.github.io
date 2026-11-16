<!--
  Selected projects: featured ones first as large cards, the rest in a sideways carousel. The split comes from each
  project's Featured flag in the backend's data, so promoting or demoting a project is a one-word data change
  (ADR-0003).

  The carousel is a plain scroll-snap strip, so it works with no script at all: it scrolls by touch, wheel,
  scrollbar and (it is focusable) the arrow keys. site.js (STEP 8) adds the previous and next buttons and the
  "Showing 1 to 3 of 6" count; they are drawn hidden, like the theme button, so a visitor without script never
  sees a dead control.
-->
<script setup>
const featured = content.projects.filter((card) => card.project.featured)
const others = content.projects.filter((card) => !card.project.featured)
</script>

<template>
  <section id="projects" class="section" aria-labelledby="projects-heading">
    <div class="container">
      <h2 id="projects-heading" class="section-title" data-reveal>Selected projects</h2>
      <p class="section-intro" data-reveal>Public work I can show. Each card says what it is, what my part was, and what is not finished.</p>
      <div class="stack">
        <ProjectCard v-for="card in featured" :key="card.project.id" :card="card" />
      </div>
      <div class="carousel" data-carousel>
        <div class="carousel-head">
          <h3 id="more-projects-heading" class="subheading" data-reveal>More projects</h3>
          <div class="carousel-controls" hidden>
            <span class="carousel-count" data-carousel-count></span>
            <button type="button" class="carousel-button" data-carousel-prev aria-controls="more-projects-track" aria-label="Previous projects">
              <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false"><path d="M15 5l-7 7 7 7" /></svg>
            </button>
            <button type="button" class="carousel-button" data-carousel-next aria-controls="more-projects-track" aria-label="Next projects">
              <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false"><path d="M9 5l7 7-7 7" /></svg>
            </button>
          </div>
        </div>
        <div id="more-projects-track" class="carousel-track" role="region" aria-labelledby="more-projects-heading" tabindex="0">
          <ProjectCard v-for="card in others" :key="card.project.id" :card="card" />
        </div>
      </div>
    </div>
  </section>
</template>
