using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.Include(d=> d.Department).Include(o=> o.Organization).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName");
            ViewBag.Organizations = new SelectList(_context.Organizations, "Id", "OrganizationName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(Employee employee)
        {
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName", employee.DepartmentId);
            ViewBag.Organizations = new SelectList(_context.Organizations, "Id", "OrganizationName", employee.OrganizationId);
            if (ModelState.IsValid)
            {
                employee.Department = await _context.Departments.FindAsync(employee.DepartmentId);
                employee.Organization = await _context.Organizations.FindAsync(employee.OrganizationId);
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult>Edit(int id)
        {
            var employee = await _context.Employees.Include(d=> d.Department).Include(o=> o.Organization).FirstOrDefaultAsync(e=> e.Id==id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName", employee.DepartmentId);
            ViewBag.Organizations = new SelectList(_context.Organizations, "Id", "OrganizationName", employee.OrganizationId);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(Employee employee)
        {
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName", employee.DepartmentId);
            ViewBag.Organizations = new SelectList(_context.Organizations, "Id", "OrganizationName", employee.OrganizationId);
            if (ModelState.IsValid)
            {
                employee.Department = await _context.Departments.FindAsync(employee.DepartmentId);
                employee.Organization = await _context.Organizations.FindAsync(employee.OrganizationId);
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult>Delete(int id)
        {
            var employee = await _context.Employees.Include(d=> d.Department).Include(o=> o.Organization).FirstOrDefaultAsync(e=> e.Id == id);
            if(employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.Include(d=> d.Department).Include(o=> o.Organization).FirstOrDefaultAsync(e=> e.Id==id);
            if (employee == null)
            {
                return NotFound();
            }
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Export()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var employees = _context.Employees.Include(d=> d.Department).Include(o => o.Organization)
                .Select(e => new Employee
                {
                    Id = e.Id,
                    EmployeeName = e.EmployeeName,
                    EmployeeEmail = e.EmployeeEmail,
                    EmployeePhone = e.EmployeePhone,
                    EmployeePosition = e.EmployeePosition,
                    Department = e.Department,
                    Organization = e.Organization
                })
                .ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Employees");

            // Add header
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Employee Name";
            worksheet.Cells[1, 3].Value = "Employee Email";
            worksheet.Cells[1, 4].Value = "Employee Phone";
            worksheet.Cells[1, 5].Value = "Employee Position";
            worksheet.Cells[1, 6].Value = "Department";
            worksheet.Cells[1, 7].Value = "Organization";

            // Format header
            using (var range = worksheet.Cells[1, 1, 1, 7])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Add data
            for (int i = 0; i < employees.Count; i++)
            {
                var emp = employees[i];
                worksheet.Cells[i + 2, 1].Value = emp.Id;
                worksheet.Cells[i + 2, 2].Value = emp.EmployeeName;
                worksheet.Cells[i + 2, 3].Value = emp.EmployeeEmail;
                worksheet.Cells[i + 2, 4].Value = emp.EmployeePhone;
                worksheet.Cells[i + 2, 5].Value = emp.EmployeePosition;
                worksheet.Cells[i + 2, 6].Value = emp.Department?.DepartmentName;
                worksheet.Cells[i + 2, 7].Value = emp.Organization?.OrganizationName;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"Employees_{DateTime.Today.Date.ToString("dd_MM_yyyy")}.xlsx";

            return File(stream, contentType, fileName);
        }
    }
}
