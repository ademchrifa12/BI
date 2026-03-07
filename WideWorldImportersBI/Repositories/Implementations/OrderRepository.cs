using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.Data.Oltp;
using WideWorldImportersBI.Entities.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Repositories.Implementations;

/// <summary>
/// Repository implementation for Order entity
/// Provides order-specific data access operations
/// </summary>
public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(OltpDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersWithCustomersAsync()
    {
        return await _dbSet
            .Include(o => o.Customer)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<IEnumerable<Order>> GetByCustomerAsync(int customerId)
    {
        return await _dbSet
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.Customer)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
            .Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetBySalespersonAsync(int salespersonId)
    {
        return await _dbSet
            .Where(o => o.SalespersonPersonId == salespersonId)
            .Include(o => o.Customer)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
    {
        return await _dbSet
            .Where(o => o.PickingCompletedWhen == null)
            .Include(o => o.Customer)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDate)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }
}
