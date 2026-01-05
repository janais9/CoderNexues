using CoderNexues.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoderNexues.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly CN_DbContext _context;

        public ProfileController(CN_DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue("UserID"));

            var user = await _context.Users
                .Include(u => u.CampUsers).ThenInclude(cu => cu.Camp)
                .Include(u => u.Submissions)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            return View(user);
        }
    }
}