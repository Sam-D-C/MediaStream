<template>
  <div>
    <header class="page-header">
      <h1>Instellingen</h1>
    </header>

    <div class="settings-card">
      <h2 class="settings-title">Mediapaden</h2>
      <p class="settings-desc">
        Geef aan waar je muziek en video's staan op de Raspberry Pi.
        Na het opslaan wordt de bibliotheek automatisch herladen.
      </p>

      <div class="settings-field">
        <label>Muziekmap</label>
        <input
          v-model="musicPath"
          type="text"
          class="settings-input"
          placeholder="/home/pi/Music"
        />
      </div>

      <div class="settings-field">
        <label>Videomap</label>
        <input
          v-model="videosPath"
          type="text"
          class="settings-input"
          placeholder="/home/pi/Videos"
        />
      </div>

      <div class="settings-actions">
        <button class="save-btn" @click="save" :disabled="saving">
          {{ saving ? 'Opslaan...' : '✓ Opslaan' }}
        </button>
      </div>

      <div v-if="message" class="settings-msg" :class="{ error: isError }">
        {{ message }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { api } from '../api.js'

const musicPath  = ref('')
const videosPath = ref('')
const saving     = ref(false)
const message    = ref('')
const isError    = ref(false)

async function load() {
  const settings = await api.getSettings()
  musicPath.value  = settings.musicPath
  videosPath.value = settings.videosPath
}

async function save() {
  saving.value = true
  message.value = ''

  const result = await api.saveSettings(musicPath.value, videosPath.value)

  if (result.message?.includes('niet')) {
    isError.value = true
    message.value = result.message
  } else {
    isError.value = false
    message.value = '✓ Opgeslagen! Bibliotheek wordt herladen.'
    window.dispatchEvent(new Event('media-refresh'))
  }

  saving.value = false
}

onMounted(load)
</script>