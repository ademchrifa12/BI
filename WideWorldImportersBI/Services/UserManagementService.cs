using System.Text;
using System.Text.Json;
using FirebaseAdmin.Auth;
using WideWorldImportersBI.DTOs;
using WideWorldImportersBI.Entities.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Services;

public class UserManagementService : IUserManagementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserManagementService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public UserManagementService(IUnitOfWork unitOfWork, ILogger<UserManagementService> logger, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _configuration = configuration;
        _httpClient = new HttpClient();
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllWithRolesAsync();
        return users.Select(MapToUserDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetWithRoleAsync(userId);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        // Check uniqueness
        if (await _unitOfWork.Users.UsernameExistsAsync(dto.Username))
            throw new InvalidOperationException("Username already exists");
        if (await _unitOfWork.Users.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Email already exists");

        var role = await _unitOfWork.Roles.GetByNameAsync(dto.Role);
        if (role == null)
            throw new InvalidOperationException($"Role '{dto.Role}' not found");

        // Create Firebase user
        string? firebaseUid = null;

        // Try Admin SDK first
        if (FirebaseAdmin.FirebaseApp.DefaultInstance != null)
        {
            try
            {
                var firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
                {
                    Email = dto.Email,
                    Password = dto.Password,
                    DisplayName = $"{dto.FirstName} {dto.LastName}",
                    EmailVerified = true
                });
                firebaseUid = firebaseUser.Uid;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Admin SDK create failed for {Email}, trying REST API", dto.Email);
            }
        }

        // Fallback: Firebase Auth REST API (works with just API key)
        if (string.IsNullOrEmpty(firebaseUid))
        {
            firebaseUid = await CreateFirebaseUserViaRestApi(dto.Email, dto.Password);
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            RoleId = role.RoleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            FirebaseUid = firebaseUid
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        user.Role = role;
        _logger.LogInformation("User {Username} created by admin (FirebaseUid: {Uid})", dto.Username, firebaseUid ?? "none");
        return MapToUserDto(user);
    }

    private async Task<string?> CreateFirebaseUserViaRestApi(string email, string password)
    {
        var apiKey = _configuration["Firebase:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("Firebase:ApiKey not configured, cannot create Firebase user via REST API");
            return null;
        }

        try
        {
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={Uri.EscapeDataString(apiKey)}";
            var payload = new { email, password, returnSecureToken = false };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var doc = JsonDocument.Parse(body);
                var uid = doc.RootElement.GetProperty("localId").GetString();
                _logger.LogInformation("Firebase user created via REST API: {Uid}", uid);
                return uid;
            }

            _logger.LogWarning("Firebase REST API signUp failed: {Body}", body);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create Firebase user via REST API");
            return null;
        }
    }

    public async Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto dto)
    {
        var user = await _unitOfWork.Users.GetWithRoleAsync(userId);
        if (user == null) return null;

        // Check uniqueness if changed
        if (user.Username.ToLower() != dto.Username.ToLower() &&
            await _unitOfWork.Users.UsernameExistsAsync(dto.Username))
            throw new InvalidOperationException("Username already exists");

        if (user.Email.ToLower() != dto.Email.ToLower() &&
            await _unitOfWork.Users.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Email already exists");

        var role = await _unitOfWork.Roles.GetByNameAsync(dto.Role);
        if (role == null)
            throw new InvalidOperationException($"Role '{dto.Role}' not found");

        // Update Firebase user if linked and Firebase is initialized
        if (!string.IsNullOrEmpty(user.FirebaseUid) && FirebaseAdmin.FirebaseApp.DefaultInstance != null)
        {
            try
            {
                var args = new UserRecordArgs
                {
                    Uid = user.FirebaseUid,
                    Email = dto.Email,
                    DisplayName = $"{dto.FirstName} {dto.LastName}",
                    Disabled = !dto.IsActive
                };
                if (!string.IsNullOrEmpty(dto.Password))
                    args.Password = dto.Password;

                await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not update Firebase user {Uid}", user.FirebaseUid);
            }
        }

        user.Username = dto.Username;
        user.Email = dto.Email;
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.RoleId = role.RoleId;
        user.IsActive = dto.IsActive;

        if (!string.IsNullOrEmpty(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        user.Role = role;
        _logger.LogInformation("User {UserId} updated by admin", userId);
        return MapToUserDto(user);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return false;

        // Delete from Firebase if linked and Firebase is initialized
        if (!string.IsNullOrEmpty(user.FirebaseUid) && FirebaseAdmin.FirebaseApp.DefaultInstance != null)
        {
            try
            {
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(user.FirebaseUid);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not delete Firebase user {Uid}", user.FirebaseUid);
            }
        }

        _unitOfWork.Users.Delete(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {UserId} deleted by admin", userId);
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
