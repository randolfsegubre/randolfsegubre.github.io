<!--
  Career at a glance: a few checkable facts under the hero. This replaces the skill percentage bars many
  portfolios use, because "90 percent JavaScript" cannot be verified and the site only states what can be
  checked (ADR-0006, ADR-0008). The strip overlaps the hero's bottom edge for depth.
-->
<template>
  <section class="stats-wrap" aria-label="Career at a glance">
    <div class="container">
      <ul class="stats">
        <li v-for="stat in content.profile.stats" :key="stat.label" class="stat" data-reveal>
          <strong class="stat-value">{{ stat.value }}</strong>
          <span class="stat-label">{{ stat.label }}</span>
        </li>
      </ul>
    </div>
  </section>
</template>
