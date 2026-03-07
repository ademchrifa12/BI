using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WideWorldImportersBI.Entities.Oltp;

/// <summary>
/// Represents an invoice line from the WideWorldImporters Sales.InvoiceLines table
/// Mapped using Database-First approach from the real OLTP database
/// </summary>
[Table("InvoiceLines", Schema = "Sales")]
public class InvoiceLine
{
    [Key]
    [Column("InvoiceLineID")]
    public int InvoiceLineId { get; set; }

    [Column("InvoiceID")]
    public int InvoiceId { get; set; }

    [Column("StockItemID")]
    public int StockItemId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("Description")]
    public string Description { get; set; } = string.Empty;

    [Column("PackageTypeID")]
    public int PackageTypeId { get; set; }

    [Column("Quantity")]
    public int Quantity { get; set; }

    [Column("UnitPrice")]
    public decimal? UnitPrice { get; set; }

    [Column("TaxRate")]
    public decimal TaxRate { get; set; }

    [Column("TaxAmount")]
    public decimal TaxAmount { get; set; }

    [Column("LineProfit")]
    public decimal LineProfit { get; set; }

    [Column("ExtendedPrice")]
    public decimal ExtendedPrice { get; set; }

    [Column("LastEditedBy")]
    public int LastEditedBy { get; set; }

    [Column("LastEditedWhen")]
    public DateTime LastEditedWhen { get; set; }

    // Navigation properties
    [ForeignKey("InvoiceId")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("StockItemId")]
    public virtual Product? StockItem { get; set; }
}
