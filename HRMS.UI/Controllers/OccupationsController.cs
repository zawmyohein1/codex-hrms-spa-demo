using HRMS.UI.DataAccess;
using HRMS.UI.Models.EntityModels;
using HRMS.UI.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace HRMS.UI.Controllers;

public class OccupationsController : BaseController
{
    private readonly AppDbContext _context;

    public OccupationsController(AppDbContext context, IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider)
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
        var occupations = await GetOccupationListAsync();
        return PartialView("_List", occupations);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new OccupationViewModel();
        return PartialView("_CreateEdit", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OccupationViewModel model)
    {
        var normalizedName = model.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            ModelState.AddModelError(nameof(model.Name), "Occupation name is required.");
        }
        else
        {
            model.Name = normalizedName;
            if (await _context.Occupations.AnyAsync(o => o.Name == model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Occupation name already exists.");
            }
        }

        if (!ModelState.IsValid)
        {
            var formHtml = await RenderPartialViewToStringAsync("_CreateEdit", model);
            return Json(new { success = false, html = formHtml });
        }

        var entity = new Occupation
        {
            Name = model.Name
        };

        _context.Occupations.Add(entity);
        await _context.SaveChangesAsync();

        var listHtml = await RenderPartialViewToStringAsync("_List", await GetOccupationListAsync());
        return Json(new { success = true, html = listHtml });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _context.Occupations.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        var viewModel = new OccupationViewModel
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return PartialView("_CreateEdit", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(OccupationViewModel model)
    {
        var normalizedName = model.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            ModelState.AddModelError(nameof(model.Name), "Occupation name is required.");
        }
        else
        {
            model.Name = normalizedName;
            if (await _context.Occupations.AnyAsync(o => o.Id != model.Id && o.Name == model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Occupation name already exists.");
            }
        }

        if (!ModelState.IsValid)
        {
            var formHtml = await RenderPartialViewToStringAsync("_CreateEdit", model);
            return Json(new { success = false, html = formHtml });
        }

        var entity = await _context.Occupations.FindAsync(model.Id);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = model.Name;
        await _context.SaveChangesAsync();

        var listHtml = await RenderPartialViewToStringAsync("_List", await GetOccupationListAsync());
        return Json(new { success = true, html = listHtml });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.Occupations.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        var viewModel = new OccupationViewModel
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
        var entity = await _context.Occupations.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        _context.Occupations.Remove(entity);
        await _context.SaveChangesAsync();

        var listHtml = await RenderPartialViewToStringAsync("_List", await GetOccupationListAsync());
        return Json(new { success = true, html = listHtml });
    }

    private async Task<List<OccupationViewModel>> GetOccupationListAsync()
    {
        return await _context.Occupations
            .OrderBy(o => o.Name)
            .Select(o => new OccupationViewModel
            {
                Id = o.Id,
                Name = o.Name
            })
            .ToListAsync();
    }
}
