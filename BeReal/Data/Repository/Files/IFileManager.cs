using BeReal.Models;
using BeReal.ViewModels;

namespace BeReal.Data.Repository.Files
{
    public interface IFileManager
    {
        List<int> Pages(int PageNumber, int PageCount);
        string GetImagePath(IFormFile formFile);
        bool RemoveImage(string image);
        void AddFile(BR_Document file);
        Task<BR_Document?> GetFileById(int? id);
        Task<BR_Document> GetFileInfo(CreatePostViewModel vm);
    }
}
