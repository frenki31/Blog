using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using BeReal.Data;
using BeReal.Data.Repository.Comments;
using BeReal.Data.Repository.Email;
using BeReal.Data.Repository.Files;
using BeReal.Data.Repository.Pages;
using BeReal.Data.Repository.Posts;
using BeReal.Data.Repository.Users;
using BeReal.Models;
using BeReal.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")));

builder.Services.AddIdentity<BR_ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var configurationBuilder = new ConfigurationBuilder().AddUserSecrets<Program>().Build(); 

builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGridSettings"));
builder.Services.AddSendGrid(options =>
{
    options.ApiKey = configurationBuilder["ApiKey"];
});

builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IPostsOperations, PostsOperations>();
builder.Services.AddTransient<IPagesOperations, PagesOperations>();
builder.Services.AddTransient<IUsersOperations, UsersOperations>();
builder.Services.AddTransient<ICommentsOperations, CommentsOperations>();
builder.Services.AddTransient<IFileManager, FileManager>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/accessDenied";
    options.Cookie.Name = "AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

builder.Services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<IDbInitializer>().Initialize();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseNotyf();

app.MapControllerRoute(
    name: "area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();