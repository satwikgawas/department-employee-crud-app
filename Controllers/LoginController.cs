using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class LoginController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["HideLayout"] = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Login login)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid Username or Password";
                ViewData["HideLayout"] = true;
                return View(login);
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == login.Username);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid Username or Password";
                ViewData["HideLayout"] = true;
                return View(login);
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, login.Password);

            if (result != PasswordVerificationResult.Success)
            {
                ViewBag.ErrorMessage = "Invalid Username or Password";
                ViewData["HideLayout"] = true;
                return View(login);
            }

            // Set session
            HttpContext.Session.SetString("Username", user.Username);
            var roleName = await _context.UserRoles
                              .Where(ur => ur.UserId == user.Id)
                              .Include(ur => ur.Role)
                              .Select(ur => ur.Role.Name)
                              .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(roleName))
                HttpContext.Session.SetString("UserRole", roleName);

            return RedirectToAction("Index", "Dashboard");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

    }
}
