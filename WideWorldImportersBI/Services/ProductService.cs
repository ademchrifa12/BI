using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Entities.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Services;

/// <summary>
/// Product service implementation
/// Handles product business logic and data transformation
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<PaginatedResponse<ProductDto>> GetProductsPagedAsync(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        var query = _unitOfWork.Products.Query().AsNoTracking();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.StockItemName.Contains(searchTerm) || 
                                     (p.Brand != null && p.Brand.Contains(searchTerm)));
        }

        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        var paginatedData = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<ProductDto>
        {
            Data = paginatedData.Select(MapToDto).ToList(),
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize,
            Success = true
        };
    }

    public async Task<ProductDetailDto?> GetProductByIdAsync(int productId)
    {
        var product = await _unitOfWork.Products.GetProductWithDetailsAsync(productId);
        if (product == null) return null;

        return MapToDetailDto(product);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var products = await _unitOfWork.Products.SearchByNameAsync(searchTerm);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var products = await _unitOfWork.Products.GetByPriceRangeAsync(minPrice, maxPrice);
        return products.Select(MapToDto);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            StockItemId = product.StockItemId,
            StockItemName = product.StockItemName,
            Brand = product.Brand,
            Size = product.Size,
            UnitPrice = product.UnitPrice,
            RecommendedRetailPrice = product.RecommendedRetailPrice,
            TaxRate = product.TaxRate,
            IsChillerStock = product.IsChillerStock,
            QuantityPerOuter = product.QuantityPerOuter
        };
    }

    private static ProductDetailDto MapToDetailDto(Product product)
    {
        return new ProductDetailDto
        {
            StockItemId = product.StockItemId,
            StockItemName = product.StockItemName,
            Brand = product.Brand,
            Size = product.Size,
            UnitPrice = product.UnitPrice,
            RecommendedRetailPrice = product.RecommendedRetailPrice,
            TaxRate = product.TaxRate,
            IsChillerStock = product.IsChillerStock,
            QuantityPerOuter = product.QuantityPerOuter,
            Barcode = product.Barcode,
            LeadTimeDays = product.LeadTimeDays,
            TypicalWeightPerUnit = product.TypicalWeightPerUnit,
            MarketingComments = product.MarketingComments,
            TotalSalesQuantity = product.InvoiceLines?.Sum(il => il.Quantity) ?? 0,
            TotalSalesAmount = product.InvoiceLines?.Sum(il => il.ExtendedPrice) ?? 0
        };
    }

    public async Task<ProductDto> CreateProductAsync(ProductCreateDto dto)
    {
        var product = new Product
        {
            StockItemName = dto.StockItemName,
            SupplierId = 1, // Valid supplier ID from database
            ColorId = null,
            UnitPackageId = 1, // Valid package type ID from database
            OuterPackageId = 1, // Valid package type ID from database
            Brand = dto.Brand,
            Size = dto.Size,
            UnitPrice = dto.UnitPrice,
            RecommendedRetailPrice = dto.RecommendedRetailPrice,
            TaxRate = dto.TaxRate,
            TypicalWeightPerUnit = 0.1m, // Default weight
            IsChillerStock = dto.IsChillerStock,
            QuantityPerOuter = dto.QuantityPerOuter,
            Barcode = dto.Barcode,
            LeadTimeDays = dto.LeadTimeDays,
            LastEditedBy = 1 // Valid PersonID from Application.People
            // ValidFrom and ValidTo are GENERATED ALWAYS columns - don't set them
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task<ProductDto?> UpdateProductAsync(int productId, ProductUpdateDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        if (product == null) return null;

        product.StockItemName = dto.StockItemName;
        product.Brand = dto.Brand;
        product.Size = dto.Size;
        product.UnitPrice = dto.UnitPrice;
        product.RecommendedRetailPrice = dto.RecommendedRetailPrice;
        product.TaxRate = dto.TaxRate;
        product.IsChillerStock = dto.IsChillerStock;
        product.QuantityPerOuter = dto.QuantityPerOuter;
        product.Barcode = dto.Barcode;
        product.LeadTimeDays = dto.LeadTimeDays;
        product.LastEditedBy = 1; // Valid PersonID from Application.People

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        if (product == null) return false;

        _unitOfWork.Products.Delete(product);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
