using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data.Repository.Email;
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
        private readonly IEmailService _emailService;
        public INotyfService _notification { get; }
        public LoginController (IUsersOperations usersOperations, INotyfService notification, IEmailService emailService)
        {
            _notification = notification;
            _usersOperations = usersOperations;
            _emailService = emailService;
        }
        [HttpGet("Login")]
        public IActionResult Index(string url)
        {
            if (!HttpContext.User.Identity!.IsAuthenticated)
            {
                var login = new LoginViewModel() { ReturnUrl = url };
                return View(login);
            }
            return RedirectToAction(nameof(Index), "Post", new { area = "Admin" });
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Index(LoginViewModel lvm)
        {
            if (!ModelState.IsValid) return View(lvm);
            var username = await _usersOperations.GetUserByUsername(lvm.Username!);
            if (username == null) return View(lvm);
            var checkPassword = await _usersOperations.CheckPasswordForLogin(username, lvm.Password!);
            if (!checkPassword)
            {
                _notification.Error("Password does not match!");
                return View(lvm);
            }
            if (!username.EmailConfirmed)
            {
                _notification.Error("Email is not confirmed");
                return View(lvm);
            }
            await _usersOperations.SignIn(lvm.Username!, lvm.Password!, lvm.RememberMe, true);
            _notification.Success("Login Successful");
            if (lvm.ReturnUrl == null)
                return RedirectToAction(nameof(Index), "Home", new { area = "" });
            else
                return Redirect(lvm.ReturnUrl);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel rvm)
        {
            if (!ModelState.IsValid)
            {
                _notification.Warning("Please fill in all the fields");
                return View(rvm);
            }
            var validateUser = await _usersOperations.ValidateUser(rvm,_usersOperations);
            if (validateUser != null)
            {
                _notification.Error(validateUser);
                return View(rvm);
            }
            var user = new BR_ApplicationUser()
            {
                FirstName = rvm.FirstName,
                LastName = rvm.LastName,
                UserName = rvm.Username,
                Email = rvm.Email,
            };
            var checkUser = await _usersOperations.CreateUser(user, rvm.Password!);
            if (checkUser.Succeeded)
            {
                await _usersOperations.GiveRoleToUser(user, Roles.User);
                string token = await _usersOperations.GenerateEmailToken(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Login", new { area = "Admin", userId = user.Id, token }, Request.Scheme);
                await _emailService.SendEmailAsync(rvm.Email!, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
                _notification.Success("User registered. Please check email for confirmation!");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View(rvm);
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _usersOperations.GetUserById(userId);
            var result = await _usersOperations.ConfirmEmail(user!,token);
            if (result.Succeeded) return View("EmailConfirmed");
            return View();
        }
    }
}