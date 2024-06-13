using BeReal.Data.Repository.Users;
using BeReal.Models;
using BeReal.ViewModels;
using Microsoft.EntityFrameworkCore;
using BeReal.Utilities;

namespace BeReal.Data.Repository.Posts
{
    public class PostsOperations : IPostsOperations
    {
        private readonly ApplicationDbContext _context;
        public PostsOperations(ApplicationDbContext context) { 
            _context = context;
        }
        //Posts
        public IQueryable<BR_Post> GetFilteredPosts(string category, string search, DateTime startDate, DateTime endDate)
        {
            var query = _context.Posts.AsQueryable();
            //order all approved posts by date desc
            query = query.Include(x => x.ApplicationUser)
                         .Include(x => x.Document)
                         .OrderByDescending(x => x.PublicationDate)
                         .Where(x => x.Approved == true);
            //filter by category
            query = string.IsNullOrEmpty(category) ? query : query.Where(post => post.Category!.Contains(category));
            //filter by searchword
            query = string.IsNullOrEmpty(search) ? query : query.Where(x => x.Title!.Contains(search) || x.Author!.Contains(search) ||
                                                                       x.ShortDescription!.Contains(search) || x.Description!.Contains(search));
            //filter by date
            if (startDate > DateTime.MinValue && endDate > DateTime.MinValue && startDate < endDate)
            {
                query = query.Where(x => x.PublicationDate >= startDate && x.PublicationDate <= endDate);
            }
            else if (startDate > DateTime.MinValue && endDate == DateTime.MinValue)
            {
                query = query.Where(x => x.PublicationDate >= startDate);
            }
            else if (endDate > DateTime.MinValue && startDate == DateTime.MinValue)
            {
                query = query.Where(x => x.PublicationDate <= endDate);
            }
            return query;
        }
        public async Task<List<BR_Post>> GetUserPosts(BR_ApplicationUser user) => await _context.Posts.Where(x => x.ApplicationUser!.Id == user.Id).ToListAsync();
        public async Task<List<BR_Post>> GetAllPosts() => await _context.Posts.Include(x => x.Document).Include(x => x.ApplicationUser).Include(x => x.Comments).ToListAsync();
        public async Task<List<BR_Post>> GetPostsOfUser(string id) => await _context.Posts.Include(x => x.Document).Include(x => x.ApplicationUser).Include(x => x.Comments).Where(x => x.ApplicationUser!.Id == id).ToListAsync();
        public async Task<BR_Post?> GetBlogPost(string slug)
        {
            return await _context.Posts.Include(p => p.Comments!)
                                           .ThenInclude(comment => comment.ApplicationUser)
                                       .Include(x => x.Comments!)
                                           .ThenInclude(comment => comment.Replies!)
                                       .Include(p => p.ApplicationUser)
                                       .Include(p => p.Document)
                                           .FirstOrDefaultAsync(x => x.Slug == slug);
        }
        public async Task<BR_Post?> GetPostById(int id) => await _context.Posts.FirstOrDefaultAsync(x => x.IDBR_Post == id);
        public async Task<BR_Post?> GetPostWithDocById(int id) => await _context.Posts.Include(x => x.Document).FirstOrDefaultAsync(x => x.IDBR_Post == id);
        public async Task<BR_Post?> GetPostBySlug(string slug) => await _context.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
        public void AddPost(BR_Post post) => _context.Posts.Add(post);
        public void RemovePost(BR_Post post) => _context.Posts.Remove(post);
        public void UpdatePost(BR_Post post) => _context.Posts.Update(post);
        public int GetPostCount(string id) => _context.Posts.Where(x => x.ApplicationUser!.Id == id).Count();
        //Save Changes
        public async Task<bool> SaveChanges() => await _context.SaveChangesAsync() > 0;
        public CreatePostViewModel GetEditViewModel(BR_Post post)
        {
            var edit = new CreatePostViewModel
            {
                Id = post.IDBR_Post,
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                Description = post.Description,
                ImageUrl = post.Image,
                Category = post.Category,
                Tags = post.Tags,
                Approved = post.Approved,
                File = post.Document != null ? new FileViewModel { Id = post.Document!.IDBR_Document, ContentType = post.Document.ContentType, Name = post.Document.FileName } : null,
            };
            return edit;
        }
        public async Task<BR_Post> GetPostValues(BR_Post post,CreatePostViewModel model, BR_ApplicationUser user, IUsersOperations _usersOperations)
        {
            var userRole = await _usersOperations.GetUserRole(user!);
            post.Title = model.Title;
            post.ShortDescription = model.ShortDescription;
            post.Description = model.Description;
            post.Category = model.Category;
            post.Tags = model.Tags;
            post.Approved = userRole[0] == Roles.Admin;
            return post;
        }
        public async Task<List<BR_Post>> GetPostsWithPagination(IQueryable<BR_Post> query, int skip, int pageSize) => await query.Skip(skip).Take(pageSize).ToListAsync();
    }
}
