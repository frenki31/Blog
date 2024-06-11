using BeReal.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace BeReal.Data.Repository
{
    public interface IRepository
    {
        //Pages
        Task<Page?> getPage(string slug);

        //Posts
        IQueryable<Post> getFilteredPosts(string category, string search, DateTime startDate, DateTime endDate);
        Task<List<Post>> getUserPosts(ApplicationUser user);
        Task<List<Post>> getAllPosts();
        Task<List<Post>> getPostsOfUser(string id);
        Task<Post?> getBlogPost(string slug);
        Task<Post?> getPostBySlug(string slug);
        Task<Post?> getPostById(int id);
        Task<Post?> getPostWithDocById(int id);
        void addPost(Post post);
        void removePost(Post post);
        void updatePost(Post post);
        int getPostCount(string id);

        //Users
        Task<ApplicationUser?> getUserById(string id);
        Task<ApplicationUser?> getLoggedUser(ClaimsPrincipal User);
        Task<ApplicationUser?> getCommentUser(ClaimsPrincipal User);
        Task<ApplicationUser?> getUserByUsername(string username);
        Task<ApplicationUser?> getUserByEmail(string username);
        Task<IList<string>> getUserRole(ApplicationUser? user);
        Task<List<ApplicationUser>> getUsers();
        Task<bool> checkPasswordForLogin(ApplicationUser user, string password);
        Task<string> generateToken(ApplicationUser user);
        Task<IdentityResult> createUser(ApplicationUser user, string password);
        Task<IdentityResult> giveRoleToUser(ApplicationUser user, string role);
        Task<IdentityResult> removeRoleFromUser(ApplicationUser user, string role);
        Task<IdentityResult> resetPassword(ApplicationUser user, string token, string password);
        Task<IdentityResult> updateUser(ApplicationUser user);
        Task<SignInResult> signIn(string username, string password, bool remember, bool trueOrFalse);
        Task logout();

        //Comments
        Task<Comment?> getCommentById(int id);
        Task<Comment?> getCommentWithReplies(int id);
        void addComment(Comment comment);
        void removeReplies(Comment comment);
        void removeComment(Comment comment);
        int getCommentCount(string id);


        //Save Changes
        Task<bool> saveChanges();
    }
}
