<!--
  The hidden logo sprite (ADR-0008): every technology logo drawn exactly once as an SVG <symbol>. The hero's
  logos and the faint page layer reference these with <use>, so the page stays small however many times a logo
  repeats. The sprite is zero-sized and hidden from assistive technology; it draws nothing by itself.
-->
<template>
  <svg width="0" height="0" style="position:absolute" aria-hidden="true" focusable="false">
    <defs>
      <symbol v-for="logo in content.logos" :id="logo.symbolId" :key="logo.symbolId" viewBox="0 0 24 24" v-html="logo.markup" />
    </defs>
  </svg>
</template>
