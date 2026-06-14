using System.Text.Json;

namespace MediaStream.Api.Services
{
    public class AppSettings
    {
        public string MusicPath { get; set; } = "./Music";
        public string VideosPath { get; set; } = "./Videos";
    }

    public class SettingsService
    {
        private readonly string _filePath;
        private AppSettings _settings;

        public SettingsService()
        {
            // Sla settings.json op naast de executable
            _filePath = Path.Combine(AppContext.BaseDirectory, "settings.json");
            _settings = Load();
        }

        private AppSettings Load()
        {
            if (!File.Exists(_filePath))
            {
                // Eerste keer — maak standaard instellingen aan
                var defaults = new AppSettings();
                Save(defaults);
                return defaults;
            }

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }

        private void Save(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_filePath, json);
        }

        public AppSettings Get() => _settings;

        public AppSettings Update(string musicPath, string videosPath)
        {
            _settings.MusicPath = musicPath;
            _settings.VideosPath = videosPath;
            Save(_settings);
            return _settings;
        }
    }
}