using CoderNexues.Data;
using CoderNexues.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoderNexues.Controllers
{
    [Authorize]
    public class CertificatesController : Controller
    {
        private readonly CN_DbContext _context;

        public CertificatesController(CN_DbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage(int campId)
        {
            var reqs = await _context.CertificateRequirements
                .Where(r => r.CampID == campId).ToListAsync();

            ViewBag.CampID = campId;
            return View(reqs);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CertificateRequirement req)
        {
            _context.Add(req);
            await _context.SaveChangesAsync();
            return RedirectToAction("Manage", new { campId = req.CampID });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRequirement(int id)
        {
            var req = await _context.CertificateRequirements.FindAsync(id);
            if (req != null)
            {
                _context.CertificateRequirements.Remove(req);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Manage", new { campId = req?.CampID });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditRequirement(int id)
        {
            var req = await _context.CertificateRequirements.FindAsync(id);
            if (req == null) return NotFound();
            return View(req);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditRequirement(CertificateRequirement req)
        {
            _context.Update(req);
            await _context.SaveChangesAsync();
            return RedirectToAction("Manage", new { campId = req.CampID });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteSubmission(int id)
        {
            var cert = await _context.StudentCertificates.FindAsync(id);
            if (cert == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue("UserID"));
            if (!User.IsInRole("Admin") && cert.StudentID != userId)
            {
                return Forbid(); 
            }

            _context.StudentCertificates.Remove(cert);
            await _context.SaveChangesAsync();

            if (User.IsInRole("Admin"))
                return RedirectToAction("Submissions", new { reqId = cert.ReqID });

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Submissions(int reqId)
        {
            var requirement = await _context.CertificateRequirements.FindAsync(reqId);
            if (requirement == null) return NotFound();

            var submissions = await _context.StudentCertificates
                .Include(s => s.Student)
                .Where(s => s.ReqID == reqId)
                .ToListAsync();

            ViewBag.CertName = requirement.CertificateName;
            ViewBag.ReqID = reqId; 

            return View(submissions);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue("UserID"));

            var campUser = await _context.CampUsers.FirstOrDefaultAsync(cu => cu.UserID == userId);
            if (campUser == null) return View("NoCamp");

            var requirements = await _context.CertificateRequirements
                .Where(r => r.CampID == campUser.CampID).ToListAsync();

            var myCerts = await _context.StudentCertificates
                .Where(c => c.StudentID == userId).ToListAsync();

            ViewBag.MyCerts = myCerts;
            return View(requirements);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Submit(int reqId, bool hasTaken, DateTime? examDate, int? score, bool isPassed)
        {
            var userId = int.Parse(User.FindFirstValue("UserID"));

            var cert = new StudentCertificate
            {
                ReqID = reqId,
                StudentID = userId,
                HasTakenExam = hasTaken,
                ExamDate = examDate,
                Score = score,
                IsPassed = isPassed
            };

            _context.Add(cert);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}