using CoderNexues.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoderNexues.Controllers
{
    public class HomeController : Controller
    {
        private readonly CN_DbContext _context;

        public HomeController(CN_DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var activeCamps = await _context.Camps
                .Where(c => c.Status == "Active")
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();

            return View(activeCamps);
        }

        public IActionResult Dedication()
        {
            return View();
        }
    }
}