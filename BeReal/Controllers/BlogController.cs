using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data.Repository;
using BeReal.Utilities;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeReal.Controllers
{
    public class BlogController : Controller
    {
        private readonly IRepository _repo;
        public INotyfService _notification { get; }
        public BlogController(INotyfService notification, IRepository repo)
        {
            _notification = notification;
            _repo = repo;
        }

        [HttpGet("[controller]/{slug}")]
        public async Task<IActionResult> Post(string slug)
        {
            if (slug == "")
            {
                _notification.Error("Post not found");
                return View();
            }
            var post = await _repo.getBlogPost(slug);
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

            var post = await _repo.getPostById(vm.Post.Id);
            if (post == null)
            {
                return NotFound();
            }

            var comment = vm.Comment;
            comment.User = await _repo.getCommentUser(User);
            comment.Post = post;
            comment.Created = DateTime.Now;

            if (comment.ParentComment != null)
            {
                var ParComment = await _repo.getCommentById(comment.ParentComment.Id);
                if (ParComment != null)
                {
                    comment.ParentComment = ParComment;
                }
            }

            _repo.addComment(comment);
            await _repo.saveChanges();

            return RedirectToAction("Post", new { slug = post.Slug });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id, string slug)
        {
            var post = await _repo.getPostBySlug(slug);
            var Comment = await _repo.getCommentWithReplies(id);
            var loggedUser = await _repo.getLoggedUser(User);
            var loggedUserRole = await _repo.getUserRole(loggedUser!);
            if (loggedUserRole[0] == Roles.Admin || loggedUser!.Id == Comment!.User!.Id)
            {
                if (Comment!.Replies != null)
                {
                    _repo.removeReplies(Comment);
                }
                _repo.removeComment(Comment);
                await _repo.saveChanges();
                _notification.Success("Comment deleted successfully");
                return RedirectToAction("Post", new { slug = post!.Slug });
            }
            return View();
        }            
    }
}
