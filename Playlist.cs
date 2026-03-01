namespace MediaStream
{
    class Playlist
    {
        public string Name;
        public List<MP3> Songs;

        // Constructor
        public Playlist(string name, List<MP3> songs)
        {
            Name = name;
            Songs = songs;
        }

        public void DisplayPlaylist()
        {
            Console.WriteLine($"Playlist: {Name}");
            foreach (var song in Songs)
            {
                Console.WriteLine($"- '{song.Name}' by {song.Artist}");
                Console.WriteLine();
            }
        }

        public void AddSongManual()
        {
            Console.WriteLine("Enter song details:");
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Cover: ");
            string cover = Console.ReadLine();
            Console.Write("Artist: ");
            string artist = Console.ReadLine();
            Console.Write("Description: ");
            string description = Console.ReadLine();
            Console.Write("Release Date: ");
            string releaseDate = Console.ReadLine();
            MP3 newSong = new MP3(name, cover, artist, description, releaseDate);
            Songs.Add(newSong);
            Console.WriteLine($"Added '{name}' to the playlist.");
            Console.WriteLine();
        }

        public void AddSong(MP3 song)
        {
            Songs.Add(song);
            Console.WriteLine($"Added '{song.Name}' to the playlist.");
            Console.WriteLine();
        }

        public void DeleteSong(MP3 song)
        {
            if (Songs.Remove(song))
            {
                Console.WriteLine($"Deleted '{song.Name}' from the playlist.");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"Song '{song.Name}' not found in the playlist."); 
                Console.WriteLine();
            }
        }
    }
}
