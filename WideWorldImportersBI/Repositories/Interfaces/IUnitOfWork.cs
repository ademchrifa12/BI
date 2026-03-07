namespace WideWorldImportersBI.Repositories.Interfaces;

/// <summary>
/// Unit of Work interface for managing transactions across multiple repositories
/// Ensures atomic operations and consistent database state
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Customer repository
    /// </summary>
    ICustomerRepository Customers { get; }

    /// <summary>
    /// Product repository
    /// </summary>
    IProductRepository Products { get; }

    /// <summary>
    /// Order repository
    /// </summary>
    IOrderRepository Orders { get; }

    /// <summary>
    /// Invoice repository
    /// </summary>
    IInvoiceRepository Invoices { get; }

    /// <summary>
    /// User repository
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Role repository
    /// </summary>
    IRoleRepository Roles { get; }

    /// <summary>
    /// Saves all changes made in this unit of work to the database
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    Task RollbackTransactionAsync();
}
