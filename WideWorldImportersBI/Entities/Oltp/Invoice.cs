using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WideWorldImportersBI.Entities.Oltp;

/// <summary>
/// Represents an invoice from the WideWorldImporters Sales.Invoices table
/// Mapped using Database-First approach from the real OLTP database
/// </summary>
[Table("Invoices", Schema = "Sales")]
public class Invoice
{
    [Key]
    [Column("InvoiceID")]
    public int InvoiceId { get; set; }

    [Column("CustomerID")]
    public int CustomerId { get; set; }

    [Column("BillToCustomerID")]
    public int BillToCustomerId { get; set; }

    [Column("OrderID")]
    public int? OrderId { get; set; }

    [Column("DeliveryMethodID")]
    public int DeliveryMethodId { get; set; }

    [Column("ContactPersonID")]
    public int ContactPersonId { get; set; }

    [Column("AccountsPersonID")]
    public int AccountsPersonId { get; set; }

    [Column("SalespersonPersonID")]
    public int SalespersonPersonId { get; set; }

    [Column("PackedByPersonID")]
    public int PackedByPersonId { get; set; }

    [Column("InvoiceDate")]
    public DateTime InvoiceDate { get; set; }

    [MaxLength(20)]
    [Column("CustomerPurchaseOrderNumber")]
    public string? CustomerPurchaseOrderNumber { get; set; }

    [Column("IsCreditNote")]
    public bool IsCreditNote { get; set; }

    [Column("CreditNoteReason")]
    public string? CreditNoteReason { get; set; }

    [Column("Comments")]
    public string? Comments { get; set; }

    [Column("DeliveryInstructions")]
    public string? DeliveryInstructions { get; set; }

    [Column("InternalComments")]
    public string? InternalComments { get; set; }

    [Column("TotalDryItems")]
    public int TotalDryItems { get; set; }

    [Column("TotalChillerItems")]
    public int TotalChillerItems { get; set; }

    [MaxLength(5)]
    [Column("DeliveryRun")]
    public string? DeliveryRun { get; set; }

    [MaxLength(5)]
    [Column("RunPosition")]
    public string? RunPosition { get; set; }

    [Column("ReturnedDeliveryData")]
    public string? ReturnedDeliveryData { get; set; }

    [Column("ConfirmedDeliveryTime")]
    public DateTime? ConfirmedDeliveryTime { get; set; }

    [Column("ConfirmedReceivedBy")]
    public string? ConfirmedReceivedBy { get; set; }

    [Column("LastEditedBy")]
    public int LastEditedBy { get; set; }

    [Column("LastEditedWhen")]
    public DateTime LastEditedWhen { get; set; }

    // Navigation properties
    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }
    
    public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
}
