using BeReal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeReal.Data.Repository
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public Repository(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        //Pages
        public async Task<Page?> getPage(string slug) => await _context.Pages.FirstOrDefaultAsync(x => x.Slug == slug);
        
        
        //Posts
        public IQueryable<Post> getFilteredPosts(string category, string search, DateTime startDate, DateTime endDate)
        {
            var query = _context.Posts.AsQueryable();
            //order all approved posts by date desc
            query = query.Include(x => x.User)
                         .Include(x => x.Document)
                         .OrderByDescending(x => x.publicationDate)
                         .Where(x => x.Approved == true);
            //filter by category
            query = string.IsNullOrEmpty(category) ? query : query.Where(post => post.Category!.ToLower().Equals(category.ToLower()));
            //filter by searchword
            query = string.IsNullOrEmpty(search) ? query : query.Where(x => x.Title!.Contains(search) || x.Author!.Contains(search) ||
                                                                       x.ShortDescription!.Contains(search) || x.Description!.Contains(search));
            //filter by date
            if (startDate > DateTime.MinValue && endDate > DateTime.MinValue && startDate < endDate)
            {
                query = query.Where(x => x.publicationDate >= startDate && x.publicationDate <= endDate);
            }
            else if (startDate > DateTime.MinValue && endDate == DateTime.MinValue)
            {
                query = query.Where(x => x.publicationDate >= startDate);
            }
            else if (endDate > DateTime.MinValue && startDate == DateTime.MinValue)
            {
                query = query.Where(x => x.publicationDate <= endDate);
            }
            return query;
        }
        public async Task<List<Post>> getUserPosts(ApplicationUser user) => await _context.Posts.Where(x => x.User!.Id == user.Id).ToListAsync();
        public async Task<List<Post>> getAllPosts() => await _context.Posts.Include(x => x.Document).Include(x => x.User).Include(x => x.Comments).ToListAsync();
        public async Task<List<Post>> getPostsOfUser(string id) => await _context.Posts.Include(x => x.Document).Include(x => x.User).Include(x => x.Comments).Where(x => x.User!.Id == id).ToListAsync();
        public async Task<Post?> getBlogPost(string slug)
        {
            return await _context.Posts.Include(p => p.Comments!)
                                           .ThenInclude(comment => comment.User)
                                       .Include(x => x.Comments!)
                                           .ThenInclude(comment => comment.Replies!)
                                       .Include(p => p.User)
                                       .Include(p => p.Document)
                                           .FirstOrDefaultAsync(x => x.Slug == slug);
        }
        public async Task<Post?> getPostById(int id) => await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);
        public async Task<Post?> getPostWithDocById(int id) => await _context.Posts.Include(x => x.Document).FirstOrDefaultAsync(x => x.Id == id);
        public async Task<Post?> getPostBySlug(string slug) => await _context.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
        public void addPost(Post post) => _context.Posts.Add(post);
        public void removePost(Post post) => _context.Posts.Remove(post);
        public void updatePost(Post post) => _context.Posts.Update(post);
        public int getPostCount(string id) => _context.Posts.Where(x => x.User!.Id == id).Count();


        //Users
        public async Task<ApplicationUser?> getUserById(string id) => await _userManager.FindByIdAsync(id);
        public async Task<IList<string>> getUserRole(ApplicationUser? user) => await _userManager.GetRolesAsync(user!);
        public async Task<ApplicationUser?> getCommentUser(ClaimsPrincipal User) => await _userManager.GetUserAsync(User);
        public async Task<ApplicationUser?> getUserByUsername(string username) => await _userManager.FindByNameAsync(username);
        public async Task<ApplicationUser?> getUserByEmail(string email) => await _userManager.FindByEmailAsync(email);
        public async Task<IdentityResult> createUser(ApplicationUser user, string password) => await _userManager.CreateAsync(user, password);
        public async Task<IdentityResult> giveRoleToUser(ApplicationUser user, string role) => await _userManager.AddToRoleAsync(user, role);
        public async Task<IdentityResult> removeRoleFromUser(ApplicationUser user, string role) => await _userManager.RemoveFromRoleAsync(user, role);
        public async Task<ApplicationUser?> getLoggedUser(ClaimsPrincipal User) => await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
        public async Task<List<ApplicationUser>> getUsers() => await _userManager.Users.ToListAsync();
        public async Task<bool> checkPasswordForLogin(ApplicationUser user, string password) => await _userManager.CheckPasswordAsync(user, password!);
        public async Task<string> generateToken(ApplicationUser user) => await _userManager.GeneratePasswordResetTokenAsync(user);
        public async Task<IdentityResult> resetPassword(ApplicationUser user, string token, string password) => await _userManager.ResetPasswordAsync(user, token, password);
        public async Task<IdentityResult> updateUser(ApplicationUser user) => await _userManager.UpdateAsync(user);
        public async Task<SignInResult> signIn(string username, string password, bool remember, bool trueOrFalse) => await _signInManager.PasswordSignInAsync(username, password, remember, trueOrFalse);
        public async Task logout() => await _signInManager.SignOutAsync();


        //Comments
        public async Task<Comment?> getCommentById(int id) => await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        public void addComment(Comment comment) => _context.Comments.Add(comment);  
        public void removeReplies(Comment comment) => _context.Comments.RemoveRange(comment.Replies!);  
        public void removeComment(Comment comment) => _context.Comments.Remove(comment);  
        public int getCommentCount(string id) => _context.Comments.Where(x => x.User!.Id == id).Count();  
        public async Task<Comment?> getCommentWithReplies(int id) => await _context.Comments.Include(c => c.Replies).FirstOrDefaultAsync(comment => comment.Id == id);
        

        //Save Changes
        public async Task<bool> saveChanges() => await _context.SaveChangesAsync() > 0;
    }
}
