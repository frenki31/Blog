using BeReal.Models;

namespace BeReal.Data.Repository.Comments
{
    public interface ICommentsOperations
    {
        //Comments
        Task<BR_Comment?> GetCommentById(int id);
        Task<BR_Comment?> GetCommentWithReplies(int id);
        void AddComment(BR_Comment comment);
        void UpdateComment(BR_Comment comment);
        void RemoveReplies(BR_Comment comment);
        void RemoveComment(BR_Comment comment);
        int GetCommentCount(string id);
        string TimeAgo(BR_Comment comment);
        //Save Changes
        Task<bool> SaveChanges();
    }
}
