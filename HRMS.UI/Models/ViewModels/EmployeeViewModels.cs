using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS.UI.Models.ViewModels;

public class EmployeeIndexViewModel
{
    public IEnumerable<SelectListItem> DepartmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> OccupationOptions { get; set; } = Enumerable.Empty<SelectListItem>();
}

public class EmployeeListItemViewModel
{
    public int Id { get; set; }

    public string EmpNo { get; set; } = string.Empty;

    public string EmpName { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;

    public string OccupationName { get; set; } = string.Empty;

    public string? JobTitle { get; set; }

    public string? Gender { get; set; }

    public DateTime HiredDate { get; set; }

    public DateTime? ResignDate { get; set; }

    public string? PhotoUrl { get; set; }
}

public class EmployeeListViewModel
{
    public IEnumerable<EmployeeListItemViewModel> Employees { get; set; } = Enumerable.Empty<EmployeeListItemViewModel>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public string? SearchTerm { get; set; }

    public int? DepartmentId { get; set; }

    public int? OccupationId { get; set; }
}

public class EmployeeFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Employee No.")]
    public string EmpNo { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [Display(Name = "Employee Name")]
    public string EmpName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Department")]
    public int? DepartmentId { get; set; }

    [Required]
    [Display(Name = "Occupation")]
    public int? OccupationId { get; set; }

    [StringLength(100)]
    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Hired Date")]
    public DateTime? HiredDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Resign Date")]
    public DateTime? ResignDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(256)]
    [EmailAddress]
    public string? Email { get; set; }

    public string? Address { get; set; }

    [Display(Name = "Photo")]
    public IFormFile? PhotoFile { get; set; }

    public string? ExistingPhotoUrl { get; set; }

    [BindNever]
    public IEnumerable<SelectListItem> DepartmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();

    [BindNever]
    public IEnumerable<SelectListItem> OccupationOptions { get; set; } = Enumerable.Empty<SelectListItem>();
}

public class EmployeeDetailsViewModel
{
    public int Id { get; set; }

    public string EmpNo { get; set; } = string.Empty;

    public string EmpName { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;

    public string OccupationName { get; set; } = string.Empty;

    public string? JobTitle { get; set; }

    public string? Gender { get; set; }

    public DateTime HiredDate { get; set; }

    public DateTime? ResignDate { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? PhotoUrl { get; set; }
}

public class EmployeeDeleteViewModel
{
    public int Id { get; set; }

    public string EmpNo { get; set; } = string.Empty;

    public string EmpName { get; set; } = string.Empty;
}
