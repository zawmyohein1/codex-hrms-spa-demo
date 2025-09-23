using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.UI.Models.EntityModels;

[Table("Employees")]
public class Employee
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string EmpNo { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string EmpName { get; set; } = string.Empty;

    [Required]
    public int DepartmentId { get; set; }

    [Required]
    public int OccupationId { get; set; }

    [StringLength(100)]
    public string? JobTitle { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime HiredDate { get; set; }

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime? ResignDate { get; set; }

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(256)]
    [EmailAddress]
    public string? Email { get; set; }

    public string? Address { get; set; }

    [StringLength(500)]
    public string? PhotoUrl { get; set; }

    public Department Department { get; set; } = default!;

    public Occupation Occupation { get; set; } = default!;
}
