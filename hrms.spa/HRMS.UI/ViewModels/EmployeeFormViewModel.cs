using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS.UI.ViewModels;

public class EmployeeFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Employee No")]
    public string EmpNo { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [Display(Name = "Employee Name")]
    public string EmpName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Department")]
    public int? DepartmentId { get; set; }

    [Required]
    [Display(Name = "Occupation")]
    public int? OccupationId { get; set; }

    [StringLength(150)]
    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [Display(Name = "Hired Date")]
    [DataType(DataType.Date)]
    public DateTime? HiredDate { get; set; }

    [Display(Name = "Resign Date")]
    [DataType(DataType.Date)]
    public DateTime? ResignDate { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Phone]
    [StringLength(50)]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(250)]
    public string? Address { get; set; }

    public string? PhotoUrl { get; set; }

    [Display(Name = "Photo")]
    public IFormFile? Photo { get; set; }

    public IEnumerable<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Occupations { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> GenderOptions { get; set; } = new List<SelectListItem>();
}
