using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WideWorldImportersBI.Entities.Oltp;

/// <summary>
/// Represents an order from the WideWorldImporters Sales.Orders table
/// Mapped using Database-First approach from the real OLTP database
/// </summary>
[Table("Orders", Schema = "Sales")]
public class Order
{
    [Key]
    [Column("OrderID")]
    public int OrderId { get; set; }

    [Column("CustomerID")]
    public int CustomerId { get; set; }

    [Column("SalespersonPersonID")]
    public int SalespersonPersonId { get; set; }

    [Column("PickedByPersonID")]
    public int? PickedByPersonId { get; set; }

    [Column("ContactPersonID")]
    public int ContactPersonId { get; set; }

    [Column("BackorderOrderID")]
    public int? BackorderOrderId { get; set; }

    [Column("OrderDate")]
    public DateTime OrderDate { get; set; }

    [Column("ExpectedDeliveryDate")]
    public DateTime ExpectedDeliveryDate { get; set; }

    [MaxLength(20)]
    [Column("CustomerPurchaseOrderNumber")]
    public string? CustomerPurchaseOrderNumber { get; set; }

    [Column("IsUndersupplyBackordered")]
    public bool IsUndersupplyBackordered { get; set; }

    [Column("Comments")]
    public string? Comments { get; set; }

    [Column("DeliveryInstructions")]
    public string? DeliveryInstructions { get; set; }

    [Column("InternalComments")]
    public string? InternalComments { get; set; }

    [Column("PickingCompletedWhen")]
    public DateTime? PickingCompletedWhen { get; set; }

    [Column("LastEditedBy")]
    public int LastEditedBy { get; set; }

    [Column("LastEditedWhen")]
    public DateTime LastEditedWhen { get; set; }

    // Navigation properties
    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }
}
