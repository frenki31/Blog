using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data.Repository.Comments;
using BeReal.Data.Repository.Posts;
using BeReal.Data.Repository.Users;
using BeReal.Utilities;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeReal.Controllers
{
    public class BlogController : Controller
    {
        private readonly ICommentsOperations _commentsOperations;
        private readonly IPostsOperations _postsOperations;
        private readonly IUsersOperations _usersOperations;
        public INotyfService _notification { get; }
        public BlogController(INotyfService notification, IPostsOperations postsOperations, ICommentsOperations commentsOperations, IUsersOperations usersOperations)
        {
            _notification = notification;
            _postsOperations = postsOperations;
            _commentsOperations = commentsOperations;
            _usersOperations = usersOperations;
        }
        [HttpGet("[controller]/{category}/{subcategory}/{slug}", Order = 1)]
        [HttpGet("[controller]/{category}/{slug}", Order = 2)]
        [HttpGet("[controller]/{slug:required}", Order = 3)]
        public async Task<IActionResult> Post(string category, string subcategory, string slug)
        {
            string url ;
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(subcategory))
                url = Url.Action("Post", new { category, subcategory, slug })!;
            else if (!string.IsNullOrEmpty(category))
                url = Url.Action("Post", new { category, slug })!;
            else 
                url = Url.Action("Post", new { slug })!;
            var post = await _postsOperations.GetBlogPost(slug, category, subcategory);
            if (post == null)
                return NotFound();
            var vm = new BlogPostViewModel() { Post = post, ReturnUrl = url };
            return View(vm);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Comment(BlogPostViewModel vm)
        {
            if (vm.Post is null || vm.Comment is null) 
                return RedirectToAction("Index", "Home");  
            var comment = vm.Comment;
            comment.ApplicationUser = await _usersOperations.GetCommentUser(User);
            comment.Post = await _postsOperations.GetPostById(vm.Post.IDBR_Post);
            comment.Created = DateTime.Now;
            if (comment.ParentComment != null)
            {
                var ParComment = await _commentsOperations.GetCommentById(comment.ParentComment.IDBR_Comment);
                comment.ParentComment = ParComment != null ? ParComment : null;
            }
            _commentsOperations.AddComment(comment);
            await _commentsOperations.SaveChanges();
            _notification.Success("Commented");
            return RedirectToAction("Post", new { slug = comment.Post!.Slug });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditComment(BlogPostViewModel vm)
        {
            if (vm.Comment is null)
                return RedirectToAction("Index", "Home");
            var post = await _postsOperations.GetPostById(vm.Post!.IDBR_Post);
            var comment = await _commentsOperations.GetCommentById(vm.Comment.IDBR_Comment);
            comment!.Message = vm.Comment.Message;
            comment.Created = DateTime.Now;
            comment.ApplicationUser = await _usersOperations.GetCommentUser(User);
            if (vm.Comment.ParentComment != null)
            {
                var ParComment = await _commentsOperations.GetCommentById(vm.Comment.ParentComment.IDBR_Comment);
                comment.ParentComment = ParComment != null ? ParComment : null;
            }
            _commentsOperations.UpdateComment(comment);
            await _commentsOperations.SaveChanges();
            _notification.Success("Comment updated");
            return RedirectToAction("Post", new { slug = post!.Slug });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id, string slug)
        {
            var post = await _postsOperations.GetPostBySlug(slug);
            var Comment = await _commentsOperations.GetCommentWithReplies(id);
            var loggedUser = await _usersOperations.GetLoggedUser(User);
            var loggedUserRole = await _usersOperations.GetUserRole(loggedUser!);
            if (loggedUserRole[0] == Roles.Admin || loggedUser!.Id == Comment!.ApplicationUser!.Id)
            {
                if (Comment!.Replies != null)
                    _commentsOperations.RemoveReplies(Comment); 
                _commentsOperations.RemoveComment(Comment);
                await _commentsOperations.SaveChanges();
                _notification.Success("Comment deleted successfully");
                return RedirectToAction("Post", new { slug = post!.Slug });
            }
            return View();
        }            
    }
}