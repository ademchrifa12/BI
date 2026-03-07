using WideWorldImportersBI.DTOs;

namespace WideWorldImportersBI.Services;

public interface IDwAnalyticsService
{
    Task<DwKpiDto> GetKpisAsync();
    Task<IEnumerable<DwSalesByProductDto>> GetTopProductsAsync(int topN = 10);
    Task<IEnumerable<DwSalesByClientDto>> GetTopClientsAsync(int topN = 10);
    Task<IEnumerable<DwSalesByEmployeDto>> GetSalesByEmployeeAsync();
}
