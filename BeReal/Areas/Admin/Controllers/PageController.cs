using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data;
using BeReal.Data.Repository;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PageController : Controller
    {
        private readonly IRepository _repo;
        private readonly IFileManager _fileManager;
        public INotyfService _notification { get; }
        public PageController(INotyfService notification, IRepository repo, IFileManager fileManager)
        {
            _repo = repo;
            _notification = notification;
            _fileManager = fileManager;
        }
        [HttpGet]
        public async Task<IActionResult> About()
        {
            var page = await _repo.getPage("about");
            var pageVm = new PageViewModel()
            {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ImageUrl = page.ImageUrl,
            };
            return View(pageVm);
        }
        [HttpPost]
        public async Task<IActionResult> About(PageViewModel vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var page = await _repo.getPage("about");
            if (page == null)
            {
                _notification.Error("Page not found");
                return View();
            }
            page.Title = vm.Title;
            page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;
            if (vm.Image != null)
            {
                if (page.ImageUrl != null)
                    _fileManager.RemoveImage(page.ImageUrl);
                page.ImageUrl = _fileManager.GetImagePath(vm.Image);
            }
            await _repo.saveChanges();
            _notification.Success("About Page Updated Successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });

        }
        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            var page = await _repo.getPage("contact");
            var pageVm = new PageViewModel()
            {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ImageUrl = page.ImageUrl,
            };
            return View(pageVm);
        }
        [HttpPost]
        public async Task<IActionResult> Contact(PageViewModel vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var page = await _repo.getPage("contact");
            if (page == null)
            {
                _notification.Error("Page not found");
                return View();
            }
            page.Title = vm.Title;
            page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;
            if (vm.Image != null)
            {
                if (page.ImageUrl != null)
                    _fileManager.RemoveImage(page.ImageUrl);
                page.ImageUrl = _fileManager.GetImagePath(vm.Image);
            }
            await _repo.saveChanges();
            _notification.Success("Contact Page Updated Successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });

        }
        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            var page = await _repo.getPage("privacy");
            var pageVm = new PageViewModel()
            {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ImageUrl = page.ImageUrl,
            };
            return View(pageVm);
        }
        [HttpPost]
        public async Task<IActionResult> Privacy(PageViewModel vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var page = await _repo.getPage("privacy");
            if (page == null)
            {
                _notification.Error("Page not found");
                return View();
            }
            page.Title = vm.Title;
            page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;
            if (vm.Image != null)
            {
                if (page.ImageUrl != null) 
                    _fileManager.RemoveImage(page.ImageUrl);
                page.ImageUrl = _fileManager.GetImagePath(vm.Image);
            }
            await _repo.saveChanges();
            _notification.Success("Privacy Page Updated Successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var page = await _repo.getPage("home");
            var vm = new PageViewModel()
            {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ImageUrl = page.ImageUrl,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(PageViewModel vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var page = await _repo.getPage("home");
            if (page == null)
            {
                _notification.Error("Page not found");
                return View(vm);
            }
            page.Title = vm.Title;
            page.Description = vm.Description;
            page.ShortDescription = vm.ShortDescription;
            if (vm.Image != null)
            {
                if (page.ImageUrl != null)
                    _fileManager.RemoveImage(page.ImageUrl);
                page.ImageUrl = _fileManager.GetImagePath(vm.Image);
            }
            await _repo.saveChanges();
            _notification.Success("Home Page updated successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }
        
    }
}
