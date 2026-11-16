<!--
  A row of product-name chips (for example "React", "Umbraco"). Rendered as a real list with an accessible
  name so a screen reader announces "Stack, list, 10 items" instead of a run of unrelated words. Chips list
  product names only, which is why they are exempt from the abbreviation rule (ADR-0006).
-->
<script setup>
defineProps({
  items: { type: Array, required: true },
  label: { type: String, required: true },
})
</script>

<template>
  <ul class="tags" :aria-label="label">
    <li v-for="item in items" :key="item" class="tag">{{ item }}</li>
  </ul>
</template>
