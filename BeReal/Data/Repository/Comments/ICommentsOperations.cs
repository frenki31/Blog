using BeReal.Models;

namespace BeReal.Data.Repository.Comments
{
    public interface ICommentsOperations
    {
        //Comments
        Task<BR_Comment?> getCommentById(int id);
        Task<BR_Comment?> getCommentWithReplies(int id);
        void addComment(BR_Comment comment);
        void removeReplies(BR_Comment comment);
        void removeComment(BR_Comment comment);
        int getCommentCount(string id);
        //Save Changes
        Task<bool> saveChanges();
    }
}
