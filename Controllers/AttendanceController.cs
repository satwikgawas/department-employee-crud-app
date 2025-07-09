using EmployeeDepartmentCRUDApp.Enums;
using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class AttendanceController : Controller
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
    }
}
