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
        public async Task<BR_Page?> GetPage(string slug) => await _context.BR_Pages.FirstOrDefaultAsync(x => x.Slug == slug);
        public PageViewModel GetPageViewModel(BR_Page page)
        {
            var vm = new PageViewModel()
            {
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
            };
            return vm;
        }
        public BR_Page UpdatePage(PageViewModel vm, BR_Page page, IFileManager _fileManager)
        {
            page.Title = vm.Title;
            page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;
            return page;
        }
        //Save Changes
        public async Task<bool> SaveChanges() => await _context.SaveChangesAsync() > 0;
    }
}
