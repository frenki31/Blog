using Microsoft.AspNetCore.Mvc;
using BeReal.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Models;
using Microsoft.AspNetCore.Authorization;
using BeReal.Utilities;
using BeReal.Data.Repository.Users;
using BeReal.Data.Repository.Posts;
using BeReal.Data.Repository.Comments;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUsersOperations _usersOperations;
        private readonly IPostsOperations _postsOperations;
        private readonly ICommentsOperations _commentsOperations;
        public INotyfService _notification { get; }

        public UserController(INotyfService notification, IUsersOperations usersOperations, IPostsOperations postsOperations, ICommentsOperations commentsOperations)
        {
            _postsOperations = postsOperations;
            _commentsOperations = commentsOperations;
            _usersOperations = usersOperations;
            _notification = notification;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _usersOperations.getUsers();
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
                var oneUser = await _usersOperations.getUserById(user.Id!);
                var role = await _usersOperations.getUserRole(oneUser!);
                user.Role = role[0];
                user.NumberPosts = _postsOperations.getPostCount(oneUser!.Id);
                user.NumberComments = _commentsOperations.getCommentCount(oneUser.Id);
            }
            return View(userViewModel);
        }
        [HttpPost]
        public IActionResult Logout()
        {
            _usersOperations.logout();
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
            if (!ModelState.IsValid) return View(rvm);
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
                if (rvm.isAdmin)
                    await _usersOperations.giveRoleToUser(user, Roles.Admin);
                else
                    await _usersOperations.giveRoleToUser(user, Roles.User);
                _notification.Success("User registered successfully!");
                return RedirectToAction("Index", "Post", new {area = "Admin"});
            }
            return View(rvm);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var thisUser = await _usersOperations.getUserById(id);
            if (thisUser == null) return View();
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
            if (!ModelState.IsValid) return View(rpvm); 
            var thisUser = await _usersOperations.getUserById(rpvm.Id!);
            if (thisUser == null) return View(rpvm);
            var token = await _usersOperations.generateToken(thisUser);
            var reset = await _usersOperations.resetPassword(thisUser, token, rpvm.NewPassword!);
            if (reset.Succeeded)
            {
                _notification.Success("Password changed successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(rpvm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string id)
        {
            var thisUser = await _usersOperations.getUserById(id);
            var userRole = await _usersOperations.getUserRole(thisUser!);
            if (thisUser == null) return RedirectToAction("Index", "User", new {area = "Admin"});
            string removeRole = userRole[0] == Roles.Admin ? Roles.Admin : Roles.User;
            string assignRole = removeRole == Roles.Admin ? Roles.User : Roles.Admin;
            await _usersOperations.removeRoleFromUser(thisUser, removeRole);
            await _usersOperations.giveRoleToUser(thisUser, assignRole);
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }
        [HttpGet("AccessDenied")]
        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}