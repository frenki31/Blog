using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Models;
using BeReal.Data.Repository.Files;
using BeReal.Data.Repository.Pages;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PageController : Controller
    {
        private readonly IPagesOperations _pagesOperations;
        private readonly IFileManager _fileManager;
        public INotyfService _notification { get; }
        public PageController(INotyfService notification, IPagesOperations pagesOperations, IFileManager fileManager)
        {
            _pagesOperations = pagesOperations;
            _notification = notification;
            _fileManager = fileManager;
        }
        [HttpGet]
        public async Task<IActionResult> About()
        {
            var page = await _pagesOperations.getPage("about");
            return View(_pagesOperations.GetPageViewModel(page!));
        }
        [HttpPost]
        public async Task<IActionResult> About(PageViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm); 
            var page = await _pagesOperations.getPage("about");
            if (page == null) return View();
            _pagesOperations.UpdatePage(vm, page, _fileManager);
            await _pagesOperations.saveChanges();
            _notification.Success("About Page Updated Successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }
        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            var page = await _pagesOperations.getPage("contact");
            return View(_pagesOperations.GetPageViewModel(page!));
        }
        [HttpPost]
        public async Task<IActionResult> Contact(PageViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var page = await _pagesOperations.getPage("contact");
            if (page == null) return View();
            _pagesOperations.UpdatePage(vm, page, _fileManager);
            await _pagesOperations.saveChanges();
            _notification.Success("Contact Page Updated Successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }
        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            var page = await _pagesOperations.getPage("privacy");
            return View(_pagesOperations.GetPageViewModel(page!));
        }
        [HttpPost]
        public async Task<IActionResult> Privacy(PageViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var page = await _pagesOperations.getPage("privacy");
            if (page == null) return View();
            _pagesOperations.UpdatePage(vm, page, _fileManager);
            await _pagesOperations.saveChanges();
            _notification.Success("Privacy Page Updated Successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var page = await _pagesOperations.getPage("home");
            return View(_pagesOperations.GetPageViewModel(page!));
        }
        [HttpPost]
        public async Task<IActionResult> Index(PageViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm); 
            var page = await _pagesOperations.getPage("home");
            if (page == null) return View(vm);
            _pagesOperations.UpdatePage(vm, page,_fileManager);
            await _pagesOperations.saveChanges();
            _notification.Success("Home Page updated successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }
    }
}
