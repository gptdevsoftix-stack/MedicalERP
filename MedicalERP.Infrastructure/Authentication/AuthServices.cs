using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Identity.Dtos;
using MedicalERP.Application.Permissions;
using MedicalERP.Domain.Identity;
using MedicalERP.Infrastructure.Identity;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MedicalERP.Infrastructure.Authentication;

public sealed class JwtTokenService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext db, IOptions<JwtOptions> options) : IJwtTokenService
{
    public async Task<TokenResponse> CreateTokenAsync(Guid userId, string? ipAddress, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new UnauthorizedAccessException("Invalid user.");
        if (!user.IsActive) throw new UnauthorizedAccessException("User is inactive.");
        var roles = await userManager.GetRolesAsync(user);
        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null) continue;
            foreach (var claim in await roleManager.GetClaimsAsync(role)) if (claim.Type == PermissionClaimTypes.Permission) permissions.Add(claim.Value);
        }
        if (user.IsPlatformAdmin) foreach (var permission in Permissions.All) permissions.Add(permission);

        var storeAccess = await db.UserStoreAccesses.AsNoTracking().IgnoreQueryFilters().Where(x => x.UserId == user.Id && x.IsActive).ToListAsync(cancellationToken);
        var defaultStoreId = storeAccess.FirstOrDefault(x => x.IsDefaultStore)?.StoreId ?? storeAccess.FirstOrDefault()?.StoreId;
        var jwtOptions = options.Value;
        var expiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenMinutes);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new("is_platform_admin", user.IsPlatformAdmin ? "true" : "false")
        };
        if (user.CompanyId.HasValue) claims.Add(new("company_id", user.CompanyId.Value.ToString()));
        if (defaultStoreId.HasValue) claims.Add(new("default_store_id", defaultStoreId.Value.ToString()));
        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));
        claims.AddRange(permissions.Select(x => new Claim(PermissionClaimTypes.Permission, x)));
        claims.AddRange(storeAccess.Select(x => new Claim("store_id", x.StoreId.ToString())));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));
        var token = new JwtSecurityToken(jwtOptions.Issuer, jwtOptions.Audience, claims, expires: expiresAt, signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        var rawRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        db.RefreshTokens.Add(new RefreshToken { UserId = user.Id, TokenHash = Hash(rawRefreshToken), ExpiresAt = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenDays), CreatedByIp = ipAddress });
        user.LastLoginAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), rawRefreshToken, expiresAt, user.Id, user.CompanyId, defaultStoreId, permissions.ToArray());
    }
    internal static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

public sealed class RefreshTokenService(ApplicationDbContext db, IJwtTokenService jwtTokenService) : IRefreshTokenService
{
    public async Task<TokenResponse> RefreshAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken)
    {
        var hash = JwtTokenService.Hash(refreshToken);
        var token = await db.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == hash, cancellationToken) ?? throw new UnauthorizedAccessException("Invalid refresh token.");
        if (!token.IsActiveToken) throw new UnauthorizedAccessException("Refresh token is no longer active.");
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = "Rotated";
        var response = await jwtTokenService.CreateTokenAsync(token.UserId, ipAddress, cancellationToken);
        token.ReplacedByTokenHash = JwtTokenService.Hash(response.RefreshToken);
        await db.SaveChangesAsync(cancellationToken);
        return response;
    }
    public async Task RevokeAsync(string refreshToken, string? ipAddress, string reason, CancellationToken cancellationToken)
    {
        var hash = JwtTokenService.Hash(refreshToken);
        var token = await db.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == hash, cancellationToken) ?? throw new UnauthorizedAccessException("Invalid refresh token.");
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        await db.SaveChangesAsync(cancellationToken);
    }
}

public sealed class PermissionAuthorizationRequirement(string permission) : IAuthorizationRequirement { public string Permission { get; } = permission; }

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
    {
        if (context.User.HasClaim("is_platform_admin", "true") || context.User.HasClaim(PermissionClaimTypes.Permission, requirement.Permission)) context.Succeed(requirement);
        return Task.CompletedTask;
    }
}

public sealed class PermissionPolicyProvider(Microsoft.Extensions.Options.IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith("Permission:", StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName["Permission:".Length..];
            var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().AddRequirements(new PermissionAuthorizationRequirement(permission)).Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }
        return base.GetPolicyAsync(policyName);
    }
}
