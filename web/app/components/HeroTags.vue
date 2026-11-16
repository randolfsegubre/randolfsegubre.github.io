<!--
  Faint code tags in the hero's side margins, as if the page were showing its own source (the idea the owner
  liked in another developer's portfolio, redrawn here in C# terms, ADR-0010). Purely decorative: hidden from
  assistive technology, no pointer events, and the stylesheet only shows them on wide screens where the
  margins are empty. The wording lives in the backend's HeroTagData, not here (ADR-0003).
-->
<template>
  <div class="hero-tags" aria-hidden="true">
    <span v-for="(tag, index) in content.heroTags" :key="tag" :class="'hero-tag hero-tag-' + (index + 1)">{{ tag }}</span>
  </div>
</template>
