using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class ModuleAssignController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ModuleAssignController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var modules = await _context.ModuleAssigns.ToListAsync();
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name");
            return View(modules);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ModuleAssign module)
        {
            if (ModelState.IsValid)
            {
                _context.ModuleAssigns.Add(module);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name", module.RoleId);
            return View(module);  
        }

    }
}