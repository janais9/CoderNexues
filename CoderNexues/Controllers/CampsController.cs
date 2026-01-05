using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoderNexues.Data;
using CoderNexues.Models;
using System.Security.Claims; 

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
                .Include(c => c.CampUsers) // جلبنا المشتركين
                .ThenInclude(cu => cu.User) // جلبنا أسماء المشتركين
                .Include(c => c.Tasks) // جلبنا المهام
                .FirstOrDefaultAsync(m => m.CampID == id);

            if (camp == null)
            {
                return NotFound();
            }

            return View(camp);
        }

        // GET: Camps/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Camps/Create
        // تم التعديل: إزالة التحقق من CampUsers و Tasks لحل مشكلة الحفظ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CampID,CampName,Description,StartDate,EndDate,Status")] Camp camp)
        {
            // نتجاهل القوائم لأنها فارغة وقت الإنشاء
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
        public async Task<IActionResult> Edit(int id, [Bind("CampID,CampName,Description,StartDate,EndDate,Status")] Camp camp)
        {
            if (id != camp.CampID)
            {
                return NotFound();
            }

            // نتجاهل القوائم وقت التعديل أيضاً
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

        // 👇👇 دالة الانضمام للمعسكر (جديدة) 👇👇
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int id)
        {
            // 1. التأكد أن المستخدم مسجل دخول
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. جلب رقم المستخدم
            var userIdString = User.FindFirstValue("UserID");
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdString);

            // 3. التحقق: هل الطالب مسجل في معسكر نشط آخر؟
            var existingCamp = await _context.CampUsers
                .Include(cu => cu.Camp)
                .FirstOrDefaultAsync(cu => cu.UserID == userId && cu.Camp.Status == "Active");

            if (existingCamp != null)
            {
                // إذا كان مسجل مسبقاً، نرسل رسالة خطأ
                TempData["Error"] = $"عفواً، أنت مسجل بالفعل في معسكر '{existingCamp.Camp.CampName}'.";
                return RedirectToAction(nameof(Index));
            }

            // 4. تسجيل الطالب
            var campUser = new CampUser
            {
                CampID = id,
                UserID = userId,
                RoleInCamp = "Student"
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
    }
}