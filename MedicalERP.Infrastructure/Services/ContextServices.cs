using System.Security.Claims;
using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Permissions;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Services;

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;
    public Guid? UserId => Guid.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    public Guid? CompanyId => Guid.TryParse(User?.FindFirstValue("company_id"), out var id) ? id : null;
    public Guid? SelectedStoreId => Guid.TryParse(accessor.HttpContext?.Request.Headers["X-Store-Id"].FirstOrDefault(), out var id) ? id : null;
    public IReadOnlyCollection<Guid> AllowedStoreIds => User?.FindAll("store_id").Select(x => Guid.TryParse(x.Value, out var id) ? id : Guid.Empty).Where(x => x != Guid.Empty).Distinct().ToArray() ?? [];
    public bool IsPlatformAdmin => string.Equals(User?.FindFirstValue("is_platform_admin"), "true", StringComparison.OrdinalIgnoreCase);
    public IReadOnlyCollection<string> Permissions => User?.FindAll(PermissionClaimTypes.Permission).Select(x => x.Value).Distinct().ToArray() ?? [];
}

public sealed class CompanyContext(IHttpContextAccessor accessor, ICurrentUserService currentUser) : ICompanyContext
{
    public Guid? CompanyId
    {
        get
        {
            if (!currentUser.IsPlatformAdmin) return currentUser.CompanyId;
            var http = accessor.HttpContext;
            var raw = http?.Request.Headers["X-Company-Id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(raw)) raw = http?.Request.Query["companyContextId"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(raw) && http?.Request.HasFormContentType == true) raw = http.Request.Form["companyContextId"].FirstOrDefault();
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }
    public Guid RequireCompanyId() => CompanyId ?? throw new UnauthorizedAccessException("A company context is required.");
}

public sealed class StoreContext(ICurrentUserService currentUser, ApplicationDbContext db) : IStoreContext
{
    public Guid? SelectedStoreId => currentUser.SelectedStoreId;
    public IReadOnlyCollection<Guid> AllowedStoreIds => currentUser.AllowedStoreIds;
    public Guid RequireSelectedStoreId()
    {
        var storeId = SelectedStoreId ?? throw new UnauthorizedAccessException("A selected store is required for this operation.");
        if (!currentUser.IsPlatformAdmin && !AllowedStoreIds.Contains(storeId)) throw new UnauthorizedAccessException("The selected store is not assigned to the current user.");
        return storeId;
    }
    public async Task EnsureStoreAccessAsync(Guid storeId, CancellationToken cancellationToken)
    {
        if (currentUser.IsPlatformAdmin) return;
        var userId = currentUser.UserId ?? throw new UnauthorizedAccessException();
        var allowed = await db.UserStoreAccesses.AsNoTracking().AnyAsync(x => x.UserId == userId && x.StoreId == storeId && x.IsActive, cancellationToken);
        if (!allowed) throw new UnauthorizedAccessException("The requested store is not assigned to the current user.");
    }
}

