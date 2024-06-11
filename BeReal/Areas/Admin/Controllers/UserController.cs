using Microsoft.AspNetCore.Mvc;
using BeReal.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Models;
using Microsoft.AspNetCore.Authorization;
using BeReal.Utilities;
using BeReal.Data.Repository;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IRepository _repo;
        public INotyfService _notification { get; }

        public UserController(INotyfService notification, IRepository repo)
        {
            _repo = repo;
            _notification = notification;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _repo.getUsers();
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
                var oneUser = await _repo.getUserById(user.Id!);
                var role = await _repo.getUserRole(oneUser!);
                user.Role = role[0];
                user.NumberPosts = _repo.getPostCount(oneUser!.Id);
                user.NumberComments = _repo.getCommentCount(oneUser.Id);
            }
            return View(userViewModel);
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
            if (!ModelState.IsValid)
            {
                return View(lvm);
            }
            var username = await _repo.getUserByUsername(lvm.Username!);
            if (username == null)
            {
                _notification.Error("Username does not exist!");
                return View(lvm);
            }
            var checkPassword = await _repo.checkPasswordForLogin(username, lvm.Password!);
            if (!checkPassword)
            {
                _notification.Error("Password does not match!");
                return View(lvm);
            }
            await _repo.signIn(lvm.Username!, lvm.Password!, lvm.RememberMe, true);
            _notification.Success("Login Successful");
            if (lvm.ReturnUrl == null)
            {
                return RedirectToAction(nameof(Index), "Home", new {area = ""});
            }else
            {
                return Redirect(lvm.ReturnUrl);
            }
        }
        [HttpPost]
        public IActionResult Logout()
        {
            _repo.logout();
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
            var checkEmail = await _repo.getUserByEmail(rvm.Email!);
            if (checkEmail != null)
            {
                _notification.Error("This email is already registered.");
                return View(rvm);
            }
            var checkUsername = await _repo.getUserByUsername(rvm.Username!);
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
            var checkUser = await _repo.createUser(user, rvm.Password!);
            if (checkUser.Succeeded)
            {
                if (rvm.isAdmin)
                {
                    await _repo.giveRoleToUser(user, Roles.Admin);
                }
                else
                {
                    await _repo.giveRoleToUser(user, Roles.User);
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
            var thisUser = await _repo.getUserById(id);
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
            var thisUser = await _repo.getUserById(rpvm.Id!);
            if (thisUser == null)
            {
                _notification.Error("User does not exist");
                return View(rpvm);
            }
            var token = await _repo.generateToken(thisUser);
            var reset = await _repo.resetPassword(thisUser, token, rpvm.NewPassword!);
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
            var checkEmail = await _repo.getUserByEmail(rvm.Email!);
            if (checkEmail != null)
            {
                _notification.Error("This email is already registered.");
                return View(rvm);
            }
            var checkUsername = await _repo.getUserByUsername(rvm.Username!);
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
            var checkUser = await _repo.createUser(user, rvm.Password!);
            if (checkUser.Succeeded)
            {
                await _repo.giveRoleToUser(user, Roles.User);
                _notification.Success("User registered successfully!");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View(rvm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string id)
        {
            var thisUser = await _repo.getUserById(id);
            var userRole = await _repo.getUserRole(thisUser!);
            if (thisUser == null)
            {
                _notification.Error("User does not exist");
                return RedirectToAction("Index", "User", new {area = "Admin"});
            }
            if (userRole[0] == Roles.Admin)
            {
                await _repo.removeRoleFromUser(thisUser, Roles.Admin);
                await _repo.giveRoleToUser(thisUser, Roles.User);
            }
            else if (userRole[0] == Roles.User)
            {
                await _repo.removeRoleFromUser(thisUser, Roles.User);
                await _repo.giveRoleToUser(thisUser, Roles.Admin);
            }
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile(string id)
        {
            var user = await _repo.getUserById(id);
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
            var oldUser = await _repo.getUserByUsername(rvm.Username!);
            if (!ModelState.IsValid)
            {
                _notification.Warning("Please fill in all the fields");
                return View(rvm);
            }
            if (oldUser!.Email != rvm.Email)
            {
                var checkEmail = await _repo.getUserByEmail(rvm.Email!);
                if (checkEmail != null)
                {
                    _notification.Error("This email is already registered.");
                    return View(rvm);
                }
            }
            if (oldUser.UserName != rvm.Username)
            {
                var checkUsername = await _repo.getUserByUsername(rvm.Username!);
                if (checkUsername != null)
                {
                    _notification.Error("This username is not available.");
                    return View(rvm);
                }
            }
            var confirmPassword = await _repo.checkPasswordForLogin(oldUser!, rvm.Password!);
            if (!confirmPassword)
            {
                _notification.Error("Password is not correct");
                return View(rvm);
            }
            oldUser.FirstName = rvm.FirstName;
            oldUser.LastName = rvm.LastName;
            oldUser.UserName = rvm.Username;
            oldUser.Email = rvm.Email;
            var checkUser = await _repo.updateUser(oldUser);   
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
            var thisUser = await _repo.getUserById(id);
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
            var thisUser = await _repo.getUserById(rpvm.Id!);
            if (thisUser == null)
            {
                _notification.Error("User does not exist");
                return View(rpvm);
            }
            var token = await _repo.generateToken(thisUser);
            var reset = await _repo.resetPassword(thisUser, token, rpvm.NewPassword!);
            if (reset.Succeeded)
            {
                _notification.Success("Password changed successfully");
                return RedirectToAction("Index", "Post", new { area = "Admin"});
            }
            return View(rpvm);
        }

        [HttpGet("AccessDenied")]
        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
