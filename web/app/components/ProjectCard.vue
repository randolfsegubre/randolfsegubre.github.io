<!--
  One project, drawn as an <article>. Featured projects show the full set of highlights; the compact ones show
  only the first two so the page stays scannable. The "Known gap" line is drawn only when the data names one,
  which is the mechanism behind the site's honesty policy: gaps are data, not something a template can forget
  (ADR-0006). The status wording and style hook arrive already chosen by the backend.
-->
<script setup>
const props = defineProps({
  // { project, statusLabel, statusClass }
  card: { type: Object, required: true },
})

const project = props.card.project

// STEP 1 of 1: decide how many highlights this card earns.
const highlights = project.featured ? project.highlights : project.highlights.slice(0, 2)
</script>

<template>
  <article :id="project.id" :class="project.featured ? 'card card-featured' : 'card'" :aria-labelledby="project.id + '-name'" data-reveal>
    <header class="card-header">
      <h3 :id="project.id + '-name'" class="card-title">{{ project.name }}</h3>
      <span :class="'badge ' + card.statusClass">{{ card.statusLabel }}</span>
    </header>
    <p class="card-tagline">{{ project.tagline }}</p>
    <p>{{ project.summary }}</p>
    <p class="card-role"><strong>My role:</strong> {{ project.role }}</p>
    <ul class="highlights">
      <li v-for="highlight in highlights" :key="highlight">{{ highlight }}</li>
    </ul>
    <p v-if="project.honestNote" class="honest-note"><strong>Known gap:</strong> {{ project.honestNote }}</p>
    <TagList :items="project.stack" :label="project.name + ' stack'" />
    <p v-if="project.links.length > 0" class="card-links">
      <ExternalLink v-for="link in project.links" :key="link.href" :link="link" />
    </p>
  </article>
</template>
