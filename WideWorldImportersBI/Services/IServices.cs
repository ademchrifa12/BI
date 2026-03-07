using WideWorldImportersBI.DTOs;

namespace WideWorldImportersBI.Services;

/// <summary>
/// Interface for authentication service
/// Handles user authentication, registration, and JWT token management
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// Registers a new user
    /// </summary>
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

    /// <summary>
    /// Gets user by ID
    /// </summary>
    Task<UserDto?> GetUserByIdAsync(int userId);

    /// <summary>
    /// Gets user by username
    /// </summary>
    Task<UserDto?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// Changes user password
    /// </summary>
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}

/// <summary>
/// Interface for customer service
/// Handles customer business logic
/// </summary>
public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDetailDto?> GetCustomerByIdAsync(int customerId);
    Task<CustomerDto> CreateCustomerAsync(CustomerCreateDto dto);
    Task<CustomerDto?> UpdateCustomerAsync(int customerId, CustomerUpdateDto dto);
    Task<bool> DeleteCustomerAsync(int customerId);
    Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm);
    Task<PaginatedResponse<CustomerDto>> GetCustomersPagedAsync(int page = 1, int pageSize = 10, string? searchTerm = null);
}

/// <summary>
/// Interface for product service
/// Handles product business logic
/// </summary>
public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDetailDto?> GetProductByIdAsync(int productId);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
    Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<PaginatedResponse<ProductDto>> GetProductsPagedAsync(int page = 1, int pageSize = 10, string? searchTerm = null);
    Task<ProductDto> CreateProductAsync(ProductCreateDto dto);
    Task<ProductDto?> UpdateProductAsync(int productId, ProductUpdateDto dto);
    Task<bool> DeleteProductAsync(int productId);
}

/// <summary>
/// Interface for order service
/// Handles order business logic
/// </summary>
public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDetailDto?> GetOrderByIdAsync(int orderId);
    Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(int customerId);
    Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<OrderDto>> GetRecentOrdersAsync(int count = 10);
    Task<PaginatedResponse<OrderDto>> GetOrdersPagedAsync(int page = 1, int pageSize = 10, string? customerName = null, DateTime? dateFrom = null, DateTime? dateTo = null);
    Task<OrderDto> CreateOrderAsync(OrderCreateDto dto);
    Task<OrderDto?> UpdateOrderAsync(int orderId, OrderUpdateDto dto);
    Task<bool> DeleteOrderAsync(int orderId);
}

/// <summary>
/// Interface for invoice service
/// Handles invoice business logic
/// </summary>
public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
    Task<InvoiceDetailDto?> GetInvoiceByIdAsync(int invoiceId);
    Task<IEnumerable<InvoiceDto>> GetInvoicesByCustomerAsync(int customerId);
    Task<IEnumerable<InvoiceDto>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<InvoiceDto>> GetRecentInvoicesAsync(int count = 10);
    Task<PaginatedResponse<InvoiceDto>> GetInvoicesPagedAsync(int page = 1, int pageSize = 10, string? customerName = null, DateTime? dateFrom = null, DateTime? dateTo = null);
    Task<InvoiceDto> CreateInvoiceAsync(InvoiceCreateDto dto);
    Task<InvoiceDto?> UpdateInvoiceAsync(int invoiceId, InvoiceUpdateDto dto);
    Task<bool> DeleteInvoiceAsync(int invoiceId);
}

/// <summary>
/// Interface for analytics service
/// Handles BI analytics and reporting
/// </summary>
public interface IAnalyticsService
{
    Task<IEnumerable<SalesByPeriodDto>> GetSalesByPeriodAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<SalesByProductDto>> GetSalesByProductAsync(int topN = 10);
    Task<IEnumerable<SalesByCustomerDto>> GetSalesByCustomerAsync(int topN = 10);
    Task<KpiDto> GetKpisAsync();
}
