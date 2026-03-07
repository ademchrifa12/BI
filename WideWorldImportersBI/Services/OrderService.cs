using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Entities.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Services;

/// <summary>
/// Order service implementation
/// Handles order business logic and data transformation
/// </summary>
public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _unitOfWork.Orders.GetOrdersWithCustomersAsync();
        return orders.Select(MapToDto);
    }

    public async Task<PaginatedResponse<OrderDto>> GetOrdersPagedAsync(int page = 1, int pageSize = 10, string? customerName = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var query = _unitOfWork.Orders.Query()
            .Include(o => o.Customer)
            .AsNoTracking();

        // Apply filters if provided
        if (!string.IsNullOrWhiteSpace(customerName))
        {
            query = query.Where(o => o.Customer != null && o.Customer.CustomerName.Contains(customerName));
        }
        if (dateFrom.HasValue)
        {
            query = query.Where(o => o.OrderDate >= dateFrom.Value);
        }
        if (dateTo.HasValue)
        {
            query = query.Where(o => o.OrderDate <= dateTo.Value);
        }

        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        var paginatedData = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<OrderDto>
        {
            Data = paginatedData.Select(MapToDto).ToList(),
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize,
            Success = true
        };
    }

    public async Task<OrderDetailDto?> GetOrderByIdAsync(int orderId)
    {
        var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
        if (order == null) return null;

        return MapToDetailDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(int customerId)
    {
        var orders = await _unitOfWork.Orders.GetByCustomerAsync(customerId);
        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var orders = await _unitOfWork.Orders.GetByDateRangeAsync(startDate, endDate);
        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderDto>> GetRecentOrdersAsync(int count = 10)
    {
        var orders = await _unitOfWork.Orders.GetRecentOrdersAsync(count);
        return orders.Select(MapToDto);
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.CustomerName ?? "Unknown",
            OrderDate = order.OrderDate,
            ExpectedDeliveryDate = order.ExpectedDeliveryDate,
            CustomerPurchaseOrderNumber = order.CustomerPurchaseOrderNumber,
            IsPickingCompleted = order.PickingCompletedWhen != null,
            PickingCompletedWhen = order.PickingCompletedWhen
        };
    }

    private static OrderDetailDto MapToDetailDto(Order order)
    {
        return new OrderDetailDto
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.CustomerName ?? "Unknown",
            OrderDate = order.OrderDate,
            ExpectedDeliveryDate = order.ExpectedDeliveryDate,
            CustomerPurchaseOrderNumber = order.CustomerPurchaseOrderNumber,
            IsPickingCompleted = order.PickingCompletedWhen != null,
            PickingCompletedWhen = order.PickingCompletedWhen,
            Comments = order.Comments,
            DeliveryInstructions = order.DeliveryInstructions,
            IsUndersupplyBackordered = order.IsUndersupplyBackordered
        };
    }

    public async Task<OrderDto> CreateOrderAsync(OrderCreateDto dto)
    {
        var order = new Order
        {
            CustomerId = dto.CustomerId,
            SalespersonPersonId = 2, // Default salesperson ID
            ContactPersonId = 2, // Default contact person ID
            OrderDate = dto.OrderDate,
            ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
            CustomerPurchaseOrderNumber = dto.CustomerPurchaseOrderNumber,
            Comments = dto.Comments,
            DeliveryInstructions = dto.DeliveryInstructions,
            IsUndersupplyBackordered = dto.IsUndersupplyBackordered,
            LastEditedBy = 1, // Valid PersonID from Application.People
            LastEditedWhen = DateTime.UtcNow
        };

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        var createdOrder = await _unitOfWork.Orders.GetByIdAsync(order.OrderId, o => o.Customer!);
        return MapToDto(createdOrder!);
    }

    public async Task<OrderDto?> UpdateOrderAsync(int orderId, OrderUpdateDto dto)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId, o => o.Customer!);
        if (order == null) return null;

        order.CustomerId = dto.CustomerId;
        order.OrderDate = dto.OrderDate;
        order.ExpectedDeliveryDate = dto.ExpectedDeliveryDate;
        order.CustomerPurchaseOrderNumber = dto.CustomerPurchaseOrderNumber;
        order.Comments = dto.Comments;
        order.DeliveryInstructions = dto.DeliveryInstructions;
        order.IsUndersupplyBackordered = dto.IsUndersupplyBackordered;
        order.LastEditedBy = 1; // Valid PersonID from Application.People
        order.LastEditedWhen = DateTime.UtcNow;

        if (dto.IsPickingCompleted && order.PickingCompletedWhen == null)
        {
            order.PickingCompletedWhen = DateTime.UtcNow;
        }

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(order);
    }

    public async Task<bool> DeleteOrderAsync(int orderId)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order == null) return false;

        _unitOfWork.Orders.Delete(order);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
