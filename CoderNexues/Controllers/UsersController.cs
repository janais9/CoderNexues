using CoderNexues.Data;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoderNexues.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly CN_DbContext _context;

        public UsersController(CN_DbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public IActionResult ChangeRole(int id, string newRole)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.Role = newRole; 
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users
                .Include(u => u.Submissions)     
                .Include(u => u.CampUsers)       
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user != null)
            {
                var evaluationsMade = _context.Evaluations.Where(e => e.EvaluatorID == id);
                _context.Evaluations.RemoveRange(evaluationsMade);

                foreach (var sub in user.Submissions)
                {
                    var evaluationsReceived = _context.Evaluations.Where(e => e.SubmissionID == sub.SubmissionID);
                    _context.Evaluations.RemoveRange(evaluationsReceived);
                }

                _context.Submissions.RemoveRange(user.Submissions);

                _context.CampUsers.RemoveRange(user.CampUsers);

                _context.Users.Remove(user);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}