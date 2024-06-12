using BeReal.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BeReal.Data.Repository.Users
{
    public interface IUsersOperations
    {
        //Users
        Task<BR_ApplicationUser?> getUserById(string id);
        Task<BR_ApplicationUser?> getLoggedUser(ClaimsPrincipal User);
        Task<BR_ApplicationUser?> getCommentUser(ClaimsPrincipal User);
        Task<BR_ApplicationUser?> getUserByUsername(string username);
        Task<BR_ApplicationUser?> getUserByEmail(string username);
        Task<IList<string>> getUserRole(BR_ApplicationUser? user);
        Task<List<BR_ApplicationUser>> getUsers();
        Task<bool> checkPasswordForLogin(BR_ApplicationUser user, string password);
        Task<string> generateToken(BR_ApplicationUser user);
        Task<IdentityResult> createUser(BR_ApplicationUser user, string password);
        Task<IdentityResult> giveRoleToUser(BR_ApplicationUser user, string role);
        Task<IdentityResult> removeRoleFromUser(BR_ApplicationUser user, string role);
        Task<IdentityResult> resetPassword(BR_ApplicationUser user, string token, string password);
        Task<IdentityResult> updateUser(BR_ApplicationUser user);
        Task<SignInResult> signIn(string username, string password, bool remember, bool trueOrFalse);
        Task logout();
    }
}
