using Groovy.Models;
using Groovy.Services;
using Groovy.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Groovy.Controllers
{
    public class DetailsController : Controller
    {

        private readonly ApiService _apiService;
        public DetailsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> UserDetails(int id)
        {
            User user = await GetUser(id);
            UserDetailsViewModel viewModel = new UserDetailsViewModel()
            {
                Id = id,
                Username = user.Username,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                CreatedAt = user.CreatedAt,
                FavouriteArtists = await GetFavouriteArtists(id),
                FavouriteGenres = await GetFavouriteGenres(id),
                FavouriteSongs = await GetFavouriteSongs(id)
            };

            return View(viewModel);
        }
        public async Task<IActionResult> SongDetails(int id)
        {
            Song song = await GetSong(id);
            if (song == null)
            {
                return BadRequest("Song or song cover / audio not found.");
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            List<Song> favouriteSongs = await GetFavouriteSongs(userId);
            SongDetailsViewModel viewModel = new SongDetailsViewModel()
            {
                UserId = userId,
                CurrentSong = song,
                RelatedSongs = await GetRelatedSongs(id),
                RecommendedSongs = await GetRecommendedSongs(userId, song.Id),
                FavouriteSongIds = favouriteSongs.Select(s => s.Id).ToList()
            };

            SongActivityModel activityModel = new SongActivityModel()
            {
                UserId = userId,
                SongId = id
            };

            await _apiService.PostReturnBoolAsync<SongActivityModel>("songs/click", activityModel);

            return View(viewModel);
        }
        public async Task<IActionResult> ArtistDetails(int id)
        {
            ArtistActivityModel activityModel = new ArtistActivityModel()
            {
                UserId = int.Parse(HttpContext.Session.GetString("UserId")),
                ArtistId = id
            };

            await _apiService.PostReturnBoolAsync<ArtistActivityModel>("genres/click", activityModel);

            return View();
        }
        public async Task<IActionResult> GenreDetails(int id)
        {
            GenreActivityModel activityModel = new GenreActivityModel()
            {
                UserId = int.Parse(HttpContext.Session.GetString("UserId")),
                GenreId = id
            };

            await _apiService.PostReturnBoolAsync<GenreActivityModel>("genres/click", activityModel);

            return View();
        }

        // Song
        private async Task<Song> GetSong(int songId)
        {
            Song song = await _apiService.GetAsync<Song>($"songs/{songId}");
            return song;
        }
        private async Task<List<Song>> GetRecommendedSongs(int userId, int currentSongId)
        {
            List<Song> songs = await _apiService.GetAsync<List<Song>>($"songs/recommended?userId={userId}&currentSongId={currentSongId}");

            Random rng = new Random();
            while (songs.Count > 5)
            {
                int deleteIndex = rng.Next(songs.Count);
                songs.RemoveAt(deleteIndex);
            }

            return songs;
        }
        private async Task<List<Song>> GetRelatedSongs(int songId)
        {
            List<Song> songs = await _apiService.GetAsync<List<Song>>($"songs/{songId}/related");

            Random rng = new Random();
            while (songs.Count > 5)
            {
                int deleteIndex = rng.Next(songs.Count);
                songs.RemoveAt(deleteIndex);
            }
            return songs;
        }

        // User
        private async Task<User> GetUser(int userId)
        {
            User user = await _apiService.GetAsync<User>($"users/{userId}");
            return user;
        }
        private async Task<List<Artist>> GetFavouriteArtists(int userId)
        {
            return await _apiService.GetAsync<List<Artist>>($"users/{userId}/favourite/artists");
        }
        private async Task<List<Genre>> GetFavouriteGenres(int userId)
        {
            return await _apiService.GetAsync<List<Genre>>($"users/{userId}/favourite/genres");
        }
        private async Task<List<Song>> GetFavouriteSongs(int userId)
        {
            return await _apiService.GetAsync<List<Song>>($"users/{userId}/favourite/songs");
        }
    }
}
