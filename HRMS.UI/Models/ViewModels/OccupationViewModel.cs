using System.ComponentModel.DataAnnotations;

namespace HRMS.UI.Models.ViewModels;

public class OccupationViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Occupation Name")]
    public string Name { get; set; } = string.Empty;
}
