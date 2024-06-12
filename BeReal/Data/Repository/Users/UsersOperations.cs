using BeReal.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
        public async Task<BR_ApplicationUser?> getUserById(string id) => await _userManager.FindByIdAsync(id);
        public async Task<IList<string>> getUserRole(BR_ApplicationUser? user) => await _userManager.GetRolesAsync(user!);
        public async Task<BR_ApplicationUser?> getCommentUser(ClaimsPrincipal User) => await _userManager.GetUserAsync(User);
        public async Task<BR_ApplicationUser?> getUserByUsername(string username) => await _userManager.FindByNameAsync(username);
        public async Task<BR_ApplicationUser?> getUserByEmail(string email) => await _userManager.FindByEmailAsync(email);
        public async Task<IdentityResult> createUser(BR_ApplicationUser user, string password) => await _userManager.CreateAsync(user, password);
        public async Task<IdentityResult> giveRoleToUser(BR_ApplicationUser user, string role) => await _userManager.AddToRoleAsync(user, role);
        public async Task<IdentityResult> removeRoleFromUser(BR_ApplicationUser user, string role) => await _userManager.RemoveFromRoleAsync(user, role);
        public async Task<BR_ApplicationUser?> getLoggedUser(ClaimsPrincipal User) => await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
        public async Task<List<BR_ApplicationUser>> getUsers() => await _userManager.Users.ToListAsync();
        public async Task<bool> checkPasswordForLogin(BR_ApplicationUser user, string password) => await _userManager.CheckPasswordAsync(user, password!);
        public async Task<string> generateToken(BR_ApplicationUser user) => await _userManager.GeneratePasswordResetTokenAsync(user);
        public async Task<IdentityResult> resetPassword(BR_ApplicationUser user, string token, string password) => await _userManager.ResetPasswordAsync(user, token, password);
        public async Task<IdentityResult> updateUser(BR_ApplicationUser user) => await _userManager.UpdateAsync(user);
        public async Task<SignInResult> signIn(string username, string password, bool remember, bool trueOrFalse) => await _signInManager.PasswordSignInAsync(username, password, remember, trueOrFalse);
        public async Task logout() => await _signInManager.SignOutAsync();
    }
}
