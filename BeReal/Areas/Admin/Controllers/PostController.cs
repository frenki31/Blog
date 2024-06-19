using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BeReal.Models;
using BeReal.Utilities;
using X.PagedList;
using BeReal.Data.Repository.Files;
using BeReal.Data.Repository.Posts;
using BeReal.Data.Repository.Users;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostController : Controller
    {
        public readonly IPostsOperations _postsOperations;
        public readonly IUsersOperations _usersOperations;
        public readonly IFileManager _fileManager;
        public INotyfService _notification { get; }
        public PostController(INotyfService notification,IPostsOperations postsOperations, IUsersOperations usersOperations,IFileManager fileManager)
        {
            _notification = notification;
            _fileManager = fileManager;
            _postsOperations = postsOperations;
            _usersOperations = usersOperations;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page) //If user is admin all the posts are displayed in the main page else only user posts
        {
            var posts = new List<BR_Post>();
            var loggedUser = await _usersOperations.GetLoggedUser(User);
            var loggedUserRole = await _usersOperations.GetUserRole(loggedUser!);
            posts = loggedUserRole[0] == Roles.Admin ? await _postsOperations.GetAllPosts() : await _postsOperations.GetPostsOfUser(loggedUser!);
            var pageSize = 5;
            var pageNumber = (page ?? 1);
            var postVms = posts.Select(x => new PostViewModel()
            {
                Id = x.IDBR_Post,
                Title = x.Title,
                ShortDescription = x.ShortDescription,
                Author = x.Author,
                publicationDate = x.PublicationDate,
                Image = x.Image,
                Category = x.Category,
                Tags = x.Tags,
                Approved = x.Approved,
                Comments = x.Comments,
                Document = x.Document,
            }).ToList();
            return View(await postVms.OrderByDescending(x => x.publicationDate).ToPagedListAsync(pageNumber, pageSize));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CreatePostViewModel() { Categories = await _postsOperations.GetCategories() };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel model) 
        {
            if (!ModelState.IsValid) return View(model); 
            var loggedUser = await _usersOperations.GetLoggedUser(User);
            BR_Document? file = null;
            if (model.File != null)
            {
                file = await _fileManager.GetFileInfo(model.File.UploadedFile!, model.File.Id, new List<string>() {".pdf", ".docx", ".xlsx", ".csv"});
                _fileManager.AddFile(file);
                await _postsOperations.SaveChanges();
            }
            BR_Document? image = null;
            if (model.Image != null)
            {
                image = await _fileManager.GetFileInfo(model.Image.UploadedFile!, model.Image.Id, new List<string>() { ".jpg", ".jpeg", ".png" });
                _fileManager.AddFile(image);
                await _postsOperations.SaveChanges();
            }
            var post = new BR_Post();
            post = await _postsOperations.GetPostValues(post, model, loggedUser!, _usersOperations);
            post.ApplicationUser = loggedUser;
            post.Author = loggedUser!.FirstName + " " + loggedUser.LastName;
            post.Document = file;
            post.Image = image;
            post.Slug = post.Title != null ? model.Title!.Trim().Replace(" ", "-") + "-" + Guid.NewGuid() : null;
            string message = post.Approved ? "Post Created Successfully" : "Waiting for approval";
            _postsOperations.AddPost(post);
            _notification.Success(message);
            await _postsOperations.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postsOperations.GetPostWithFilesById(id);
            if (post == null) return View();
            var loggedUser = await _usersOperations.GetLoggedUser(User);
            var loggedUserRole = await _usersOperations.GetUserRole(loggedUser!);
            if (loggedUserRole[0] != Roles.Admin && loggedUser!.Id != post.ApplicationUser!.Id)
            {
                _notification.Error("You are not authorized");
                return RedirectToAction("Index");
            }
            var edit = _postsOperations.GetEditViewModel(post);
            return View(edit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CreatePostViewModel vm) 
        {
            if (!ModelState.IsValid) return View(vm);
            var loggedUser = await _usersOperations.GetLoggedUser(User);
            var post = await _postsOperations.GetPostWithFilesById(vm.Id);
            if (post == null) return View();
            post = await _postsOperations.GetPostValues(post, vm, loggedUser!, _usersOperations);
            post.Document = vm.File != null ? await _fileManager.GetFileInfo(vm.File.UploadedFile!, vm.File.Id, new List<string>() { ".pdf", ".docx", ".xlsx", ".csv" }) : post.Document;
            post.Image = vm.Image != null ? await _fileManager.GetFileInfo(vm.Image.UploadedFile!, vm.Image.Id, new List<string>() { ".jpg", ".jpeg", ".png" }) : post.Image;
            string message = post.Approved == true ? "Post Updated Successfully" : "Post Updated. Waiting for approval";
            await _postsOperations.SaveChanges();
            _notification.Success(message);
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id) // Deletes the post with the comments, document and image related
        {
            var post = await _postsOperations.GetPostById(id);
            var loggedUser = await _usersOperations.GetLoggedUser(User);
            var loggedUserRole = await _usersOperations.GetUserRole(loggedUser!);
            if (loggedUserRole[0] == Roles.Admin || loggedUser!.Id == post!.ApplicationUser!.Id)
            {
                _postsOperations.removePostComments(post!);
                _postsOperations.removePostDocument(post!);
                _postsOperations.removePostImage(post!);
                _postsOperations.RemovePost(post!);
                await _postsOperations.SaveChanges();
                _notification.Success("Post Deleted Successfully");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id) //Admin approves the post
        {
            var post = await _postsOperations.GetPostById(id);
            if (post == null)
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            post.Approved = true;
            _notification.Success("Post was approved");
            _postsOperations.UpdatePost(post);
            await _postsOperations.SaveChanges();
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }
    }
}