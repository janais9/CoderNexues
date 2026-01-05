using CoderNexues.Data;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;

namespace CoderNexues.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly CN_DbContext _context;

        public UsersController(CN_DbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public IActionResult ChangeRole(int id, string newRole)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.Role = newRole; 
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}