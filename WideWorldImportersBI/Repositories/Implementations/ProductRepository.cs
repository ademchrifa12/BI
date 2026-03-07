using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.Data.Oltp;
using WideWorldImportersBI.Entities.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Repositories.Implementations;

/// <summary>
/// Repository implementation for Product (StockItem) entity
/// Provides product-specific data access operations
/// </summary>
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(OltpDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetProductsWithSalesAsync()
    {
        return await _dbSet
            .Include(p => p.InvoiceLines)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithDetailsAsync(int productId)
    {
        return await _dbSet
            .Include(p => p.InvoiceLines)
                .ThenInclude(il => il.Invoice)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.StockItemId == productId);
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm)
    {
        return await _dbSet
            .Where(p => p.StockItemName.Contains(searchTerm))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetBySupplierAsync(int supplierId)
    {
        return await _dbSet
            .Where(p => p.SupplierId == supplierId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByBrandAsync(string brand)
    {
        return await _dbSet
            .Where(p => p.Brand == brand)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetChillerStockAsync()
    {
        return await _dbSet
            .Where(p => p.IsChillerStock)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _dbSet
            .Where(p => p.UnitPrice >= minPrice && p.UnitPrice <= maxPrice)
            .AsNoTracking()
            .ToListAsync();
    }
}
