using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WideWorldImportersBI.Entities.Oltp;

/// <summary>
/// Represents a customer from the WideWorldImporters Sales.Customers table
/// Mapped using Database-First approach from the real OLTP database
/// </summary>
[Table("Customers", Schema = "Sales")]
public class Customer
{
    [Key]
    [Column("CustomerID")]
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("CustomerName")]
    public string CustomerName { get; set; } = string.Empty;

    [Column("BillToCustomerID")]
    public int BillToCustomerId { get; set; }

    [Column("CustomerCategoryID")]
    public int CustomerCategoryId { get; set; }

    [Column("BuyingGroupID")]
    public int? BuyingGroupId { get; set; }

    [Column("PrimaryContactPersonID")]
    public int PrimaryContactPersonId { get; set; }

    [Column("AlternateContactPersonID")]
    public int? AlternateContactPersonId { get; set; }

    [Column("DeliveryMethodID")]
    public int DeliveryMethodId { get; set; }

    [Column("DeliveryCityID")]
    public int DeliveryCityId { get; set; }

    [Column("PostalCityID")]
    public int PostalCityId { get; set; }

    [Column("CreditLimit")]
    public decimal? CreditLimit { get; set; }

    [Column("AccountOpenedDate")]
    public DateTime AccountOpenedDate { get; set; }

    [Column("StandardDiscountPercentage")]
    public decimal StandardDiscountPercentage { get; set; }

    [Column("IsStatementSent")]
    public bool IsStatementSent { get; set; }

    [Column("IsOnCreditHold")]
    public bool IsOnCreditHold { get; set; }

    [Column("PaymentDays")]
    public int PaymentDays { get; set; }

    [MaxLength(20)]
    [Column("PhoneNumber")]
    public string? PhoneNumber { get; set; }

    [MaxLength(20)]
    [Column("FaxNumber")]
    public string? FaxNumber { get; set; }

    [MaxLength(5)]
    [Column("DeliveryRun")]
    public string? DeliveryRun { get; set; }

    [MaxLength(5)]
    [Column("RunPosition")]
    public string? RunPosition { get; set; }

    [MaxLength(256)]
    [Column("WebsiteURL")]
    public string? WebsiteUrl { get; set; }

    [MaxLength(60)]
    [Column("DeliveryAddressLine1")]
    public string? DeliveryAddressLine1 { get; set; }

    [MaxLength(60)]
    [Column("DeliveryAddressLine2")]
    public string? DeliveryAddressLine2 { get; set; }

    [MaxLength(10)]
    [Column("DeliveryPostalCode")]
    public string? DeliveryPostalCode { get; set; }

    [MaxLength(60)]
    [Column("PostalAddressLine1")]
    public string? PostalAddressLine1 { get; set; }

    [MaxLength(60)]
    [Column("PostalAddressLine2")]
    public string? PostalAddressLine2 { get; set; }

    [MaxLength(10)]
    [Column("PostalPostalCode")]
    public string? PostalPostalCode { get; set; }

    [Column("LastEditedBy")]
    public int LastEditedBy { get; set; }

    [Column("ValidFrom")]
    public DateTime ValidFrom { get; set; }

    [Column("ValidTo")]
    public DateTime ValidTo { get; set; }

    // Navigation properties
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
