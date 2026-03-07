using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Entities.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Services;

/// <summary>
/// Invoice service implementation
/// Handles invoice business logic and data transformation
/// </summary>
public class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(IUnitOfWork unitOfWork, ILogger<InvoiceService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
    {
        var invoices = await _unitOfWork.Invoices.GetInvoicesWithLinesAsync();
        return invoices.Select(MapToDto);
    }

    public async Task<PaginatedResponse<InvoiceDto>> GetInvoicesPagedAsync(int page = 1, int pageSize = 10, string? customerName = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var query = _unitOfWork.Invoices.Query()
            .Include(i => i.Customer)
            .Include(i => i.InvoiceLines)
            .AsNoTracking();

        // Apply filters if provided
        if (!string.IsNullOrWhiteSpace(customerName))
        {
            query = query.Where(i => i.Customer != null && i.Customer.CustomerName.Contains(customerName));
        }
        if (dateFrom.HasValue)
        {
            query = query.Where(i => i.InvoiceDate >= dateFrom.Value);
        }
        if (dateTo.HasValue)
        {
            query = query.Where(i => i.InvoiceDate <= dateTo.Value);
        }

        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        
        var paginatedData = await query
            .OrderByDescending(i => i.InvoiceDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<InvoiceDto>
        {
            Data = paginatedData.Select(MapToDto).ToList(),
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize,
            Success = true
        };
    }

    public async Task<InvoiceDetailDto?> GetInvoiceByIdAsync(int invoiceId)
    {
        var invoice = await _unitOfWork.Invoices.GetInvoiceWithDetailsAsync(invoiceId);
        if (invoice == null) return null;

        return MapToDetailDto(invoice);
    }

    public async Task<IEnumerable<InvoiceDto>> GetInvoicesByCustomerAsync(int customerId)
    {
        var invoices = await _unitOfWork.Invoices.GetByCustomerAsync(customerId);
        return invoices.Select(MapToDto);
    }

    public async Task<IEnumerable<InvoiceDto>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var invoices = await _unitOfWork.Invoices.GetByDateRangeAsync(startDate, endDate);
        return invoices.Select(MapToDto);
    }

    public async Task<IEnumerable<InvoiceDto>> GetRecentInvoicesAsync(int count = 10)
    {
        var invoices = await _unitOfWork.Invoices.GetRecentInvoicesAsync(count);
        return invoices.Select(MapToDto);
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            InvoiceId = invoice.InvoiceId,
            CustomerId = invoice.CustomerId,
            CustomerName = invoice.Customer?.CustomerName ?? "Unknown",
            InvoiceDate = invoice.InvoiceDate,
            IsCreditNote = invoice.IsCreditNote,
            TotalLines = invoice.InvoiceLines?.Count ?? 0,
            TotalAmount = invoice.InvoiceLines?.Sum(il => il.ExtendedPrice) ?? 0,
            TotalTax = invoice.InvoiceLines?.Sum(il => il.TaxAmount) ?? 0
        };
    }

    private static InvoiceDetailDto MapToDetailDto(Invoice invoice)
    {
        return new InvoiceDetailDto
        {
            InvoiceId = invoice.InvoiceId,
            CustomerId = invoice.CustomerId,
            CustomerName = invoice.Customer?.CustomerName ?? "Unknown",
            InvoiceDate = invoice.InvoiceDate,
            IsCreditNote = invoice.IsCreditNote,
            OrderId = invoice.OrderId,
            Comments = invoice.Comments,
            DeliveryInstructions = invoice.DeliveryInstructions,
            TotalDryItems = invoice.TotalDryItems,
            TotalChillerItems = invoice.TotalChillerItems,
            TotalLines = invoice.InvoiceLines?.Count ?? 0,
            TotalAmount = invoice.InvoiceLines?.Sum(il => il.ExtendedPrice) ?? 0,
            TotalTax = invoice.InvoiceLines?.Sum(il => il.TaxAmount) ?? 0,
            Lines = invoice.InvoiceLines?.Select(il => new InvoiceLineDto
            {
                InvoiceLineId = il.InvoiceLineId,
                StockItemId = il.StockItemId,
                StockItemName = il.StockItem?.StockItemName ?? "Unknown",
                Description = il.Description,
                Quantity = il.Quantity,
                UnitPrice = il.UnitPrice,
                TaxRate = il.TaxRate,
                TaxAmount = il.TaxAmount,
                ExtendedPrice = il.ExtendedPrice,
                LineProfit = il.LineProfit
            }).ToList() ?? new List<InvoiceLineDto>()
        };
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(InvoiceCreateDto dto)
    {
        var invoice = new Invoice
        {
            CustomerId = dto.CustomerId,
            BillToCustomerId = dto.CustomerId, // Same as CustomerId by default
            InvoiceDate = dto.InvoiceDate,
            OrderId = dto.OrderId,
            DeliveryMethodId = 1, // Valid delivery method ID from database
            ContactPersonId = 2, // Valid PersonID from Application.People
            AccountsPersonId = 2, // Valid PersonID from Application.People
            SalespersonPersonId = 2, // Valid PersonID from Application.People
            PackedByPersonId = 2, // Valid PersonID from Application.People
            IsCreditNote = dto.IsCreditNote,
            Comments = dto.Comments,
            DeliveryInstructions = dto.DeliveryInstructions,
            TotalDryItems = dto.TotalDryItems,
            TotalChillerItems = dto.TotalChillerItems,
            LastEditedBy = 1, // Valid PersonID from Application.People
            LastEditedWhen = DateTime.UtcNow
        };

        await _unitOfWork.Invoices.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        var createdInvoice = await _unitOfWork.Invoices.GetByIdAsync(invoice.InvoiceId, i => i.Customer!);
        return MapToDto(createdInvoice!);
    }

    public async Task<InvoiceDto?> UpdateInvoiceAsync(int invoiceId, InvoiceUpdateDto dto)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId, i => i.Customer!);
        if (invoice == null) return null;

        invoice.CustomerId = dto.CustomerId;
        invoice.InvoiceDate = dto.InvoiceDate;
        invoice.OrderId = dto.OrderId;
        invoice.IsCreditNote = dto.IsCreditNote;
        invoice.Comments = dto.Comments;
        invoice.DeliveryInstructions = dto.DeliveryInstructions;
        invoice.TotalDryItems = dto.TotalDryItems;
        invoice.TotalChillerItems = dto.TotalChillerItems;
        invoice.LastEditedBy = 1; // Valid PersonID from Application.People
        invoice.LastEditedWhen = DateTime.UtcNow;

        _unitOfWork.Invoices.Update(invoice);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(invoice);
    }

    public async Task<bool> DeleteInvoiceAsync(int invoiceId)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
        if (invoice == null) return false;

        // Note: Deletion may fail if invoice has related CustomerTransactions
        // due to FK constraint. This is expected behavior for referenced invoices.
        try
        {
            _unitOfWork.Invoices.Delete(invoice);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FK_Sales_CustomerTransactions") == true)
        {
            _logger.LogWarning("Cannot delete invoice {InvoiceId} - has related customer transactions", invoiceId);
            return false;
        }
    }
}
