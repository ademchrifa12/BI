using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WideWorldImportersBI.Controllers;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Services;

namespace WideWorldImportersBI.Tests.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<ICustomerService> _customerServiceMock = new();
    private readonly Mock<ILogger<CustomersController>> _loggerMock = new();

    private CustomersController CreateController()
    {
        return new CustomersController(_customerServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Search_EmptyTerm_ReturnsBadRequest()
    {
        var controller = CreateController();

        var result = await controller.Search(string.Empty);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetById_NotFound_ReturnsNotFound()
    {
        _customerServiceMock.Setup(s => s.GetCustomerByIdAsync(99)).ReturnsAsync((CustomerDetailDto?)null);
        var controller = CreateController();

        var result = await controller.GetById(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenServiceReturnsFalse_ReturnsBadRequest()
    {
        _customerServiceMock.Setup(s => s.DeleteCustomerAsync(7)).ReturnsAsync(false);
        var controller = CreateController();

        var result = await controller.Delete(7);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithPaginatedPayload()
    {
        _customerServiceMock
            .Setup(s => s.GetCustomersPagedAsync(1, 10, null))
            .ReturnsAsync(new PaginatedResponse<CustomerDto>
            {
                Data = new List<CustomerDto>
                {
                    new CustomerDto { CustomerId = 1, CustomerName = "Acme" }
                },
                TotalRecords = 1,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1,
                Success = true
            });

        var controller = CreateController();

        var result = await controller.GetAll(1, 10, null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<PaginatedResponse<CustomerDto>>(ok.Value);
        Assert.True(payload.Success);
        Assert.Single(payload.Data);
    }
}
