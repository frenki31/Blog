using BeReal.Data;
using Microsoft.AspNetCore.Identity;
using BeReal.Models;

namespace BeReal.Utilities
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BR_ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDbContext context, UserManager<BR_ApplicationUser> userManager,
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
                    var admin = new BR_ApplicationUser()
                    {
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        FirstName = "admin",
                        LastName = "admin"
                    };
                    var result = _userManager.CreateAsync(admin, "Admin@1234").GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(admin, Roles.Admin).GetAwaiter().GetResult();
                    var token = _userManager.GenerateEmailConfirmationTokenAsync(admin).GetAwaiter().GetResult();   
                    _userManager.ConfirmEmailAsync(admin, token).GetAwaiter().GetResult();
                }
                var pages = new List<BR_Page>()
                {
                    new BR_Page() {
                        Title = "Home",
                        Slug = "home"
                    },
                    new BR_Page() {
                        Title = "About",
                        Slug = "about"
                    },
                    new BR_Page() {
                        Title = "Contact",
                        Slug = "contact"
                    },
                    new BR_Page() {
                        Title = "Privacy Policy",
                        Slug = "privacy"
                    },
                };
                foreach (var page in pages)
                {
                    if (_context.BR_Pages.FirstOrDefault(x => x.Slug == page.Slug) == null)
                        _context.BR_Pages.AddRange(page);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
