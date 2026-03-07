using WideWorldImportersBI.Entities.Oltp;

namespace WideWorldImportersBI.Repositories.Interfaces;

/// <summary>
/// Repository interface for Invoice entity operations
/// Extends generic repository with invoice-specific methods
/// </summary>
public interface IInvoiceRepository : IRepository<Invoice>
{
    /// <summary>
    /// Gets invoices with customer details
    /// </summary>
    Task<IEnumerable<Invoice>> GetInvoicesWithCustomersAsync();

    /// <summary>
    /// Gets an invoice with its lines and customer
    /// </summary>
    Task<Invoice?> GetInvoiceWithDetailsAsync(int invoiceId);

    /// <summary>
    /// Gets invoices by customer
    /// </summary>
    Task<IEnumerable<Invoice>> GetByCustomerAsync(int customerId);

    /// <summary>
    /// Gets invoices within a date range
    /// </summary>
    Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets credit note invoices
    /// </summary>
    Task<IEnumerable<Invoice>> GetCreditNotesAsync();

    /// <summary>
    /// Gets recent invoices
    /// </summary>
    Task<IEnumerable<Invoice>> GetRecentInvoicesAsync(int count);

    /// <summary>
    /// Gets invoices with their lines for analytics
    /// </summary>
    Task<IEnumerable<Invoice>> GetInvoicesWithLinesAsync();

    /// <summary>
    /// Gets invoice lines with product details
    /// </summary>
    Task<IEnumerable<InvoiceLine>> GetInvoiceLinesWithProductsAsync(int invoiceId);
}
