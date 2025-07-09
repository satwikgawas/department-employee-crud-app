using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class PayrollController : Controller
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
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName", payroll.EmployeeId);
            if (ModelState.IsValid)
            {
                payroll.Employee = await _context.Employees.FindAsync(payroll.EmployeeId);
                await _context.Payrolls.AddAsync(payroll);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           
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
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "EmployeeName", payroll.EmployeeId);
            if (ModelState.IsValid)
            {
                payroll.Employee = await _context.Employees.FindAsync(payroll.EmployeeId);
                _context.Payrolls.Update(payroll);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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

    }
}
