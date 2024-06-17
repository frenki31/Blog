using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BeReal.Models;

namespace BeReal.Data
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
        }
        public DbSet<BR_ApplicationUser> BR_ApplicationUsers { get; set; }
        public DbSet<BR_Post> BR_Posts { get; set; }
        public DbSet<BR_Page> BR_Pages { get; set; }  
        public DbSet<BR_Comment> BR_Comments { get; set; }
        public DbSet<BR_Document> BR_Files { get; set; }
        public DbSet<BR_Category> BR_Categories { get; set; }
    }
}
