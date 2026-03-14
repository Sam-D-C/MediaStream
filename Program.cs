namespace MediaStream
{
    public class Program
    {
        public static void Main()
        {
            SongService.Refresh();
            SongService.PrintAllSongs();
        }
    }
}