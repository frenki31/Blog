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
        public async Task<BR_Comment?> getCommentById(int id) => await _context.Comments.FirstOrDefaultAsync(c => c.IDBR_Comment == id);
        public void addComment(BR_Comment comment) => _context.Comments.Add(comment);
        public void removeReplies(BR_Comment comment) => _context.Comments.RemoveRange(comment.Replies!);
        public void removeComment(BR_Comment comment) => _context.Comments.Remove(comment);
        public int getCommentCount(string id) => _context.Comments.Where(x => x.ApplicationUser!.Id == id).Count();
        public async Task<BR_Comment?> getCommentWithReplies(int id) => await _context.Comments.Include(c => c.Replies).FirstOrDefaultAsync(comment => comment.IDBR_Comment == id);
        //Save Changes
        public async Task<bool> saveChanges() => await _context.SaveChangesAsync() > 0;
    }
}
