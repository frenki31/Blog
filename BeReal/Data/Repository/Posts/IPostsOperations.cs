using BeReal.Models;

namespace BeReal.Data.Repository.Posts
{
    public interface IPostsOperations
    {
        //Posts
        IQueryable<BR_Post> getFilteredPosts(string category, string search, DateTime startDate, DateTime endDate);
        Task<List<BR_Post>> getUserPosts(BR_ApplicationUser user);
        Task<List<BR_Post>> getAllPosts();
        Task<List<BR_Post>> getPostsOfUser(string id);
        Task<BR_Post?> getBlogPost(string slug);
        Task<BR_Post?> getPostBySlug(string slug);
        Task<BR_Post?> getPostById(int id);
        Task<BR_Post?> getPostWithDocById(int id);
        void addPost(BR_Post post);
        void removePost(BR_Post post);
        void updatePost(BR_Post post);
        int getPostCount(string id);
        //Save Changes
        Task<bool> saveChanges();
    }
}
