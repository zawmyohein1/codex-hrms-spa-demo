using HRMS.UI.DataAccess;
using HRMS.UI.Models.EntityModels;
using HRMS.UI.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace HRMS.UI.Controllers;

public class DepartmentsController : BaseController
{
    private readonly AppDbContext _context;

    public DepartmentsController(AppDbContext context, IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider)
        : base(viewEngine, tempDataProvider)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> LoadList()
    {
        var departments = await GetDepartmentListAsync();
        return PartialView("_List", departments);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new DepartmentViewModel();
        return PartialView("_CreateEdit", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DepartmentViewModel model)
    {
        var normalizedName = model.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            ModelState.AddModelError(nameof(model.Name), "Department name is required.");
        }
        else
        {
            model.Name = normalizedName;
            if (await _context.Departments.AnyAsync(d => d.Name == model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Department name already exists.");
            }
        }

        if (!ModelState.IsValid)
        {
            var formHtml = await RenderPartialViewToStringAsync("_CreateEdit", model);
            return Json(new { success = false, html = formHtml });
        }

        var entity = new Department
        {
            Name = model.Name
        };

        _context.Departments.Add(entity);
        await _context.SaveChangesAsync();

        var listHtml = await RenderPartialViewToStringAsync("_List", await GetDepartmentListAsync());
        return Json(new { success = true, html = listHtml });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _context.Departments.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        var viewModel = new DepartmentViewModel
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return PartialView("_CreateEdit", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DepartmentViewModel model)
    {
        var normalizedName = model.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            ModelState.AddModelError(nameof(model.Name), "Department name is required.");
        }
        else
        {
            model.Name = normalizedName;
            if (await _context.Departments.AnyAsync(d => d.Id != model.Id && d.Name == model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Department name already exists.");
            }
        }

        if (!ModelState.IsValid)
        {
            var formHtml = await RenderPartialViewToStringAsync("_CreateEdit", model);
            return Json(new { success = false, html = formHtml });
        }

        var entity = await _context.Departments.FindAsync(model.Id);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = model.Name;
        await _context.SaveChangesAsync();

        var listHtml = await RenderPartialViewToStringAsync("_List", await GetDepartmentListAsync());
        return Json(new { success = true, html = listHtml });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.Departments.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        var viewModel = new DepartmentViewModel
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return PartialView("_Delete", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await _context.Departments.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        _context.Departments.Remove(entity);
        await _context.SaveChangesAsync();

        var listHtml = await RenderPartialViewToStringAsync("_List", await GetDepartmentListAsync());
        return Json(new { success = true, html = listHtml });
    }

    private async Task<List<DepartmentViewModel>> GetDepartmentListAsync()
    {
        return await _context.Departments
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentViewModel
            {
                Id = d.Id,
                Name = d.Name
            })
            .ToListAsync();
    }
}
