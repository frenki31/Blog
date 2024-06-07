using Microsoft.AspNetCore.Mvc;
using BeReal.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using BeReal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BeReal.Utilities;
using System.Runtime.Intrinsics.Arm;
using Microsoft.AspNetCore.Mvc.Rendering;
using BeReal.Data;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }

        public UserController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, 
            INotyfService notification)
        {
            _notification = notification;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModel = users.Select(x => new UserViewModel()
            {
                Id = x.Id,
                Username = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
            }).ToList();

            foreach(var user in userViewModel)
            {
                var oneUser = await _userManager.FindByIdAsync(user.Id!);
                var role = await _userManager.GetRolesAsync(oneUser!);
                user.Role = role.FirstOrDefault();
                var postCount = _context.Posts.Where(x => x.User!.Id == oneUser!.Id).Count();
                user.NumberPosts = postCount;
            }
            return View(userViewModel);
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity!.IsAuthenticated)
            {
                return View(new LoginViewModel());
            }
            return RedirectToAction(nameof(Index), "Post", new { area = "Admin" });
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
            var checkPassword = await _userManager.CheckPasswordAsync(username, lvm.Password!);
            if (!checkPassword)
            {
                _notification.Error("Password does not match!");
                return View(lvm);
            }
            await _signInManager.PasswordSignInAsync(lvm.Username!, lvm.Password!, lvm.RememberMe, true);
            _notification.Success("Login Successful");
            return RedirectToAction(nameof(Index), "Post", new {area = "Admin"});
        }
        [HttpPost]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            _notification.Success("Logged Out");
            return RedirectToAction("Index", "Home", new {area = ""});
        } 
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel rvm)
        {
            if (!ModelState.IsValid) { return View(rvm); }
            var checkEmail = await _userManager.FindByEmailAsync(rvm.Email!);
            if (checkEmail != null)
            {
                _notification.Error("This email is already registered.");
                return View(rvm);
            }
            var checkUsername = await _userManager.FindByNameAsync(rvm.Username!);
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
            var user = new ApplicationUser()
            {
                FirstName = rvm.FirstName,
                LastName = rvm.LastName,
                UserName = rvm.Username,
                Email = rvm.Email,
            };
            var checkUser = await _userManager.CreateAsync(user, rvm.Password!);
            if (checkUser.Succeeded)
            {
                if (rvm.isAdmin)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Admin);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, Roles.User);
                }
                _notification.Success("User registered successfully!");
                return RedirectToAction("Index", "Post", new {area = "Admin"});
            }
            return View(rvm);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var thisUser = await _userManager.FindByIdAsync(id);
            if (thisUser == null)
            {
                _notification.Error("User does not exist");
                return View();
            }
            var resetVm = new ResetPasswordViewModel()
            {
                Id = thisUser.Id,
                Username = thisUser.UserName,
            };
            return View(resetVm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel rpvm)
        {
            if (!ModelState.IsValid) { return View(rpvm); }
            var thisUser = await _userManager.FindByIdAsync(rpvm.Id!);
            if (thisUser == null)
            {
                _notification.Error("User does not exist");
                return View(rpvm);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(thisUser);
            var reset = await _userManager.ResetPasswordAsync(thisUser, token, rpvm.NewPassword!);
            if (reset.Succeeded)
            {
                _notification.Success("Password changed successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(rpvm);
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
            var checkEmail = await _userManager.FindByEmailAsync(rvm.Email!);
            if (checkEmail != null)
            {
                _notification.Error("This email is already registered.");
                return View(rvm);
            }
            var checkUsername = await _userManager.FindByNameAsync(rvm.Username!);
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
            var user = new ApplicationUser()
            {
                FirstName = rvm.FirstName,
                LastName = rvm.LastName,
                UserName = rvm.Username,
                Email = rvm.Email,
            };
            var checkUser = await _userManager.CreateAsync(user, rvm.Password!);
            if (checkUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.User);
                _notification.Success("User registered successfully!");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View(rvm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string id)
        {
            var thisUser = await _userManager.FindByIdAsync(id);
            var userRole = await _userManager.GetRolesAsync(thisUser!);
            if (thisUser == null)
            {
                _notification.Error("User does not exist");
                return RedirectToAction("Index", "User", new {area = "Admin"});
            }
            if (userRole[0] == Roles.Admin)
            {
                await _userManager.RemoveFromRoleAsync(thisUser, Roles.Admin);
                await _userManager.AddToRoleAsync(thisUser, Roles.User);
            }
            else if (userRole[0] == Roles.User)
            {
                await _userManager.RemoveFromRoleAsync(thisUser, Roles.User);
                await _userManager.AddToRoleAsync(thisUser, Roles.Admin);
            }
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _notification.Error("User does not exist");
                return View();
            }
            var userVM = new ProfileViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
            };
            return View(userVM);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(ProfileViewModel rvm)
        {
            var oldUser = await _userManager.FindByNameAsync(rvm.Username!);
            if (!ModelState.IsValid)
            {
                _notification.Warning("Please fill in all the fields");
                return View(rvm);
            }
            if (oldUser!.Email != rvm.Email)
            {
                var checkEmail = await _userManager.FindByEmailAsync(rvm.Email!);
                if (checkEmail != null)
                {
                    _notification.Error("This email is already registered.");
                    return View(rvm);
                }
            }
            if (oldUser.UserName != rvm.Username)
            {
                var checkUsername = await _userManager.FindByNameAsync(rvm.Username!);
                if (checkUsername != null)
                {
                    _notification.Error("This username is not available.");
                    return View(rvm);
                }
            }
            var confirmPassword = await _userManager.CheckPasswordAsync(oldUser!, rvm.Password!);
            if (!confirmPassword)
            {
                _notification.Error("Password is not correct");
                return View(rvm);
            }
            oldUser.FirstName = rvm.FirstName;
            oldUser.LastName = rvm.LastName;
            oldUser.UserName = rvm.Username;
            oldUser.Email = rvm.Email;
            var checkUser = await _userManager.UpdateAsync(oldUser);   
            if (checkUser.Succeeded)
            {
                _notification.Success("User profile updated successfully!");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View(rvm);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ResetUserPassword(string id)
        {
            var thisUser = await _userManager.FindByIdAsync(id);
            if (thisUser == null)
            {
                _notification.Error("User does not exist");
                return View();
            }
            var resetVm = new ResetPasswordViewModel()
            {
                Id = thisUser.Id,
                Username = thisUser.UserName,
            };
            return View(resetVm);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ResetUserPassword(ResetPasswordViewModel rpvm)
        {
            if (!ModelState.IsValid) { return View(rpvm); }
            var thisUser = await _userManager.FindByIdAsync(rpvm.Id!);
            if (thisUser == null)
            {
                _notification.Error("User does not exist");
                return View(rpvm);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(thisUser);
            var reset = await _userManager.ResetPasswordAsync(thisUser, token, rpvm.NewPassword!);
            if (reset.Succeeded)
            {
                _notification.Success("Password changed successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(rpvm);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user!);
            return RedirectToAction(nameof(Index),"Home", new { area = ""});
        }
        [HttpGet("AccessDenied")]
        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
