using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WideWorldImportersBI.Entities.DataWarehouse;

[Table("Dim_Employe")]
public class DimEmploye
{
    [Key]
    [Column("Employe_SK")]
    public int EmployeSK { get; set; }

    [Column("PersonID")]
    public int? PersonId { get; set; }

    [MaxLength(200)]
    [Column("FullName")]
    public string? FullName { get; set; }

    [MaxLength(50)]
    [Column("PhoneNumber")]
    public string? PhoneNumber { get; set; }

    [MaxLength(50)]
    [Column("FaxNumber")]
    public string? FaxNumber { get; set; }

    [MaxLength(200)]
    [Column("EmailAddress")]
    public string? EmailAddress { get; set; }

    public virtual ICollection<FactVentes> Sales { get; set; } = new List<FactVentes>();
}
