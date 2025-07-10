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
              return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                                  .Include(u => u.UserRoles)
                                  .SingleOrDefaultAsync(u => u.Username == login.Username);

                if (user != null)
                {
                    var passwordHasher = new PasswordHasher<User>();
                    var result = passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        var userRole = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "User";
                        HttpContext.Session.SetString("Username", user.Username);
                        HttpContext.Session.SetString("UserRole", userRole);
                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                ViewBag.ErrorMessage = "Invalid Username or Password";
            }
            return View(login);
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
