using Microsoft.EntityFrameworkCore.Storage;
using WideWorldImportersBI.Data.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Repositories.Implementations;

/// <summary>
/// Unit of Work implementation for managing transactions across repositories
/// Ensures atomic operations and consistent database state
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly OltpDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;

    private ICustomerRepository? _customers;
    private IProductRepository? _products;
    private IOrderRepository? _orders;
    private IInvoiceRepository? _invoices;
    private IUserRepository? _users;
    private IRoleRepository? _roles;

    public UnitOfWork(OltpDbContext context)
    {
        _context = context;
    }

    public ICustomerRepository Customers
    {
        get { return _customers ??= new CustomerRepository(_context); }
    }

    public IProductRepository Products
    {
        get { return _products ??= new ProductRepository(_context); }
    }

    public IOrderRepository Orders
    {
        get { return _orders ??= new OrderRepository(_context); }
    }

    public IInvoiceRepository Invoices
    {
        get { return _invoices ??= new InvoiceRepository(_context); }
    }

    public IUserRepository Users
    {
        get { return _users ??= new UserRepository(_context); }
    }

    public IRoleRepository Roles
    {
        get { return _roles ??= new RoleRepository(_context); }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
