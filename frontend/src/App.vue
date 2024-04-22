<template>
  <v-app id="inspire">
    <v-app-bar>
      <v-app-bar-nav-icon @click="drawer = !drawer" class="text-black"/>
      <v-app-bar-title>MLSAT</v-app-bar-title>
      <v-toolbar-title>{{ appBarTitle }}</v-toolbar-title>
    </v-app-bar>
    <v-navigation-drawer
        v-model="drawer"
        temporary
    >
      <v-list height="inherit" class="d-flex flex-column">
        <template v-for="(item, index) in menuItems" :key="index">
          <v-list-item class="text" @click="openProjectsView()">{{ item }}</v-list-item>
        </template>
      </v-list>
    </v-navigation-drawer>
    <v-main class="bg-grey-lighten-2">
      <router-view/>
    </v-main>
  </v-app>
</template>

<script setup>
import {ref} from 'vue'

const drawer = ref(null)
</script>

<script>
import {mapGetters} from "vuex";

export default {
  computed: {
    ...mapGetters(['appBarTitle']),
  },
  data: () => ({
    text: '',
    drawer: null,
    menuItems: ["Проекты",],
  }),

  methods: {
    openProjectsView(){
      this.$router.push({name: 'projects'}).catch(() => {
      });
    },
  }
}
</script>
<style>
.v-navigation-drawer__scrim {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: unset !important;
  opacity: 0.2;
  transition: opacity 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  z-index: 1;
}
</style>