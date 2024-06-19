using BeReal.Data.Repository.Users;
using BeReal.Models;
using BeReal.ViewModels;

namespace BeReal.Data.Repository.Posts
{
    public interface IPostsOperations
    {
        //Posts
        IQueryable<BR_Post> GetFilteredPosts(string category, string subcategory, string search, DateTime startDate, DateTime endDate); //apply filters to get the posts
        Task<List<BR_Post>> GetUserPosts(BR_ApplicationUser user); //retrieve the posts of a single user 
        Task<List<BR_Post>> GetAllPosts(); //get all posts
        Task<List<BR_Post>> GetPostsOfUser(BR_ApplicationUser user); //retrieve all the posts with all information included for those posts related to a user
        Task<BR_Post?> GetBlogPost(string slug, string category, string subcategory); //retrieve a single post to display
        Task<BR_Post?> GetPostBySlug(string slug); //get a single post using the slug
        Task<BR_Post?> GetPostById(int id); //get all data of a post using the id
        Task<BR_Post?> GetPostWithFilesById(int id);
        void AddPost(BR_Post post);
        void RemovePost(BR_Post post);
        void UpdatePost(BR_Post post);
        int GetPostCount(string id); //get the count of posts a user has posted
        CreatePostViewModel GetEditViewModel(BR_Post post);
        Task<BR_Post> GetPostValues(BR_Post post, CreatePostViewModel model, BR_ApplicationUser user, IUsersOperations _usersOperations); //return a post with all values
        Task<List<BR_Post>> GetPostsWithPagination(IQueryable<BR_Post> query, int skip, int pageSize);
        Task<List<BR_Category>> GetCategories();
        void removePostComments(BR_Post post); //remove the comments related to post
        void removePostDocument(BR_Post post); //remove the document related to post
        void removePostImage(BR_Post post); //remove the image related to post
        //Save Changes
        Task<bool> SaveChanges();
    }
}
