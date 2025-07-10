using EmployeeDepartmentCRUDApp.Enums;
using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class LeaveController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public LeaveController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var leaves = await _context.Leaves.Include(e => e.Employee).ToListAsync();
            return View(leaves);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName");
            ViewBag.LeaveTypes = Enum.GetValues(typeof(LeaveType))
                                .Cast<LeaveType>()
                                .Select(e => new SelectListItem
                                {
                                    Value = ((int)e).ToString(),
                                    Text = e.ToString()
                                }).ToList();
            ViewBag.Status = Enum.GetValues(typeof(Status))
                     .Cast<Status>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.ToString()
                     }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Leave leave)
        {
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName", leave.EmployeeId);
            ViewBag.LeaveTypes = Enum.GetValues(typeof(LeaveType))
                            .Cast<LeaveType>()
                            .Select(e => new SelectListItem
                            {
                                Value = ((int)e).ToString(),
                                Text = e.ToString()
                            }).ToList();
            ViewBag.Status = Enum.GetValues(typeof(Status))
                     .Cast<Status>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.ToString()
                     }).ToList();
            if (ModelState.IsValid)
            {
                leave.Employee = await _context.Employees.FindAsync(leave.EmployeeId);
                _context.Leaves.Add(leave);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leave);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var leave = await _context.Leaves.Include(e => e.Employee).FirstOrDefaultAsync(a => a.Id == id);
            if (leave == null)
            {
                return NotFound();
            }
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName", leave.EmployeeId);
            ViewBag.LeaveTypes = Enum.GetValues(typeof(LeaveType))
                             .Cast<LeaveType>()
                             .Select(e => new SelectListItem
                             {
                                 Value = ((int)e).ToString(),
                                 Text = e.ToString()
                             }).ToList();
            ViewBag.Status = Enum.GetValues(typeof(Status))
                     .Cast<Status>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.ToString()
                     }).ToList();
            return View(leave);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Leave leave)
        {
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName", leave.EmployeeId);
            ViewBag.LeaveTypes = Enum.GetValues(typeof(LeaveType))
                             .Cast<LeaveType>()
                             .Select(e => new SelectListItem
                             {
                                 Value = ((int)e).ToString(),
                                 Text = e.ToString()
                             }).ToList();
            ViewBag.Status = Enum.GetValues(typeof(Status))
                     .Cast<Status>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.ToString()
                     }).ToList();
            if (ModelState.IsValid)
            {
                leave.Employee = await _context.Employees.FindAsync(leave.EmployeeId);
                _context.Leaves.Update(leave);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leave);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var leave = await _context.Leaves.Include(e => e.Employee).FirstOrDefaultAsync(a => a.Id == id);
            if (leave == null)
            {
                return NotFound();
            }
            return View(leave);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leave = await _context.Leaves.Include(e => e.Employee).FirstOrDefaultAsync(a => a.Id == id);
            if (leave == null)
            {
                return NotFound();
            }
            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Export()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var leaves = _context.Leaves.Include(e => e.Employee)
                .Select(l => new Leave
                {
                    Id = l.Id,
                    Employee = l.Employee,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    LeaveType = l.LeaveType,
                    Reason = l.Reason,
                    Status = l.Status
                })
                .ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Leaves");

            // Add header
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Employee";
            worksheet.Cells[1, 3].Value = "Start Date";
            worksheet.Cells[1, 4].Value = "End Date";
            worksheet.Cells[1, 5].Value = "Leave Type";
            worksheet.Cells[1, 6].Value = "Leave Reason";
            worksheet.Cells[1, 7].Value = "Status";

            // Format header
            using (var range = worksheet.Cells[1, 1, 1, 7])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Add data
            for (int i = 0; i < leaves.Count; i++)
            {
                var leave = leaves[i];
                worksheet.Cells[i + 2, 1].Value = leave.Id;
                worksheet.Cells[i + 2, 2].Value = leave.Employee?.EmployeeName;
                var dateCell = worksheet.Cells[i + 2, 3];
                dateCell.Value = leave.StartDate;
                dateCell.Style.Numberformat.Format = "dd-MM-yyyy";
                var dateCell2 = worksheet.Cells[i + 2, 4];
                dateCell2.Value = leave.StartDate;
                dateCell2.Style.Numberformat.Format = "dd-MM-yyyy";
                worksheet.Cells[i + 2, 5].Value = leave.LeaveType;
                worksheet.Cells[i + 2, 6].Value = leave.Reason;
                worksheet.Cells[i + 2, 7].Value = leave.Status;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"Leaves_{DateTime.Today.Date.ToString("dd_MM_yyyy")}.xlsx";

            return File(stream, contentType, fileName);
        }
    }
}
