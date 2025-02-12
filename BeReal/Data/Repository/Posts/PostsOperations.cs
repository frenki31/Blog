using BeReal.Data.Repository.Users;
using BeReal.Models;
using BeReal.ViewModels;
using Microsoft.EntityFrameworkCore;
using BeReal.Utilities;
using BeReal.Data.Repository.Files;

namespace BeReal.Data.Repository.Posts
{
    public class PostsOperations : IPostsOperations
    {
        private readonly ApplicationDbContext _context;
        public PostsOperations(ApplicationDbContext context) { 
            _context = context;
        }
        //Posts
        public async Task<HomeViewModel> GetHomeViewModel(PageViewModel homePage, string category,string subcategory, string search, string startDate, string endDate, int page, IPostsOperations _postsOperations, IFileManager _fileManager)
        {
            var query = _context.BR_Posts.AsNoTracking().AsQueryable();
            //order all approved posts by date desc
            query = query.Include(x => x.ApplicationUser)
                         .Include(x => x.Document)
                         .Include(x => x.Image)
                         .OrderByDescending(x => x.PublicationDate)
                         .Where(x => x.Approved == true);
            //filter by category
            query = string.IsNullOrEmpty(category) ? query : query.Where(post => EF.Functions.Like(post.Category, $"%{category}%"));
            //filter by subcategory
            query = string.IsNullOrEmpty(subcategory) ? query : query.Where(post => EF.Functions.Like(post.Category, $"%{subcategory}%"));
            //filter by searchword
            query = string.IsNullOrEmpty(search) ? query : query.Where(x => EF.Functions.Like(x.Title, $"%{search}%") || EF.Functions.Like(x.Author, $"%{search}%") ||
                                                                       EF.Functions.Like(x.ShortDescription, $"%{search}%") || EF.Functions.Like(x.Description, $"%{search}%"));
            //filter by date
            DateTime? actualStartDate = null;
            DateTime? actualEndDate = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                try {
                    actualStartDate = DateTime.Parse(startDate);
                }
                catch (FormatException) {
                    Console.WriteLine($"Error parsing start date: {startDate}");
                }
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                try {
                    actualEndDate = DateTime.Parse(endDate);
                }
                catch (FormatException) {
                    Console.WriteLine($"Error parsing end date: {endDate}");
                }
            }
            if (actualStartDate > DateTime.MinValue && actualEndDate > DateTime.MinValue)
            {
                query = query.Where(x => x.PublicationDate >= actualStartDate && x.PublicationDate <= actualEndDate);
            }
            else if (actualStartDate > DateTime.MinValue)
            {
                query = query.Where(x => x.PublicationDate >= actualStartDate);
            }
            else if (actualEndDate > DateTime.MinValue)
            {
                query = query.Where(x => x.PublicationDate <= actualEndDate);
            }
            int pageSize = 5;
            int skip = pageSize * (page - 1);
            int postCount = query.Count();
            int pageCount = (int)Math.Ceiling((double)postCount / pageSize);
            var viewModel = new HomeViewModel()
            {
                Page = homePage,
                Category = category,
                SubCategory = subcategory,
                Search = search,
                StartDate = startDate,
                EndDate = endDate,
                PageNumber = page,
                NextPage = postCount > skip + pageSize,
                PageCount = pageCount,
                Categories = await _postsOperations.GetCategories(),
                Posts = await _postsOperations.GetPostsWithPagination(query, skip, pageSize),
                Pages = _fileManager.Pages(page, pageCount),
            };
            return viewModel;
        }
        public async Task<List<BR_Post>> GetUserPosts(BR_ApplicationUser user) => await _context.BR_Posts.Include(x => x.Image).Where(x => x.ApplicationUser!.Id == user.Id).ToListAsync();
        public async Task<List<BR_Post>> GetAllPosts() => await _context.BR_Posts.AsNoTracking().Include(x => x.Document).Include(x => x.ApplicationUser).Include(x => x.Comments).Include(x => x.Image).ToListAsync();
        public async Task<List<BR_Post>> GetPostsOfUser(BR_ApplicationUser user) => await _context.BR_Posts.AsNoTracking().Include(x => x.Document).Include(x => x.ApplicationUser).Include(x => x.Comments).Include(x => x.Image).Where(x => x.ApplicationUser!.Id == user.Id).ToListAsync();
        public async Task<BR_Post?> GetBlogPost(string slug)
        {
            return await _context.BR_Posts.Include(p => p.Comments!)
                                           .ThenInclude(comment => comment.ApplicationUser)
                                       .Include(x => x.Comments!)
                                           .ThenInclude(comment => comment.Replies!)
                                       .Include(p => p.ApplicationUser)
                                       .Include(p => p.Document)
                                       .Include(p => p.Image)
                                           .FirstOrDefaultAsync(x => x.Slug == slug);
        }
        public async Task<BR_Post?> GetPostById(int id) => await _context.BR_Posts.Include(x=> x.Comments).Include(x => x.Document).Include(x => x.Image).FirstOrDefaultAsync(x => x.IDBR_Post == id);
        public async Task<BR_Post?> GetPostWithFilesById(int id) => await _context.BR_Posts.Include(x => x.Document).Include(x => x.Image).FirstOrDefaultAsync(x => x.IDBR_Post == id);
        public async Task<BR_Post?> GetPostBySlug(string slug) => await _context.BR_Posts.FirstOrDefaultAsync(p => p.Slug == slug);
        public void AddPost(BR_Post post) => _context.BR_Posts.Add(post);
        public void RemovePost(BR_Post post) => _context.BR_Posts.Remove(post);
        public void UpdatePost(BR_Post post) => _context.BR_Posts.Update(post);
        public int GetPostCount(string id) => _context.BR_Posts.Where(x => x.ApplicationUser!.Id == id).Count();
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
                Image = post.Image != null ? new FileViewModel { Id = post.Image!.IDBR_Document, ContentType = post.Image.ContentType, Name = post.Image.FileName } : null,
                Category = post.Category,
                Tags = post.Tags,
                Approved = post.Approved,
                Categories = _context.BR_Categories.ToList(),
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
        public async Task<List<BR_Category>> GetCategories() => await _context.BR_Categories.Include(x => x.Subcategories).ToListAsync(); 
        public void removePostComments(BR_Post post) { _context.BR_Comments.RemoveRange(post.Comments!); }
        public void removePostDocument(BR_Post post) { _context.BR_Files.Remove(post.Document!); }
        public void removePostImage(BR_Post post) { _context.BR_Files.Remove(post.Image!); }
    }
}