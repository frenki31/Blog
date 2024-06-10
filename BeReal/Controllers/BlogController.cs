using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data;
using BeReal.Models;
using BeReal.Utilities;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BeReal.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public INotyfService _notification { get; }
        public BlogController(ApplicationDbContext context, INotyfService notification, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notification = notification;
            _userManager = userManager;
        }

        [HttpGet("[controller]/{slug}")]
        public async Task<IActionResult> Post(string slug)
        {
            if (slug == "")
            {
                _notification.Error("Post not found");
                return View();
            }
            var post = await _context.Posts.Include(p => p.Comments!)
                                                .ThenInclude(comment => comment.User)
                                           .Include(x => x.Comments!)
                                                .ThenInclude(comment => comment.Replies!)
                                           .Include(p => p.User)
                                           .FirstOrDefaultAsync(x => x.Slug == slug);
            if (post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            var vm = new BlogPostViewModel()
            {
                Post = post,
                ReturnUrl = Url.Action("Post", new { slug })
            };
            return View(vm);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Comment(BlogPostViewModel vm)
        {
            if (vm.Post is null || vm.Comment is null)
            {
                return RedirectToAction("Index", "Home");
            }

            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == vm.Post.Id);
            if (post == null)
            {
                return NotFound();
            }

            var comment = vm.Comment;
            comment.User = await _userManager.GetUserAsync(User);
            comment.Post = post;
            comment.Created = DateTime.Now;

            if (comment.ParentComment != null)
            {
                var ParComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == comment.ParentComment.Id);
                if (ParComment != null)
                {
                    comment.ParentComment = ParComment;
                }
            }

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Post", new { slug = post.Slug });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id, string slug)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
            var Comment = await _context.Comments.Include(c => c.Replies).FirstOrDefaultAsync(comment => comment.Id == id);
            var loggedUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedUserRole = await _userManager.GetRolesAsync(loggedUser!);
            if (loggedUserRole[0] == Roles.Admin || loggedUser!.Id == Comment!.User!.Id)
            {
                if (Comment!.Replies != null)
                {
                    _context.Comments.RemoveRange(Comment.Replies);
                }
                _context.Comments.Remove(Comment!);
                await _context.SaveChangesAsync();
                _notification.Success("Comment deleted successfully");
                return RedirectToAction("Post", new { slug = post!.Slug });
            }
            return View();
        }            
    }
}
