using CoderNexues.Data;
using CoderNexues.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoderNexues.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly CN_DbContext _context;

        public ReportsController(CN_DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? campId, DateTime? startDate, DateTime? endDate)
        {
            var vm = new DashboardAnalyticsViewModel
            {
                SelectedCampId = campId,
                StartDate = startDate,
                EndDate = endDate,
                Alerts = new List<SmartAlert>(),
                // نملأ قائمة المعسكرات للعرض
                AllCamps = await _context.Camps.ToListAsync()
            };

            // 1. تجهيز الاستعلام الأساسي (مع الفلترة)
            var query = _context.Submissions
                .Include(s => s.Task).ThenInclude(t => t.Camp)
                .Include(s => s.Student)
                .Include(s => s.Evaluation).ThenInclude(e => e.Evaluator)
                .AsQueryable();

            // تطبيق الفلاتر
            if (campId.HasValue) query = query.Where(s => s.Task.CampID == campId);
            if (startDate.HasValue) query = query.Where(s => s.SubmittedAt >= startDate);
            if (endDate.HasValue) query = query.Where(s => s.SubmittedAt <= endDate);

            var data = await query.ToListAsync(); // جلب البيانات

            // 2. الحسابات الأساسية (للبطاقات الملونة)
            var uniqueStudents = data.Select(s => s.Student).DistinctBy(s => s.UserID).ToList();
            vm.TotalStudents = uniqueStudents.Count;
            vm.TotalSubmissions = data.Count; // عدد الحلول
                                              // عدد المعسكرات النشطة نجيبه من جدول المعسكرات مباشرة للأمان
            vm.ActiveCampsCount = await _context.Camps.CountAsync(c => c.Status == "Active");
            // (ملاحظة: تأكد إنك ضفت ActiveCampsCount في الموديل أو استخدم ActiveCamps القديم)

            // 3. الرسم البياني (أداء المعسكرات)
            var campGroups = data.GroupBy(s => s.Task.Camp.CampName).ToList();
            vm.CampNames = campGroups.Select(g => g.Key).ToList();
            vm.CampAvgScores = campGroups.Select(g => g.Average(s => s.Evaluation?.Score ?? 0)).ToList();

            // 4. تحليل المدربين
            var evaluators = data.Where(s => s.Evaluation != null).Select(s => s.Evaluation.Evaluator).DistinctBy(e => e.UserID);
            vm.TrainerStats = new List<TrainerStatsDto>();
            foreach (var trainer in evaluators)
            {
                var trainerEvals = data.Where(s => s.Evaluation != null && s.Evaluation.EvaluatorID == trainer.UserID).ToList();
                double avgSpeed = trainerEvals.Any() ? trainerEvals.Average(x => (x.Evaluation.EvaluatedAt - x.SubmittedAt).TotalHours) : 0;

                vm.TrainerStats.Add(new TrainerStatsDto
                {
                    Name = trainer.FullName,
                    CompletedEvaluations = trainerEvals.Count,
                    StudentCount = trainerEvals.Select(x => x.StudentID).Distinct().Count(),
                    AvgResponseTimeHours = Math.Round(avgSpeed, 1)
                });
            }

            vm.UngradedTasksCount = data.Count(s => s.Evaluation == null);
            vm.DropoutCount = await _context.Users.CountAsync(u => u.Role == "Student" && !u.IsActive);

            if (vm.UngradedTasksCount > 5) vm.Alerts.Add(new SmartAlert { Level = "Warning", Message = $"يوجد {vm.UngradedTasksCount} مهمة بانتظار التصحيح" });
            if (vm.DropoutCount > 0) vm.Alerts.Add(new SmartAlert { Level = "Danger", Message = $"هناك {vm.DropoutCount} طلاب منسحبين/محظورين" });

            ViewBag.CampsList = new SelectList(vm.AllCamps, "CampID", "CampName", campId);

            return View(vm);
        }

        public async Task<IActionResult> CampDetails(int id)
        {
            var camp = await _context.Camps.FindAsync(id);
            if (camp == null) return NotFound();

            var viewModel = new DashboardViewModel();

            var totalPossibleScore = await _context.Tasks
                .Where(t => t.CampID == id)
                .SumAsync(t => t.MaxScore);

            if (totalPossibleScore == 0) totalPossibleScore = 1;

            viewModel.TopStudents = await _context.CampUsers
                .Where(cu => cu.CampID == id && cu.User.Role == "Student")
                .Select(cu => new TopStudentDto
                {
                    StudentName = cu.User.FullName,
                    CampName = camp.CampName,
                    TotalScore = cu.User.Submissions
                                .Where(s => s.Task.CampID == id)
                                .Sum(s => (int?)s.Evaluation.Score) ?? 0
                })
                .OrderByDescending(s => s.TotalScore)
                .Take(3)
                .ToListAsync();


            var allStudentsStats = await _context.CampUsers
                .Where(cu => cu.CampID == id && cu.User.Role == "Student")
                .Select(cu => new TopStudentDto
                {
                    StudentName = cu.User.FullName,
                    CampName = camp.CampName,
                    TotalScore = cu.User.Submissions
                                .Where(s => s.Task.CampID == id)
                                .Sum(s => (int?)s.Evaluation.Score) ?? 0
                })
                .ToListAsync();

            viewModel.StrugglingStudents = allStudentsStats
                .Where(s => s.TotalScore < (totalPossibleScore * 0.5))
                .OrderBy(s => s.TotalScore)
                .Take(3)
                .ToList();

            ViewBag.CampName = camp.CampName;
            return View(viewModel);
        }

    }
}