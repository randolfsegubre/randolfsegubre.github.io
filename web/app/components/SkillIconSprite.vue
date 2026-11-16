<!--
  The hidden sprite for the skills cloud's own icons (ADR-0010): the extra logos and the generic line glyphs,
  each drawn once as an SVG <symbol>. The technology logos the cloud also uses live in the page-wide sprite
  (LogoSprite). Zero-sized and hidden from assistive technology; it draws nothing by itself.
-->
<template>
  <svg width="0" height="0" style="position:absolute" aria-hidden="true" focusable="false">
    <defs>
      <symbol v-for="icon in content.skillIcons" :id="icon.symbolId" :key="icon.symbolId" viewBox="0 0 24 24" v-html="icon.markup" />
    </defs>
  </svg>
</template>
