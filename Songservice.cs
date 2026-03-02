namespace MediaStream
{
    public class SongService
    {
        private static readonly string _musicPath = "C:\\Users\\samde\\Documents\\MediaStream\\Music";
        private static List<Song> _songs = new();
        private static bool found = false;

        public static void LoadSongs()
        {
            var files = Directory.GetFiles(_musicPath, "*.mp3");

            foreach (var file in files)
            {
                try
                {
                    var tagFile = TagLib.File.Create(file);

                    _songs.Add(new Song
                    {
                        Name = tagFile.Tag.Title ?? "Unkown",
                        Cover = "Unknown.jpg",
                        Artist = tagFile.Tag.FirstPerformer ?? "Unknown",
                        Description = tagFile.Tag.Comment ?? "Unknown",
                        ReleaseDate = tagFile.Tag.Year > 0
                                      ? tagFile.Tag.Year.ToString()
                                      : "Unknown"
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fout bij laden van bestand: {file}");
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine();
                }
            }
        }

        public static List<Song> GetAllSongs()
        {
            _songs.Clear();
            LoadSongs();
            return _songs;
        }

        public static void PrintAllSongs()
        {
            _songs.Clear();
            LoadSongs();
            Console.WriteLine($"Available Songs [{_songs.Count}]:");
            foreach (var song in _songs)
            {
                Console.WriteLine($"- '{song.Name}' by '{song.Artist}' (Released: {song.ReleaseDate})");
            }
            Console.WriteLine();
        }

        public static void GetById(string name)
        {
            _songs.Clear();
            LoadSongs();

            Console.WriteLine($"Searching for song with Name: {name}");

            foreach (var song in _songs)
            {
                if (song.Name == name)
                {
                    Console.WriteLine($"Found: '{song.Name}' by '{song.Artist}' (Released: {song.ReleaseDate})");
                    if (found == false)
                    {
                        found = true;
                    }
                }
            }
            if (!found)
            {
                Console.WriteLine($"No song found with Name: {name}");
                found = false;
            }
            Console.WriteLine();
        }
    }
}
