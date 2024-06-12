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

        [HttpGet("[controller]/{slug}")]
        public async Task<IActionResult> Post(string slug)
        {
            if (slug == "") 
                return View(); 
            var post = await _postsOperations.getBlogPost(slug);
            if (post == null) 
                return View(); 
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
                return RedirectToAction("Index", "Home"); 
            var post = await _postsOperations.getPostById(vm.Post.IDBR_Post);
            if (post == null) 
                return NotFound(); 
            var comment = vm.Comment;
            comment.ApplicationUser = await _usersOperations.GetCommentUser(User);
            comment.Post = post;
            comment.Created = DateTime.Now;
            if (comment.ParentComment != null)
            {
                var ParComment = await _commentsOperations.getCommentById(comment.ParentComment.IDBR_Comment);
                comment.ParentComment = ParComment != null ? ParComment : null;
            }
            _commentsOperations.addComment(comment);
            await _commentsOperations.saveChanges();
            _notification.Success("Commented");
            return RedirectToAction("Post", new { slug = post.Slug });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id, string slug)
        {
            var post = await _postsOperations.getPostBySlug(slug);
            var Comment = await _commentsOperations.getCommentWithReplies(id);
            var loggedUser = await _usersOperations.GetLoggedUser(User);
            var loggedUserRole = await _usersOperations.GetUserRole(loggedUser!);
            if (loggedUserRole[0] == Roles.Admin || loggedUser!.Id == Comment!.ApplicationUser!.Id)
            {
                if (Comment!.Replies != null)
                    _commentsOperations.removeReplies(Comment); 
                _commentsOperations.removeComment(Comment);
                await _commentsOperations.saveChanges();
                _notification.Success("Comment deleted successfully");
                return RedirectToAction("Post", new { slug = post!.Slug });
            }
            return View();
        }            
    }
}