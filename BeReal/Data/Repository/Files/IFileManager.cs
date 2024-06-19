using BeReal.Models;

namespace BeReal.Data.Repository.Files
{
    public interface IFileManager
    {
        List<int> Pages(int PageNumber, int PageCount); //pagination
        void AddFile(BR_Document file); 
        Task<BR_Document?> GetFileById(int? id); 
        Task<BR_Document> GetFileInfo(IFormFile file, int id, List<string> suffixes); //return a file or image to add in db
        Task<(byte[], string, string)> GetFile(int? id, IFileManager _fileManager); //return a file or image for download or display
    }
}
