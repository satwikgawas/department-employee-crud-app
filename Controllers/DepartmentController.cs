using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class DepartmentController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Departments.Include(o=> o.Organization).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Organizations = new SelectList(_context.Organizations, "Id", "OrganizationName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {        
            if (ModelState.IsValid)
            {
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Organizations = new SelectList(_context.Organizations, "Id", "OrganizationName", department.OrganizationId);
            return View(department);
        }

        [HttpGet]
        public async Task<IActionResult>Edit(int id)
        {
            var dept = await _context.Departments.Include(o => o.Organization).FirstOrDefaultAsync(d => d.Id == id);
            if (dept == null)
            {
                return NotFound();
            }
            ViewBag.Organizations = new SelectList(_context.Organizations, "Id", "OrganizationName", dept.OrganizationId);
            return View(dept);      
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Update(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Organizations = new SelectList(_context.Organizations, "Id", "OrganizationName", department.OrganizationId);
            return View(department);

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dept = await _context.Departments.Include(o=> o.Organization).FirstOrDefaultAsync(d=> d.Id == id);
            if (dept == null)
            {
                return NotFound();
            }
            return View(dept);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>DeleteConfirmed(int id)
        {
            var dept = await _context.Departments.Include(o => o.Organization).FirstOrDefaultAsync(d => d.Id == id);
            if (dept == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Export()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var departments = _context.Departments.Include(o=> o.Organization)
                .Select(d => new Department{
                    Id=d.Id,
                    DepartmentName = d.DepartmentName,
                    Organization = d.Organization
                })
                .ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Departments");

            // Add header
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Department Name";
            worksheet.Cells[1, 3].Value = "Organization";

            // Format header
            using (var range = worksheet.Cells[1, 1, 1, 3])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Add data
            for (int i = 0; i < departments.Count; i++)
            {
                var dept = departments[i];
                worksheet.Cells[i + 2, 1].Value = dept.Id;
                worksheet.Cells[i + 2, 2].Value = dept.DepartmentName;
                worksheet.Cells[i + 2, 3].Value = dept.Organization?.OrganizationName;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"Departments_{DateTime.Today.Date.ToString("dd_MM_yyyy")}.xlsx";

            return File(stream, contentType, fileName);
        }
    }
}
