using BeReal.Data;
using Microsoft.AspNetCore.Identity;
using BeReal.Models;

namespace BeReal.Utilities
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            try
            {
                if (!_context.Roles.Any())
                {
                    _roleManager.CreateAsync(new IdentityRole(Roles.Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(Roles.User)).GetAwaiter().GetResult();
                }
                if (!_context.Users.Any(x => x.UserName == "admin"))
                {
                    var admin = new ApplicationUser()
                    {
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        FirstName = "admin",
                        LastName = "admin"
                    };
                    var result = _userManager.CreateAsync(admin, "Admin@1234").GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(admin, Roles.Admin).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
