namespace MediaStream
{
    class MP3
    {
        public string Name;
        public string Cover;
        public string Artist;
        public string Description;
        public string ReleaseDate;

        // Constructor
        public MP3(string name, string cover, string artist, string description, string releaseDate)
        {
            Name = name;
            Cover = cover;
            Artist = artist;
            Description = description;
            ReleaseDate = releaseDate;
        }
    }
}
