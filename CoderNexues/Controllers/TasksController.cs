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
            var tasks = await _context.Tasks.Include(t => t.Camp).ToListAsync();
            return View(tasks);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var task = await _context.Tasks
                .Include(t => t.Camp)
                .FirstOrDefaultAsync(m => m.TaskID == id);

            if (task == null) return NotFound();

            return View(task);
        }

        [Authorize(Roles = "Admin,Trainer")]
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
            if (ModelState.IsValid)
            {
                _context.Add(c_task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CampID"] = new SelectList(_context.Camps, "CampID", "CampName", c_task.CampID);
            return View(c_task);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

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