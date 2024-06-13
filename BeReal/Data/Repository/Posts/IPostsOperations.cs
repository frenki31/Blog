using BeReal.Data.Repository.Users;
using BeReal.Models;
using BeReal.ViewModels;

namespace BeReal.Data.Repository.Posts
{
    public interface IPostsOperations
    {
        //Posts
        IQueryable<BR_Post> GetFilteredPosts(string category, string search, DateTime startDate, DateTime endDate);
        Task<List<BR_Post>> GetUserPosts(BR_ApplicationUser user);
        Task<List<BR_Post>> GetAllPosts();
        Task<List<BR_Post>> GetPostsOfUser(string id);
        Task<BR_Post?> GetBlogPost(string slug);
        Task<BR_Post?> GetPostBySlug(string slug);
        Task<BR_Post?> GetPostById(int id);
        Task<BR_Post?> GetPostWithDocById(int id);
        void AddPost(BR_Post post);
        void RemovePost(BR_Post post);
        void UpdatePost(BR_Post post);
        int GetPostCount(string id);
        CreatePostViewModel GetEditViewModel(BR_Post post);
        Task<BR_Post> GetPostValues(BR_Post post, CreatePostViewModel model, BR_ApplicationUser user, IUsersOperations _usersOperations);
        //Save Changes
        Task<bool> SaveChanges();
    }
}
