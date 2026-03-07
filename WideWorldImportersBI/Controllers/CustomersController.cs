using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Services;

namespace WideWorldImportersBI.Controllers;

/// <summary>
/// Customers controller for managing customer data
/// Implements role-based authorization: Admin for write operations, User for read operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all customers with pagination
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="search">Search term for customer name</param>
    /// <returns>Paginated list of customers</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(PaginatedResponse<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, string? search = null)
    {
        var result = await _customerService.GetCustomersPagedAsync(page, pageSize, search);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific customer by ID
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Customer details with related data</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        
        if (customer == null)
        {
            return NotFound(ApiResponse<CustomerDetailDto>.ErrorResponse($"Customer with ID {id} not found"));
        }

        return Ok(ApiResponse<CustomerDetailDto>.SuccessResponse(customer));
    }

    /// <summary>
    /// Searches customers by name
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="term">Search term</param>
    /// <returns>List of matching customers</returns>
    [HttpGet("search")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest(ApiResponse<IEnumerable<CustomerDto>>.ErrorResponse("Search term is required"));
        }

        var customers = await _customerService.SearchCustomersAsync(term);
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(customers));
    }

    /// <summary>
    /// Creates a new customer
    /// Accessible by: Admin only
    /// </summary>
    /// <param name="dto">Customer creation data</param>
    /// <returns>Created customer</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CustomerDto>.ErrorResponse("Invalid request data"));
        }

        var customer = await _customerService.CreateCustomerAsync(dto);
        
        _logger.LogInformation("Customer created: {CustomerId} by user {User}", 
            customer.CustomerId, User.Identity?.Name);

        return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId },
            ApiResponse<CustomerDto>.SuccessResponse(customer, "Customer created successfully"));
    }

    /// <summary>
    /// Updates an existing customer
    /// Accessible by: Admin only
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="dto">Customer update data</param>
    /// <returns>Updated customer</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CustomerDto>.ErrorResponse("Invalid request data"));
        }

        var customer = await _customerService.UpdateCustomerAsync(id, dto);
        
        if (customer == null)
        {
            return NotFound(ApiResponse<CustomerDto>.ErrorResponse($"Customer with ID {id} not found"));
        }

        _logger.LogInformation("Customer updated: {CustomerId} by user {User}", 
            id, User.Identity?.Name);

        return Ok(ApiResponse<CustomerDto>.SuccessResponse(customer, "Customer updated successfully"));
    }

    /// <summary>
    /// Deletes a customer
    /// Accessible by: Admin only
    /// Note: Cannot delete customers with existing orders or invoices
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _customerService.DeleteCustomerAsync(id);
        
        if (!result)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(
                $"Cannot delete customer with ID {id}. Customer may have existing orders or invoices."));
        }

        _logger.LogInformation("Customer deleted: {CustomerId} by user {User}", 
            id, User.Identity?.Name);

        return Ok(ApiResponse<bool>.SuccessResponse(true, "Customer deleted successfully"));
    }
}
