using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Services;

namespace WideWorldImportersBI.Controllers;

/// <summary>
/// Invoices controller for accessing invoice data
/// Read-only operations accessible by authenticated users
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(IInvoiceService invoiceService, ILogger<InvoicesController> logger)
    {
        _invoiceService = invoiceService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all invoices with pagination
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="customerName">Filter by customer name</param>
    /// <param name="dateFrom">Filter by start date</param>
    /// <param name="dateTo">Filter by end date</param>
    /// <returns>Paginated list of invoices</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(PaginatedResponse<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, string? customerName = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var result = await _invoiceService.GetInvoicesPagedAsync(page, pageSize, customerName, dateFrom, dateTo);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific invoice by ID
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="id">Invoice ID</param>
    /// <returns>Invoice details with line items</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<InvoiceDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
        
        if (invoice == null)
        {
            return NotFound(ApiResponse<InvoiceDetailDto>.ErrorResponse($"Invoice with ID {id} not found"));
        }

        return Ok(ApiResponse<InvoiceDetailDto>.SuccessResponse(invoice));
    }

    /// <summary>
    /// Gets invoices for a specific customer
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <returns>List of invoices for the customer</returns>
    [HttpGet("customer/{customerId}")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<InvoiceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(int customerId)
    {
        var invoices = await _invoiceService.GetInvoicesByCustomerAsync(customerId);
        return Ok(ApiResponse<IEnumerable<InvoiceDto>>.SuccessResponse(invoices));
    }

    /// <summary>
    /// Gets invoices within a date range
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of invoices within the date range</returns>
    [HttpGet("date-range")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<InvoiceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest(ApiResponse<IEnumerable<InvoiceDto>>.ErrorResponse("Start date must be before end date"));
        }

        var invoices = await _invoiceService.GetInvoicesByDateRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<InvoiceDto>>.SuccessResponse(invoices));
    }

    /// <summary>
    /// Gets recent invoices
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="count">Number of invoices to retrieve (default: 10)</param>
    /// <returns>List of recent invoices</returns>
    [HttpGet("recent")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<InvoiceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 10)
    {
        if (count <= 0 || count > 100)
        {
            count = 10;
        }

        var invoices = await _invoiceService.GetRecentInvoicesAsync(count);
        return Ok(ApiResponse<IEnumerable<InvoiceDto>>.SuccessResponse(invoices));
    }

    /// <summary>
    /// Creates a new invoice
    /// Accessible by: Admin only
    /// </summary>
    /// <param name="dto">Invoice creation data</param>
    /// <returns>Created invoice</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<InvoiceDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] InvoiceCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<InvoiceDto>.ErrorResponse("Invalid request data"));
        }

        var invoice = await _invoiceService.CreateInvoiceAsync(dto);
        
        _logger.LogInformation("Invoice created: {InvoiceId} by user {User}", 
            invoice.InvoiceId, User.Identity?.Name);

        return CreatedAtAction(nameof(GetById), new { id = invoice.InvoiceId },
            ApiResponse<InvoiceDto>.SuccessResponse(invoice, "Invoice created successfully"));
    }

    /// <summary>
    /// Updates an existing invoice
    /// Accessible by: Admin only
    /// </summary>
    /// <param name="id">Invoice ID</param>
    /// <param name="dto">Invoice update data</param>
    /// <returns>Updated invoice</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<InvoiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] InvoiceUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<InvoiceDto>.ErrorResponse("Invalid request data"));
        }

        var invoice = await _invoiceService.UpdateInvoiceAsync(id, dto);
        
        if (invoice == null)
        {
            return NotFound(ApiResponse<InvoiceDto>.ErrorResponse($"Invoice with ID {id} not found"));
        }

        _logger.LogInformation("Invoice updated: {InvoiceId} by user {User}", 
            id, User.Identity?.Name);

        return Ok(ApiResponse<InvoiceDto>.SuccessResponse(invoice, "Invoice updated successfully"));
    }

    /// <summary>
    /// Deletes an invoice
    /// Accessible by: Admin only
    /// Note: Deletes all associated invoice lines
    /// </summary>
    /// <param name="id">Invoice ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _invoiceService.DeleteInvoiceAsync(id);
        
        if (!result)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(
                $"Cannot delete invoice with ID {id}."));
        }

        _logger.LogInformation("Invoice deleted: {InvoiceId} by user {User}", 
            id, User.Identity?.Name);

        return Ok(ApiResponse<bool>.SuccessResponse(true, "Invoice deleted successfully"));
    }
}
