using BeReal.Models;

namespace BeReal.Data.Repository.Comments
{
    public interface ICommentsOperations
    {
        //Comments
        Task<BR_Comment?> GetCommentById(int id); 
        Task<BR_Comment?> GetCommentWithReplies(int id); //include replies for a comment
        void AddComment(BR_Comment comment);
        void UpdateComment(BR_Comment comment);
        void RemoveReplies(BR_Comment comment); //remove the replies of a comment
        void RemoveComment(BR_Comment comment);
        int GetCommentCount(string id);
        string TimeAgo(BR_Comment comment); //check when was the comment posted
        //Save Changes
        Task<bool> SaveChanges();
    }
}
