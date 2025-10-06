
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using PortfolioBuilder.Data;
using PortfolioBuilder.Models;
using PortfolioBuilder.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=portfolio.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContext>();

// Register a simple email sender that writes emails to the Emails folder
builder.Services.AddSingleton<IEmailSender, SimpleEmailSender>();

var app = builder.Build();

// Ensure upload and email folders exist
var uploadPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads");
if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        // Seed demo user if not exists
        var demo = await userManager.FindByNameAsync("demo");
        if (demo == null)
        {
            demo = new ApplicationUser
            {
                UserName = "demo",
                Email = "demo@example.com",
                FullName = "Demo User",
                Bio = "This is a demo user. Edit this profile from the dashboard.",
                EmailConfirmed = true
            };
            var res = await userManager.CreateAsync(demo, "Demo@1234");
            if (res.Succeeded)
            {
                // Add sample projects and skills
                db.Projects.Add(new Project { UserId = demo.Id, Title = "Portfolio Builder", Description = "A simple portfolio builder app created for demo.", Link = "https://github.com/example/portfolio" });
                db.Projects.Add(new Project { UserId = demo.Id, Title = "IoT Mini Project", Description = "ESP32 based temperature and gas monitor.", Link = "" });
                db.Skills.Add(new Skill { UserId = demo.Id, Name = "C#", Proficiency = 85 });
                db.Skills.Add(new Skill { UserId = demo.Id, Name = "ASP.NET Core", Proficiency = 80 });
                db.SaveChanges();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("DB seed error: " + ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
