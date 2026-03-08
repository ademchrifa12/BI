using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using WideWorldImportersBI.Repositories.Interfaces;

namespace WideWorldImportersBI;

public class FirebaseClaimsTransformation : IClaimsTransformation
{
    private readonly IServiceProvider _serviceProvider;

    public FirebaseClaimsTransformation(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Already transformed
        if (principal.HasClaim(c => c.Type == ClaimTypes.Role))
            return principal;

        var firebaseUid = principal.FindFirst("user_id")?.Value
                       ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
            return principal;

        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Look up user by Firebase UID
        var user = await unitOfWork.Users.GetByFirebaseUidAsync(firebaseUid);

        // Fallback: look up by email
        if (user == null)
        {
            var email = principal.FindFirst("email")?.Value
                     ?? principal.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                user = await unitOfWork.Users.GetByEmailAsync(email);
                if (user != null)
                {
                    // Link Firebase UID
                    user.FirebaseUid = firebaseUid;
                    await unitOfWork.SaveChangesAsync();
                    // Reload with role
                    user = await unitOfWork.Users.GetWithRoleAsync(user.UserId);
                }
            }
        }

        if (user?.Role == null)
            return principal;

        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null)
            return principal;

        identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.RoleName));
        identity.AddClaim(new Claim("userId", user.UserId.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));

        return principal;
    }
}
