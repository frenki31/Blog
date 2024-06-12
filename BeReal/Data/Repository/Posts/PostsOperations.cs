using BeReal.Models;
using Microsoft.EntityFrameworkCore;

namespace BeReal.Data.Repository.Posts
{
    public class PostsOperations : IPostsOperations
    {
        private readonly ApplicationDbContext _context;
        public PostsOperations(ApplicationDbContext context) { 
            _context = context;
        }
        //Posts
        public IQueryable<BR_Post> getFilteredPosts(string category, string search, DateTime startDate, DateTime endDate)
        {
            var query = _context.Posts.AsQueryable();
            //order all approved posts by date desc
            query = query.Include(x => x.ApplicationUser)
                         .Include(x => x.Document)
                         .OrderByDescending(x => x.PublicationDate)
                         .Where(x => x.Approved == true);
            //filter by category
            query = string.IsNullOrEmpty(category) ? query : query.Where(post => post.Category!.ToLower().Equals(category.ToLower()));
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
        public async Task<List<BR_Post>> getUserPosts(BR_ApplicationUser user) => await _context.Posts.Where(x => x.ApplicationUser!.Id == user.Id).ToListAsync();
        public async Task<List<BR_Post>> getAllPosts() => await _context.Posts.Include(x => x.Document).Include(x => x.ApplicationUser).Include(x => x.Comments).ToListAsync();
        public async Task<List<BR_Post>> getPostsOfUser(string id) => await _context.Posts.Include(x => x.Document).Include(x => x.ApplicationUser).Include(x => x.Comments).Where(x => x.ApplicationUser!.Id == id).ToListAsync();
        public async Task<BR_Post?> getBlogPost(string slug)
        {
            return await _context.Posts.Include(p => p.Comments!)
                                           .ThenInclude(comment => comment.ApplicationUser)
                                       .Include(x => x.Comments!)
                                           .ThenInclude(comment => comment.Replies!)
                                       .Include(p => p.ApplicationUser)
                                       .Include(p => p.Document)
                                           .FirstOrDefaultAsync(x => x.Slug == slug);
        }
        public async Task<BR_Post?> getPostById(int id) => await _context.Posts.FirstOrDefaultAsync(x => x.IDBR_Post == id);
        public async Task<BR_Post?> getPostWithDocById(int id) => await _context.Posts.Include(x => x.Document).FirstOrDefaultAsync(x => x.IDBR_Post == id);
        public async Task<BR_Post?> getPostBySlug(string slug) => await _context.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
        public void addPost(BR_Post post) => _context.Posts.Add(post);
        public void removePost(BR_Post post) => _context.Posts.Remove(post);
        public void updatePost(BR_Post post) => _context.Posts.Update(post);
        public int getPostCount(string id) => _context.Posts.Where(x => x.ApplicationUser!.Id == id).Count();
        //Save Changes
        public async Task<bool> saveChanges() => await _context.SaveChangesAsync() > 0;
    }
}
