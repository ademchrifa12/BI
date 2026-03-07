using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WideWorldImportersBI.Entities.DataWarehouse;

[Table("Dim_Client")]
public class DimClient
{
    [Key]
    [Column("Client_SK")]
    public int ClientSK { get; set; }

    [Column("CustomerID")]
    public int? CustomerId { get; set; }

    [MaxLength(200)]
    [Column("CustomerName")]
    public string? CustomerName { get; set; }

    [MaxLength(200)]
    [Column("CategoryName")]
    public string? CategoryName { get; set; }

    [MaxLength(200)]
    [Column("BuyingGroupName")]
    public string? BuyingGroupName { get; set; }

    [MaxLength(200)]
    [Column("CityName")]
    public string? CityName { get; set; }

    [Column("CreditLimit")]
    public decimal? CreditLimit { get; set; }

    [Column("PaymentDays")]
    public int? PaymentDays { get; set; }

    public virtual ICollection<FactVentes> Sales { get; set; } = new List<FactVentes>();
}
