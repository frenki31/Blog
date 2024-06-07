using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data;
using BeReal.Models;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var post = await _context.Posts.Include(p => p.Comments)!
                                           .FirstOrDefaultAsync(x => x.Slug == slug);
            if (post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            var vm = new BlogPostViewModel()
            {
                Id = post.Id,
                Author = post.Author,
                Description = post.Description,
                PublicationDate = post.publicationDate,
                ShortDescription = post.ShortDescription!,
                Title = post.Title,
                ImageUrl = post.Image,
                Tags = post.Tags,
                Category = post.Category,
                Comments = post.Comments,
            };
            return View(vm);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Comment(int postId, string content)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            // Get the post
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }
            // Create a new comment
            var comment = new Comment
            {
                Message = content,
                PostId = postId,
                UserId = user.Id
            };
            // Add the comment to the context and save changes
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = postId });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Reply(int commentId, string content)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Get the comment
            var parentComment = await _context.Comments.FindAsync(commentId);
            if (parentComment == null)
            {
                return NotFound();
            }

            // Create a new comment
            var reply = new Comment
            {
                Message = content,
                PostId = parentComment.PostId,
                UserId = user.Id,
                ParentCommentId = commentId
            };

            // Add the reply to the context and save changes
            _context.Comments.Add(reply);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = parentComment.PostId });
        }

        /*[HttpPost]
        [Authorize]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }
            var post = await _context.Posts
                                     .Include(p => p.Comments)!
                                     .FirstOrDefaultAsync(x => x.Id == vm.PostId);
            if (vm.MainCommentId == 0)
            {
                post!.Comments = post.Comments ?? new List<Comment>();
                post.Comments.Add(new Comment
                {
                    Message = vm.Message,
                    Created = DateTime.Now,
                });
                _context.Posts.Update(post);
            }
            else
            {
                var comment = new Comment()
                {
                    MainCommentId = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now,
                };
                _context.Comments.Add(comment);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Post", new { slug = post!.Slug });
        }*/
    }
}
