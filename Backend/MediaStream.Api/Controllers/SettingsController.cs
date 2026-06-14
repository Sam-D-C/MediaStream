using Microsoft.AspNetCore.Mvc;
using MediaStream.Api.Services;

namespace MediaStream.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly SettingsService _settings;
        private readonly MediaService _media;

        public SettingsController(SettingsService settings, MediaService media)
        {
            _settings = settings;
            _media = media;
        }

        // GET /api/settings
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_settings.Get());
        }

        // POST /api/settings
        [HttpPost]
        public IActionResult Update([FromBody] UpdateSettingsRequest req)
        {
            // Controleer of de paden bestaan
            if (!Directory.Exists(req.MusicPath))
                return BadRequest(new { message = $"Muziekpad bestaat niet: {req.MusicPath}" });

            if (!Directory.Exists(req.VideosPath))
                return BadRequest(new { message = $"Videopad bestaat niet: {req.VideosPath}" });

            _settings.Update(req.MusicPath, req.VideosPath);

            // Herlaad de mediabibliotheek met nieuwe paden
            _media.UpdatePaths(req.MusicPath, req.VideosPath);

            return Ok(new { message = "Instellingen opgeslagen en bibliotheek herladen." });
        }
    }

    public record UpdateSettingsRequest(string MusicPath, string VideosPath);
}