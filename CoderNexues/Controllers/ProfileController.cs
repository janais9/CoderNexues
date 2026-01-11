using CoderNexues.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
                .Include(u => u.Submissions).ThenInclude(s => s.Task)
                .Include(u => u.Submissions).ThenInclude(s => s.Evaluation)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = int.Parse(User.FindFirstValue("UserID"));
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string FullName, string Email, string? NewPassword)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FullName = FullName;
            user.Email = Email;

            if (!string.IsNullOrEmpty(NewPassword))
            {
                using (var sha256 = SHA256.Create())
                {
                    var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(NewPassword));
                    var builder = new StringBuilder();
                    foreach (var b in bytes)
                    {
                        builder.Append(b.ToString("x2"));
                    }
                    user.PasswordHash = builder.ToString();
                }
            }

            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MyGrades()
        {
            var userId = int.Parse(User.FindFirstValue("UserID"));

            var user = await _context.Users
                .Include(u => u.Submissions).ThenInclude(s => s.Task).ThenInclude(t => t.Camp)
                .Include(u => u.Submissions).ThenInclude(s => s.Evaluation)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            return View(user);
        }

    }
}