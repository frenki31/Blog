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
        public DbSet<BR_ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<BR_Post> Posts { get; set; }
        public DbSet<BR_Page> Pages { get; set; }  
        public DbSet<BR_Comment> Comments { get; set; }
        public DbSet<BR_Document> Files { get; set; }
    }
}
