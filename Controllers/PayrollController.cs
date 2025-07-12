using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class PayrollController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public PayrollController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var payrolls = await _context.Payrolls.Include(e=> e.Employee).ToListAsync();
            return View(payrolls);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(Payroll payroll)
        {
           
            if (ModelState.IsValid)
            {
                await _context.Payrolls.AddAsync(payroll);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName", payroll.EmployeeId);
            return View(payroll);
        }

        [HttpGet]
        public async Task<IActionResult>Edit(int id)
        {
            var payroll = await _context.Payrolls.Include(e => e.Employee).FirstOrDefaultAsync(p => p.Id == id);
            if(payroll == null)
            {
                return NotFound();
            }
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName", payroll.EmployeeId);
            return View(payroll);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(Payroll payroll)
        {
            if (ModelState.IsValid)
            {
                _context.Payrolls.Update(payroll);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName", payroll.EmployeeId);
            return View(payroll);
        }

        [HttpGet]
        public async Task<IActionResult>Delete(int id)
        {
            var payroll = await _context.Payrolls.Include(e => e.Employee).FirstOrDefaultAsync(p => p.Id == id);
            if(payroll == null)
            {
                return NotFound();
            }
            return View(payroll);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>DeleteConfirmed(int id)
        {
            var payroll = await _context.Payrolls.Include(e => e.Employee).FirstOrDefaultAsync(p => p.Id == id);
            if (payroll == null)
            {
                return NotFound();
            }
            _context.Payrolls.Remove(payroll);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Export()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var payrolls = _context.Payrolls
                .Select(p => new Payroll
                {
                    Id = p.Id,
                    Employee = p.Employee,
                    BasicSalary = p.BasicSalary,
                    HRA = p.HRA,
                    DA = p.DA,
                    Deductions = p.Deductions,
                    NetPlay = p.NetPlay,
                    PayDate = p.PayDate,
                    PayMonth = p.PayMonth
                })
                .ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Payrolls");

            // Add header
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Employee";
            worksheet.Cells[1, 3].Value = "Basic Salary";
            worksheet.Cells[1, 4].Value = "HRA";
            worksheet.Cells[1, 5].Value = "DA";
            worksheet.Cells[1, 6].Value = "Deductions";
            worksheet.Cells[1, 7].Value = "NetPlay";
            worksheet.Cells[1, 8].Value = "PayDate";
            worksheet.Cells[1, 9].Value = "PayNonth";
            // Format header
            using (var range = worksheet.Cells[1, 1, 1, 9])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Add data
            for (int i = 0; i < payrolls.Count; i++)
            {
                var payroll = payrolls[i];
                worksheet.Cells[i + 2, 1].Value = payroll.Id;
                worksheet.Cells[i + 2, 2].Value = payroll.Employee?.EmployeeName;
                worksheet.Cells[i + 2, 3].Value = payroll.BasicSalary;
                worksheet.Cells[i + 2, 4].Value = payroll.HRA;
                worksheet.Cells[i + 2, 5].Value = payroll.DA;
                worksheet.Cells[i + 2, 6].Value = payroll.Deductions;
                worksheet.Cells[i + 2, 7].Value = payroll.NetPlay;
                var dateCell = worksheet.Cells[i + 2, 8];
                dateCell.Value = payroll.PayDate;
                dateCell.Style.Numberformat.Format = "dd-MM-yyyy";
                worksheet.Cells[i + 2, 9].Value = payroll.PayMonth;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"payrolls_{DateTime.Today.Date.ToString("dd_MM_yyyy")}.xlsx";

            return File(stream, contentType, fileName);
        }

    }
}
