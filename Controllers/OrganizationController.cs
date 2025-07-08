using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganizationController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var organizations = await _context.Organizations.ToListAsync();
            return View(organizations);
        }

        [HttpGet]
        public async Task<IActionResult>Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(Organization organization)
        {
            if (ModelState.IsValid)
            {
                await _context.Organizations.AddAsync(organization);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }

        [HttpGet]
        public async Task<IActionResult>Edit(int id)
        {
            var organization = await _context.Organizations.FindAsync(id);
            if(organization == null)
            {
                return NotFound();
            }
            return View(organization);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(Organization organization)
        {
            if (ModelState.IsValid)
            {
                _context.Organizations.Update(organization);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }

        [HttpGet]
        public async Task<IActionResult>Delete(int id)
        {
            var organization = await _context.Organizations.FindAsync(id);
            if(organization == null)
            {
                return NotFound();
            }
            return View(organization);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>DeleteConfirmed(int id)
        {
            var orgaization = await _context.Organizations.FindAsync(id);
            if(orgaization == null)
            {
                return NotFound();
            }
            _context.Organizations.Remove(orgaization);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
