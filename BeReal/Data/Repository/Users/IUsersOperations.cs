using BeReal.Models;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BeReal.Data.Repository.Users
{
    public interface IUsersOperations
    {
        //Users
        Task<BR_ApplicationUser?> GetUserById(string id);
        Task<BR_ApplicationUser?> GetLoggedUser(ClaimsPrincipal User);
        Task<BR_ApplicationUser?> GetCommentUser(ClaimsPrincipal User);
        Task<BR_ApplicationUser?> GetUserByUsername(string username);
        Task<BR_ApplicationUser?> GetUserByEmail(string username);
        Task<IList<string>> GetUserRole(BR_ApplicationUser? user);
        Task<List<BR_ApplicationUser>> GetUsers();
        Task<bool> CheckPasswordForLogin(BR_ApplicationUser user, string password);
        Task<string> GenerateToken(BR_ApplicationUser user);
        Task<IdentityResult> CreateUser(BR_ApplicationUser user, string password);
        Task<IdentityResult> GiveRoleToUser(BR_ApplicationUser user, string role);
        Task<IdentityResult> RemoveRoleFromUser(BR_ApplicationUser user, string role);
        Task<IdentityResult> ResetPassword(BR_ApplicationUser user, string token, string password);
        Task<IdentityResult> UpdateUser(BR_ApplicationUser user);
        Task<SignInResult> SignIn(string username, string password, bool remember, bool trueOrFalse);
        Task<string> ValidateUser(RegisterViewModel rvm, IUsersOperations _usersOperations);
        Task<string> ValidateResetPassword(ResetPasswordViewModel rpvm, IUsersOperations _usersOperations);
        Task Logout();
    }
}
