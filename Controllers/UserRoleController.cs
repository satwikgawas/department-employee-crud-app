using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class UserRoleController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public UserRoleController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserRoles.Include(u => u.User).Include(r=> r.Role).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = new SelectList(_context.Users, "Id", "Username");
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = new SelectList(_context.Users, "Id", "Username", userRole.UserId);
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name", userRole.RoleId);
            return View(userRole);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.Id == id);
            if (userRole == null)
            {
                return NotFound();
            }
            ViewBag.Users = new SelectList(_context.Users, "Id", "Username", userRole.UserId);
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name", userRole.RoleId);
            return View(userRole);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                _context.UserRoles.Update(userRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = new SelectList(_context.Users, "Id", "Username", userRole.UserId);
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name", userRole.RoleId);
            return View(userRole);

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.Id == id);
            if (userRole == null)
            {
                return NotFound();
            }
            return View(userRole);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.Id == id);
            if (userRole == null)
            {
                return NotFound();
            }

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Export()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var usersRoles = _context.UserRoles
                .Select(u => new UserRole
                {
                    Id = u.Id,
                    User = u.User,
                    Role = u.Role
                })
                .ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("UserRoles");

            // Add header
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "User";
            worksheet.Cells[1, 3].Value = "Role";

            // Format header
            using (var range = worksheet.Cells[1, 1, 1, 3])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Add data
            for (int i = 0; i < usersRoles.Count; i++)
            {
                var userRole = usersRoles[i];
                worksheet.Cells[i + 2, 1].Value = userRole.Id;
                worksheet.Cells[i + 2, 2].Value = userRole.User?.Username;
                worksheet.Cells[i + 2, 3].Value = userRole.Role?.Name;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"UserRoles_{DateTime.Today.Date.ToString("dd_MM_yyyy")}.xlsx";

            return File(stream, contentType, fileName);
        }
    }
}
