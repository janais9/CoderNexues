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

            viewModel.TopStudents = await _context.Users
                .Where(u => u.Role == "Student")
                .Select(u => new TopStudentDto
                {
                    StudentName = u.FullName,
                    CampName = u.CampUsers.FirstOrDefault().Camp.CampName ?? "غير مسجل",
                    TotalScore = u.Submissions.Sum(s => (int?)s.Evaluation.Score) ?? 0
                })
                .OrderByDescending(s => s.TotalScore)
                .Take(3)
                .ToListAsync();

            return View(viewModel);
        }
    }
}