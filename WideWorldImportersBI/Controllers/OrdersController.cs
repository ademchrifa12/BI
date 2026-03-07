using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Services;

namespace WideWorldImportersBI.Controllers;

/// <summary>
/// Orders controller for accessing order data
/// Read-only operations accessible by authenticated users
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all orders with pagination
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="customerName">Filter by customer name</param>
    /// <param name="dateFrom">Filter by start date</param>
    /// <param name="dateTo">Filter by end date</param>
    /// <returns>Paginated list of orders</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(PaginatedResponse<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, string? customerName = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var result = await _orderService.GetOrdersPagedAsync(page, pageSize, customerName, dateFrom, dateTo);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific order by ID
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Order details</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<OrderDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        
        if (order == null)
        {
            return NotFound(ApiResponse<OrderDetailDto>.ErrorResponse($"Order with ID {id} not found"));
        }

        return Ok(ApiResponse<OrderDetailDto>.SuccessResponse(order));
    }

    /// <summary>
    /// Gets orders for a specific customer
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <returns>List of orders for the customer</returns>
    [HttpGet("customer/{customerId}")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(int customerId)
    {
        var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
        return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders));
    }

    /// <summary>
    /// Gets orders within a date range
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of orders within the date range</returns>
    [HttpGet("date-range")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest(ApiResponse<IEnumerable<OrderDto>>.ErrorResponse("Start date must be before end date"));
        }

        var orders = await _orderService.GetOrdersByDateRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders));
    }

    /// <summary>
    /// Gets recent orders
    /// Accessible by: Admin, User
    /// </summary>
    /// <param name="count">Number of orders to retrieve (default: 10)</param>
    /// <returns>List of recent orders</returns>
    [HttpGet("recent")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 10)
    {
        if (count <= 0 || count > 100)
        {
            count = 10;
        }

        var orders = await _orderService.GetRecentOrdersAsync(count);
        return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders));
    }

    /// <summary>
    /// Creates a new order
    /// Accessible by: Admin only
    /// </summary>
    /// <param name="dto">Order creation data</param>
    /// <returns>Created order</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<OrderDto>.ErrorResponse("Invalid request data"));
        }

        var order = await _orderService.CreateOrderAsync(dto);
        
        _logger.LogInformation("Order created: {OrderId} by user {User}", 
            order.OrderId, User.Identity?.Name);

        return CreatedAtAction(nameof(GetById), new { id = order.OrderId },
            ApiResponse<OrderDto>.SuccessResponse(order, "Order created successfully"));
    }

    /// <summary>
    /// Updates an existing order
    /// Accessible by: Admin only
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="dto">Order update data</param>
    /// <returns>Updated order</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] OrderUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<OrderDto>.ErrorResponse("Invalid request data"));
        }

        var order = await _orderService.UpdateOrderAsync(id, dto);
        
        if (order == null)
        {
            return NotFound(ApiResponse<OrderDto>.ErrorResponse($"Order with ID {id} not found"));
        }

        _logger.LogInformation("Order updated: {OrderId} by user {User}", 
            id, User.Identity?.Name);

        return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order updated successfully"));
    }

    /// <summary>
    /// Deletes an order
    /// Accessible by: Admin only
    /// Note: Cannot delete orders that have been invoiced
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _orderService.DeleteOrderAsync(id);
        
        if (!result)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(
                $"Cannot delete order with ID {id}. Order may have existing invoices."));
        }

        _logger.LogInformation("Order deleted: {OrderId} by user {User}", 
            id, User.Identity?.Name);

        return Ok(ApiResponse<bool>.SuccessResponse(true, "Order deleted successfully"));
    }
}
