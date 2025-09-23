using HRMS.UI.DataAccess;
using HRMS.UI.Models.EntityModels;
using Microsoft.AspNetCore.Http;
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

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var departments = await _context.Departments
            .OrderBy(d => d.Name)
            .ToListAsync();
        return PartialView("_List", departments);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return PartialView("_CreateEdit", new Department());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Department department)
    {
        if (await _context.Departments.AnyAsync(d => d.Name == department.Name))
        {
            ModelState.AddModelError(nameof(Department.Name), "A department with the same name already exists.");
        }

        if (!ModelState.IsValid)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return PartialView("_CreateEdit", department);
        }

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return NotFound();
        }

        return PartialView("_CreateEdit", department);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Department department)
    {
        if (id != department.Id)
        {
            return BadRequest();
        }

        if (await _context.Departments.AnyAsync(d => d.Id != department.Id && d.Name == department.Name))
        {
            ModelState.AddModelError(nameof(Department.Name), "A department with the same name already exists.");
        }

        if (!ModelState.IsValid)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return PartialView("_CreateEdit", department);
        }

        _context.Entry(department).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return NotFound();
        }

        return PartialView("_Delete", department);
    }

    [HttpPost, ActionName("Delete")]
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
}
