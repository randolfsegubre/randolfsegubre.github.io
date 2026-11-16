<!--
  A link that leaves the site. Every outbound link goes through this component so the safety and
  accessibility details are written once: rel="noopener noreferrer" stops the new tab controlling this one,
  and the visually hidden text tells screen reader users the link opens a new tab. Pass a class to style it
  as a button. Pass an icon (the shapes of a 24 by 24 drawing) to show the profile icon before the label;
  the icon is decorative and the label names the link.
-->
<script setup>
defineProps({
  // { label, href }
  link: { type: Object, required: true },
  cssClass: { type: String, default: undefined },
  // The shapes inside a 24 by 24 view box (GitHub or LinkedIn), when the link shows an icon.
  iconMarkup: { type: String, default: undefined },
})
</script>

<template>
  <a :href="link.href" target="_blank" rel="noopener noreferrer" :class="cssClass"><svg v-if="iconMarkup" class="link-icon" viewBox="0 0 24 24" aria-hidden="true" focusable="false" v-html="iconMarkup" />{{ link.label }}<span class="visually-hidden"> (opens in a new tab)</span></a>
</template>
