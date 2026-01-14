using CoderNexues.Data;
using CoderNexues.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoderNexues.Controllers
{
    [Authorize(Roles = "Admin,Trainer")] 
    public class EvaluationsController : Controller
    {
        private readonly CN_DbContext _context;

        public EvaluationsController(CN_DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Review(int taskId)
        {
            var submissions = await _context.Submissions
                .Include(s => s.Student) // عشان نعرف اسم الطالب
                .Include(s => s.Task)    // عشان نعرف تفاصيل المهمة
                .Include(s => s.Evaluation) // عشان نعرف هل تم تقييمه ولا لا
                .Where(s => s.TaskID == taskId)
                .ToListAsync();

            if (!submissions.Any())
            {
                ViewBag.Message = "لا يوجد تسليمات لهذه المهمة حتى الآن.";
            }
            else
            {
                ViewBag.TaskTitle = submissions.First().Task.Title;
            }

            return View(submissions);
        }

        // 2. صفحة التقييم (GET)
        public async Task<IActionResult> Grade(int submissionId)
        {
            var submission = await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Task)
                .Include(s => s.Evaluation)
                .FirstOrDefaultAsync(s => s.SubmissionID == submissionId);

            if (submission == null) return NotFound();

            ViewData["CategoryID"] = new SelectList(_context.FeedbackCategories, "CategoryID", "CategoryName");

            if (submission.Evaluation != null)
            {
                return View(submission.Evaluation);
            }

            // إذا تقييم جديد
            return View(new Evaluation { SubmissionID = submissionId, Submission = submission });
        }

        // 3. حفظ التقييم (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Grade(Evaluation evaluation)
        {
            ModelState.Remove("Submission");
            ModelState.Remove("Evaluator");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                if (evaluation.EvaluationID > 0) 
                {
                    var oldEvaluation = await _context.Evaluations.AsNoTracking()
                        .FirstOrDefaultAsync(e => e.EvaluationID == evaluation.EvaluationID);

                    if (oldEvaluation != null)
                    {
                        evaluation.EvaluatorID = oldEvaluation.EvaluatorID;
                        evaluation.EvaluatedAt = DateTime.Now; // نحدث وقت التعديل

                        _context.Update(evaluation);
                    }
                }
                else 
                {
                    var userId = int.Parse(User.FindFirst("UserID").Value);
                    evaluation.EvaluatorID = userId;
                    evaluation.EvaluatedAt = DateTime.Now;
                    _context.Add(evaluation);
                }

                await _context.SaveChangesAsync();

                var sub = await _context.Submissions.FindAsync(evaluation.SubmissionID);
                return RedirectToAction("Review", new { taskId = sub.TaskID });
            }

            ViewData["CategoryID"] = new SelectList(_context.FeedbackCategories, "CategoryID", "CategoryName", evaluation.CategoryID);
            return View(evaluation);
        }
    }
}