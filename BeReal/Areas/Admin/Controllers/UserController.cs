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
            var users = await _usersOperations.GetUsers();
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
                var oneUser = await _usersOperations.GetUserById(user.Id!);
                var role = await _usersOperations.GetUserRole(oneUser!);
                user.Role = role[0];
                user.NumberPosts = _postsOperations.GetPostCount(oneUser!.Id);
                user.NumberComments = _commentsOperations.GetCommentCount(oneUser.Id);
            }
            return View(userViewModel);
        }
        [HttpPost]
        public IActionResult Logout()
        {
            _usersOperations.Logout();
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
            var validationResult = await _usersOperations.ValidateUser(rvm, _usersOperations);
            if (validationResult != null)
            {
                _notification.Error(validationResult);
                return View(rvm);
            }
            var user = new BR_ApplicationUser()
            {
                FirstName = rvm.FirstName,
                LastName = rvm.LastName,
                UserName = rvm.Username,
                Email = rvm.Email,
                EmailConfirmed = true,
            };
            var checkUser = await _usersOperations.CreateUser(user, rvm.Password!);
            if (checkUser.Succeeded)
            {
                string role = rvm.isAdmin ? Roles.Admin : Roles.User;
                await _usersOperations.GiveRoleToUser(user, role);
                _notification.Success("User registered successfully!");
                return RedirectToAction("Index", "Post", new {area = "Admin"});
            }
            return View(rvm);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var thisUser = await _usersOperations.GetUserById(id);
            if (thisUser == null) return View();
            var resetVm = new ResetPasswordViewModel() { Id = thisUser.Id, Username = thisUser.UserName };
            return View(resetVm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel rpvm)
        {
            if (!ModelState.IsValid) return View(rpvm);
            var validatePasswordReset = await _usersOperations.ValidateResetPassword(rpvm, _usersOperations);
            if (validatePasswordReset != null) {
                _notification.Error(validatePasswordReset);
                return View(rpvm);
            }
            _notification.Success("Password changed successfully");
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string id)
        {
            var thisUser = await _usersOperations.GetUserById(id);
            var userRole = await _usersOperations.GetUserRole(thisUser!);
            if (thisUser == null) return RedirectToAction("Index", "User", new {area = "Admin"});
            string removeRole = userRole[0] == Roles.Admin ? Roles.Admin : Roles.User;
            string assignRole = removeRole == Roles.Admin ? Roles.User : Roles.Admin;
            await _usersOperations.RemoveRoleFromUser(thisUser, removeRole);
            await _usersOperations.GiveRoleToUser(thisUser, assignRole);
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