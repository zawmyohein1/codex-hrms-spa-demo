using HRMS.UI.DataAccess;
using HRMS.UI.Models.EntityModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var occupations = await _context.Occupations
            .OrderBy(o => o.Name)
            .ToListAsync();
        return PartialView("_List", occupations);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return PartialView("_CreateEdit", new Occupation());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Occupation occupation)
    {
        if (await _context.Occupations.AnyAsync(o => o.Name == occupation.Name))
        {
            ModelState.AddModelError(nameof(Occupation.Name), "An occupation with the same name already exists.");
        }

        if (!ModelState.IsValid)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return PartialView("_CreateEdit", occupation);
        }

        _context.Occupations.Add(occupation);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var occupation = await _context.Occupations.FindAsync(id);
        if (occupation == null)
        {
            return NotFound();
        }

        return PartialView("_CreateEdit", occupation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Occupation occupation)
    {
        if (id != occupation.Id)
        {
            return BadRequest();
        }

        if (await _context.Occupations.AnyAsync(o => o.Id != occupation.Id && o.Name == occupation.Name))
        {
            ModelState.AddModelError(nameof(Occupation.Name), "An occupation with the same name already exists.");
        }

        if (!ModelState.IsValid)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return PartialView("_CreateEdit", occupation);
        }

        _context.Entry(occupation).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var occupation = await _context.Occupations.FindAsync(id);
        if (occupation == null)
        {
            return NotFound();
        }

        return PartialView("_Delete", occupation);
    }

    [HttpPost, ActionName("Delete")]
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
}
