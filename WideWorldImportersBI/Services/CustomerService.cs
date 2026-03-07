using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Entities.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Services;

/// <summary>
/// Customer service implementation
/// Handles customer business logic and data transformation
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _unitOfWork.Customers.GetCustomersWithOrdersAsync();
        return customers.Select(MapToDto);
    }

    public async Task<PaginatedResponse<CustomerDto>> GetCustomersPagedAsync(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        var query = _unitOfWork.Customers.Query().AsNoTracking();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.CustomerName.Contains(searchTerm));
        }

        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        var paginatedData = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<CustomerDto>
        {
            Data = paginatedData.Select(MapToDto).ToList(),
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize,
            Success = true
        };
    }

    public async Task<CustomerDetailDto?> GetCustomerByIdAsync(int customerId)
    {
        var customer = await _unitOfWork.Customers.GetCustomerWithDetailsAsync(customerId);
        if (customer == null) return null;

        return MapToDetailDto(customer);
    }

    public async Task<CustomerDto> CreateCustomerAsync(CustomerCreateDto dto)
    {
        // Get a reference customer to get default values
        var referenceCustomer = await _unitOfWork.Customers.FirstOrDefaultAsync(c => true);
        
        var customer = new Customer
        {
            CustomerName = dto.CustomerName,
            PhoneNumber = dto.PhoneNumber ?? "555-0000",
            FaxNumber = dto.FaxNumber ?? "N/A",
            WebsiteUrl = dto.WebsiteUrl ?? "http://example.com",
            DeliveryAddressLine1 = dto.DeliveryAddressLine1 ?? "No Address Provided",
            DeliveryAddressLine2 = dto.DeliveryAddressLine2,
            DeliveryPostalCode = dto.DeliveryPostalCode ?? "00000",
            PostalAddressLine1 = dto.PostalAddressLine1 ?? "No Postal Address",
            PostalAddressLine2 = dto.PostalAddressLine2,
            PostalPostalCode = dto.PostalPostalCode ?? "00000",
            CreditLimit = dto.CreditLimit,
            AccountOpenedDate = DateTime.UtcNow,
            IsOnCreditHold = false,
            IsStatementSent = false,
            StandardDiscountPercentage = 0,
            PaymentDays = 30,
            // Default foreign keys (would need to be configured properly in production)
            BillToCustomerId = referenceCustomer?.BillToCustomerId ?? 1,
            CustomerCategoryId = referenceCustomer?.CustomerCategoryId ?? 1,
            PrimaryContactPersonId = referenceCustomer?.PrimaryContactPersonId ?? 1,
            DeliveryMethodId = referenceCustomer?.DeliveryMethodId ?? 1,
            DeliveryCityId = referenceCustomer?.DeliveryCityId ?? 1,
            PostalCityId = referenceCustomer?.PostalCityId ?? 1,
            LastEditedBy = 1
            // ValidFrom and ValidTo are GENERATED ALWAYS columns - don't set them
        };

        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Customer created: {CustomerId} - {CustomerName}", customer.CustomerId, customer.CustomerName);

        return MapToDto(customer);
    }

    public async Task<CustomerDto?> UpdateCustomerAsync(int customerId, CustomerUpdateDto dto)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
        if (customer == null) return null;

        customer.CustomerName = dto.CustomerName;
        customer.PhoneNumber = dto.PhoneNumber;
        customer.WebsiteUrl = dto.WebsiteUrl;
        customer.DeliveryAddressLine1 = dto.DeliveryAddressLine1;
        customer.DeliveryAddressLine2 = dto.DeliveryAddressLine2;
        customer.DeliveryPostalCode = dto.DeliveryPostalCode;
        customer.CreditLimit = dto.CreditLimit;
        customer.IsOnCreditHold = dto.IsOnCreditHold;
        customer.LastEditedBy = 1; // Valid PersonID from Application.People

        _unitOfWork.Customers.Update(customer);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Customer updated: {CustomerId} - {CustomerName}", customer.CustomerId, customer.CustomerName);

        return MapToDto(customer);
    }

    public async Task<bool> DeleteCustomerAsync(int customerId)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
        if (customer == null) return false;

        // Check if customer has orders or invoices
        var hasOrders = await _unitOfWork.Orders.AnyAsync(o => o.CustomerId == customerId);
        var hasInvoices = await _unitOfWork.Invoices.AnyAsync(i => i.CustomerId == customerId);

        if (hasOrders || hasInvoices)
        {
            _logger.LogWarning("Cannot delete customer {CustomerId} - has related orders or invoices", customerId);
            return false;
        }

        _unitOfWork.Customers.Delete(customer);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Customer deleted: {CustomerId}", customerId);

        return true;
    }

    public async Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm)
    {
        var customers = await _unitOfWork.Customers.SearchByNameAsync(searchTerm);
        return customers.Select(MapToDto);
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            CustomerId = customer.CustomerId,
            CustomerName = customer.CustomerName,
            PhoneNumber = customer.PhoneNumber,
            WebsiteUrl = customer.WebsiteUrl,
            DeliveryAddressLine1 = customer.DeliveryAddressLine1,
            DeliveryAddressLine2 = customer.DeliveryAddressLine2,
            DeliveryPostalCode = customer.DeliveryPostalCode,
            CreditLimit = customer.CreditLimit,
            IsOnCreditHold = customer.IsOnCreditHold,
            AccountOpenedDate = customer.AccountOpenedDate,
            OrderCount = customer.Orders?.Count ?? 0
        };
    }

    private static CustomerDetailDto MapToDetailDto(Customer customer)
    {
        return new CustomerDetailDto
        {
            CustomerId = customer.CustomerId,
            CustomerName = customer.CustomerName,
            PhoneNumber = customer.PhoneNumber,
            FaxNumber = customer.FaxNumber,
            WebsiteUrl = customer.WebsiteUrl,
            DeliveryAddressLine1 = customer.DeliveryAddressLine1,
            DeliveryAddressLine2 = customer.DeliveryAddressLine2,
            DeliveryPostalCode = customer.DeliveryPostalCode,
            CreditLimit = customer.CreditLimit,
            IsOnCreditHold = customer.IsOnCreditHold,
            AccountOpenedDate = customer.AccountOpenedDate,
            StandardDiscountPercentage = customer.StandardDiscountPercentage,
            PaymentDays = customer.PaymentDays,
            IsStatementSent = customer.IsStatementSent,
            OrderCount = customer.Orders?.Count ?? 0,
            RecentOrders = customer.Orders?
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new OrderSummaryDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    ExpectedDeliveryDate = o.ExpectedDeliveryDate,
                    IsPickingCompleted = o.PickingCompletedWhen != null
                }).ToList() ?? new List<OrderSummaryDto>(),
            RecentInvoices = customer.Invoices?
                .OrderByDescending(i => i.InvoiceDate)
                .Take(5)
                .Select(i => new InvoiceSummaryDto
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceDate = i.InvoiceDate,
                    TotalAmount = i.InvoiceLines?.Sum(il => il.ExtendedPrice) ?? 0,
                    IsCreditNote = i.IsCreditNote
                }).ToList() ?? new List<InvoiceSummaryDto>()
        };
    }
}
