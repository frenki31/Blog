using BeReal.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using BeReal.ViewModels;
using NuGet.Common;

namespace BeReal.Data.Repository.Users
{
    public class UsersOperations : IUsersOperations
    {
        private readonly UserManager<BR_ApplicationUser> _userManager;
        private readonly SignInManager<BR_ApplicationUser> _signInManager;
        public UsersOperations(UserManager<BR_ApplicationUser> userManager, SignInManager<BR_ApplicationUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        //Users
        public async Task<string> GenerateEmailToken(BR_ApplicationUser user) => await _userManager.GenerateEmailConfirmationTokenAsync(user); 
        public async Task<IdentityResult> ConfirmEmail(BR_ApplicationUser user, string token) => await _userManager.ConfirmEmailAsync(user, token); 
        public async Task<BR_ApplicationUser?> GetUserById(string id) => await _userManager.FindByIdAsync(id);
        public async Task<IList<string>> GetUserRole(BR_ApplicationUser? user) => await _userManager.GetRolesAsync(user!);
        public async Task<BR_ApplicationUser?> GetCommentUser(ClaimsPrincipal User) => await _userManager.GetUserAsync(User);
        public async Task<BR_ApplicationUser?> GetUserByUsername(string username) => await _userManager.FindByNameAsync(username);
        public async Task<BR_ApplicationUser?> GetUserByEmail(string email) => await _userManager.FindByEmailAsync(email);
        public async Task<IdentityResult> CreateUser(BR_ApplicationUser user, string password) => await _userManager.CreateAsync(user, password);
        public async Task<IdentityResult> GiveRoleToUser(BR_ApplicationUser user, string role) => await _userManager.AddToRoleAsync(user, role);
        public async Task<IdentityResult> RemoveRoleFromUser(BR_ApplicationUser user, string role) => await _userManager.RemoveFromRoleAsync(user, role);
        public async Task<BR_ApplicationUser?> GetLoggedUser(ClaimsPrincipal User) => await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
        public async Task<List<BR_ApplicationUser>> GetUsers() => await _userManager.Users.Where(x => x.EmailConfirmed).ToListAsync();
        public async Task<bool> CheckPasswordForLogin(BR_ApplicationUser user, string password) => await _userManager.CheckPasswordAsync(user, password!);
        public async Task<string> GenerateToken(BR_ApplicationUser user) => await _userManager.GeneratePasswordResetTokenAsync(user);
        public async Task<IdentityResult> ResetPassword(BR_ApplicationUser user, string token, string password) => await _userManager.ResetPasswordAsync(user, token, password);
        public async Task<IdentityResult> UpdateUser(BR_ApplicationUser user) => await _userManager.UpdateAsync(user);
        public async Task<SignInResult> SignIn(string username, string password, bool remember, bool trueOrFalse) => await _signInManager.PasswordSignInAsync(username, password, remember, trueOrFalse);
        public async Task Logout() => await _signInManager.SignOutAsync();
        public async Task<string> ValidateUser(RegisterViewModel rvm, IUsersOperations _usersOperations)
        {
            var checkEmail = await _usersOperations.GetUserByEmail(rvm.Email!);
            if (checkEmail != null) return "This email is already registered.";
            var checkUsername = await _usersOperations.GetUserByUsername(rvm.Username!);
            if (checkUsername != null) return "This username is not available.";
            if (rvm.Password != rvm.ConfirmPassword) return "Passwords do not match";
            return null!;
        }
        public async Task<string> ValidateResetPassword(ResetPasswordViewModel rpvm, IUsersOperations _usersOperations)
        {
            var thisUser = await _usersOperations.GetUserById(rpvm.Id!);
            if (thisUser == null) return "User not found";
            var token = await _usersOperations.GenerateToken(thisUser);
            var reset = await _usersOperations.ResetPassword(thisUser, token, rpvm.NewPassword!);
            if (reset.Succeeded) return null!;
            return "Password reset failed";
        }
    }
}
