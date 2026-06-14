using Microsoft.AspNetCore.Mvc;
using MediaStream.Api.Services;

namespace MediaStream.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistsController : ControllerBase
    {
        private readonly PlaylistService _playlists;
        private readonly MediaService _media;

        public PlaylistsController(PlaylistService playlists, MediaService media)
        {
            _playlists = playlists;
            _media = media;
        }

        // GET /api/playlists
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_playlists.GetAll());
        }

        // GET /api/playlists/{naam}
        // Geeft playlist terug inclusief volledige songinfo
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var playlist = _playlists.GetByName(name);
            if (playlist == null) return NotFound();

            // Zet songIds om naar echte Song objecten
            var songs = playlist.SongIds
                .Select(id => _media.GetSongById(id))
                .Where(s => s != null)
                .Select(s => new {
                    s!.ID,
                    s.Name,
                    s.Artist,
                    s.Album,
                    s.HasCover,
                    s.DurationSeconds
                });

            return Ok(new { playlist.Name, playlist.CreatedAt, Songs = songs });
        }

        // POST /api/playlists
        // Body: { "name": "Mijn Playlist" }
        [HttpPost]
        public IActionResult Create([FromBody] CreatePlaylistRequest req)
        {
            try
            {
                var playlist = _playlists.Create(req.Name);
                return Ok(playlist);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // DELETE /api/playlists/{naam}
        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            var deleted = _playlists.Delete(name);
            if (!deleted) return NotFound();
            return Ok(new { message = $"Playlist '{name}' verwijderd." });
        }

        // POST /api/playlists/{naam}/songs
        // Body: { "songId": "Bohemian-Rhapsody" }
        [HttpPost("{name}/songs")]
        public IActionResult AddSong(string name, [FromBody] AddSongRequest req)
        {
            var success = _playlists.AddSong(name, req.SongId);
            if (!success) return NotFound(new { message = $"Playlist '{name}' niet gevonden." });
            return Ok(new { message = "Nummer toegevoegd." });
        }

        // DELETE /api/playlists/{naam}/songs/{songId}
        [HttpDelete("{name}/songs/{songId}")]
        public IActionResult RemoveSong(string name, string songId)
        {
            var success = _playlists.RemoveSong(name, songId);
            if (!success) return NotFound();
            return Ok(new { message = "Nummer verwijderd." });
        }
    }

    public record CreatePlaylistRequest(string Name);
    public record AddSongRequest(string SongId);
}