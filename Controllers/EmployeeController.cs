using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.Include(d=> d.Department).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(Employee employee)
        {
            if(ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName", employee.DepartmentId);
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult>Edit(int id)
        {
            var employee = await _context.Employees.Include(d=> d.Department).FirstOrDefaultAsync(e=> e.Id==id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName", employee.DepartmentId);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName", employee.DepartmentId);
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult>Delete(int id)
        {
            var employee = await _context.Employees.Include(d=> d.Department).FirstOrDefaultAsync(e=> e.Id == id);
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
            var employee = await _context.Employees.Include(d=> d.Department).FirstOrDefaultAsync(e=> e.Id==id);
            if (employee == null)
            {
                return NotFound();
            }
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
