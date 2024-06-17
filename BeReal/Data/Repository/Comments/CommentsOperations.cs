using BeReal.Models;
using Microsoft.EntityFrameworkCore;

namespace BeReal.Data.Repository.Comments
{
    public class CommentsOperations : ICommentsOperations
    {
        private readonly ApplicationDbContext _context;
        public CommentsOperations(ApplicationDbContext context)
        {
            _context = context;
        }
        //Comments
        public async Task<BR_Comment?> GetCommentById(int id) => await _context.BR_Comments.FirstOrDefaultAsync(c => c.IDBR_Comment == id);
        public void AddComment(BR_Comment comment) => _context.BR_Comments.Add(comment);
        public void UpdateComment(BR_Comment comment) => _context.BR_Comments.Update(comment);
        public void RemoveReplies(BR_Comment comment) => _context.BR_Comments.RemoveRange(comment.Replies!);
        public void RemoveComment(BR_Comment comment) => _context.BR_Comments.Remove(comment);
        public int GetCommentCount(string id) => _context.BR_Comments.Where(x => x.ApplicationUser!.Id == id).Count();
        public async Task<BR_Comment?> GetCommentWithReplies(int id) => await _context.BR_Comments.Include(c => c.Replies).FirstOrDefaultAsync(comment => comment.IDBR_Comment == id);
        //Save Changes
        public async Task<bool> SaveChanges() => await _context.SaveChangesAsync() > 0;
        public string TimeAgo(BR_Comment comment)
        {
            TimeSpan timeDifference = DateTime.Now - comment.Created;
            string timeAgo = string.Empty;
            if (timeDifference.TotalMinutes < 60)
            {
                timeAgo = $"{Math.Floor(timeDifference.TotalMinutes)} minutes ago";
            }
            else if (timeDifference.TotalHours < 24)
            {
                timeAgo = $"{Math.Floor(timeDifference.TotalHours)} hours ago";
            }
            else if (timeDifference.TotalDays < 30)
            {
                timeAgo = $"{Math.Floor(timeDifference.TotalDays)} days ago";
            }
            else if (timeDifference.TotalDays < 365)
            {
                timeAgo = $"{Math.Floor(timeDifference.TotalDays / 30)} months ago";
            }
            else
            {
                timeAgo = $"{Math.Floor(timeDifference.TotalDays / 365)} years ago";
            }
            return timeAgo;
        }
    }
}
