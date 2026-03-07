using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.Data.Oltp;
using WideWorldImportersBI.Entities.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Repositories.Implementations;

/// <summary>
/// Repository implementation for Role entity
/// Provides role-specific data access operations
/// </summary>
public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(OltpDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.RoleName.ToLower() == roleName.ToLower());
    }

    public async Task<IEnumerable<Role>> GetRolesWithUsersAsync()
    {
        return await _dbSet
            .Include(r => r.Users)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> RoleNameExistsAsync(string roleName)
    {
        return await _dbSet
            .AnyAsync(r => r.RoleName.ToLower() == roleName.ToLower());
    }
}
