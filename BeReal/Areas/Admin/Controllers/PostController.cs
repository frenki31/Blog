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
        public async Task<IActionResult> Index(int? page)
        {
            var posts = new List<BR_Post>();
            var loggedUser = await _usersOperations.GetLoggedUser(User);
            var loggedUserRole = await _usersOperations.GetUserRole(loggedUser!);
            posts = loggedUserRole[0] == Roles.Admin ? await _postsOperations.GetAllPosts() : await _postsOperations.GetPostsOfUser(loggedUser!.Id);
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
        public IActionResult Create()
        {
            return View(new CreatePostViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            if (!ModelState.IsValid) return View(model); 
            var loggedUser = await _usersOperations.GetLoggedUser(User);
            BR_Document? file = null;
            if (model.File != null)
            {
                file = await _fileManager.GetFileInfo(model);
                _fileManager.AddFile(file);
                await _postsOperations.SaveChanges();
            }
            var post = new BR_Post();
            post = await _postsOperations.GetPostValues(post, model, loggedUser!, _usersOperations);
            post.ApplicationUser = loggedUser;
            post.Author = loggedUser!.FirstName + " " + loggedUser.LastName;
            post.Document = file;
            if (post.Title != null)
            {
                string slug = model.Title!.Trim().Replace(" ", "-");
                post.Slug = slug + "-" + Guid.NewGuid();
            }
            post.Image = model.Image != null ? _fileManager.GetImagePath(model.Image) : null;
            string message = post.Approved ? "Post Created Successfully" : "Waiting for approval";
            _postsOperations.AddPost(post);
            _notification.Success(message);
            await _postsOperations.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postsOperations.GetPostWithDocById(id);
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
            var post = await _postsOperations.GetPostWithDocById(vm.Id);
            if (post == null) return View();
            post = await _postsOperations.GetPostValues(post, vm, loggedUser!, _usersOperations);
            post.Document = vm.File != null ? await _fileManager.GetFileInfo(vm) : post.Document;
            if (vm.Image != null)
            {
                if (post.Image != null) 
                    _fileManager.RemoveImage(post.Image);
                post.Image = _fileManager.GetImagePath(vm.Image);
            }
            string message = post.Approved == true ? "Post Updated Successfully" : "Post Updated. Waiting for approval";
            await _postsOperations.SaveChanges();
            _notification.Success(message);
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _postsOperations.GetPostById(id);
            var loggedUser = await _usersOperations.GetLoggedUser(User);
            var loggedUserRole = await _usersOperations.GetUserRole(loggedUser!);
            if (loggedUserRole[0] == Roles.Admin || loggedUser!.Id == post!.ApplicationUser!.Id)
            {
                _postsOperations.RemovePost(post!);
                await _postsOperations.SaveChanges();
                _notification.Success("Post Deleted Successfully");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Download(int? id)
        {
            var (fileData, contentType, fileName) = await _fileManager.DownloadFile(id,_fileManager);
            return File(fileData, contentType, fileName);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
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