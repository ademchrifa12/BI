using WideWorldImportersBI.Entities.Oltp;

namespace WideWorldImportersBI.Repositories.Interfaces;

/// <summary>
/// Repository interface for Role entity operations
/// Used for role-based access control
/// </summary>
public interface IRoleRepository : IRepository<Role>
{
    /// <summary>
    /// Gets a role by name
    /// </summary>
    Task<Role?> GetByNameAsync(string roleName);

    /// <summary>
    /// Gets all roles with their users
    /// </summary>
    Task<IEnumerable<Role>> GetRolesWithUsersAsync();

    /// <summary>
    /// Checks if role name exists
    /// </summary>
    Task<bool> RoleNameExistsAsync(string roleName);
}
