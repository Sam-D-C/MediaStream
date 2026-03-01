namespace MediaStream
{
    public class Program
    {
        public static void Main()
        {
            Song music = new Song("Shape of You", "cover.jpg", "Ed Sheeran", "A popular song by Ed Sheeran", "2017-01-06");
            Song music2 = new Song("Blinding Lights", "cover2.jpg", "The Weeknd", "A hit song by The Weeknd", "2019-11-29");
            Song music3 = new Song("Levitating", "cover3.jpg", "Dua Lipa", "A catchy song by Dua Lipa", "2020-03-27");

            Playlist playlist = new Playlist("Playlist1", new List<Song> { music, music2, music3 });

            User sam = new User( "Sam",  "email",  "password");
            sam.AddToLikes(music);
            sam.likes.DisplayPlaylist();
            sam.AddToLikes(music);
            sam.likes.DisplayPlaylist();

        }
    }
}