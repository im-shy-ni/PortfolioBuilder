
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioBuilder.Data;
using System.Threading.Tasks;

namespace PortfolioBuilder.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly ApplicationDbContext _db;
        public PortfolioController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> View(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == id);
            if (user == null) return NotFound();
            var projects = await _db.Projects.Where(p => p.UserId == user.Id).ToListAsync();
            var skills = await _db.Skills.Where(s => s.UserId == user.Id).ToListAsync();
            ViewBag.User = user;
            ViewBag.Projects = projects;
            ViewBag.Skills = skills;
            return View();
        }
    }
}
