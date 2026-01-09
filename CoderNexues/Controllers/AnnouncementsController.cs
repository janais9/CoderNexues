using CoderNexues.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoderNexues.Controllers
{
    [Authorize]
    public class AnnouncementsController : Controller
    {
        private readonly CN_DbContext _context;

        public AnnouncementsController(CN_DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var allAnnouncements = await _context.Announcements
                    .Include(a => a.Camp)
                    .OrderByDescending(a => a.PostedAt)
                    .ToListAsync();
                return View(allAnnouncements);
            }

            var userId = int.Parse(User.FindFirstValue("UserID"));

            var userCampIds = await _context.CampUsers
                .Where(cu => cu.UserID == userId)
                .Select(cu => cu.CampID)
                .ToListAsync();

            var myAnnouncements = await _context.Announcements
                .Include(a => a.Camp)
                .Where(a => userCampIds.Contains(a.CampID))
                .OrderByDescending(a => a.PostedAt)
                .ToListAsync();

            return View(myAnnouncements);
        }
    }
}