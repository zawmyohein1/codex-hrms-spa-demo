using System.Collections.Generic;
using HRMS.UI.EntityModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS.UI.ViewModels;

public class OccupationFormViewModel
{
    public Occupation Occupation { get; set; } = new();

    public IEnumerable<SelectListItem> Departments { get; set; } = new List<SelectListItem>();

    public string Action { get; set; } = string.Empty;
}
