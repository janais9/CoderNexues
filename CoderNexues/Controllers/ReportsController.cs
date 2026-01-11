using CoderNexues.Data;
using CoderNexues.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalStudents = await _context.Users.CountAsync(u => u.Role == "Student"),
                ActiveCamps = await _context.Camps.CountAsync(c => c.Status == "Active"),
                TotalSubmissions = await _context.Submissions.CountAsync()
            };

            ViewBag.Camps = await _context.Camps.ToListAsync();

            return View(viewModel);
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