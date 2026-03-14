using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WideWorldImportersBI.Controllers;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Services;

namespace WideWorldImportersBI.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();
    private readonly Mock<ILogger<AuthController>> _loggerMock = new();

    private AuthController CreateControllerWithUserClaims(params Claim[] claims)
    {
        var controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);
        var identity = new ClaimsIdentity(claims, "TestAuth");

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        return controller;
    }

    [Fact]
    public async Task Login_EmptyToken_ReturnsBadRequest()
    {
        var controller = CreateControllerWithUserClaims();

        var result = await controller.Login(new FirebaseLoginDto { IdToken = string.Empty });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var payload = Assert.IsType<AuthResponseDto>(badRequest.Value);
        Assert.False(payload.Success);
    }

    [Fact]
    public async Task Login_InvalidToken_ReturnsUnauthorized()
    {
        _authServiceMock
            .Setup(s => s.LoginWithFirebaseAsync("bad-token"))
            .ReturnsAsync(new AuthResponseDto { Success = false, Message = "Invalid" });

        var controller = CreateControllerWithUserClaims();

        var result = await controller.Login(new FirebaseLoginDto { IdToken = "bad-token" });

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        var payload = Assert.IsType<AuthResponseDto>(unauthorized.Value);
        Assert.False(payload.Success);
    }

    [Fact]
    public async Task GetCurrentUser_InvalidClaim_ReturnsUnauthorized()
    {
        var controller = CreateControllerWithUserClaims(new Claim("userId", "not-a-number"));

        var result = await controller.GetCurrentUser();

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task GetCurrentUser_UserNotFound_ReturnsNotFound()
    {
        _authServiceMock.Setup(s => s.GetUserByIdAsync(42)).ReturnsAsync((UserDto?)null);
        var controller = CreateControllerWithUserClaims(new Claim("userId", "42"));

        var result = await controller.GetCurrentUser();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetCurrentUser_ValidUser_ReturnsOk()
    {
        _authServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(new UserDto
        {
            UserId = 1,
            Username = "admin",
            Email = "admin@wideworldimporters.com",
            FirstName = "System",
            LastName = "Administrator",
            Role = "Admin",
            IsActive = true
        });

        var controller = CreateControllerWithUserClaims(new Claim("userId", "1"));

        var result = await controller.GetCurrentUser();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }
}
