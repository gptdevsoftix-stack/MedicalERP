using System.Security.Claims;
using MedicalERP.Application.Permissions;
using MedicalERP.Domain.Identity;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MedicalERP.Infrastructure.Identity;

public sealed class ApplicationClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ApplicationDbContext db,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>(userManager, roleManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("is_platform_admin", user.IsPlatformAdmin ? "true" : "false"));

        if (user.CompanyId.HasValue)
        {
            identity.AddClaim(new Claim("company_id", user.CompanyId.Value.ToString()));
        }

        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var roleNames = await UserManager.GetRolesAsync(user);
        foreach (var roleName in roleNames)
        {
            var role = await RoleManager.FindByNameAsync(roleName);
            if (role is null) continue;

            var roleClaims = await RoleManager.GetClaimsAsync(role);
            foreach (var claim in roleClaims.Where(x => x.Type == PermissionClaimTypes.Permission))
            {
                permissions.Add(claim.Value);
            }
        }

        if (user.IsPlatformAdmin)
        {
            foreach (var permission in Permissions.All)
            {
                permissions.Add(permission);
            }
        }

        foreach (var permission in permissions)
        {
            identity.AddClaim(new Claim(PermissionClaimTypes.Permission, permission));
        }

        var storeAccess = await db.UserStoreAccesses
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(x => x.UserId == user.Id && x.IsActive)
            .ToListAsync();

        var defaultStoreId = storeAccess.FirstOrDefault(x => x.IsDefaultStore)?.StoreId
            ?? storeAccess.FirstOrDefault()?.StoreId;

        if (defaultStoreId.HasValue)
        {
            identity.AddClaim(new Claim("default_store_id", defaultStoreId.Value.ToString()));
        }

        foreach (var storeId in storeAccess.Select(x => x.StoreId).Distinct())
        {
            identity.AddClaim(new Claim("store_id", storeId.ToString()));
        }

        return identity;
    }
}
