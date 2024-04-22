import {createRouter, createWebHistory} from 'vue-router'
import ProjectView from "@/views/ProjectView.vue";
import ProjectsListView from "@/views/ProjectsListView.vue";
import MainView from "@/views/MainView.vue";
import ModelView from "@/views/modelView.vue";

const routes = [
    {
        path: '/',
        name: 'home',
        component: MainView
    },
    {
        path: '/projects',
        name: 'projects',
        component: ProjectsListView
    },
    {
        path: '/project/:selectedProjectID',
        name: 'project-template',
        component: ProjectView,
        props: true,
        children: [
        ]
    },
    {
        path: '/modelView/:selectedProjectID/:type',
        name: 'modelView',
        component: ModelView,
        props: true,
    }
]

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes
})

export default router
