<!--
  Quiet technology logos scattered around the page (ADR-0008): faint silhouettes with a very slow drift, so
  they read as background texture and never as content. The "hero" variant is the layer inside the hero
  (positioned by percentage of the hero, drawn in the hero's light text color); "page" is the layer fixed
  behind every other section (drawn in the theme's accent color).

  The backend gives each layer its own share of the technologies and their slots (SiteContentBuilder), so every
  logo appears once, not twice, and this component only draws them. Each logo is a reference to the shared
  sprite (LogoSprite). Purely decorative, so the layer is hidden from assistive technology. Slots marked "edge"
  stay on phones, where only a few are shown.
-->
<script setup>
defineProps({
  variant: { type: String, required: true },
  // [{ key, symbolId, edge, style }]
  slots: { type: Array, required: true },
})
</script>

<template>
  <div :class="'tech-logos tech-logos--' + variant" aria-hidden="true">
    <span v-for="slot in slots" :key="slot.key" :class="slot.edge ? 'tech-logo tech-logo--edge' : 'tech-logo'" :style="slot.style" :data-tech="slot.key"><svg viewBox="0 0 24 24" focusable="false"><use :href="'#' + slot.symbolId" /></svg></span>
  </div>
</template>
