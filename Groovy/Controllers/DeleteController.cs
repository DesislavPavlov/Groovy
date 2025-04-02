using Groovy.Services;
using Microsoft.AspNetCore.Mvc;

namespace Groovy.Controllers
{
    public class DeleteController : Controller
    {
        private readonly ApiService _apiService;
        public DeleteController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSong(int songId)
        {
            // Validate id
            if (songId <= 0)
            {
                return BadRequest(new { error = "Invalid song ID" });
            }

            // Reset IndexViewModel
            HttpContext.Session.Remove("IndexViewModel");

            // Delete
            if (!await _apiService.DeleteAsync($"songs/{songId}"))
                return BadRequest(new { error = "Could not delete song." });

            return Ok(new { message = "Successfully deleted song!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteArtist(int artistId)
        {
            // Validate id
            if (artistId <= 0)
            {
                return BadRequest(new { error = "Invalid artist ID" });
            }

            // Reset IndexViewModel
            HttpContext.Session.Remove("IndexViewModel");

            // Delete
            if (!await _apiService.DeleteAsync($"artists/{artistId}"))
                return BadRequest(new { error = "Could not delete artist." });

            return Ok(new { message = "Successfully deleted artist!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGenre(int genreId)
        {
            // Validate id
            if (genreId <= 0)
            {
                return BadRequest(new { error = "Invalid genre ID" });
            }

            // Reset IndexViewModel
            HttpContext.Session.Remove("IndexViewModel");

            // Delete
            if (!await _apiService.DeleteAsync($"genres/{genreId}"))
                return BadRequest(new { error = "Could not delete genre." });

            return Ok(new { message = "Successfully deleted genre!" });
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            // Authorize
            int currentUserId = int.Parse(HttpContext.Session.GetString("UserId"));
            if (currentUserId != id)
            {
                return Content("НЕ СИ ОТОРИЗИРАН ДА ТРИЕШ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }

            // Delete
            bool success = await _apiService.DeleteAsync($"users/{id}");
            if (!success)
            {
                return BadRequest($"Invalid user id {id}");
            }

            return RedirectToAction("Login", "Authentication");
        }
    }
}
