<template>
  <v-container fluid>
    <v-row class="mx-0 my-0 pb-2" height="inherit">
      <v-list height="inherit" class="d-flex px-0 py-0">
        <template v-for="(item, index) in menuItems" :key="index">
          <v-list-item class="text" @click="handleMenuItemClick(item)">{{ item }}</v-list-item>
        </template>
      </v-list>
    </v-row>
    <v-list height="inherit" class="d-flex flex-column">
      <template v-for="(project, projectIndex) in projects" :key="'project_' + projectIndex">
        <v-list-item class="text" @click="openProjectTemplate(project.id);">{{
            project.title
          }}
        </v-list-item>
      </template>
    </v-list>
    <v-dialog v-model="createProject" persistent width="auto">
      <v-card>
        <v-card-title>Выберите название проекта</v-card-title>
        <v-card-text>
          <v-text-field v-model="text"/>
        </v-card-text>
        <v-card-actions>
          <v-btn @click="createProject=false">
            Отмена
          </v-btn>
          <v-btn @click="newProject()">
            Добавить
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>
<script>

import ProjectsRequest from "@/services/ProjectsRequest";
import {mapActions} from "vuex";

export default {


  data: () => ({
    projects: [],
    menuItems: ["Создать проект",],
    createProject: false,
    text: '',
  }),
  methods: {
    ...mapActions(['updateAppBarTitle']),
    changeAppBarTitle() {
      this.updateAppBarTitle('');
    },

    openProjectTemplate(item) {
      let selectedProjectID = parseInt(item);
      this.$router.push({name: 'project-template', params: {selectedProjectID}}).catch(() => {
      });
    },

    async getProjects() {
      const project = new ProjectsRequest();
      let projects
      await project.getProjects().then(x => {
        projects = x.data
      }).catch(error => {
        console.error(error);
        return [];
      });
      this.projects = projects;
      this.changeAppBarTitle()
      return this.projects;
    },

    handleMenuItemClick(item) {
      if (item === 'Создать проект') {
        this.createProject = true
      }
    },

    async newProject() {
      const project = new ProjectsRequest()
      let body = {
        "title": this.text,
      }
      this.text = ''
      await project.postProjects(body).catch(x => {
        console.log(x)
      }).finally(() => {
        this.createProject = false
        this.getProjects()
      })
    },

    initialize() {
      this.getProjects()
    }
  },

  computed: {},

  watch: {},

  created() {
    this.initialize()
  }
}

</script>
<style scoped>

</style>