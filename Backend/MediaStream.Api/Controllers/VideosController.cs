using Microsoft.AspNetCore.Mvc;
using MediaStream.Api.Services;

namespace MediaStream.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideosController : ControllerBase
    {
        private readonly MediaService _media;

        public VideosController(MediaService media)
        {
            _media = media;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_media.GetAllVideos());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var video = _media.GetVideoById(id);
            if (video == null) return NotFound();
            return Ok(new { video.ID, video.Name, video.FileSizeBytes });
        }

        [HttpGet("{id}/stream")]
        public IActionResult Stream(string id)
        {
            var video = _media.GetVideoById(id);
            if (video == null) return NotFound();

            var ext = Path.GetExtension(video.FilePath).ToLower();
            var contentType = ext switch
            {
                ".mp4" => "video/mp4",
                ".mkv" => "video/x-matroska",
                ".webm" => "video/webm",
                _ => "application/octet-stream"
            };

            return PhysicalFile(video.FilePath, contentType, enableRangeProcessing: true);
        }
    }
}