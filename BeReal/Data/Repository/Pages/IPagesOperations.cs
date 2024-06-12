using BeReal.Data.Repository.Files;
using BeReal.Models;
using BeReal.ViewModels;
namespace BeReal.Data.Repository.Pages
{
    public interface IPagesOperations
    {
        //Pages
        Task<BR_Page?> getPage(string slug);
        PageViewModel GetPageViewModel(BR_Page page);
        BR_Page UpdatePage(PageViewModel vm, BR_Page page, IFileManager _fileManager);
        //Save Changes
        Task<bool> saveChanges();
    }
}
