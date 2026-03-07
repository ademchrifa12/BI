using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.Data.Oltp;
using WideWorldImportersBI.Entities.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Repositories.Implementations;

/// <summary>
/// Repository implementation for Invoice entity
/// Provides invoice-specific data access operations
/// </summary>
public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(OltpDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesWithCustomersAsync()
    {
        return await _dbSet
            .Include(i => i.Customer)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Invoice?> GetInvoiceWithDetailsAsync(int invoiceId)
    {
        return await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.InvoiceLines)
                .ThenInclude(il => il.StockItem)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
    }

    public async Task<IEnumerable<Invoice>> GetByCustomerAsync(int customerId)
    {
        return await _dbSet
            .Where(i => i.CustomerId == customerId)
            .Include(i => i.Customer)
            .Include(i => i.InvoiceLines)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
            .Include(i => i.Customer)
            .OrderByDescending(i => i.InvoiceDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetCreditNotesAsync()
    {
        return await _dbSet
            .Where(i => i.IsCreditNote)
            .Include(i => i.Customer)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetRecentInvoicesAsync(int count)
    {
        return await _dbSet
            .Include(i => i.Customer)
            .OrderByDescending(i => i.InvoiceDate)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesWithLinesAsync()
    {
        return await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.InvoiceLines)
                .ThenInclude(il => il.StockItem)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<InvoiceLine>> GetInvoiceLinesWithProductsAsync(int invoiceId)
    {
        return await _context.InvoiceLines
            .Where(il => il.InvoiceId == invoiceId)
            .Include(il => il.StockItem)
            .AsNoTracking()
            .ToListAsync();
    }
}
