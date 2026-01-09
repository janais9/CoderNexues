using CoderNexues.Data;
using CoderNexues.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoderNexues.Controllers
{
    [Authorize(Roles = "Admin,Trainer")] // للمدربين والادارة فقط
    public class EvaluationsController : Controller
    {
        private readonly CN_DbContext _context;

        public EvaluationsController(CN_DbContext context)
        {
            _context = context;
        }

        // 1. عرض قائمة التسليمات لمهمة معينة
        // الرابط بيكون: /Evaluations/Review/5
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

            // تعبئة القائمة المنسدلة بأنواع الملاحظات
            ViewData["CategoryID"] = new SelectList(_context.FeedbackCategories, "CategoryID", "CategoryName");

            // إذا كان مقيّم من قبل، نعرض التقييم القديم للتعديل
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
            // التحقق من القيم
            if (evaluation.Score < 0) ModelState.AddModelError("Score", "الدرجة لا يمكن أن تكون سالبة");

            // لأننا لا نمرر كل الكائنات في الفورم، نتجاهل التحقق منها
            ModelState.Remove("Submission");
            ModelState.Remove("Evaluator");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                // هل هو تعديل لتقييم قديم أم جديد؟
                if (evaluation.EvaluationID > 0)
                {
                    _context.Update(evaluation);
                }
                else
                {
                    // تسجيل اسم المدرب الحالي
                    var userId = int.Parse(User.FindFirst("UserID").Value);
                    evaluation.EvaluatorID = userId;
                    evaluation.EvaluatedAt = DateTime.Now;

                    _context.Add(evaluation);
                }

                await _context.SaveChangesAsync();

                // نرجع لقائمة التسليمات لنفس المهمة
                // نحتاج نجيب TaskId عشان نرجع لنفس الصفحة
                var sub = await _context.Submissions.FindAsync(evaluation.SubmissionID);
                return RedirectToAction("Review", new { taskId = sub.TaskID });
            }

            // في حال الخطأ نعيد تعبئة القائمة
            ViewData["CategoryID"] = new SelectList(_context.FeedbackCategories, "CategoryID", "CategoryName", evaluation.CategoryID);
            return View(evaluation);
        }
    }
}