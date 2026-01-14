using CoderNexues.Data;
using CoderNexues.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; 
using System.Threading.Tasks;

namespace CoderNexues.Controllers
{
    public class CampsController : Controller
    {
        private readonly CN_DbContext _context;

        public CampsController(CN_DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var camps = await _context.Camps
                                      .Include(c => c.CampUsers) 
                                      .ToListAsync();
            return View(camps);
        }

        // GET: Camps/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camp = await _context.Camps
                .Include(c => c.CampUsers) // جيب المشتركين
                .ThenInclude(cu => cu.User) // جلبنا أسماء المشتركين
                .Include(c => c.Tasks) // جيب المهام
                .Include(c => c.Schedules)
                .Include(c => c.Announcements)
                .FirstOrDefaultAsync(m => m.CampID == id);

            if (camp == null)
            {
                return NotFound();
            }

            return View(camp);
        }

        // GET: Camps/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Camps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("CampID,CampName,Description,StartDate,EndDate,Status")] Camp camp)
        {
            ModelState.Remove("CampUsers");
            ModelState.Remove("Tasks");

            if (ModelState.IsValid)
            {
                _context.Add(camp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(camp);
        }

        // GET: Camps/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camp = await _context.Camps.FindAsync(id);
            if (camp == null)
            {
                return NotFound();
            }
            return View(camp);
        }

        // POST: Camps/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("CampID,CampName,Description,StartDate,EndDate,Status")] Camp camp)
        {
            if (id != camp.CampID)
            {
                return NotFound();
            }

            ModelState.Remove("CampUsers");
            ModelState.Remove("Tasks");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(camp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CampExists(camp.CampID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(camp);
        }

        // GET: Camps/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camp = await _context.Camps
                .FirstOrDefaultAsync(m => m.CampID == id);
            if (camp == null)
            {
                return NotFound();
            }

            return View(camp);
        }

        // POST: Camps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var camp = await _context.Camps.FindAsync(id);
            if (camp != null)
            {
                _context.Camps.Remove(camp);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int id)
        {
            // 1. التأكد أن المستخدم مسجل دخول
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. جيب لمستخدم
            var userIdString = User.FindFirstValue("UserID");
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdString);

            // 3. التحقق: هل الطالب مسجل في معسكر نشط آخر؟
            var existingCamp = await _context.CampUsers
                .Include(cu => cu.Camp)
                .FirstOrDefaultAsync(cu => cu.UserID == userId && cu.Camp.Status == "Active");

            if (existingCamp != null && User.IsInRole("Student"))
            {
                TempData["Error"] = $"عفواً، أنت مسجل بالفعل في معسكر '{existingCamp.Camp.CampName}'. الطلاب مسموح لهم بمعسكر واحد فقط.";
                return RedirectToAction(nameof(Index));
            }

            // 4. تسجيل الطالب
            var campUser = new CampUser
            {
                CampID = id,
                UserID = userId,
                RoleInCamp = User.FindFirst(ClaimTypes.Role)?.Value ?? "Student"
                // هنا نحفظ دوره الحقيقي (عشان لو مدرب انضم، ينحفظ كمدرب مو كطالب)
            };

            _context.CampUsers.Add(campUser);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم الانضمام للمعسكر بنجاح!";
            return RedirectToAction(nameof(Index));
        }

        private bool CampExists(int id)
        {
            return _context.Camps.Any(e => e.CampID == id);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<IActionResult> PostAnnouncement(int campId, string title, string content)
        {
            var announcement = new Announcement
            {
                CampID = campId,
                Title = title,
                Content = content,
                PostedAt = DateTime.Now
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = campId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageParticipants(int id)
        {
            var camp = await _context.Camps
                .Include(c => c.CampUsers)
                .ThenInclude(cu => cu.User)
                .FirstOrDefaultAsync(c => c.CampID == id);

            if (camp == null) return NotFound();

            return View(camp);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveParticipant(int campUserId, int campId)
        {
            var participant = await _context.CampUsers.FindAsync(campUserId);
            if (participant != null)
            {
                _context.CampUsers.Remove(participant);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageParticipants", new { id = campId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageSchedule(int id)
        {
            var camp = await _context.Camps
                .Include(c => c.Schedules)
                .FirstOrDefaultAsync(c => c.CampID == id);

            if (camp == null) return NotFound();
            return View(camp);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddScheduleItem(int campId, DateTime date, string title, string? description, string type)
        {
            var item = new CampSchedule
            {
                CampID = campId,
                Date = date,
                Title = title,
                Description = description ?? "",

                Type = type
            };

            _context.CampSchedules.Add(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageSchedule", new { id = campId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteScheduleItem(int scheduleId, int campId)
        {
            var item = await _context.CampSchedules.FindAsync(scheduleId);
            if (item != null)
            {
                _context.CampSchedules.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageSchedule", new { id = campId });
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyPlan()
        {
            var userId = int.Parse(User.FindFirstValue("UserID"));

            var campUser = await _context.CampUsers
                .Include(c => c.Camp)
                .FirstOrDefaultAsync(cu => cu.UserID == userId);

            if (campUser == null)
            {
                TempData["Error"] = "أنت غير مسجل في أي معسكر بعد.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Details", new { id = campUser.CampID });
        }
    }
}