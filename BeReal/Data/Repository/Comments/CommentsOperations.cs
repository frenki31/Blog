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
        public async Task<BR_Comment?> GetCommentById(int id) => await _context.Comments.FirstOrDefaultAsync(c => c.IDBR_Comment == id);
        public void AddComment(BR_Comment comment) => _context.Comments.Add(comment);
        public void RemoveReplies(BR_Comment comment) => _context.Comments.RemoveRange(comment.Replies!);
        public void RemoveComment(BR_Comment comment) => _context.Comments.Remove(comment);
        public int GetCommentCount(string id) => _context.Comments.Where(x => x.ApplicationUser!.Id == id).Count();
        public async Task<BR_Comment?> GetCommentWithReplies(int id) => await _context.Comments.Include(c => c.Replies).FirstOrDefaultAsync(comment => comment.IDBR_Comment == id);
        //Save Changes
        public async Task<bool> SaveChanges() => await _context.SaveChangesAsync() > 0;
    }
}
