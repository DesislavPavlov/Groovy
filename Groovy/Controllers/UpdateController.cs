using Groovy.Models;
using Groovy.Services;
using Microsoft.AspNetCore.Mvc;

namespace Groovy.Controllers
{
    public class UpdateController : Controller
    {
        private readonly ApiService _apiService;
        public UpdateController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSong(int songId, string title, string coverUrl, string songUrl, string color, string artists, string genres, IFormFile croppedImage, IFormFile audioFile)
        {
            // Reset IndexViewModel
            HttpContext.Session.Remove("IndexViewModel");

            // Setup mutipart data
            Dictionary<string, string> formFields = new Dictionary<string, string>
            {
                { "Title", title },
                { "CoverUrl", coverUrl },
                { "SongUrl", songUrl },
                { "Color", color },
                { "ArtistIds", artists },
                { "GenreIds", genres }
            };

            Dictionary<string, IFormFile> files = new Dictionary<string, IFormFile>();
            if (croppedImage != null)
            {
                files.Add("Image", croppedImage);
            }
            if (audioFile != null)
            {
                files.Add("Audio", audioFile);
            }

            // Update song
            bool success = await _apiService.PutMultipartAsync($"songs/{songId}", formFields, files);
            if (!success)
            {
                return BadRequest(new { error = "Error updating song." });
            }

            return Ok(songId);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateArtist(int artistId, string name, string imageUrl, string color, string genres, IFormFile croppedImage)
        {
            // Reset IndexViewModel
            HttpContext.Session.Remove("IndexViewModel");

            // Setup mutipart data
            Dictionary<string, string> formFields = new Dictionary<string, string>
            {
                { "Name", name },
                { "ImageUrl", imageUrl },
                { "Color", color },
                { "GenreIds", genres }
            };

            Dictionary<string, IFormFile> files = new Dictionary<string, IFormFile>();
            if (croppedImage != null)
            {
                files.Add("Image", croppedImage);
            }

            // Update artist
            bool success = await _apiService.PutMultipartAsync($"artists/{artistId}", formFields, files);
            if (!success)
            {
                return BadRequest(new { error = "Error updating artist." });
            }

            return Ok(artistId);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGenre(int genreId, string name, string color)
        {
            // Reset IndexViewModel
            HttpContext.Session.Remove("IndexViewModel");

            Genre genre = new Genre()
            {
                Name = name,
                Color = color,
            };

            // Update genre
            bool success = await _apiService.PutAsync<Genre>($"genres/{genreId}", genre);
            if (!success)
            {
                return BadRequest(new { error = "Error updating genre." });
            }

            return Ok(genre);
        }
    }
}
