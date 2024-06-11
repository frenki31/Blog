using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BeReal.Models;
using BeReal.Utilities;
using X.PagedList;
using BeReal.Data.Repository;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostController : Controller
    {
        public readonly IRepository _repo;
        public readonly IFileManager _fileManager;
        public INotyfService _notification { get; }
        public PostController(INotyfService notification,IRepository repo,IFileManager fileManager)
        {
            _notification = notification;
            _fileManager = fileManager;
            _repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            var posts = new List<Post>();
            var loggedUser = await _repo.getLoggedUser(User);
            var loggedUserRole = await _repo.getUserRole(loggedUser!);
            if (loggedUserRole[0] == Roles.Admin)
            {
                posts = await _repo.getAllPosts();
            }
            else
            {
                posts = await _repo.getPostsOfUser(loggedUser!.Id);
            }
            var pageSize = 5;
            var pageNumber = (page ?? 1);
            var postVms = posts.Select(x => new PostViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                ShortDescription = x.ShortDescription,
                Author = x.Author,
                publicationDate = x.publicationDate,
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
            if (!ModelState.IsValid) { return View(model); }
            var loggedUser = await _repo.getLoggedUser(User);
            var loggedUserRole = await _repo.getUserRole(loggedUser!);
            Document? file = null;
            if (model.File != null)
            {
                file = await _fileManager.GetFileInfo(model);
                _fileManager.AddFile(file);
                await _repo.saveChanges();
            }
            var post = new Post
            {
                Title = model.Title,
                ShortDescription = model.ShortDescription,
                Description = model.Description,
                Category = model.Category,
                Tags = model.Tags,
                User = loggedUser,
                Author = loggedUser!.FirstName + " " + loggedUser.LastName,
                Document = file,
            };
            if (post.Title != null)
            {
                string slug = model.Title!.Trim();
                slug = slug.Replace(" ", "-");
                post.Slug = slug + "-" + Guid.NewGuid();
            }
            if (model.Image != null)
            {
                post.Image = _fileManager.GetImagePath(model.Image);
            }
            if (loggedUserRole[0] == Roles.Admin)
            {
                post.Approved = true;
                _notification.Success("Post Created Successfully");
            }else if (loggedUserRole[0] == Roles.User)
            {
                post.Approved = false;
                _notification.Success("Waiting for approval");
            }
            _repo.addPost(post);
            await _repo.saveChanges();
            
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _repo.getPostWithDocById(id);
            if (post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            
            var loggedUser = await _repo.getLoggedUser(User);
            var loggedUserRole = await _repo.getUserRole(loggedUser!);
            if (loggedUserRole[0] != Roles.Admin && loggedUser!.Id != post.User!.Id)
            {
                _notification.Error("You are not authorized");
                return RedirectToAction("Index");
            }
            var edit = new CreatePostViewModel { 
                Id = post.Id,
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                Description = post.Description,
                ImageUrl = post.Image,
                Category = post.Category,
                Tags = post.Tags,
                Approved = post.Approved,
            };
            if (post.Document != null)
            {
                edit.File = new FileViewModel { Id = post.Document!.Id, ContentType = post.Document.ContentType, Name = post.Document.FileName };
            }
            return View(edit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CreatePostViewModel vm) 
        { 
            var loggedUser = await _repo.getLoggedUser(User);
            var loggedUserRole = await _repo.getUserRole(loggedUser!);
            if (!ModelState.IsValid) {  return View(vm); }
            var post = await _repo.getPostWithDocById(vm.Id);
            if (post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            post.Title = vm.Title;
            post.ShortDescription = vm.ShortDescription;
            post.Description = vm.Description;
            post.Category = vm.Category;
            post.Tags = vm.Tags;
            post.Document = vm.File != null ? await _fileManager.GetFileInfo(vm) : post.Document;
            if (vm.Image != null)
            {
                if (post.Image != null) 
                    _fileManager.RemoveImage(post.Image);
                post.Image = _fileManager.GetImagePath(vm.Image);
            }
            post.Approved = loggedUserRole[0] == Roles.Admin ? true : false;
            string message = post.Approved == true ? "Post Updated Successfully" : "Post Updated. Waiting for approval";
            await _repo.saveChanges();
            _notification.Success(message);
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _repo.getPostById(id);
            var loggedUser = await _repo.getLoggedUser(User);
            var loggedUserRole = await _repo.getUserRole(loggedUser!);
            if (loggedUserRole[0] == Roles.Admin || loggedUser!.Id == post!.User!.Id)
            {
                _repo.removePost(post!);
                await _repo.saveChanges();
                _notification.Success("Post Deleted Successfully");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var file = await _fileManager.GetFileById(id);
            if (file == null)
            {
                return NotFound();
            }
            return File(file.Data!, file.ContentType!, file.FileName);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var post = await _repo.getPostById(id);
            if (post == null)
            {
                _notification.Error("Post does not exist");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            post.Approved = true;
            _notification.Success("Post was approved");
            _repo.updatePost(post);
            await _repo.saveChanges();
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }
        
    }
}
