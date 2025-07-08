using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class DepartmentController : Controller
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
    }
}
