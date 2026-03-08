using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Services;

namespace WideWorldImportersBI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates using a Firebase ID token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] FirebaseLoginDto request)
    {
        if (string.IsNullOrEmpty(request.IdToken))
            return BadRequest(new AuthResponseDto { Success = false, Message = "ID token is required" });

        var result = await _authService.LoginWithFirebaseAsync(request.IdToken);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    /// <summary>
    /// Gets the current authenticated user's information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse<UserDto>.ErrorResponse("Invalid token"));
        }

        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));
        }

        return Ok(ApiResponse<UserDto>.SuccessResponse(user));
    }

    /// <summary>
    /// Changes the password for the authenticated user
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse<bool>.ErrorResponse("Invalid token"));
        }

        var result = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

        if (!result)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse("Current password is incorrect"));
        }

        return Ok(ApiResponse<bool>.SuccessResponse(true, "Password changed successfully"));
    }
}

public class FirebaseLoginDto
{
    public string IdToken { get; set; } = string.Empty;
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
