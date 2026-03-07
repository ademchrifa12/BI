using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WideWorldImportersBI.Entities.Oltp;

/// <summary>
/// Represents a product/stock item from the WideWorldImporters Warehouse.StockItems table
/// Mapped using Database-First approach from the real OLTP database
/// </summary>
[Table("StockItems", Schema = "Warehouse")]
public class Product
{
    [Key]
    [Column("StockItemID")]
    public int StockItemId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("StockItemName")]
    public string StockItemName { get; set; } = string.Empty;

    [Column("SupplierID")]
    public int SupplierId { get; set; }

    [Column("ColorID")]
    public int? ColorId { get; set; }

    [Column("UnitPackageID")]
    public int UnitPackageId { get; set; }

    [Column("OuterPackageID")]
    public int OuterPackageId { get; set; }

    [MaxLength(50)]
    [Column("Brand")]
    public string? Brand { get; set; }

    [MaxLength(20)]
    [Column("Size")]
    public string? Size { get; set; }

    [Column("LeadTimeDays")]
    public int LeadTimeDays { get; set; }

    [Column("QuantityPerOuter")]
    public int QuantityPerOuter { get; set; }

    [Column("IsChillerStock")]
    public bool IsChillerStock { get; set; }

    [MaxLength(50)]
    [Column("Barcode")]
    public string? Barcode { get; set; }

    [Column("TaxRate")]
    public decimal TaxRate { get; set; }

    [Column("UnitPrice")]
    public decimal UnitPrice { get; set; }

    [Column("RecommendedRetailPrice")]
    public decimal? RecommendedRetailPrice { get; set; }

    [Column("TypicalWeightPerUnit")]
    public decimal TypicalWeightPerUnit { get; set; }

    [Column("MarketingComments")]
    public string? MarketingComments { get; set; }

    [Column("InternalComments")]
    public string? InternalComments { get; set; }

    [Column("CustomFields")]
    public string? CustomFields { get; set; }

    [Column("Tags")]
    public string? Tags { get; set; }

    [Column("SearchDetails")]
    public string? SearchDetails { get; set; }

    [Column("LastEditedBy")]
    public int LastEditedBy { get; set; }

    [Column("ValidFrom")]
    public DateTime ValidFrom { get; set; }

    [Column("ValidTo")]
    public DateTime ValidTo { get; set; }

    // Navigation properties
    public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
}
