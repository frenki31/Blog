using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data;
using BeReal.Models.Comments;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeReal.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }
        public BlogController(ApplicationDbContext context, INotyfService notification)
        {
            _context = context;
            _notification = notification;
        }

        [HttpGet("[controller]/{slug}")]
        public async Task<IActionResult> Post(string slug)
        {
            if (slug == "")
            {
                _notification.Error("Post not found");
                return View();
            }
            var post = await _context.Posts.Include(p => p.MainComments)!
                                           .ThenInclude(mc => mc.SubComments)!
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
                MainComments = post.MainComments,
            };
            return View(vm);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }
            var post = await _context.Posts
                                     .Include(p => p.MainComments)!
                                     .ThenInclude(mc => mc.SubComments)
                                     .FirstOrDefaultAsync(x => x.Id == vm.PostId);
            if (vm.MainCommentId == 0)
            {
                post!.MainComments = post.MainComments ?? new List<MainComment>();
                post.MainComments.Add(new MainComment
                {
                    Message = vm.Message,
                    Created = DateTime.Now,
                });
                _context.Posts.Update(post);
            }
            else
            {
                var comment = new SubComment()
                {
                    MainCommentId = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now,
                };
                _context.SubComments.Add(comment);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Post", new { slug = post!.Slug });
        }
    }
}
