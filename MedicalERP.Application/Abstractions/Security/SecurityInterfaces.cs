using MedicalERP.Application.Identity.Dtos;

namespace MedicalERP.Application.Abstractions.Security;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? CompanyId { get; }
    Guid? SelectedStoreId { get; }
    IReadOnlyCollection<Guid> AllowedStoreIds { get; }
    bool IsPlatformAdmin { get; }
    IReadOnlyCollection<string> Permissions { get; }
}

public interface ICompanyContext { Guid? CompanyId { get; } Guid RequireCompanyId(); }
public interface IStoreContext { Guid? SelectedStoreId { get; }
    IReadOnlyCollection<Guid> AllowedStoreIds { get; } Guid RequireSelectedStoreId(); 
    Task EnsureStoreAccessAsync(Guid storeId, CancellationToken cancellationToken);
}
public interface IJwtTokenService {
    Task<TokenResponse> CreateTokenAsync(Guid userId, string? ipAddress, 
        CancellationToken cancellationToken);
}
public interface IRefreshTokenService {
    Task<TokenResponse> RefreshAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken); 
    Task RevokeAsync(string refreshToken, string? ipAddress, string reason, CancellationToken cancellationToken);
}
