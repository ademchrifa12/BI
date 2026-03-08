using WideWorldImportersBI.Entities.Oltp;

namespace WideWorldImportersBI.Repositories.Interfaces;

/// <summary>
/// Repository interface for User entity operations
/// Used for authentication and user management
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Gets a user by username
    /// </summary>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// Gets a user by email
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Gets a user with their role
    /// </summary>
    Task<User?> GetWithRoleAsync(int userId);

    /// <summary>
    /// Gets a user by username with their role
    /// </summary>
    Task<User?> GetByUsernameWithRoleAsync(string username);

    /// <summary>
    /// Gets active users
    /// </summary>
    Task<IEnumerable<User>> GetActiveUsersAsync();

    /// <summary>
    /// Gets users by role
    /// </summary>
    Task<IEnumerable<User>> GetByRoleAsync(int roleId);

    /// <summary>
    /// Checks if username exists
    /// </summary>
    Task<bool> UsernameExistsAsync(string username);

    /// <summary>
    /// Checks if email exists
    /// </summary>
    Task<bool> EmailExistsAsync(string email);

    /// <summary>
    /// Updates the last login timestamp
    /// </summary>
    Task UpdateLastLoginAsync(int userId);

    /// <summary>
    /// Gets a user by Firebase UID
    /// </summary>
    Task<User?> GetByFirebaseUidAsync(string firebaseUid);

    /// <summary>
    /// Gets all users with their roles
    /// </summary>
    Task<IEnumerable<User>> GetAllWithRolesAsync();
}
