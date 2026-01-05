using CoderNexues.Data;
using CoderNexues.Models;
using CoderNexues.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CoderNexues.Controllers
{
    public class AccountController : Controller
    {
        private readonly CN_DbContext _context;

        public AccountController(CN_DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "هذا البريد مسجل مسبقاً");
                    return View(model);
                }

                var passwordHash = HashPassword(model.Password);

                var newUser = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = passwordHash,
                    Role = "Student", 
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

                if (user != null && user.PasswordHash == HashPassword(model.Password))
                {
                    if (!user.IsActive)
                    {
                        ModelState.AddModelError(string.Empty, "تم تعطيل هذا الحساب، يرجى مراجعة الإدارة.");
                        return View(model);
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("UserID", user.UserID.ToString()) // نحفظ رقم المستخدم لنستخدمه لاحقاً
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}