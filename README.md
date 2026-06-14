# 🎵 MediaStream

Een zelfgebouwde lokale streaming service voor muziek en video's. Ontwikkeld om te draaien op een Raspberry Pi in een feestschuur/keet, bedienbaar vanaf elk apparaat in het netwerk — telefoon, tablet of laptop — zonder installatie.

---

## Waarom gebouwd?

MediaStream is ontwikkeld als een persoonlijk full-stack leerproject. Het doel was om een eigen Spotify-achtige ervaring te bouwen die:

- Draait op een **Raspberry Pi** aangesloten op een monitor en muziekboxen in een feestschuur
- Bereikbaar is voor een vriendengroep via het lokale netwerk
- Volledig **zonder internet** werkt — alle muziek en video's staan lokaal
- Geen abonnement of externe diensten vereist

Tegelijkertijd was dit een manier om **full-stack development** te leren: een C# backend bouwen, een Vue.js frontend opzetten en de twee laten communiceren via een REST API.

---

## Technologieën

| Onderdeel | Technologie | Reden |
|-----------|-------------|-------|
| Backend | ASP.NET Core 8 (C#) | Krachtig, snel, goede streaming ondersteuning |
| Frontend | Vue 3 + Vite | Modern, begrijpelijk, reactief |
| Navigatie | Vue Router | Navigatie zonder pagina te herladen |
| MP3 metadata | TagLib# | Uitlezen van titel, artiest, albumhoes |
| Opslag | JSON bestanden | Eenvoudig, geen database nodig |
| Deployment | Self-contained executable | Geen .NET installatie vereist op de Pi |

---

## Functionaliteiten

- 🎵 **Muziek afspelen** — mp3 bestanden streamen met seekbare voortgangsbalk
- 🎬 **Video's afspelen** — mp4, mkv en webm met ingebouwde speler
- 🔍 **Zoeken** — zoek op titel of artiest
- 📋 **Playlists** — aanmaken, beheren en afspelen (opgeslagen op de Pi)
- ⏭ **Wachtrij** — nummers toevoegen, herordenen en verwijderen
- 🔄 **Automatisch doorspelen** — volgende nummer start vanzelf
- ⚙️ **Instellingen** — mediapaden aanpassen via de UI, geen code nodig
- 📱 **Netwerktoegang** — bedienbaar vanaf elk apparaat in het netwerk

---

## Architectuur

```
┌─────────────────────────┐         REST API (JSON)        ┌──────────────────────────┐
│   FRONTEND (Vue 3)      │ ◄────────────────────────────► │   BACKEND (ASP.NET Core) │
│   Browser / localhost   │                                 │   localhost:5000         │
│                         │   GET  /api/songs               │                          │
│   - MusicView           │   GET  /api/songs/{id}/stream   │   - SongsController      │
│   - VideoView           │   GET  /api/videos              │   - VideosController     │
│   - PlaylistView        │   GET  /api/playlists           │   - PlaylistsController  │
│   - QueueView           │   POST /api/playlists           │   - SettingsController   │
│   - SettingsView        │   GET  /api/settings            │                          │
└─────────────────────────┘                                 └──────────────────────────┘
                                                                       │
                                                            ┌──────────┴───────────┐
                                                            │   Bestanden op schijf │
                                                            │   playlists.json      │
                                                            │   settings.json       │
                                                            └──────────────────────┘
```

---

## Mappenstructuur

```
MediaStream/
├── Backend/
│   └── MediaStream.Api/
│       ├── Controllers/
│       │   ├── SongsController.cs        ← REST endpoints voor muziek
│       │   ├── VideosController.cs       ← REST endpoints voor video
│       │   ├── PlaylistsController.cs    ← Playlist beheer
│       │   └── SettingsController.cs     ← Instellingen lezen/opslaan
│       ├── Models/
│       │   └── MediaModels.cs            ← Song, Video, Playlist klassen
│       ├── Services/
│       │   ├── MediaService.cs           ← Bestanden laden en streamen
│       │   ├── PlaylistService.cs        ← Playlists opslaan als JSON
│       │   └── SettingsService.cs        ← Instellingen opslaan als JSON
│       ├── wwwroot/                      ← Gebouwde Vue frontend (auto gegenereerd)
│       ├── Program.cs                    ← App setup en Dependency Injection
│       └── appsettings.json             ← Configuratie
│
├── Frontend/
│   └── MediaStream/
│       ├── src/
│       │   ├── views/
│       │   │   ├── MusicView.vue         ← Muziekbibliotheek
│       │   │   ├── VideoView.vue         ← Videobibliotheek
│       │   │   ├── PlaylistView.vue      ← Playlist bekijken en beheren
│       │   │   ├── QueueView.vue         ← Wachtrij beheren
│       │   │   └── SettingsView.vue      ← Mediapaden instellen
│       │   ├── components/
│       │   │   └── ContextMenu.vue       ← Rechtsklik menu
│       │   ├── App.vue                   ← Root component + audiospeler
│       │   ├── api.js                    ← Alle communicatie met de backend
│       │   ├── main.js                   ← App initialisatie + Vue Router
│       │   └── style.css                 ← Globale stijlen
│       ├── vite.config.js                ← Vite configuratie + proxy
│       └── package.json
```

---

## Installatie — Development

### Vereisten
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18 of hoger)
- Visual Studio 2022 of VS Code

### Backend starten

```bash
cd Backend/MediaStream.Api
dotnet restore
dotnet run
# → API draait op http://localhost:5000
```

### Frontend starten

```bash
cd Frontend/MediaStream
npm install
npm run dev
# → Open http://localhost:5173
```

De Vite dev server stuurt `/api` requests automatisch door naar de backend via een proxy.

---

## Installatie — Productie (Raspberry Pi)

### Stap 1 — Frontend bouwen

```bash
cd Frontend/MediaStream
npm run build
# → Bestanden worden geplaatst in Backend/MediaStream.Api/wwwroot/
```

### Stap 2 — Backend publishen

**Voor Raspberry Pi (Linux ARM64):**
```bash
cd Backend/MediaStream.Api
dotnet publish -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true -o ./publish/pi
```

**Voor Windows:**
```bash
cd Backend/MediaStream.Api
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/windows
```

### Stap 3 — Bestanden naar de Pi kopiëren

Kopieer de volledige inhoud van `publish/pi/` naar de Pi, bijvoorbeeld naar `/home/pi/mediastream/`.

### Stap 4 — Starten op de Pi

```bash
chmod +x /home/pi/mediastream/MediaStream.Api
/home/pi/mediastream/MediaStream.Api
```

Open daarna op elk apparaat in het netwerk:
```
http://[ip-adres-van-pi]:5000
```

### Stap 5 — Automatisch opstarten (optioneel)

```bash
sudo nano /etc/systemd/system/mediastream.service
```

```ini
[Unit]
Description=MediaStream
After=network.target

[Service]
WorkingDirectory=/home/pi/mediastream
ExecStart=/home/pi/mediastream/MediaStream.Api
Restart=always
User=pi

[Install]
WantedBy=multi-user.target
```

```bash
sudo systemctl enable mediastream
sudo systemctl start mediastream
```

---

## Gebruik

### Eerste keer opstarten
1. Start de app
2. Ga naar **⚙ Instellingen** in de sidebar
3. Vul de paden in naar je muziek en videomap
4. Klik op **Opslaan** — de bibliotheek laadt automatisch

### Muziek afspelen
- Klik op een nummer om het af te spelen
- Hover over een nummer voor extra knoppen:
  - **⏭** — toevoegen aan wachtrij
  - **≡** — toevoegen aan playlist
- Rechtsklik voor het volledige contextmenu

### Playlists
- Aanmaken via het contextmenu of de ≡ knop
- Bekijken via de sidebar onder **Playlists**
- Verwijder nummers met de rode ✕ knop
- Speel alles af met **▶ Alles afspelen**

### Wachtrij
- Zichtbaar via **⏭ Wachtrij** in de sidebar
- Herorden nummers met de ▲ en ▼ pijltjes
- Verwijder nummers met de rode ✕ knop

---

## Mediapaden aanpassen

Paden worden opgeslagen in `settings.json` naast de executable. Je kunt ze aanpassen via de UI (**⚙ Instellingen**) zonder de app te herstarten of code aan te passen.

---

## Ondersteunde bestandsformaten

| Type | Formaten |
|------|----------|
| Muziek | `.mp3` |
| Video | `.mp4`, `.mkv`, `.webm`, `.avi` |

---

## Ontwikkeld door

Sam — als full-stack leerproject, gebouwd van scratch met hulp van Claude (Anthropic).
