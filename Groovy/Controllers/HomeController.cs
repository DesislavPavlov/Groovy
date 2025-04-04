using Groovy.Models;
using Groovy.Services;
using Groovy.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;

namespace Groovy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiService _apiService;

        public HomeController(ILogger<HomeController> logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }


        // Views
        public async Task<IActionResult> Index()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Authentication");
            }
            else
            {
                IndexViewModel viewModel;

                int userId = int.Parse(HttpContext.Session.GetString("UserId"));
                string indexViewModelJson = HttpContext.Session.GetString("IndexViewModel");

                List<Song> favouriteSongs = await GetFavouriteSongs(userId);
                List<int> favouriteSongIds = favouriteSongs.Select(s => s.Id).ToList();

                if (indexViewModelJson == null)
                {
                    User user = await _apiService.GetAsync<User>($"users/{userId}");
                    if (user == null)
                    {
                        return NotFound("Нещо се обърка при намирането на този потребител в базата данни!");
                    }


                    viewModel = new IndexViewModel()
                    {
                        UserId = user.Id,
                        Username = user.Username,
                        AvatarUrl = user.AvatarUrl,
                        Songs = await GetRecommendedSongs(userId),
                        Artists = await GetArtists(),
                        Genres = await GetGenres(),
                        FavouriteSongIds = favouriteSongIds,
                        TopSongs = await GetTopSongs(),
                        ArtistGenreRelations = await GetArtistGenreRelations(),
                        SongArtistRelations = await GetSongArtistRelations(),
                        SongGenreRelations = await GetSongGenreRelations()
                    };

                    string jsonViewModel = JsonConvert.SerializeObject(viewModel);
                    HttpContext.Session.SetString("IndexViewModel", jsonViewModel);
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<IndexViewModel>(indexViewModelJson);
                    viewModel.TopSongs = await GetTopSongs();
                }

                return View(viewModel);
            }
        }

        public async Task<IActionResult> Songs(string searchTerm)
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));
            List<Song> favouriteSongs = await GetFavouriteSongs(userId);

            SongsViewModel viewModel = new SongsViewModel();

            //viewModel.FavouriteSongIds = favouriteSongs.Select(s => s.Id).ToList();
            //viewModel.Artists = await GetArtists();
            //viewModel.Genres = await GetGenres();
            //viewModel.SongArtistRelations = await GetSongArtistRelations();
            //viewModel.SongGenreRelations = await GetSongGenreRelations();

            if (string.IsNullOrEmpty(searchTerm))
            {
                viewModel.Songs = await GetAllSongs();
                viewModel.SearchTerm = "";
            }
            else
            {
                viewModel.Songs = await GetSearchedSongs(searchTerm);
                viewModel.SearchTerm = searchTerm;
            }

            return View(viewModel);
        }

        // Partial views
        [HttpPost]
        public IActionResult LoadView([FromBody] PartialViewRequestDataViewModel request)
        {
            if (request == null || request.IndexViewModel == null)
            {
                return BadRequest("Invalid JSON data received.");
            }

            if (string.IsNullOrEmpty(request.ViewName))
                return BadRequest("Грешка при зареждане на ресурси.");

            ViewBag.Username = HttpContext.Session.GetString("Username");

            return PartialView(request.ViewName, request.IndexViewModel);
        }

        // Get
        private async Task<List<Artist>> GetArtists()
        {
            List<Artist> artists = await _apiService.GetAsync<List<Artist>>("artists");
            return artists;
        }
        private async Task<List<Genre>> GetGenres()
        {
            List<Genre> genres = await _apiService.GetAsync<List<Genre>>("genres");
            return genres;
        }
        private async Task<List<Song>> GetFavouriteSongs(int userId)
        {
            List<Song> songs = await _apiService.GetAsync<List<Song>>($"users/{userId}/favourite/songs");
            return songs;
        }
        private async Task<List<Song>> GetRecommendedSongs(int userId)
        {
            List<Song> songs = await _apiService.GetAsync<List<Song>>($"songs/recommended?userId={userId}");

            Random rng = new Random();
            while (songs.Count > 20)
            {
                int deleteIndex = rng.Next(songs.Count);
                songs.RemoveAt(deleteIndex);
            }

            return songs;
        }
        private async Task<List<Song>> GetAllSongs()
        {
            List<Song> songs = await _apiService.GetAsync<List<Song>>("songs");
            return songs;
        }
        private async Task<List<TrendingSong>> GetTopSongs()
        {
            List<TrendingSong> songs = await _apiService.GetAsync<List<TrendingSong>>("songs/trending");
            return songs;
        }
        private async Task<List<Song>> GetSearchedSongs(string searchTerm)
        {
            List<Song> songs = await _apiService.GetAsync<List<Song>>($"songs/search?searchTerm={searchTerm}");
            return songs;
        }
        private async Task<Dictionary<int, List<int>>> GetArtistGenreRelations()
        {
            Dictionary<int, List<int>> relations = await _apiService.GetAsync<Dictionary<int, List<int>>>("artists/genres");
            return relations;
        }
        private async Task<Dictionary<int, List<int>>> GetSongArtistRelations()
        {
            Dictionary<int, List<int>> relations = await _apiService.GetAsync<Dictionary<int, List<int>>>("songs/artists");
            return relations;
        }
        private async Task<Dictionary<int, List<int>>> GetSongGenreRelations()
        {
            Dictionary<int, List<int>> relations = await _apiService.GetAsync<Dictionary<int, List<int>>>("songs/genres");
            return relations;
        }

        // Update IndexViewModel favourites
        [HttpPost]
        public async Task<ActionResult> UpdateFavouriteSongs()
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));
            string indexViewModelJson = HttpContext.Session.GetString("IndexViewModel");
            IndexViewModel viewModel = JsonConvert.DeserializeObject<IndexViewModel>(indexViewModelJson);

            List<Song> favouriteSongs = await GetFavouriteSongs(userId);
            List<int> favouriteSongIds = favouriteSongs.Select(s => s.Id).ToList();
            viewModel.FavouriteSongIds = favouriteSongIds;

            HttpContext.Session.Remove("IndexViewModel");
            HttpContext.Session.SetString("IndexViewModel", JsonConvert.SerializeObject(viewModel));

            return Ok();
        }



        // IDK
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}