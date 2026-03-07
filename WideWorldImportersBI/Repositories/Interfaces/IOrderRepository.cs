using WideWorldImportersBI.Entities.Oltp;

namespace WideWorldImportersBI.Repositories.Interfaces;

/// <summary>
/// Repository interface for Order entity operations
/// Extends generic repository with order-specific methods
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Gets orders with customer details
    /// </summary>
    Task<IEnumerable<Order>> GetOrdersWithCustomersAsync();

    /// <summary>
    /// Gets an order with its customer
    /// </summary>
    Task<Order?> GetOrderWithDetailsAsync(int orderId);

    /// <summary>
    /// Gets orders by customer
    /// </summary>
    Task<IEnumerable<Order>> GetByCustomerAsync(int customerId);

    /// <summary>
    /// Gets orders within a date range
    /// </summary>
    Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets orders by salesperson
    /// </summary>
    Task<IEnumerable<Order>> GetBySalespersonAsync(int salespersonId);

    /// <summary>
    /// Gets pending orders (not yet picked)
    /// </summary>
    Task<IEnumerable<Order>> GetPendingOrdersAsync();

    /// <summary>
    /// Gets recent orders
    /// </summary>
    Task<IEnumerable<Order>> GetRecentOrdersAsync(int count);
}
