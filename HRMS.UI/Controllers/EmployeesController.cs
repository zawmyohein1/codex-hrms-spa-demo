using HRMS.UI.DataAccess;
using HRMS.UI.Models.EntityModels;
using HRMS.UI.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace HRMS.UI.Controllers;

public class EmployeesController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public EmployeesController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var model = new EmployeeIndexViewModel
        {
            DepartmentOptions = await _context.Departments
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToListAsync(),
            OccupationOptions = await _context.Occupations
                .OrderBy(o => o.Name)
                .Select(o => new SelectListItem
                {
                    Value = o.Id.ToString(),
                    Text = o.Name
                })
                .ToListAsync()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> List(string? searchTerm, int? departmentId, int? occupationId, int page = 1, int pageSize = 20)
    {
        page = Math.Max(page, 1);
        pageSize = pageSize switch
        {
            10 => 10,
            20 => 20,
            50 => 50,
            _ => 20
        };

        var query = _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Occupation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(e => e.EmpNo.Contains(term) || e.EmpName.Contains(term));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        if (occupationId.HasValue)
        {
            query = query.Where(e => e.OccupationId == occupationId.Value);
        }

        var totalCount = await query.CountAsync();
        var totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;
        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        var skip = (page - 1) * pageSize;

        var employees = await query
            .OrderBy(e => e.EmpNo)
            .Skip(skip)
            .Take(pageSize)
            .Select(e => new EmployeeListItemViewModel
            {
                Id = e.Id,
                EmpNo = e.EmpNo,
                EmpName = e.EmpName,
                DepartmentName = e.Department.Name,
                OccupationName = e.Occupation.Name,
                JobTitle = e.JobTitle,
                Gender = e.Gender,
                HiredDate = e.HiredDate,
                ResignDate = e.ResignDate,
                PhotoUrl = e.PhotoUrl
            })
            .ToListAsync();

        var model = new EmployeeListViewModel
        {
            Employees = employees,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            SearchTerm = searchTerm,
            DepartmentId = departmentId,
            OccupationId = occupationId
        };

        return PartialView("_List", model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new EmployeeFormViewModel
        {
            HiredDate = DateTime.Today
        };
        await PopulateLookupsAsync(model);
        return PartialView("_CreateEdit", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeFormViewModel model)
    {
        await ValidateEmployeeAsync(model);

        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(model);
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return PartialView("_CreateEdit", model);
        }

        var employee = new Employee
        {
            EmpNo = model.EmpNo.Trim(),
            EmpName = model.EmpName.Trim(),
            DepartmentId = model.DepartmentId!.Value,
            OccupationId = model.OccupationId!.Value,
            JobTitle = string.IsNullOrWhiteSpace(model.JobTitle) ? null : model.JobTitle.Trim(),
            Gender = string.IsNullOrWhiteSpace(model.Gender) ? null : model.Gender.Trim(),
            HiredDate = model.HiredDate!.Value.Date,
            ResignDate = model.ResignDate?.Date,
            DateOfBirth = model.DateOfBirth?.Date,
            Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim(),
            Address = string.IsNullOrWhiteSpace(model.Address) ? null : model.Address.Trim()
        };

        employee.PhotoUrl = await SavePhotoAsync(model.PhotoFile, null);

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Employee created successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        var model = new EmployeeFormViewModel
        {
            Id = employee.Id,
            EmpNo = employee.EmpNo,
            EmpName = employee.EmpName,
            DepartmentId = employee.DepartmentId,
            OccupationId = employee.OccupationId,
            JobTitle = employee.JobTitle,
            Gender = employee.Gender,
            HiredDate = employee.HiredDate,
            ResignDate = employee.ResignDate,
            DateOfBirth = employee.DateOfBirth,
            Phone = employee.Phone,
            Email = employee.Email,
            Address = employee.Address,
            ExistingPhotoUrl = employee.PhotoUrl
        };

        await PopulateLookupsAsync(model);
        return PartialView("_CreateEdit", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        await ValidateEmployeeAsync(model);

        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(model);
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return PartialView("_CreateEdit", model);
        }

        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        employee.EmpNo = model.EmpNo.Trim();
        employee.EmpName = model.EmpName.Trim();
        employee.DepartmentId = model.DepartmentId!.Value;
        employee.OccupationId = model.OccupationId!.Value;
        employee.JobTitle = string.IsNullOrWhiteSpace(model.JobTitle) ? null : model.JobTitle.Trim();
        employee.Gender = string.IsNullOrWhiteSpace(model.Gender) ? null : model.Gender.Trim();
        employee.HiredDate = model.HiredDate!.Value.Date;
        employee.ResignDate = model.ResignDate?.Date;
        employee.DateOfBirth = model.DateOfBirth?.Date;
        employee.Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim();
        employee.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
        employee.Address = string.IsNullOrWhiteSpace(model.Address) ? null : model.Address.Trim();
        employee.PhotoUrl = await SavePhotoAsync(model.PhotoFile, employee.PhotoUrl);

        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Employee updated successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Occupation)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        var model = new EmployeeDetailsViewModel
        {
            Id = employee.Id,
            EmpNo = employee.EmpNo,
            EmpName = employee.EmpName,
            DepartmentName = employee.Department.Name,
            OccupationName = employee.Occupation.Name,
            JobTitle = employee.JobTitle,
            Gender = employee.Gender,
            HiredDate = employee.HiredDate,
            ResignDate = employee.ResignDate,
            DateOfBirth = employee.DateOfBirth,
            Phone = employee.Phone,
            Email = employee.Email,
            Address = employee.Address,
            PhotoUrl = employee.PhotoUrl
        };

        return PartialView("_Details", model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        var model = new EmployeeDeleteViewModel
        {
            Id = employee.Id,
            EmpNo = employee.EmpNo,
            EmpName = employee.EmpName
        };

        return PartialView("_Delete", model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        var photoUrl = employee.PhotoUrl;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        DeletePhotoFile(photoUrl);

        return Json(new { success = true, message = "Employee deleted successfully." });
    }

    private async Task PopulateLookupsAsync(EmployeeFormViewModel model)
    {
        var departmentItems = await _context.Departments
            .OrderBy(d => d.Name)
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name,
                Selected = model.DepartmentId.HasValue && model.DepartmentId.Value == d.Id
            })
            .ToListAsync();

        var occupationItems = await _context.Occupations
            .OrderBy(o => o.Name)
            .Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.Name,
                Selected = model.OccupationId.HasValue && model.OccupationId.Value == o.Id
            })
            .ToListAsync();

        model.DepartmentOptions = departmentItems;
        model.OccupationOptions = occupationItems;
    }

    private async Task ValidateEmployeeAsync(EmployeeFormViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.EmpNo))
        {
            var empNo = model.EmpNo.Trim();
            var exists = await _context.Employees
                .AnyAsync(e => e.EmpNo == empNo && e.Id != model.Id);
            if (exists)
            {
                ModelState.AddModelError(nameof(EmployeeFormViewModel.EmpNo), "Employee number must be unique.");
            }
        }

        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            var email = model.Email.Trim();
            var exists = await _context.Employees
                .AnyAsync(e => e.Email == email && e.Id != model.Id);
            if (exists)
            {
                ModelState.AddModelError(nameof(EmployeeFormViewModel.Email), "Email address must be unique.");
            }
        }

        if (model.HiredDate.HasValue && model.ResignDate.HasValue && model.ResignDate.Value.Date < model.HiredDate.Value.Date)
        {
            ModelState.AddModelError(nameof(EmployeeFormViewModel.ResignDate), "Resign date cannot be earlier than hired date.");
        }
    }

    private async Task<string?> SavePhotoAsync(IFormFile? photoFile, string? existingPath)
    {
        if (photoFile == null || photoFile.Length == 0)
        {
            return existingPath;
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(photoFile.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await photoFile.CopyToAsync(stream);
        }

        DeletePhotoFile(existingPath);

        return $"/uploads/{fileName}";
    }

    private void DeletePhotoFile(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        var trimmedPath = relativePath.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(_environment.WebRootPath, trimmedPath);
        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }
    }
}
