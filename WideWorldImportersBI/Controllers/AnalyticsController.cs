using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Services;

namespace WideWorldImportersBI.Controllers;

/// <summary>
/// Analytics controller for Business Intelligence dashboard
/// Provides aggregated sales data for charts and KPIs
/// These endpoints are designed to be consumed by:
/// - Angular frontend dashboard
/// - ETL processes (SSIS) for populating the Data Warehouse
/// - External BI tools (Power BI, Tableau, etc.)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Gets sales aggregated by time period (year/month)
    /// Used for: Line charts showing sales trends over time
    /// ETL Usage: Can be used to populate FactSales with pre-aggregated data
    /// </summary>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <returns>Sales grouped by year and month</returns>
    [HttpGet("sales/period")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SalesByPeriodDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSalesByPeriod(
        [FromQuery] DateTime? startDate = null, 
        [FromQuery] DateTime? endDate = null)
    {
        _logger.LogInformation("Analytics request: Sales by period from {StartDate} to {EndDate} by user {User}", 
            startDate, endDate, User.Identity?.Name);

        var data = await _analyticsService.GetSalesByPeriodAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<SalesByPeriodDto>>.SuccessResponse(data, 
            "Sales by period retrieved successfully"));
    }

    /// <summary>
    /// Gets sales aggregated by product
    /// Used for: Bar charts showing top selling products
    /// ETL Usage: Can be used to populate product-related facts in the Data Warehouse
    /// </summary>
    /// <param name="topN">Number of top products to return (default: 10)</param>
    /// <returns>Sales grouped by product, ordered by total sales descending</returns>
    [HttpGet("sales/product")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SalesByProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSalesByProduct([FromQuery] int topN = 10)
    {
        if (topN <= 0 || topN > 100)
        {
            topN = 10;
        }

        _logger.LogInformation("Analytics request: Top {TopN} products by sales by user {User}", 
            topN, User.Identity?.Name);

        var data = await _analyticsService.GetSalesByProductAsync(topN);
        return Ok(ApiResponse<IEnumerable<SalesByProductDto>>.SuccessResponse(data, 
            "Sales by product retrieved successfully"));
    }

    /// <summary>
    /// Gets sales aggregated by customer
    /// Used for: Pie charts showing customer contribution to sales
    /// ETL Usage: Can be used to populate customer-related facts in the Data Warehouse
    /// </summary>
    /// <param name="topN">Number of top customers to return (default: 10)</param>
    /// <returns>Sales grouped by customer, ordered by total sales descending</returns>
    [HttpGet("sales/customer")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SalesByCustomerDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSalesByCustomer([FromQuery] int topN = 10)
    {
        if (topN <= 0 || topN > 100)
        {
            topN = 10;
        }

        _logger.LogInformation("Analytics request: Top {TopN} customers by sales by user {User}", 
            topN, User.Identity?.Name);

        var data = await _analyticsService.GetSalesByCustomerAsync(topN);
        return Ok(ApiResponse<IEnumerable<SalesByCustomerDto>>.SuccessResponse(data, 
            "Sales by customer retrieved successfully"));
    }

    /// <summary>
    /// Gets key performance indicators (KPIs) for the dashboard
    /// Used for: KPI cards showing total revenue, orders, customers, etc.
    /// ETL Usage: These metrics can be used for executive dashboards and reporting
    /// </summary>
    /// <returns>Aggregated KPIs including revenue, orders, customers, profit margin</returns>
    [HttpGet("kpis")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<KpiDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetKpis()
    {
        _logger.LogInformation("Analytics request: KPIs by user {User}", User.Identity?.Name);

        var data = await _analyticsService.GetKpisAsync();
        return Ok(ApiResponse<KpiDto>.SuccessResponse(data, 
            "KPIs retrieved successfully"));
    }

    /// <summary>
    /// Gets comprehensive dashboard data in a single call
    /// Optimized for the Angular dashboard to minimize HTTP requests
    /// </summary>
    /// <returns>All dashboard data including sales by period, product, customer, and KPIs</returns>
    [HttpGet("dashboard")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<DashboardDataDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardData()
    {
        _logger.LogInformation("Analytics request: Full dashboard data by user {User}", User.Identity?.Name);

        var salesByPeriod = await _analyticsService.GetSalesByPeriodAsync();
        var salesByProduct = await _analyticsService.GetSalesByProductAsync(10);
        var salesByCustomer = await _analyticsService.GetSalesByCustomerAsync(10);
        var kpis = await _analyticsService.GetKpisAsync();

        var dashboardData = new DashboardDataDto
        {
            SalesByPeriod = salesByPeriod.ToList(),
            SalesByProduct = salesByProduct.ToList(),
            SalesByCustomer = salesByCustomer.ToList(),
            Kpis = kpis
        };

        return Ok(ApiResponse<DashboardDataDto>.SuccessResponse(dashboardData, 
            "Dashboard data retrieved successfully"));
    }
}

/// <summary>
/// DTO for combined dashboard data
/// </summary>
public class DashboardDataDto
{
    public List<SalesByPeriodDto> SalesByPeriod { get; set; } = new();
    public List<SalesByProductDto> SalesByProduct { get; set; } = new();
    public List<SalesByCustomerDto> SalesByCustomer { get; set; } = new();
    public KpiDto Kpis { get; set; } = new();
}
