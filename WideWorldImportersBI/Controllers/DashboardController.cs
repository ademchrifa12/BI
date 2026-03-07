using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Services;

namespace WideWorldImportersBI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDwAnalyticsService _dwService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDwAnalyticsService dwService, ILogger<DashboardController> logger)
    {
        _dwService = dwService;
        _logger = logger;
    }

    /// GET /api/dashboard/kpis
    [HttpGet("kpis")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetKpis()
    {
        _logger.LogInformation("Dashboard KPIs requested by {User}", User.Identity?.Name);
        var data = await _dwService.GetKpisAsync();
        return Ok(ApiResponse<DwKpiDto>.SuccessResponse(data, "KPIs from Data Warehouse"));
    }

    /// GET /api/dashboard/sales/by-product?topN=10
    [HttpGet("sales/by-product")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetTopProducts([FromQuery] int topN = 10)
    {
        var data = await _dwService.GetTopProductsAsync(topN);
        return Ok(ApiResponse<IEnumerable<DwSalesByProductDto>>.SuccessResponse(data, "Top products"));
    }

    /// GET /api/dashboard/sales/by-client?topN=10
    [HttpGet("sales/by-client")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetTopClients([FromQuery] int topN = 10)
    {
        var data = await _dwService.GetTopClientsAsync(topN);
        return Ok(ApiResponse<IEnumerable<DwSalesByClientDto>>.SuccessResponse(data, "Top clients"));
    }

    /// GET /api/dashboard/sales/by-employee
    [HttpGet("sales/by-employee")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetSalesByEmployee()
    {
        var data = await _dwService.GetSalesByEmployeeAsync();
        return Ok(ApiResponse<IEnumerable<DwSalesByEmployeDto>>.SuccessResponse(data, "Sales by employee"));
    }
}

