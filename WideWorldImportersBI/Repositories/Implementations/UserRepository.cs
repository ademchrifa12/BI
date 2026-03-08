using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.Data.Oltp;
using WideWorldImportersBI.Entities.Oltp;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI.Repositories.Implementations;

/// <summary>
/// Repository implementation for User entity
/// Provides user-specific data access operations for authentication
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(OltpDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<User?> GetWithRoleAsync(int userId)
    {
        return await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetByUsernameWithRoleAsync(string username)
    {
        return await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbSet
            .Where(u => u.IsActive)
            .Include(u => u.Role)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(int roleId)
    {
        return await _dbSet
            .Where(u => u.RoleId == roleId)
            .Include(u => u.Role)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet
            .AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await _dbSet.FindAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
        }
    }

    public async Task<User?> GetByFirebaseUidAsync(string firebaseUid)
    {
        return await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
    }

    public async Task<IEnumerable<User>> GetAllWithRolesAsync()
    {
        return await _dbSet
            .Include(u => u.Role)
            .OrderBy(u => u.Username)
            .AsNoTracking()
            .ToListAsync();
    }
}
