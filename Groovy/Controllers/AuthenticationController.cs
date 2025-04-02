using Groovy.Models;
using Groovy.Services;
using Groovy.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Groovy.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly PasswordService _passwordService;
        private readonly ApiService _apiService;

        public AuthenticationController(PasswordService passwordService, ApiService apiService)
        {
            _passwordService = passwordService;
            _apiService = apiService;

        }


        public IActionResult Login()
        {
            HttpContext.Session.Remove("IndexViewModel");

            if (TempData["error"] is string error)
            {
                ViewBag.Error = error;
            }

            return View();
        }

        public async Task<IActionResult> Register()
        {
            RegisterViewModel registerViewModel = new RegisterViewModel();
            registerViewModel.AvatarBlobUrls = await GetAvatarUrisAsync();

            HttpContext.Session.Remove("IndexViewModel");

            if (TempData["error"] is string error)
            {
                ViewBag.Error = error;
            }

            return View(registerViewModel);
        }


        // Handling login / register
        [HttpPost]
        public async Task<IActionResult> HandleUserRegister(RegisterViewModel viewModel)
        {
            ;
            if (viewModel.Password != viewModel.ConfirmPassword)
            {
                TempData["error"] = "Паролите не съответстват!";
                return RedirectToAction("Register");
            }

            if (string.IsNullOrEmpty(viewModel.AvatarUrl))
            {
                viewModel.AvatarUrl = "https://localhost:7021/uploads/avatar_normal.webp";
            }

            User user = new User()
            {
                Username = viewModel.Username,
                Email = viewModel.Email,
                Password = viewModel.Password,
                AvatarUrl = viewModel.AvatarUrl,
            };

            var hashedPassword = _passwordService.HashPassword(user.Password);
            user.Password_Hash = hashedPassword;

            User createdUser = await _apiService.PostAsync<User>("users", user);

            if (createdUser == null)
            {
                TempData["error"] = "Името или електронната поща вече са заети!";
                return RedirectToAction("Register");
            }

            HttpContext.Session.SetString("UserId", createdUser.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("AvatarUrl", user.AvatarUrl);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> HandleUserLogin(User user)
        {
            bool loginValid = false;

            User dbUser = await _apiService.GetAsync<User>($"users/username/{user.Username}");
            if (dbUser != null)
            {
                bool passwordValid = _passwordService.VerifyPassword(user.Password, dbUser.Password_Hash);

                if (passwordValid)
                {
                    loginValid = true;

                    HttpContext.Session.SetString("UserId", dbUser.Id.ToString());
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("AvatarUrl", dbUser.AvatarUrl);
                }
            }

            if (loginValid)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = "Невалидно име или парола!";
                return RedirectToAction("Login");
            }
        }

        public IActionResult HandleGuestLogin()
        {
            User guest = new User()
            {
                Username = "Гост",
                Email = "guest@gmail.com",
                Password = "Tiger12345"
            };

            return RedirectToAction("HandleUserLogin", guest);
        }


        private async Task<Dictionary<string, string>> GetAvatarUrisAsync()
        {
            string[] uris = await _apiService.GetAsync<string[]>("files/avatars");
            Dictionary<string, string> avatarUrls = new Dictionary<string, string>();
            avatarUrls.Add("Guitar avatar", uris[0]);
            avatarUrls.Add("Normal avatar", uris[1]);
            avatarUrls.Add("Rock avatar", uris[2]);
            avatarUrls.Add("Singer avatar", uris[3]);

            return avatarUrls;
        }
    }
}
