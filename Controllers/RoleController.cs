using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class RoleController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(u => u.Id == id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Update(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(role);

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(d => d.Id == id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(d => d.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Export()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var users = _context.Roles
                .Select(r => new Role
                {
                    Id = r.Id,
                    Name = r.Name,
                })
                .ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Roles");

            // Add header
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "RoleName";
            // Format header
            using (var range = worksheet.Cells[1, 1, 1, 2])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Add data
            for (int i = 0; i < users.Count; i++)
            {
                var dept = users[i];
                worksheet.Cells[i + 2, 1].Value = dept.Id;
                worksheet.Cells[i + 2, 2].Value = dept.Name;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"Roles_{DateTime.Today.Date.ToString("dd_MM_yyyy")}.xlsx";

            return File(stream, contentType, fileName);
        }
    }
}
