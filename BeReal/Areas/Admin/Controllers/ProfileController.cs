using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data.Repository.Users;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProfileController : Controller
    {
        private readonly IUsersOperations _usersOperations;
        public INotyfService _notification { get; }
        public ProfileController(IUsersOperations usersOperations,INotyfService notification)
        {
            _usersOperations = usersOperations;
            _notification = notification;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile(string id)
        {
            var user = await _usersOperations.getUserById(id);
            if (user == null) return View();
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
            var oldUser = await _usersOperations.getUserByUsername(rvm.Username!);
            if (!ModelState.IsValid)
            {
                _notification.Warning("Please fill in all the fields");
                return View(rvm);
            }
            if (oldUser!.Email != rvm.Email)
            {
                var checkEmail = await _usersOperations.getUserByEmail(rvm.Email!);
                if (checkEmail != null)
                {
                    _notification.Error("This email is already registered.");
                    return View(rvm);
                }
            }
            if (oldUser.UserName != rvm.Username)
            {
                var checkUsername = await _usersOperations.getUserByUsername(rvm.Username!);
                if (checkUsername != null)
                {
                    _notification.Error("This username is not available.");
                    return View(rvm);
                }
            }
            var confirmPassword = await _usersOperations.checkPasswordForLogin(oldUser!, rvm.Password!);
            if (!confirmPassword)
            {
                _notification.Error("Password is not correct");
                return View(rvm);
            }
            oldUser.FirstName = rvm.FirstName;
            oldUser.LastName = rvm.LastName;
            oldUser.UserName = rvm.Username;
            oldUser.Email = rvm.Email;
            var checkUser = await _usersOperations.updateUser(oldUser);
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
            var thisUser = await _usersOperations.getUserById(id);
            if (thisUser == null) return View();
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
            if (!ModelState.IsValid) return View(rpvm);
            var thisUser = await _usersOperations.getUserById(rpvm.Id!);
            if (thisUser == null) return View(rpvm);
            var token = await _usersOperations.generateToken(thisUser);
            var reset = await _usersOperations.resetPassword(thisUser, token, rpvm.NewPassword!);
            if (reset.Succeeded)
            {
                _notification.Success("Password changed successfully");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View(rpvm);
        }
    }
}
