import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import App from './App.vue'
import MusicView from './views/MusicView.vue'
import VideoView from './views/VideoView.vue'
import PlaylistView from './views/PlaylistView.vue'
import QueueView from './views/QueueView.vue'
import './style.css'
import SettingsView from './views/SettingsView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/',                    component: MusicView },
    { path: '/videos',              component: VideoView },
    { path: '/playlist/:name',      component: PlaylistView },
    { path: '/queue',               component: QueueView },
    { path: '/settings', component: SettingsView },
  ]
})

createApp(App).use(router).mount('#app')