<!--
  Employment history, newest first, as a list of articles. Roles done through a staffing or consulting firm show
  the firm as the employer and the end client on the same line, which matches how the work was actually
  organised. The client is omitted when there is none.
-->
<template>
  <section id="experience" class="section" aria-labelledby="experience-heading">
    <div class="container">
      <h2 id="experience-heading" class="section-title" data-reveal>Experience</h2>
      <p class="section-intro" data-reveal>9+ years of professional C# and .NET, newest first.</p>
      <ol class="timeline">
        <li v-for="role in content.experience" :key="role.id" data-reveal>
          <article class="role" :aria-labelledby="role.id + '-title'">
            <header>
              <h3 :id="role.id + '-title'" class="role-title">{{ role.title }}</h3>
              <p class="role-company"><span>{{ role.company }}</span><span v-if="role.client" class="role-client">, client {{ role.client }}</span></p>
              <p class="role-period">{{ role.period }}{{ role.location ? ' · ' + role.location : '' }}</p>
            </header>
            <ul class="highlights">
              <li v-for="highlight in role.highlights" :key="highlight">{{ highlight }}</li>
            </ul>
            <TagList :items="role.stack" :label="role.company + ' technologies'" />
          </article>
        </li>
      </ol>
    </div>
  </section>
</template>
