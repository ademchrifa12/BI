using System.ComponentModel.DataAnnotations;

namespace WideWorldImportersBI.DTOs;

#region Authentication DTOs

/// <summary>
/// DTO for user login request
/// </summary>
public class LoginRequestDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO for user registration request
/// </summary>
public class RegisterRequestDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// DTO for authentication response
/// </summary>
public class AuthResponseDto
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public DateTime? Expiration { get; set; }
    public string? Message { get; set; }
    public UserDto? User { get; set; }
}

/// <summary>
/// DTO for user information
/// </summary>
public class UserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// DTO for creating a user (admin)
/// </summary>
public class CreateUserDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; } = "User";
}

/// <summary>
/// DTO for updating a user (admin)
/// </summary>
public class UpdateUserDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; } = "User";

    public bool IsActive { get; set; } = true;

    [StringLength(100, MinimumLength = 6)]
    public string? Password { get; set; }
}

#endregion

#region Customer DTOs

/// <summary>
/// DTO for customer list display
/// </summary>
public class CustomerDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? DeliveryAddressLine1 { get; set; }
    public string? DeliveryAddressLine2 { get; set; }
    public string? DeliveryPostalCode { get; set; }
    public decimal? CreditLimit { get; set; }
    public bool IsOnCreditHold { get; set; }
    public DateTime AccountOpenedDate { get; set; }
    public int OrderCount { get; set; }
}

/// <summary>
/// DTO for customer details with related data
/// </summary>
public class CustomerDetailDto : CustomerDto
{
    public string? FaxNumber { get; set; }
    public decimal StandardDiscountPercentage { get; set; }
    public int PaymentDays { get; set; }
    public bool IsStatementSent { get; set; }
    public List<OrderSummaryDto> RecentOrders { get; set; } = new();
    public List<InvoiceSummaryDto> RecentInvoices { get; set; } = new();
}

/// <summary>
/// DTO for creating/updating a customer
/// </summary>
public class CustomerCreateDto
{
    [Required(ErrorMessage = "Customer name is required")]
    [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters")]
    public string CustomerName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Phone(ErrorMessage = "Invalid fax number")]
    [StringLength(20)]
    public string? FaxNumber { get; set; }

    [Url(ErrorMessage = "Invalid URL")]
    [StringLength(256)]
    public string? WebsiteUrl { get; set; }

    [StringLength(60)]
    public string? DeliveryAddressLine1 { get; set; }

    [StringLength(60)]
    public string? DeliveryAddressLine2 { get; set; }

    [StringLength(10)]
    public string? DeliveryPostalCode { get; set; }

    [StringLength(60)]
    public string? PostalAddressLine1 { get; set; }

    [StringLength(60)]
    public string? PostalAddressLine2 { get; set; }

    [StringLength(10)]
    public string? PostalPostalCode { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Credit limit must be positive")]
    public decimal? CreditLimit { get; set; }
}

/// <summary>
/// DTO for updating a customer
/// </summary>
public class CustomerUpdateDto : CustomerCreateDto
{
    public bool IsOnCreditHold { get; set; }
}

#endregion

#region Product DTOs

/// <summary>
/// DTO for product list display
/// </summary>
public class ProductDto
{
    public int StockItemId { get; set; }
    public string StockItemName { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Size { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? RecommendedRetailPrice { get; set; }
    public decimal TaxRate { get; set; }
    public bool IsChillerStock { get; set; }
    public int QuantityPerOuter { get; set; }
}

/// <summary>
/// DTO for product details
/// </summary>
public class ProductDetailDto : ProductDto
{
    public string? Barcode { get; set; }
    public int LeadTimeDays { get; set; }
    public decimal TypicalWeightPerUnit { get; set; }
    public string? MarketingComments { get; set; }
    public int TotalSalesQuantity { get; set; }
    public decimal TotalSalesAmount { get; set; }
}

/// <summary>
/// DTO for creating a product
/// </summary>
public class ProductCreateDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
    public string StockItemName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Brand { get; set; }

    [StringLength(20)]
    public string? Size { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Unit price must be positive")]
    public decimal UnitPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? RecommendedRetailPrice { get; set; }

    [Range(0, 100, ErrorMessage = "Tax rate must be between 0 and 100")]
    public decimal TaxRate { get; set; }

    public bool IsChillerStock { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity per outer must be at least 1")]
    public int QuantityPerOuter { get; set; }

    [StringLength(128)]
    public string? Barcode { get; set; }

    [Range(0, int.MaxValue)]
    public int LeadTimeDays { get; set; }
}

/// <summary>
/// DTO for updating a product
/// </summary>
public class ProductUpdateDto : ProductCreateDto
{
}

#endregion

#region Order DTOs

/// <summary>
/// DTO for order list display
/// </summary>
public class OrderDto
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public string? CustomerPurchaseOrderNumber { get; set; }
    public bool IsPickingCompleted { get; set; }
    public DateTime? PickingCompletedWhen { get; set; }
}

/// <summary>
/// DTO for order summary (embedded in other DTOs)
/// </summary>
public class OrderSummaryDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public bool IsPickingCompleted { get; set; }
}

/// <summary>
/// DTO for order details
/// </summary>
public class OrderDetailDto : OrderDto
{
    public string? Comments { get; set; }
    public string? DeliveryInstructions { get; set; }
    public bool IsUndersupplyBackordered { get; set; }
}

/// <summary>
/// DTO for creating an order
/// </summary>
public class OrderCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be valid")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Order date is required")]
    public DateTime OrderDate { get; set; }

    [Required(ErrorMessage = "Expected delivery date is required")]
    public DateTime ExpectedDeliveryDate { get; set; }

    [StringLength(20)]
    public string? CustomerPurchaseOrderNumber { get; set; }

    [StringLength(2000)]
    public string? Comments { get; set; }

    [StringLength(2000)]
    public string? DeliveryInstructions { get; set; }

    public bool IsUndersupplyBackordered { get; set; }
}

/// <summary>
/// DTO for updating an order
/// </summary>
public class OrderUpdateDto : OrderCreateDto
{
    public bool IsPickingCompleted { get; set; }
}

#endregion

#region Invoice DTOs

/// <summary>
/// DTO for invoice list display
/// </summary>
public class InvoiceDto
{
    public int InvoiceId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public bool IsCreditNote { get; set; }
    public int TotalLines { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalTax { get; set; }
}

/// <summary>
/// DTO for invoice summary (embedded in other DTOs)
/// </summary>
public class InvoiceSummaryDto
{
    public int InvoiceId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCreditNote { get; set; }
}

/// <summary>
/// DTO for invoice details with lines
/// </summary>
public class InvoiceDetailDto : InvoiceDto
{
    public int? OrderId { get; set; }
    public string? Comments { get; set; }
    public string? DeliveryInstructions { get; set; }
    public int TotalDryItems { get; set; }
    public int TotalChillerItems { get; set; }
    public List<InvoiceLineDto> Lines { get; set; } = new();
}

/// <summary>
/// DTO for invoice line items
/// </summary>
public class InvoiceLineDto
{
    public int InvoiceLineId { get; set; }
    public int StockItemId { get; set; }
    public string StockItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ExtendedPrice { get; set; }
    public decimal LineProfit { get; set; }
}

/// <summary>
/// DTO for creating an invoice
/// </summary>
public class InvoiceCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be valid")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Invoice date is required")]
    public DateTime InvoiceDate { get; set; }

    public int? OrderId { get; set; }

    public bool IsCreditNote { get; set; }

    [StringLength(2000)]
    public string? Comments { get; set; }

    [StringLength(2000)]
    public string? DeliveryInstructions { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalDryItems { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalChillerItems { get; set; }
}

/// <summary>
/// DTO for updating an invoice
/// </summary>
public class InvoiceUpdateDto : InvoiceCreateDto
{
}

#endregion

#region Analytics DTOs

/// <summary>
/// DTO for sales by period analytics
/// </summary>
public class SalesByPeriodDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalProfit { get; set; }
    public int TransactionCount { get; set; }
}

/// <summary>
/// DTO for sales by product analytics
/// </summary>
public class SalesByProductDto
{
    public int StockItemId { get; set; }
    public string StockItemName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalProfit { get; set; }
    public int TransactionCount { get; set; }
}

/// <summary>
/// DTO for sales by customer analytics
/// </summary>
public class SalesByCustomerDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public int TotalInvoices { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalProfit { get; set; }
}

/// <summary>
/// DTO for KPI dashboard
/// </summary>
public class KpiDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalInvoices { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public DateRangeInfo DateRange { get; set; } = new();
}

/// <summary>
/// DTO for date range information
/// </summary>
public class DateRangeInfo
{
    public DateTime? EarliestDate { get; set; }
    public DateTime? LatestDate { get; set; }
}

#endregion


#region DW Dashboard DTOs

public class DwKpiDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal TotalTax { get; set; }
    public int TotalTransactions { get; set; }
    public int UniqueClients { get; set; }
    public int UniqueProducts { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public decimal ProfitMarginPercent { get; set; }
}

public class DwSalesByProductDto
{
    public int ProduitSK { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? StockGroup { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalProfit { get; set; }
}

public class DwSalesByClientDto
{
    public int ClientSK { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? City { get; set; }
    public int TotalTransactions { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalProfit { get; set; }
}

public class DwSalesByEmployeDto
{
    public int EmployeSK { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int TotalTransactions { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalProfit { get; set; }
}

#endregion
#region Common DTOs

/// <summary>
/// Generic API response wrapper
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

/// <summary>
/// Paginated response wrapper
/// </summary>
public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalRecords { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool Success { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}

#endregion

