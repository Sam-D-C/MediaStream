<template>
  <Teleport to="body">
    <div
      v-if="visible"
      class="ctx-backdrop"
      @click="close"
    />
    <div
      v-if="visible"
      class="ctx-menu"
      :style="{ top: y + 'px', left: x + 'px' }"
    >
      <div class="ctx-header">{{ song?.name }}</div>

      <button class="ctx-item" @click="onAddToQueue">
        <span>⏭</span> Toevoegen aan wachtrij
      </button>

      <div class="ctx-divider" />

      <div class="ctx-label">Toevoegen aan playlist</div>

      <div v-if="playlists.length === 0" class="ctx-empty">
        Nog geen playlists
      </div>

      <button
        v-for="pl in playlists"
        :key="pl.name"
        class="ctx-item"
        @click="onAddToPlaylist(pl.name)"
      >
        <span>♪</span> {{ pl.name }}
      </button>

      <div class="ctx-divider" />

      <button class="ctx-item" @click="onNewPlaylist">
        <span>＋</span> Nieuwe playlist...
      </button>
    </div>
  </Teleport>
</template>

<script setup>
import { ref, inject } from 'vue'
import { api } from '../api.js'

const visible  = ref(false)
const x        = ref(0)
const y        = ref(0)
const song     = ref(null)
const playlists = ref([])

const addToQueue = inject('addToQueue')

function calcPosition(event) {
  const menuWidth  = 220
  const menuHeight = 300
  const padding    = 8

  let x = event.clientX
  let y = event.clientY

  if (x + menuWidth > window.innerWidth - padding) {
    x = window.innerWidth - menuWidth - padding
  }
  if (y + menuHeight > window.innerHeight - padding) {
    y = window.innerHeight - menuHeight - padding
  }

  return { x, y }
}

async function open(event, s) {
  event.preventDefault()
  song.value = s
  const pos = calcPosition(event)
  x.value = pos.x
  y.value = pos.y
  visible.value = true
  playlists.value = await api.getPlaylists()
}

function close() {
  visible.value = false
}

function onAddToQueue() {
  addToQueue(song.value)
  close()
}

async function onAddToPlaylist(name) {
  await api.addToPlaylist(name, song.value.id)
  close()
}

async function openPlaylistOnly(event, s) {
  event.preventDefault()
  song.value = s
  const pos = calcPosition(event)
  x.value = pos.x
  y.value = pos.y
  playlists.value = await api.getPlaylists()
  visible.value = true
}

async function onNewPlaylist() {
  const name = prompt('Naam van de nieuwe playlist:')
  if (!name?.trim()) return
  await api.createPlaylist(name.trim())
  await api.addToPlaylist(name.trim(), song.value.id)
  window.dispatchEvent(new Event('playlists-changed'))
  close()
}

// Stel open beschikbaar voor parent via defineExpose
defineExpose({ open, openPlaylistOnly })
</script>

<style scoped>
.ctx-backdrop {
  position: fixed;
  inset: 0;
  z-index: 99;
}

.ctx-menu {
  position: fixed;
  z-index: 100;
  background: #1b1f24;
  border: 1px solid #313842;
  border-radius: 12px;
  padding: 6px;
  min-width: 220px;
  box-shadow: 0 16px 40px rgba(0,0,0,.5);
}

.ctx-header {
  padding: 8px 12px;
  font-size: 13px;
  font-weight: 600;
  color: #f5b30b;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.ctx-divider {
  height: 1px;
  background: #313842;
  margin: 4px 0;
}

.ctx-label {
  padding: 6px 12px 2px;
  font-size: 11px;
  text-transform: uppercase;
  letter-spacing: 0.6px;
  color: #9f9a90;
}

.ctx-empty {
  padding: 6px 12px;
  font-size: 13px;
  color: #9f9a90;
}

.ctx-item {
  display: flex;
  align-items: center;
  gap: 10px;
  width: 100%;
  padding: 10px 12px;
  background: none;
  border: none;
  border-radius: 8px;
  color: #e8e4dc;
  font-size: 14px;
  cursor: pointer;
  text-align: left;
  transition: background 0.1s;
}

.ctx-item:hover {
  background: rgba(245,179,11,.1);
  color: #f5b30b;
}
</style>