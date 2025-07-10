using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace EmployeeDepartmentCRUDApp.Controllers
{
    public class OrganizationController : BaseController
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

        [HttpGet]
        public IActionResult Export()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var organizations = _context.Organizations
                .Select(o => new Organization
                {
                    Id = o.Id,
                    OrganizationName = o.OrganizationName,
                    OrganizationEmail = o.OrganizationEmail,
                    OrganizationGSTNo = o.OrganizationGSTNo,
                    OrganizationAddress = o.OrganizationAddress,
                    OrganizationPublishedAt = o.OrganizationPublishedAt
                })
                .ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Organizations");

            // Add header
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Organization Name";
            worksheet.Cells[1, 3].Value = "Organization Email";
            worksheet.Cells[1, 4].Value = "Organization GST No";
            worksheet.Cells[1, 5].Value = "Organization Address";
            worksheet.Cells[1, 6].Value = "Organization Published At";
            // Format header
            using (var range = worksheet.Cells[1, 1, 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Add data
            for (int i = 0; i < organizations.Count; i++)
            {
                var org = organizations[i];
                worksheet.Cells[i + 2, 1].Value = org.Id;
                worksheet.Cells[i + 2, 2].Value = org.OrganizationName;
                worksheet.Cells[i + 2, 3].Value = org.OrganizationEmail;
                worksheet.Cells[i + 2, 4].Value = org.OrganizationGSTNo;
                worksheet.Cells[i + 2, 5].Value = org.OrganizationAddress;
                var dateCell = worksheet.Cells[i + 2, 6];
                dateCell.Value = org.OrganizationPublishedAt;
                dateCell.Style.Numberformat.Format = "dd-MM-yyyy";
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"organizations_{DateTime.Today.Date.ToString("dd_MM_yyyy")}.xlsx";

            return File(stream, contentType, fileName);
        }
    }
}
