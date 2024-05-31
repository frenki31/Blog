using Microsoft.AspNetCore.Mvc;
using BeReal.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using BeReal.Models;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotyfService _notification;

        public UserController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, 
            INotyfService notification)
        {
            _notification = notification;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            if (!ModelState.IsValid)
            {
                return View(lvm);
            }
            var username = _userManager.Users.FirstOrDefault(x => x.UserName == lvm.Username);
            if (username == null)
            {
                _notification.Error("Username does not exist!");
                return View(lvm);
            }
            var checkPassword = await _userManager.CheckPasswordAsync(username, lvm.Password);
            if (!checkPassword)
            {
                _notification.Error("Password does not match!");
                return View(lvm);
            }
            await _signInManager.PasswordSignInAsync(lvm.Username, lvm.Password, lvm.RememberMe, true);
            _notification.Success("Login Successful");
            return RedirectToAction(nameof(Index), "User", new {area = "Admin"});
        }
    }
}
