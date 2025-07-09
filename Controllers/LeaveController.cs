using EmployeeDepartmentCRUDApp.Enums;
using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class LeaveController : Controller
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
    }
}
