using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoderNexues.Data;
using CoderNexues.Models;
using Microsoft.AspNetCore.Authorization;

namespace CoderNexues.Controllers
{
    public class TasksController : Controller
    {
        private readonly CN_DbContext _context;

        public TasksController(CN_DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tasksQuery = _context.Tasks.Include(t => t.Camp).AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                userId = int.Parse(User.FindFirst("UserID").Value);

                var myCampIds = await _context.CampUsers
                    .Where(cu => cu.UserID == userId)
                    .Select(cu => cu.CampID)
                    .ToListAsync();

                tasksQuery = tasksQuery.Where(t => myCampIds.Contains(t.CampID));
            }

            return View(await tasksQuery.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var task = await _context
                .Tasks.Include(t => t.Camp)
                .FirstOrDefaultAsync(m => m.TaskID == id);
            if (task == null) return NotFound();
            return View(task);
        }

        [Authorize(Roles = "Trainer")]
        public IActionResult Create()
        {
            ViewData["CampID"] = new SelectList(_context.Camps, "CampID", "CampName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<IActionResult> Create(C_Task c_task)
        {
            ModelState.Remove("Camp");
            ModelState.Remove("Submissions");

            if (ModelState.IsValid)
            {
                _context.Add(c_task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CampID"] = new SelectList(_context.Camps, "CampID", "CampName", c_task.CampID);
            return View(c_task);
        }

        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            ViewData["CampID"] = new SelectList(_context.Camps, "CampID", "CampName", task.CampID);
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Edit(int id, C_Task c_task)
        {
            if (id != c_task.TaskID) return NotFound();

            ModelState.Remove("Camp");
            ModelState.Remove("Submissions");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(c_task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Tasks.Any(e => e.TaskID == c_task.TaskID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CampID"] = new SelectList(_context.Camps, "CampID", "CampName", c_task.CampID);
            return View(c_task);
        }


        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var task = await _context.Tasks.Include(t => t.Camp).FirstOrDefaultAsync(m => m.TaskID == id);
            if (task == null) return NotFound();
            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}