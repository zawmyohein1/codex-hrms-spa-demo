using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS.UI.ViewModels;

public class EmployeesIndexViewModel
{
    public IEnumerable<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Occupations { get; set; } = new List<SelectListItem>();
}
