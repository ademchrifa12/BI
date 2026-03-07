using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.Data.DataWarehouse;
using WideWorldImportersBI.DTOs;

namespace WideWorldImportersBI.Services;

public class DwAnalyticsService : IDwAnalyticsService
{
    private readonly DwDbContext _context;
    private readonly ILogger<DwAnalyticsService> _logger;

    public DwAnalyticsService(DwDbContext context, ILogger<DwAnalyticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DwKpiDto> GetKpisAsync()
    {
        _logger.LogInformation("Fetching DW KPIs");

        var totals = await _context.FactVentes
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalRevenue      = g.Sum(f => f.MontantHT ?? 0),
                TotalProfit       = g.Sum(f => f.Profit ?? 0),
                TotalTax          = g.Sum(f => f.Taxe ?? 0),
                TotalTransactions = g.Count(),
            })
            .FirstOrDefaultAsync();

        var uniqueClients  = await _context.FactVentes.Select(f => f.ClientSK).Distinct().CountAsync();
        var uniqueProducts = await _context.FactVentes.Select(f => f.ProduitSK).Distinct().CountAsync();

        var revenue = totals?.TotalRevenue ?? 0;
        var profit  = totals?.TotalProfit  ?? 0;

        return new DwKpiDto
        {
            TotalRevenue            = revenue,
            TotalProfit             = profit,
            TotalTax                = totals?.TotalTax ?? 0,
            TotalTransactions       = totals?.TotalTransactions ?? 0,
            UniqueClients           = uniqueClients,
            UniqueProducts          = uniqueProducts,
            AverageTransactionValue = totals?.TotalTransactions > 0
                ? revenue / totals!.TotalTransactions : 0,
            ProfitMarginPercent     = revenue > 0 ? Math.Round(profit / revenue * 100, 2) : 0
        };
    }

    public async Task<IEnumerable<DwSalesByProductDto>> GetTopProductsAsync(int topN = 10)
    {
        _logger.LogInformation("Fetching DW top {TopN} products", topN);

        return await _context.FactVentes
            .AsNoTracking()
            .Join(_context.DimProduits,
                f => f.ProduitSK,
                p => p.ProduitSK,
                (f, p) => new { f, p })
            .GroupBy(x => new { x.p.ProduitSK, x.p.StockItemName, x.p.Brand, x.p.StockGroupName })
            .Select(g => new DwSalesByProductDto
            {
                ProduitSK     = g.Key.ProduitSK,
                ProductName   = g.Key.StockItemName ?? string.Empty,
                Brand         = g.Key.Brand,
                StockGroup    = g.Key.StockGroupName,
                TotalQuantity = g.Sum(x => x.f.Quantite ?? 0),
                TotalRevenue  = g.Sum(x => x.f.MontantHT ?? 0),
                TotalProfit   = g.Sum(x => x.f.Profit ?? 0),
            })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(topN)
            .ToListAsync();
    }

    public async Task<IEnumerable<DwSalesByClientDto>> GetTopClientsAsync(int topN = 10)
    {
        _logger.LogInformation("Fetching DW top {TopN} clients", topN);

        return await _context.FactVentes
            .AsNoTracking()
            .Join(_context.DimClients,
                f => f.ClientSK,
                c => c.ClientSK,
                (f, c) => new { f, c })
            .GroupBy(x => new { x.c.ClientSK, x.c.CustomerName, x.c.CategoryName, x.c.CityName })
            .Select(g => new DwSalesByClientDto
            {
                ClientSK          = g.Key.ClientSK,
                CustomerName      = g.Key.CustomerName ?? string.Empty,
                Category          = g.Key.CategoryName,
                City              = g.Key.CityName,
                TotalTransactions = g.Count(),
                TotalQuantity     = g.Sum(x => x.f.Quantite ?? 0),
                TotalRevenue      = g.Sum(x => x.f.MontantHT ?? 0),
                TotalProfit       = g.Sum(x => x.f.Profit ?? 0),
            })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(topN)
            .ToListAsync();
    }

    public async Task<IEnumerable<DwSalesByEmployeDto>> GetSalesByEmployeeAsync()
    {
        _logger.LogInformation("Fetching DW sales by employee");

        return await _context.FactVentes
            .AsNoTracking()
            .Join(_context.DimEmployes,
                f => f.EmployeSK,
                e => e.EmployeSK,
                (f, e) => new { f, e })
            .GroupBy(x => new { x.e.EmployeSK, x.e.FullName })
            .Select(g => new DwSalesByEmployeDto
            {
                EmployeSK         = g.Key.EmployeSK,
                FullName          = g.Key.FullName ?? string.Empty,
                TotalTransactions = g.Count(),
                TotalQuantity     = g.Sum(x => x.f.Quantite ?? 0),
                TotalRevenue      = g.Sum(x => x.f.MontantHT ?? 0),
                TotalProfit       = g.Sum(x => x.f.Profit ?? 0),
            })
            .OrderByDescending(x => x.TotalRevenue)
            .ToListAsync();
    }
}
