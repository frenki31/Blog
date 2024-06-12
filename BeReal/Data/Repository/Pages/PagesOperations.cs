using Microsoft.EntityFrameworkCore;
using BeReal.Models;
using BeReal.ViewModels;
using BeReal.Data.Repository.Files;

namespace BeReal.Data.Repository.Pages
{
    public class PagesOperations : IPagesOperations
    {
        private readonly ApplicationDbContext _context;
        public PagesOperations(ApplicationDbContext context) {
            _context = context;
        }
        //Pages
        public async Task<BR_Page?> getPage(string slug) => await _context.Pages.FirstOrDefaultAsync(x => x.Slug == slug);
        public PageViewModel GetPageViewModel(BR_Page page)
        {
            var vm = new PageViewModel()
            {
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ImageUrl = page.ImageUrl,
            };
            return vm;
        }
        public BR_Page UpdatePage(PageViewModel vm, BR_Page page, IFileManager _fileManager)
        {
            page.Title = vm.Title;
            page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;
            if (vm.Image != null)
            {
                if (page.ImageUrl != null)
                    _fileManager.RemoveImage(page.ImageUrl);
                page.ImageUrl = _fileManager.GetImagePath(vm.Image);
            }
            return page;
        }
        //Save Changes
        public async Task<bool> saveChanges() => await _context.SaveChangesAsync() > 0;
    }
}
