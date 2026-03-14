using Microsoft.AspNetCore.Mvc;
using Moq;
using WideWorldImportersBI.Controllers;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Services;

namespace WideWorldImportersBI.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserManagementService> _userServiceMock = new();

    private UsersController CreateController()
    {
        return new UsersController(_userServiceMock.Object);
    }

    [Fact]
    public async Task GetById_NotFound_ReturnsNotFound()
    {
        _userServiceMock.Setup(s => s.GetUserByIdAsync(5)).ReturnsAsync((UserDto?)null);
        var controller = CreateController();

        var result = await controller.GetById(5);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ServiceValidationError_ReturnsBadRequest()
    {
        _userServiceMock
            .Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>()))
            .ThrowsAsync(new InvalidOperationException("Email already exists"));

        var controller = CreateController();

        var dto = new CreateUserDto
        {
            Username = "user1",
            Email = "user1@example.com",
            Password = "Password123!",
            FirstName = "U",
            LastName = "One",
            Role = "User"
        };

        var result = await controller.Create(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenDeleted_ReturnsNoContent()
    {
        _userServiceMock.Setup(s => s.DeleteUserAsync(2)).ReturnsAsync(true);
        var controller = CreateController();

        var result = await controller.Delete(2);

        Assert.IsType<NoContentResult>(result);
    }
}
