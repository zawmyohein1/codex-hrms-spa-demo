using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HRMS.UI.Data;
using HRMS.UI.EntityModels;
using HRMS.UI.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HRMS.UI.Controllers;

public class EmployeesController : Controller
{
    private const int PageSize = 10;
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public EmployeesController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new EmployeesIndexViewModel
        {
            Departments = await GetDepartmentSelectListAsync(null),
            Occupations = await GetOccupationSelectListAsync(null)
        };

        return View(viewModel);
    }

    public async Task<IActionResult> List(string? searchEmpNo, string? searchEmpName, int? departmentId, int? occupationId, int pageNo = 1)
    {
        if (pageNo < 1)
        {
            pageNo = 1;
        }

        searchEmpNo = string.IsNullOrWhiteSpace(searchEmpNo) ? null : searchEmpNo.Trim();
        searchEmpName = string.IsNullOrWhiteSpace(searchEmpName) ? null : searchEmpName.Trim();

        var query = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Occupation)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchEmpNo))
        {
            query = query.Where(e => e.EmpNo.Contains(searchEmpNo));
        }

        if (!string.IsNullOrWhiteSpace(searchEmpName))
        {
            query = query.Where(e => e.EmpName.Contains(searchEmpName));
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
        var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

        if (totalPages > 0 && pageNo > totalPages)
        {
            pageNo = totalPages;
        }

        var employees = await query
            .OrderBy(e => e.EmpName)
            .ThenBy(e => e.EmpNo)
            .Skip((pageNo - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var viewModel = new EmployeeListViewModel
        {
            Employees = employees,
            CurrentPage = totalPages == 0 ? 1 : pageNo,
            TotalPages = totalPages,
            PageSize = PageSize,
            TotalCount = totalCount,
            SearchEmpNo = searchEmpNo,
            SearchEmpName = searchEmpName,
            SelectedDepartmentId = departmentId,
            SelectedOccupationId = occupationId
        };

        return PartialView("_List", viewModel);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = await BuildFormViewModelAsync(new EmployeeFormViewModel());
        return PartialView("_CreateEdit", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildFormViewModelAsync(viewModel);
            return PartialView("_CreateEdit", invalidModel);
        }

        var employee = new Employee
        {
            EmpNo = viewModel.EmpNo,
            EmpName = viewModel.EmpName,
            DepartmentId = viewModel.DepartmentId!.Value,
            OccupationId = viewModel.OccupationId!.Value,
            JobTitle = viewModel.JobTitle,
            Gender = viewModel.Gender,
            HiredDate = viewModel.HiredDate,
            ResignDate = viewModel.ResignDate,
            DateOfBirth = viewModel.DateOfBirth,
            Phone = viewModel.Phone,
            Email = viewModel.Email,
            Address = viewModel.Address
        };

        var photoPath = await SavePhotoAsync(viewModel.Photo);
        if (!string.IsNullOrEmpty(photoPath))
        {
            employee.PhotoUrl = photoPath;
        }

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        var viewModel = await BuildFormViewModelAsync(new EmployeeFormViewModel
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
            PhotoUrl = employee.PhotoUrl
        });

        return PartialView("_CreateEdit", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeFormViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildFormViewModelAsync(viewModel);
            return PartialView("_CreateEdit", invalidModel);
        }

        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        employee.EmpNo = viewModel.EmpNo;
        employee.EmpName = viewModel.EmpName;
        employee.DepartmentId = viewModel.DepartmentId!.Value;
        employee.OccupationId = viewModel.OccupationId!.Value;
        employee.JobTitle = viewModel.JobTitle;
        employee.Gender = viewModel.Gender;
        employee.HiredDate = viewModel.HiredDate;
        employee.ResignDate = viewModel.ResignDate;
        employee.DateOfBirth = viewModel.DateOfBirth;
        employee.Phone = viewModel.Phone;
        employee.Email = viewModel.Email;
        employee.Address = viewModel.Address;

        if (viewModel.Photo != null && viewModel.Photo.Length > 0)
        {
            DeletePhoto(employee.PhotoUrl);
            var photoPath = await SavePhotoAsync(viewModel.Photo);
            employee.PhotoUrl = photoPath;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await EmployeeExists(employee.Id))
            {
                return NotFound();
            }

            throw;
        }

        return Json(new { success = true });
    }

    public async Task<IActionResult> Details(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Occupation)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        return PartialView("_Details", employee);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Occupation)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        return PartialView("_Delete", employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        DeletePhoto(employee.PhotoUrl);

        return Json(new { success = true });
    }

    private async Task<EmployeeFormViewModel> BuildFormViewModelAsync(EmployeeFormViewModel viewModel)
    {
        viewModel.Departments = await GetDepartmentSelectListAsync(viewModel.DepartmentId);
        viewModel.Occupations = await GetOccupationSelectListAsync(viewModel.OccupationId);
        viewModel.GenderOptions = new[]
        {
            new SelectListItem { Text = "- Select Gender -", Value = string.Empty, Selected = string.IsNullOrEmpty(viewModel.Gender) },
            new SelectListItem { Text = "Female", Value = "Female", Selected = string.Equals(viewModel.Gender, "Female", StringComparison.OrdinalIgnoreCase) },
            new SelectListItem { Text = "Male", Value = "Male", Selected = string.Equals(viewModel.Gender, "Male", StringComparison.OrdinalIgnoreCase) },
            new SelectListItem { Text = "Other", Value = "Other", Selected = string.Equals(viewModel.Gender, "Other", StringComparison.OrdinalIgnoreCase) }
        };

        return viewModel;
    }

    private async Task<List<SelectListItem>> GetDepartmentSelectListAsync(int? selectedId)
    {
        return await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString(),
                Selected = selectedId.HasValue && d.Id == selectedId.Value
            })
            .ToListAsync();
    }

    private async Task<List<SelectListItem>> GetOccupationSelectListAsync(int? selectedId)
    {
        return await _context.Occupations
            .Include(o => o.Department)
            .AsNoTracking()
            .OrderBy(o => o.Title)
            .Select(o => new SelectListItem
            {
                Text = o.Department == null ? o.Title : $"{o.Title} ({o.Department.Name})",
                Value = o.Id.ToString(),
                Selected = selectedId.HasValue && o.Id == selectedId.Value
            })
            .ToListAsync();
    }

    private async Task<bool> EmployeeExists(int id)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id);
    }

    private async Task<string?> SavePhotoAsync(IFormFile? photo)
    {
        if (photo == null || photo.Length == 0)
        {
            return null;
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await photo.CopyToAsync(stream);
        }

        return Path.Combine("uploads", fileName).Replace('\\', '/');
    }

    private void DeletePhoto(string? photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return;
        }

        var relativePath = photoUrl.Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

        if (System.IO.File.Exists(fullPath))
        {
            try
            {
                System.IO.File.Delete(fullPath);
            }
            catch
            {
                // Ignore delete failures.
            }
        }
    }
}
