using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WideWorldImportersBI.Entities.DataWarehouse;

[Table("Dim_Produit")]
public class DimProduit
{
    [Key]
    [Column("Produit_SK")]
    public int ProduitSK { get; set; }

    [Column("StockItemID")]
    public int? StockItemId { get; set; }

    [MaxLength(300)]
    [Column("StockItemName")]
    public string? StockItemName { get; set; }

    [MaxLength(200)]
    [Column("Brand")]
    public string? Brand { get; set; }

    [MaxLength(50)]
    [Column("Size")]
    public string? Size { get; set; }

    [Column("UnitPrice")]
    public decimal? UnitPrice { get; set; }

    [Column("TaxRate")]
    public decimal? TaxRate { get; set; }

    [Column("IsChillerStock")]
    public bool? IsChillerStock { get; set; }

    [MaxLength(200)]
    [Column("StockGroupName")]
    public string? StockGroupName { get; set; }

    [MaxLength(200)]
    [Column("ColorName")]
    public string? ColorName { get; set; }

    public virtual ICollection<FactVentes> Sales { get; set; } = new List<FactVentes>();
}
