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
            var user = await _usersOperations.GetUserById(id);
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
        public async Task<IActionResult> EditProfile(ProfileViewModel rvm, string id)
        {
            var oldUser = await _usersOperations.GetUserByUsername(rvm.Username!);
            if (!ModelState.IsValid)
            {
                _notification.Warning("Please fill in all the fields");
                return View(rvm);
            }
            if (oldUser!.Email != rvm.Email)
            {
                var checkEmail = await _usersOperations.GetUserByEmail(rvm.Email!);
                if (checkEmail != null)
                {
                    _notification.Error("This email is already registered.");
                    return View(rvm);
                }
            }
            var confirmPassword = await _usersOperations.CheckPasswordForLogin(oldUser!, rvm.Password!);
            if (!confirmPassword)
            {
                _notification.Error("Password is not correct");
                return View(rvm);
            }
            oldUser.FirstName = rvm.FirstName;
            oldUser.LastName = rvm.LastName;
            oldUser.Email = rvm.Email;
            var checkUser = await _usersOperations.UpdateUser(oldUser);
            if (checkUser.Succeeded)
            {
                _notification.Success("User profile updated successfully!");
                return RedirectToAction("Profile", "Home", new { area = "", id = id});
            }
            return View(rvm);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var thisUser = await _usersOperations.GetUserById(id);
            if (thisUser == null) return View();
            var resetVm = new ResetPasswordViewModel() { Id = thisUser.Id, Username = thisUser.UserName, };
            return View(resetVm);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel rpvm, string id)
        {
            if (!ModelState.IsValid) return View(rpvm);
            var validatePasswordReset = await _usersOperations.ValidateResetPassword(rpvm, _usersOperations);
            if (validatePasswordReset != null)
            {
                _notification.Error(validatePasswordReset);
                return View(rpvm);
            }
            _notification.Success("Password changed successfully");
            return RedirectToAction("Profile", "Home", new { area = "", id= id});
        }
    }
}