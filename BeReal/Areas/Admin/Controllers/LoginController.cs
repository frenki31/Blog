using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data.Repository.Comments;
using BeReal.Data.Repository.Posts;
using BeReal.Data.Repository.Users;
using BeReal.Models;
using BeReal.Utilities;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly IUsersOperations _usersOperations;
        public INotyfService _notification { get; }
        public LoginController (IUsersOperations usersOperations, INotyfService notification)
        {
            _notification = notification;
            _usersOperations = usersOperations;
        }
        [HttpGet("Login")]
        public IActionResult Login(string url)
        {
            if (!HttpContext.User.Identity!.IsAuthenticated)
            {
                var login = new LoginViewModel()
                {
                    ReturnUrl = url
                };
                return View(login);
            }
            return RedirectToAction(nameof(Index), "Post", new { area = "Admin" });
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            if (!ModelState.IsValid) return View(lvm);
            var username = await _usersOperations.getUserByUsername(lvm.Username!);
            if (username == null) return View(lvm);
            var checkPassword = await _usersOperations.checkPasswordForLogin(username, lvm.Password!);
            if (!checkPassword)
            {
                _notification.Error("Password does not match!");
                return View(lvm);
            }
            await _usersOperations.signIn(lvm.Username!, lvm.Password!, lvm.RememberMe, true);
            _notification.Success("Login Successful");
            if (lvm.ReturnUrl == null)
                return RedirectToAction(nameof(Index), "Home", new { area = "" });
            else
                return Redirect(lvm.ReturnUrl);
        }
        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View(new RegisterViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterViewModel rvm)
        {
            if (!ModelState.IsValid)
            {
                _notification.Warning("Please fill in all the fields");
                return View(rvm);
            }
            var checkEmail = await _usersOperations.getUserByEmail(rvm.Email!);
            if (checkEmail != null)
            {
                _notification.Error("This email is already registered.");
                return View(rvm);
            }
            var checkUsername = await _usersOperations.getUserByUsername(rvm.Username!);
            if (checkUsername != null)
            {
                _notification.Error("This username is not available.");
                return View(rvm);
            }
            if (rvm.Password != rvm.ConfirmPassword)
            {
                _notification.Error("Passwords do not match");
                return View(rvm);
            }
            var user = new BR_ApplicationUser()
            {
                FirstName = rvm.FirstName,
                LastName = rvm.LastName,
                UserName = rvm.Username,
                Email = rvm.Email,
            };
            var checkUser = await _usersOperations.createUser(user, rvm.Password!);
            if (checkUser.Succeeded)
            {
                await _usersOperations.giveRoleToUser(user, Roles.User);
                _notification.Success("User registered successfully!");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View(rvm);
        }
    }
}
