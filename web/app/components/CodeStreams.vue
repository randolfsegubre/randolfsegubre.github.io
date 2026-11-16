<!--
  Lanes of code scrolling slowly and endlessly, like a looping animated background (ADR-0008). Lanes have no
  positions of their own: the stylesheet lays them out as equal-width grid columns across the full width, so
  two lanes can never overlap. Each lane's code is rendered twice in a row and the stylesheet scrolls exactly
  half its height, so the loop has no visible jump.

  Purely decorative: hidden from assistive technology, no pointer events, and the stylesheet stops the motion
  for visitors who prefer reduced motion.
-->
<script setup>
defineProps({
  variant: { type: String, required: true },
})
</script>

<template>
  <div :class="'code-streams code-streams--' + variant" aria-hidden="true">
    <div v-for="(lane, index) in content.codeStreams" :key="index" class="code-stream" :style="'--dur:' + lane.seconds + 's;--dir:' + (lane.reverse ? 'reverse' : 'normal')"><pre v-text="lane.code + '\n' + lane.code" /></div>
  </div>
</template>
