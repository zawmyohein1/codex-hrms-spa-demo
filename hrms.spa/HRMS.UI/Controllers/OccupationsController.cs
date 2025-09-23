using HRMS.UI.Data;
using HRMS.UI.EntityModels;
using HRMS.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HRMS.UI.Controllers;

public class OccupationsController : Controller
{
    private readonly AppDbContext _context;

    public OccupationsController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> List()
    {
        var occupations = await _context.Occupations
            .Include(o => o.Department)
            .AsNoTracking()
            .OrderBy(o => o.Title)
            .ToListAsync();
        return PartialView("_List", occupations);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = await BuildFormViewModel(new Occupation(), nameof(Create));
        return PartialView("_Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Occupation occupation)
    {
        if (!ModelState.IsValid)
        {
            var invalidViewModel = await BuildFormViewModel(occupation, nameof(Create));
            return PartialView("_Form", invalidViewModel);
        }

        _context.Occupations.Add(occupation);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var occupation = await _context.Occupations.FindAsync(id);
        if (occupation == null)
        {
            return NotFound();
        }

        var viewModel = await BuildFormViewModel(occupation, nameof(Edit));
        return PartialView("_Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Occupation occupation)
    {
        if (id != occupation.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            var invalidViewModel = await BuildFormViewModel(occupation, nameof(Edit));
            return PartialView("_Form", invalidViewModel);
        }

        try
        {
            _context.Update(occupation);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await OccupationExists(occupation.Id))
            {
                return NotFound();
            }

            throw;
        }

        return Json(new { success = true });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var occupation = await _context.Occupations
            .Include(o => o.Department)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
        if (occupation == null)
        {
            return NotFound();
        }

        return PartialView("_Delete", occupation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var occupation = await _context.Occupations.FindAsync(id);
        if (occupation == null)
        {
            return NotFound();
        }

        _context.Occupations.Remove(occupation);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    private async Task<OccupationFormViewModel> BuildFormViewModel(Occupation occupation, string action)
    {
        var departments = await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString(),
                Selected = d.Id == occupation.DepartmentId
            })
            .ToListAsync();

        return new OccupationFormViewModel
        {
            Occupation = occupation,
            Departments = departments,
            Action = action
        };
    }

    private async Task<bool> OccupationExists(int id)
    {
        return await _context.Occupations.AnyAsync(e => e.Id == id);
    }
}
