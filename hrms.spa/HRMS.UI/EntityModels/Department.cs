using System.ComponentModel.DataAnnotations;

namespace HRMS.UI.EntityModels;

public class Department
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; }

    public ICollection<Occupation> Occupations { get; set; } = new List<Occupation>();
}
