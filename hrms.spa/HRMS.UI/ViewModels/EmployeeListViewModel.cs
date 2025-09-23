using System.Collections.Generic;
using HRMS.UI.EntityModels;

namespace HRMS.UI.ViewModels;

public class EmployeeListViewModel
{
    public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int? SelectedDepartmentId { get; set; }
    public int? SelectedOccupationId { get; set; }
    public string? SearchEmpNo { get; set; }
    public string? SearchEmpName { get; set; }
}
