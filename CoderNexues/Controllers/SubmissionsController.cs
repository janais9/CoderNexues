using CoderNexues.Data;
using CoderNexues.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoderNexues.Controllers
{
    [Authorize] 
    public class SubmissionsController : Controller
    {
        private readonly CN_DbContext _context;

        public SubmissionsController(CN_DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Upload(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null) return NotFound();

            ViewBag.TaskTitle = task.Title;
            ViewBag.TaskID = taskId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int taskId, string submissionLink)
        {
            if (string.IsNullOrEmpty(submissionLink))
            {
                ModelState.AddModelError("", "يجب إرفاق الرابط");
                return View();
            }

            var userId = int.Parse(User.FindFirstValue("UserID"));

            var existingSub = await _context.Submissions
                .FirstOrDefaultAsync(s => s.TaskID == taskId && s.StudentID == userId);

            if (existingSub != null)
            {
                existingSub.SubmissionLink = submissionLink;
                existingSub.SubmittedAt = DateTime.Now;
                _context.Update(existingSub);
                TempData["Success"] = "تم تحديث تسليمك بنجاح!";
            }
            else
            {
                var submission = new Submission
                {
                    TaskID = taskId,
                    StudentID = userId,
                    SubmissionLink = submissionLink,
                    SubmittedAt = DateTime.Now,
                    Status = "Submitted"
                };
                _context.Add(submission);
                TempData["Success"] = "تم رفع الحل بنجاح! 🚀";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Tasks"); 
        }
    }
}