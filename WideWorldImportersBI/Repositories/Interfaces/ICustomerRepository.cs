using WideWorldImportersBI.Entities.Oltp;

namespace WideWorldImportersBI.Repositories.Interfaces;

/// <summary>
/// Repository interface for Customer entity operations
/// Extends generic repository with customer-specific methods
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// Gets customers with their orders
    /// </summary>
    Task<IEnumerable<Customer>> GetCustomersWithOrdersAsync();

    /// <summary>
    /// Gets a customer with their orders and invoices
    /// </summary>
    Task<Customer?> GetCustomerWithDetailsAsync(int customerId);

    /// <summary>
    /// Searches customers by name
    /// </summary>
    Task<IEnumerable<Customer>> SearchByNameAsync(string searchTerm);

    /// <summary>
    /// Gets customers by category
    /// </summary>
    Task<IEnumerable<Customer>> GetByCategoryAsync(int categoryId);

    /// <summary>
    /// Gets active customers (not on credit hold)
    /// </summary>
    Task<IEnumerable<Customer>> GetActiveCustomersAsync();

    /// <summary>
    /// Gets top customers by order count
    /// </summary>
    Task<IEnumerable<Customer>> GetTopCustomersAsync(int count);
}
