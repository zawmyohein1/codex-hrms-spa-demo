using HRMS.UI.Data;
using HRMS.UI.EntityModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.UI.Controllers;

public class DepartmentsController : Controller
{
    private readonly AppDbContext _context;

    public DepartmentsController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> List()
    {
        var departments = await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .ToListAsync();
        return PartialView("_List", departments);
    }

    public IActionResult Create()
    {
        return PartialView("_Form", new Department());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Department department)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_Form", department);
        }

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return NotFound();
        }

        return PartialView("_Form", department);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Department department)
    {
        if (id != department.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return PartialView("_Form", department);
        }

        try
        {
            _context.Update(department);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await DepartmentExists(department.Id))
            {
                return NotFound();
            }

            throw;
        }

        return Json(new { success = true });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var department = await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        if (department == null)
        {
            return NotFound();
        }

        return PartialView("_Delete", department);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return NotFound();
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    private async Task<bool> DepartmentExists(int id)
    {
        return await _context.Departments.AnyAsync(e => e.Id == id);
    }
}
