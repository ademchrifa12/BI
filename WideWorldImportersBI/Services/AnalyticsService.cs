using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.Data.Oltp;
using WideWorldImportersBI.DTOs;

namespace WideWorldImportersBI.Services;

/// <summary>
/// Analytics service implementation
/// Provides BI analytics and dashboard KPIs using complex aggregation queries
/// These endpoints are designed to be consumed by ETL processes and BI tools
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly OltpDbContext _context;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(OltpDbContext context, ILogger<AnalyticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets sales aggregated by year and month
    /// This query performs JOIN between Invoices and InvoiceLines, then GROUP BY year/month
    /// Output: Total sales, tax, profit, and transaction count per period
    /// </summary>
    public async Task<IEnumerable<SalesByPeriodDto>> GetSalesByPeriodAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        _logger.LogInformation("Fetching sales by period from {StartDate} to {EndDate}", startDate, endDate);

        var query = from invoice in _context.Invoices
                    join line in _context.InvoiceLines on invoice.InvoiceId equals line.InvoiceId
                    where !invoice.IsCreditNote // Exclude credit notes from sales
                    select new { invoice, line };

        // Apply date filters if provided
        if (startDate.HasValue)
            query = query.Where(x => x.invoice.InvoiceDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(x => x.invoice.InvoiceDate <= endDate.Value);

        var salesByPeriod = await query
            .GroupBy(x => new { x.invoice.InvoiceDate.Year, x.invoice.InvoiceDate.Month })
            .Select(g => new SalesByPeriodDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = "", // Will be set after query
                TotalSales = g.Sum(x => x.line.ExtendedPrice),
                TotalTax = g.Sum(x => x.line.TaxAmount),
                TotalProfit = g.Sum(x => x.line.LineProfit),
                TransactionCount = g.Select(x => x.invoice.InvoiceId).Distinct().Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .AsNoTracking()
            .ToListAsync();

        // Set month names
        foreach (var item in salesByPeriod)
        {
            item.MonthName = new DateTime(item.Year, item.Month, 1).ToString("MMMM");
        }

        _logger.LogInformation("Retrieved {Count} periods of sales data", salesByPeriod.Count);
        return salesByPeriod;
    }

    /// <summary>
    /// Gets sales aggregated by product
    /// This query performs JOIN between InvoiceLines and StockItems, then GROUP BY product
    /// Output: Total quantity, sales, profit, and transaction count per product
    /// </summary>
    public async Task<IEnumerable<SalesByProductDto>> GetSalesByProductAsync(int topN = 10)
    {
        _logger.LogInformation("Fetching top {TopN} products by sales", topN);

        var salesByProduct = await (from line in _context.InvoiceLines
                                    join product in _context.Products on line.StockItemId equals product.StockItemId
                                    join invoice in _context.Invoices on line.InvoiceId equals invoice.InvoiceId
                                    where !invoice.IsCreditNote
                                    group new { line, product } by new { line.StockItemId, product.StockItemName } into g
                                    select new SalesByProductDto
                                    {
                                        StockItemId = g.Key.StockItemId,
                                        StockItemName = g.Key.StockItemName,
                                        TotalQuantity = g.Sum(x => x.line.Quantity),
                                        TotalSales = g.Sum(x => x.line.ExtendedPrice),
                                        TotalProfit = g.Sum(x => x.line.LineProfit),
                                        TransactionCount = g.Count()
                                    })
                                   .OrderByDescending(x => x.TotalSales)
                                   .Take(topN)
                                   .AsNoTracking()
                                   .ToListAsync();

        _logger.LogInformation("Retrieved {Count} products for sales analysis", salesByProduct.Count);
        return salesByProduct;
    }

    /// <summary>
    /// Gets sales aggregated by customer
    /// This query performs JOIN between Customers, Orders, Invoices, and InvoiceLines
    /// Output: Total orders, invoices, sales, and profit per customer
    /// </summary>
    public async Task<IEnumerable<SalesByCustomerDto>> GetSalesByCustomerAsync(int topN = 10)
    {
        _logger.LogInformation("Fetching top {TopN} customers by sales", topN);

        var salesByCustomer = await (from customer in _context.Customers
                                     join invoice in _context.Invoices on customer.CustomerId equals invoice.CustomerId
                                     join line in _context.InvoiceLines on invoice.InvoiceId equals line.InvoiceId
                                     where !invoice.IsCreditNote
                                     group new { customer, invoice, line } by new { customer.CustomerId, customer.CustomerName } into g
                                     select new SalesByCustomerDto
                                     {
                                         CustomerId = g.Key.CustomerId,
                                         CustomerName = g.Key.CustomerName,
                                         TotalOrders = g.Select(x => x.invoice.OrderId).Where(id => id != null).Distinct().Count(),
                                         TotalInvoices = g.Select(x => x.invoice.InvoiceId).Distinct().Count(),
                                         TotalSales = g.Sum(x => x.line.ExtendedPrice),
                                         TotalProfit = g.Sum(x => x.line.LineProfit)
                                     })
                                    .OrderByDescending(x => x.TotalSales)
                                    .Take(topN)
                                    .AsNoTracking()
                                    .ToListAsync();

        _logger.LogInformation("Retrieved {Count} customers for sales analysis", salesByCustomer.Count);
        return salesByCustomer;
    }

    /// <summary>
    /// Gets key performance indicators (KPIs) for the dashboard
    /// Aggregates multiple metrics across the entire dataset
    /// Output: Total revenue, orders, customers, products, average order value, profit margin
    /// </summary>
    public async Task<KpiDto> GetKpisAsync()
    {
        _logger.LogInformation("Calculating KPIs");

        // Total Revenue and Profit from invoice lines (excluding credit notes)
        var salesMetrics = await (from line in _context.InvoiceLines
                                  join invoice in _context.Invoices on line.InvoiceId equals invoice.InvoiceId
                                  where !invoice.IsCreditNote
                                  select line)
                                 .GroupBy(x => 1)
                                 .Select(g => new
                                 {
                                     TotalRevenue = g.Sum(x => x.ExtendedPrice),
                                     TotalProfit = g.Sum(x => x.LineProfit)
                                 })
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync();

        // Count of distinct entities
        var totalOrders = await _context.Orders.CountAsync();
        var totalCustomers = await _context.Customers.CountAsync();
        var totalProducts = await _context.Products.CountAsync();
        var totalInvoices = await _context.Invoices.Where(i => !i.IsCreditNote).CountAsync();

        // Date range
        var earliestDate = await _context.Invoices.MinAsync(i => (DateTime?)i.InvoiceDate);
        var latestDate = await _context.Invoices.MaxAsync(i => (DateTime?)i.InvoiceDate);

        // Calculate average order value
        decimal avgOrderValue = 0;
        if (totalInvoices > 0 && salesMetrics != null)
        {
            avgOrderValue = salesMetrics.TotalRevenue / totalInvoices;
        }

        // Calculate profit margin
        decimal profitMargin = 0;
        if (salesMetrics != null && salesMetrics.TotalRevenue > 0)
        {
            profitMargin = (salesMetrics.TotalProfit / salesMetrics.TotalRevenue) * 100;
        }

        var kpi = new KpiDto
        {
            TotalRevenue = salesMetrics?.TotalRevenue ?? 0,
            TotalProfit = salesMetrics?.TotalProfit ?? 0,
            TotalOrders = totalOrders,
            TotalCustomers = totalCustomers,
            TotalProducts = totalProducts,
            TotalInvoices = totalInvoices,
            AverageOrderValue = Math.Round(avgOrderValue, 2),
            ProfitMargin = Math.Round(profitMargin, 2),
            DateRange = new DateRangeInfo
            {
                EarliestDate = earliestDate,
                LatestDate = latestDate
            }
        };

        _logger.LogInformation("KPIs calculated: Revenue={Revenue}, Orders={Orders}, Customers={Customers}",
            kpi.TotalRevenue, kpi.TotalOrders, kpi.TotalCustomers);

        return kpi;
    }
}
