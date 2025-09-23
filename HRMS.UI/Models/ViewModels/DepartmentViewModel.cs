using System.ComponentModel.DataAnnotations;

namespace HRMS.UI.Models.ViewModels;

public class DepartmentViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Department Name")]
    public string Name { get; set; } = string.Empty;
}
