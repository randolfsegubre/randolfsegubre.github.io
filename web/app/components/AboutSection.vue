<!--
  "About Me" and the skills together (ADR-0010): his own paragraphs, then every technology he has used as a
  cloud of spheres with .NET at the centre, on a purple band like the hero and footer, and the full grouped
  list kept in an ordinary section underneath. This component only draws what the data says; the words live in
  the content files profile.json and skill-universe.json, and the positions come from SkillUniverseLayout (ADR-0003).

  Works without script: the cloud is a 3D scene, so without script the map is hidden and every caption is
  listed instead. With script (site.js, STEP 5) the markup becomes a floating, rotatable 3D cloud: each star
  carries its depth and its place on the sphere (data-depth, data-azimuth, data-elevation, data-radius) so the
  script only has to project it, and a star becomes a button that lights its connections and shows its caption.
-->
<script setup>
const universe = content.universe
</script>

<template>
  <SkillIconSprite />
  <section id="about" class="section section-about about-band" aria-labelledby="about-heading">
    <div class="container">
      <h2 id="about-heading" class="section-title" data-reveal>About Me</h2>
      <div class="about-grid">
        <div class="about-copy" data-reveal>
          <p v-for="paragraph in content.profile.about" :key="paragraph">{{ paragraph }}</p>
        </div>

        <div class="about-skills-intro" data-reveal>
          <h3 id="skills-heading" class="about-sub">Skills</h3>
          <p class="section-intro">{{ universe.intro }}</p>
        </div>
      </div>

      <div class="universe" data-universe :data-journey="universe.journey.join(',')" data-reveal>
        <div class="universe-bar">
          <button type="button" class="universe-walk" data-universe-walk hidden>Walk my journey</button>
          <button type="button" class="universe-control" data-universe-spin hidden>Pause rotation</button>
          <button type="button" class="universe-control" data-universe-turn="-1" hidden>Turn left</button>
          <button type="button" class="universe-control" data-universe-turn="1" hidden>Turn right</button>
          <span class="universe-hint">Drag or swipe to turn it. Choose a sphere to bring it to the front.</span>
        </div>

        <div class="universe-scroll" role="region" aria-label="Skills cloud. Drag it or use the arrow keys to turn it." tabindex="0">
          <div class="universe-map">
            <!-- Dust and lines are drawn as decoration; the stars below carry the meaning. Script positions all of it in 3D. -->
            <svg class="universe-lines" viewBox="0 0 1000 1000" aria-hidden="true" focusable="false">
              <line v-for="link in universe.links" :key="link.fromId + link.toId" class="universe-link" :data-link-from="link.fromId" :data-link-to="link.toId" />
            </svg>
            <ul class="universe-stars" aria-label="Technologies in the cloud">
              <li v-for="star in universe.stars" :key="star.id" :class="['star', 'star-depth-' + star.depth, { 'star-main': star.main, 'tone-dark': star.onDark }]" :data-star="star.id" :data-depth="star.depth" :data-azimuth="star.azimuth" :data-elevation="star.elevation" :data-radius="star.radius"><span class="orb" aria-hidden="true"><svg class="orb-icon" viewBox="0 0 24 24" :fill="star.iconFill" focusable="false"><use :href="'#' + star.iconSymbolId" /></svg></span><span :class="star.depth === 0 ? 'star-name visually-hidden' : 'star-name'">{{ star.label }}</span></li>
            </ul>
          </div>
        </div>

        <!-- Script shows one caption at a time in a live region; without script this whole list is the caption view. -->
        <ul class="universe-captions" aria-label="Where I used each technology">
          <li v-for="star in universe.stars" :key="star.id" class="universe-caption" :data-caption="star.id"><strong>{{ star.label }}</strong> {{ star.caption }}</li>
        </ul>
      </div>
    </div>
  </section>
  <section class="section section-skills-list" aria-label="All skills">
    <div class="container">
      <details class="skills-all" data-reveal>
        <summary>All skills, grouped</summary>
        <div class="grid">
          <div v-for="group in content.skills" :key="group.id" class="skill-group">
            <h3 class="skill-title">{{ group.label }}</h3>
            <TagList :items="group.items" :label="group.label" />
          </div>
        </div>
      </details>
    </div>
  </section>
</template>
