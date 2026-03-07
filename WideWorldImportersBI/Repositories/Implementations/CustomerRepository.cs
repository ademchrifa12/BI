using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.Data.Oltp;
using WideWorldImportersBI.Entities.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Repositories.Implementations;

/// <summary>
/// Repository implementation for Customer entity
/// Provides customer-specific data access operations
/// </summary>
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(OltpDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Customer>> GetCustomersWithOrdersAsync()
    {
        return await _dbSet
            .Include(c => c.Orders)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Customer?> GetCustomerWithDetailsAsync(int customerId)
    {
        return await _dbSet
            .Include(c => c.Orders)
            .Include(c => c.Invoices)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public async Task<IEnumerable<Customer>> SearchByNameAsync(string searchTerm)
    {
        return await _dbSet
            .Where(c => c.CustomerName.Contains(searchTerm))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(c => c.CustomerCategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
    {
        return await _dbSet
            .Where(c => !c.IsOnCreditHold)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetTopCustomersAsync(int count)
    {
        return await _dbSet
            .Include(c => c.Orders)
            .OrderByDescending(c => c.Orders.Count)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }
}
