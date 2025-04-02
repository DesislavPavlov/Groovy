using Groovy.Models;
using Groovy.Services;
using Microsoft.AspNetCore.Mvc;

namespace Groovy.Controllers
{
    public class UploadController : Controller
    {
        private readonly ApiService _apiService;
        public UploadController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadSong(string artists, string genres, IFormFile croppedImage, IFormFile audioFile, string title, string color)
        {
            // Reset IndexViewModel
            HttpContext.Session.Remove("IndexViewModel");

            // Setup mutipart data
            Dictionary<string, string> formFields = new Dictionary<string, string>
            {
                { "Title", title },
                { "Color", color },
                { "ArtistIds", artists },
                { "GenreIds", genres }
            };

            Dictionary<string, IFormFile> files = new Dictionary<string, IFormFile>()
            {
                { "Cover", croppedImage },
                { "Audio", audioFile }
            };

            // Send add song request
            bool success = await _apiService.PostMultipartAsync("songs", formFields, files);
            if (!success)
            {
                return BadRequest(new { error = "Error with adding song." });
            }

            return Ok(new { message = "Successfully created song!" });
        }

        [HttpPost]
        public async Task<IActionResult> UploadArtist(string genres, IFormFile croppedImage, string name, string color)
        {
            // Reset IndexViewModel
            HttpContext.Session.Remove("IndexViewModel");

            // Setup mutipart data
            Dictionary<string, string> formFields = new Dictionary<string, string>
            {
                { "Name", name },
                { "Color", color },
                { "GenreIds", genres }
            };

            Dictionary<string, IFormFile> files = new Dictionary<string, IFormFile>()
            {
                { "Image", croppedImage }
            };

            // Send add artist request
            bool success = await _apiService.PostMultipartAsync("artists", formFields, files);
            if (!success)
            {
                return BadRequest(new { error = "Error with adding artist." });
            }

            return Ok(new { message = "Successfully created artist!" });
        }

        [HttpPost]
        public async Task<IActionResult> UploadGenre(string name, string color)
        {
            // Reset IndexViewModel
            HttpContext.Session.Remove("IndexViewModel");

            Genre genre = new Genre()
            {
                Name = name,
                Color = color,
            };

            // Add genre
            Genre createdGenre = await _apiService.PostAsync<Genre>("genres", genre);
            if (genre.Id <= 0)
            {
                return BadRequest(new { error = "Error adding genre." });
            }

            return Ok(genre);
        }
    }
}
