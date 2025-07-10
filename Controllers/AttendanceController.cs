using EmployeeDepartmentCRUDApp.Enums;
using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class AttendanceController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var attendances = await _context.Attendances.Include(e => e.Employee).ToListAsync();
            return View(attendances);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName");
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
        public async Task<IActionResult> Create(Attendance attendance)
        {
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName", attendance.EmployeeId);
            ViewBag.Status = Enum.GetValues(typeof(Status))
                          .Cast<Status>()
                          .Select(e => new SelectListItem
                          {
                              Value = ((int)e).ToString(),
                              Text = e.ToString()
                          }).ToList();
            if (ModelState.IsValid)
            {
                attendance.Employee = await _context.Employees.FindAsync(attendance.EmployeeId);
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(attendance);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var attendance = await _context.Attendances.Include(e => e.Employee).FirstOrDefaultAsync(a => a.Id == id);
            if (attendance == null)
            {
                return NotFound();
            }
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName", attendance.EmployeeId);
            ViewBag.Status = Enum.GetValues(typeof(Status))
                     .Cast<Status>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.ToString()
                     }).ToList();
            return View(attendance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Attendance attendance)
        {
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName", attendance.EmployeeId);
            ViewBag.Status = Enum.GetValues(typeof(Status))
                     .Cast<Status>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.ToString()
                     }).ToList();
            if (ModelState.IsValid)
            {
                attendance.Employee = await _context.Employees.FindAsync(attendance.EmployeeId);
                _context.Attendances.Update(attendance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(attendance);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var attendance = await _context.Attendances.Include(e => e.Employee).FirstOrDefaultAsync(a => a.Id == id);
            if (attendance == null)
            {
                return NotFound();
            }
            return View(attendance);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.Include(e => e.Employee).FirstOrDefaultAsync(a => a.Id == id);
            if (attendance == null)
            {
                return NotFound();
            }
            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Export()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var attendances = _context.Attendances.Include(e => e.Employee)
                .Select(a => new Attendance
                {
                    Id = a.Id,
                    Employee = a.Employee,
                    Date = a.Date,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    Status = a.Status
                })
                .ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Attendances");

            // Add header
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Employee";
            worksheet.Cells[1, 3].Value = "Date";
            worksheet.Cells[1, 4].Value = "Check In Time";
            worksheet.Cells[1, 5].Value = "Check Out Time";
            worksheet.Cells[1, 6].Value = "Status";

            // Format header
            using (var range = worksheet.Cells[1, 1, 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Add data
            for (int i = 0; i < attendances.Count; i++)
            {
                var attend = attendances[i];
                worksheet.Cells[i + 2, 1].Value = attend.Id;
                worksheet.Cells[i + 2, 2].Value = attend.Employee?.EmployeeName;
                var dateCell = worksheet.Cells[i + 2, 3];
                dateCell.Value = attend.Date;
                dateCell.Style.Numberformat.Format = "dd-MM-yyyy";
                var timeCell = worksheet.Cells[i + 2, 4];
                timeCell.Value = attend.CheckInTime;
                timeCell.Style.Numberformat.Format = "hh:mm AM/PM";
                var timeCell2 = worksheet.Cells[i + 2, 5];
                timeCell2.Value = attend.CheckOutTime;
                timeCell2.Style.Numberformat.Format = "hh:mm AM/PM";
                worksheet.Cells[i + 2, 6].Value = attend.Status;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"Attendances_{DateTime.Today.Date.ToString("dd_MM_yyyy")}.xlsx";

            return File(stream, contentType, fileName);
        }
    }
}
