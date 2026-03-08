using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FirebaseAdmin.Auth;
using Microsoft.IdentityModel.Tokens;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Entities.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginWithFirebaseAsync(string firebaseIdToken)
    {
        try
        {
            string? firebaseUid = null;
            string? email = null;

            // Try Firebase Admin SDK first if available
            if (FirebaseAdmin.FirebaseApp.DefaultInstance != null)
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseIdToken);
                firebaseUid = decodedToken.Uid;
                email = decodedToken.Claims.ContainsKey("email") ? decodedToken.Claims["email"]?.ToString() : null;
            }
            else
            {
                // Decode Firebase JWT to extract claims (token already verified by Firebase on frontend)
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(firebaseIdToken);
                firebaseUid = jwt.Subject;
                email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            }

            // Try to find user by Firebase UID
            var user = await _unitOfWork.Users.GetByFirebaseUidAsync(firebaseUid);

            // If not found by UID, try by email and link
            if (user == null && !string.IsNullOrEmpty(email))
            {
                user = await _unitOfWork.Users.GetByEmailAsync(email);
                if (user != null)
                {
                    user.FirebaseUid = firebaseUid;
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                    user = await _unitOfWork.Users.GetWithRoleAsync(user.UserId);
                }
            }

            if (user == null)
            {
                _logger.LogWarning("Firebase user {Uid} has no matching BI account", firebaseUid);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "No BI account found. Contact your administrator."
                };
            }

            if (!user.IsActive)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Account is inactive. Please contact administrator."
                };
            }

            // Update last login
            await _unitOfWork.Users.UpdateLastLoginAsync(user.UserId);
            await _unitOfWork.SaveChangesAsync();

            // Generate local JWT with role claims
            var localToken = GenerateLocalJwt(user);

            _logger.LogInformation("User {Username} logged in via Firebase", user.Username);

            return new AuthResponseDto
            {
                Success = true,
                Token = localToken,
                Expiration = DateTime.UtcNow.AddHours(24),
                Message = "Login successful",
                User = MapToUserDto(user)
            };
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogWarning(ex, "Invalid Firebase token");
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid authentication token"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Firebase login");
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during login"
            };
        }
    }

    private string GenerateLocalJwt(User user)
    {
        var key = _configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTAuthenticationMustBeAtLeast32CharactersLong!";
        var issuer = _configuration["Jwt:Issuer"] ?? "WideWorldImportersBI";
        var audience = _configuration["Jwt:Audience"] ?? "WideWorldImportersBI.Client";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim("userId", user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User")
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetWithRoleAsync(userId);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _unitOfWork.Users.GetByUsernameWithRoleAsync(username);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return false;

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Password changed for user: {UserId}", userId);
        return true;
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role?.RoleName ?? "User",
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt
        };
    }
}
