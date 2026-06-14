using Microsoft.AspNetCore.Mvc;
using MediaStream.Api.Services;

namespace MediaStream.Api.Controllers
{
    [ApiController] // This attribute indicates that the controller responds to web API requests.
    [Route("api/[controller]")] // This attribute indicates a automatic route for the controller based on its name. In this case, it will be "api/songs".
    public class SongsController : ControllerBase
    // This class is a controller that handles HTTP requests related to songs. It inherits from ControllerBase,
    // which provides basic functionality for handling web API requests.
    {
        private readonly MediaService _media;

        public SongsController(MediaService media)
        {
            _media = media;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_media.GetAllSongs());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var song = _media.GetSongById(id);
            if (song == null) return NotFound();
            return Ok(song);
        }

        [HttpGet("{id}/cover")]
        public IActionResult GetCover(string id)
        {
            var cover = _media.GetCoverArt(id);
            if (cover == null) return NotFound();
            return File(cover, "image/jpeg");
        }

        [HttpGet("{id}/stream")]
        public IActionResult Stream(string id)
        {
            var song = _media.GetSongById(id);
            if (song == null) return NotFound();
            return PhysicalFile(song.FilePath, "audio/mpeg", enableRangeProcessing: true);
        }

        [HttpPost("refresh")]
        public IActionResult Refresh()
        {
            _media.RefreshLibrary();
            return Ok(new { message = "Refreshing your library, just a sec..." });
        }
    }
}