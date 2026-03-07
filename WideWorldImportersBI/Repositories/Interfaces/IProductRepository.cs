using WideWorldImportersBI.Entities.Oltp;

namespace WideWorldImportersBI.Repositories.Interfaces;

/// <summary>
/// Repository interface for Product (StockItem) entity operations
/// Extends generic repository with product-specific methods
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// Gets products with their invoice lines
    /// </summary>
    Task<IEnumerable<Product>> GetProductsWithSalesAsync();

    /// <summary>
    /// Gets a product with its sales details
    /// </summary>
    Task<Product?> GetProductWithDetailsAsync(int productId);

    /// <summary>
    /// Searches products by name
    /// </summary>
    Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm);

    /// <summary>
    /// Gets products by supplier
    /// </summary>
    Task<IEnumerable<Product>> GetBySupplierAsync(int supplierId);

    /// <summary>
    /// Gets products by brand
    /// </summary>
    Task<IEnumerable<Product>> GetByBrandAsync(string brand);

    /// <summary>
    /// Gets chiller stock products
    /// </summary>
    Task<IEnumerable<Product>> GetChillerStockAsync();

    /// <summary>
    /// Gets products within a price range
    /// </summary>
    Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
}
