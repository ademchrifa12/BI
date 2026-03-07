using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Services;

namespace WideWorldImportersBI.Controllers;

/// <summary>
/// Products controller for managing product/stock item data
/// Read-only operations accessible by authenticated users
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all products with pagination
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="search">Search term for product name or brand</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(PaginatedResponse<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, string? search = null)
    {
        var result = await _productService.GetProductsPagedAsync(page, pageSize, search);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific product by ID
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="id">Product/Stock Item ID</param>
    /// <returns>Product details with sales information</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<ProductDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        
        if (product == null)
        {
            return NotFound(ApiResponse<ProductDetailDto>.ErrorResponse($"Product with ID {id} not found"));
        }

        return Ok(ApiResponse<ProductDetailDto>.SuccessResponse(product));
    }

    /// <summary>
    /// Searches products by name
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="term">Search term</param>
    /// <returns>List of matching products</returns>
    [HttpGet("search")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest(ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("Search term is required"));
        }

        var products = await _productService.SearchProductsAsync(term);
        return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products));
    }

    /// <summary>
    /// Gets products within a price range
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="minPrice">Minimum price</param>
    /// <param name="maxPrice">Maximum price</param>
    /// <returns>List of products within the price range</returns>
    [HttpGet("price-range")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
        {
            return BadRequest(ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("Invalid price range"));
        }

        var products = await _productService.GetProductsByPriceRangeAsync(minPrice, maxPrice);
        return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products));
    }

    /// <summary>
    /// Creates a new product
    /// Accessible by: Admin only
    /// </summary>
    /// <param name="dto">Product creation data</param>
    /// <returns>Created product</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<ProductDto>.ErrorResponse("Invalid request data"));
        }

        var product = await _productService.CreateProductAsync(dto);
        
        _logger.LogInformation("Product created: {ProductId} by user {User}", 
            product.StockItemId, User.Identity?.Name);

        return CreatedAtAction(nameof(GetById), new { id = product.StockItemId },
            ApiResponse<ProductDto>.SuccessResponse(product, "Product created successfully"));
    }

    /// <summary>
    /// Updates an existing product
    /// Accessible by: Admin only
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="dto">Product update data</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<ProductDto>.ErrorResponse("Invalid request data"));
        }

        var product = await _productService.UpdateProductAsync(id, dto);
        
        if (product == null)
        {
            return NotFound(ApiResponse<ProductDto>.ErrorResponse($"Product with ID {id} not found"));
        }

        _logger.LogInformation("Product updated: {ProductId} by user {User}", 
            id, User.Identity?.Name);

        return Ok(ApiResponse<ProductDto>.SuccessResponse(product, "Product updated successfully"));
    }

    /// <summary>
    /// Deletes a product
    /// Accessible by: Admin only
    /// Note: Cannot delete products that have been used in invoices
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        
        if (!result)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(
                $"Cannot delete product with ID {id}. Product may have existing invoice lines."));
        }

        _logger.LogInformation("Product deleted: {ProductId} by user {User}", 
            id, User.Identity?.Name);

        return Ok(ApiResponse<bool>.SuccessResponse(true, "Product deleted successfully"));
    }
}
