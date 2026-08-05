using System.Security.Claims;
using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Common;
using MedicalERP.Application.Identity.Dtos;
using MedicalERP.Application.Permissions;
using MedicalERP.Domain.Identity;
using MedicalERP.Infrastructure.Identity;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Services;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ApplicationDbContext db,
    IJwtTokenService jwt,
    ICompanyContext companyContext,
    IStoreContext storeContext) : IIdentityService
{
    public async Task<TokenResponse> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ?? throw new UnauthorizedAccessException("Invalid credentials.");
        if (!await userManager.CheckPasswordAsync(user, request.Password)) throw new UnauthorizedAccessException("Invalid credentials.");
        return await jwt.CreateTokenAsync(user.Id, ipAddress, cancellationToken);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ?? throw new KeyNotFoundException("User not found.");
        return await userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ?? throw new KeyNotFoundException("User not found.");
        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(QueryParameters query, CancellationToken cancellationToken)
    {
        var companyId = companyContext.CompanyId;
        var source = userManager.Users.AsNoTracking().Where(x => companyId == null || x.CompanyId == companyId);
        if (!string.IsNullOrWhiteSpace(query.Search)) source = source.Where(x => (x.Email ?? string.Empty).Contains(query.Search) || x.FirstName.Contains(query.Search) || x.LastName.Contains(query.Search));
        var total = await source.CountAsync(cancellationToken);
        var users = await source.OrderBy(x => x.Email).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(cancellationToken);
        var dtos = new List<UserDto>();
        foreach (var user in users) dtos.Add(await MapUserAsync(user, cancellationToken));
        return new PagedResult<UserDto>(dtos, query.Page, query.PageSize, total);
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(id.ToString()) ?? throw new KeyNotFoundException("User not found.");
        return await MapUserAsync(user, cancellationToken);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var companyId = companyContext.RequireCompanyId();
        foreach (var storeId in request.StoreIds) await storeContext.EnsureStoreAccessAsync(storeId, cancellationToken);
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true,
            CompanyId = companyId,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        if (request.Roles.Count > 0) await userManager.AddToRolesAsync(user, request.Roles);
        db.UserStoreAccesses.AddRange(request.StoreIds.Select((id, index) => new UserStoreAccess { UserId = user.Id, CompanyId = companyId, StoreId = id, IsDefaultStore = index == 0 }));
        await db.SaveChangesAsync(cancellationToken);
        return await MapUserAsync(user, cancellationToken);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(id.ToString()) ?? throw new KeyNotFoundException("User not found.");
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.IsActive = request.IsActive;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        return await MapUserAsync(user, cancellationToken);
    }

    public async Task SetUserActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(id.ToString()) ?? throw new KeyNotFoundException("User not found.");
        user.IsActive = isActive;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
    }

    public async Task AssignStoresAsync(AssignStoresRequest request, CancellationToken cancellationToken)
    {
        var companyId = companyContext.RequireCompanyId();
        foreach (var storeId in request.StoreIds) await storeContext.EnsureStoreAccessAsync(storeId, cancellationToken);
        var current = await db.UserStoreAccesses.Where(x => x.UserId == request.UserId).ToListAsync(cancellationToken);
        db.UserStoreAccesses.RemoveRange(current);
        db.UserStoreAccesses.AddRange(request.StoreIds.Select(id => new UserStoreAccess { UserId = request.UserId, CompanyId = companyId, StoreId = id, IsDefaultStore = request.DefaultStoreId == id }));
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignRolesAsync(AssignRolesRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new KeyNotFoundException("User not found.");
        var current = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, current);
        if (request.Roles.Count > 0) await userManager.AddToRolesAsync(user, request.Roles);
    }

    public async Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken cancellationToken)
    {
        var companyId = companyContext.CompanyId;
        var roles = await roleManager.Roles.AsNoTracking().Where(x => x.CompanyId == null || x.CompanyId == companyId).OrderBy(x => x.Name).ToListAsync(cancellationToken);
        var result = new List<RoleDto>();
        foreach (var role in roles) result.Add(await MapRoleAsync(role));
        return result;
    }

    public async Task<RoleDto> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(id.ToString()) ?? throw new KeyNotFoundException("Role not found.");
        return await MapRoleAsync(role);
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = new ApplicationRole { Name = request.Name, Description = request.Description, CompanyId = companyContext.RequireCompanyId() };
        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        return await MapRoleAsync(role);
    }

    public async Task<RoleDto> UpdateRoleAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(id.ToString()) ?? throw new KeyNotFoundException("Role not found.");
        role.Description = request.Description;
        role.IsActive = request.IsActive;
        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        return await MapRoleAsync(role);
    }

    public async Task SetRoleActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(id.ToString()) ?? throw new KeyNotFoundException("Role not found.");
        role.IsActive = isActive;
        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
    }

    public async Task ManageRolePermissionsAsync(Guid roleId, ManageRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        var invalid = request.Permissions.Except(Permissions.All, StringComparer.OrdinalIgnoreCase).ToArray();
        if (invalid.Length > 0) throw new InvalidOperationException($"Invalid permissions: {string.Join(", ", invalid)}");
        var role = await roleManager.FindByIdAsync(roleId.ToString()) ?? throw new KeyNotFoundException("Role not found.");
        var claims = await roleManager.GetClaimsAsync(role);
        foreach (var claim in claims.Where(x => x.Type == PermissionClaimTypes.Permission)) await roleManager.RemoveClaimAsync(role, claim);
        foreach (var permission in request.Permissions.Distinct(StringComparer.OrdinalIgnoreCase)) await roleManager.AddClaimAsync(role, new Claim(PermissionClaimTypes.Permission, permission));
    }

    private async Task<UserDto> MapUserAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var stores = await db.UserStoreAccesses.AsNoTracking().IgnoreQueryFilters().Where(x => x.UserId == user.Id && x.IsActive).Select(x => x.StoreId).ToListAsync(cancellationToken);
        var roles = (await userManager.GetRolesAsync(user)).ToArray();
        return new UserDto(user.Id, user.CompanyId, user.Email ?? string.Empty, user.FirstName, user.LastName, user.IsPlatformAdmin, user.IsActive, stores, roles);
    }

    private async Task<RoleDto> MapRoleAsync(ApplicationRole role)
    {
        var permissions = (await roleManager.GetClaimsAsync(role)).Where(x => x.Type == PermissionClaimTypes.Permission).Select(x => x.Value).ToArray();
        return new RoleDto(role.Id, role.CompanyId, role.Name ?? string.Empty, role.Description, role.IsSystemRole, role.IsActive, permissions);
    }
}


