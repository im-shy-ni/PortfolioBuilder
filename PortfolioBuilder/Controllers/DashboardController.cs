
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioBuilder.Data;
using PortfolioBuilder.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace PortfolioBuilder.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public DashboardController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.User = user;
            var projects = _db.Projects.Where(p => p.UserId == user.Id).ToList();
            var skills = _db.Skills.Where(s => s.UserId == user.Id).ToList();
            ViewBag.Projects = projects;
            ViewBag.Skills = skills;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(string title, string description, string link)
        {
            var user = await _userManager.GetUserAsync(User);
            _db.Projects.Add(new Project { UserId = user.Id, Title = title, Description = description, Link = link });
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddSkill(string name, int proficiency = 0)
        {
            var user = await _userManager.GetUserAsync(User);
            _db.Skills.Add(new Skill { UserId = user.Id, Name = name, Proficiency = proficiency });
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(Microsoft.AspNetCore.Http.IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                var fileName = user.UserName + Path.GetExtension(file.FileName);
                var path = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                user.ProfilePictureUrl = "/uploads/" + fileName;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var p = _db.Projects.FirstOrDefault(x => x.Id == id && x.UserId == user.Id);
            if (p != null) { _db.Projects.Remove(p); _db.SaveChanges(); }
            return RedirectToAction("Index"); 
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var s = _db.Skills.FirstOrDefault(x => x.Id == id && x.UserId == user.Id);
            if (s != null) { _db.Skills.Remove(s); _db.SaveChanges(); }
            return RedirectToAction("Index"); 
        }

        [HttpPost]
        public async Task<IActionResult> EditProject(int id, string title, string description, string link)
        {
            var user = await _userManager.GetUserAsync(User);
            var p = _db.Projects.FirstOrDefault(x => x.Id == id && x.UserId == user.Id);
            if (p != null) { p.Title = title; p.Description = description; p.Link = link; _db.SaveChanges(); }
            return RedirectToAction("Index"); 
        }

        [HttpPost]
        public async Task<IActionResult> EditSkill(int id, string name, int proficiency = 0)
        {
            var user = await _userManager.GetUserAsync(User);
            var s = _db.Skills.FirstOrDefault(x => x.Id == id && x.UserId == user.Id);
            if (s != null) { s.Name = name; s.Proficiency = proficiency; _db.SaveChanges(); }
            return RedirectToAction("Index"); 
        }
    }
}
