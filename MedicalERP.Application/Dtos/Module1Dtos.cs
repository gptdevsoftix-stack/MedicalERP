using MedicalERP.Domain.Enums;

namespace MedicalERP.Application.Companies.Dtos
{
    public sealed record CompanyDto(Guid Id, string Name, string Code, string? LegalName, string? Email, string? Phone, string? Address, string? City, string? State, string Country, string? TaxNumber, string CurrencyCode, string TimeZone, string SubscriptionStatus, bool IsActive);
    public sealed record CreateCompanyRequest(string Name, string Code, string? LegalName, string? Email, string? Phone, string? Address, string? City, string? State, string Country, string? TaxNumber, string CurrencyCode, string TimeZone);
    public sealed record UpdateCompanyRequest(string Name, string? LegalName, string? Email, string? Phone, string? Address, string? City, string? State, string Country, string? TaxNumber, string CurrencyCode, string TimeZone);
}

namespace MedicalERP.Application.Stores.Dtos
{
    public sealed record StoreDto(Guid Id, Guid CompanyId, string Name, string Code, string? Email, string? Phone, string? Address, string? City, string? State, string Country, string? TaxNumber, string CurrencyCode, string TimeZone, bool IsHeadOffice, bool IsActive);
    public sealed record CreateStoreRequest(string Name, string Code, string? Email, string? Phone, string? Address, string? City, string? State, string Country, string? TaxNumber, string CurrencyCode, string TimeZone, bool IsHeadOffice);
    public sealed record UpdateStoreRequest(string Name, string? Email, string? Phone, string? Address, string? City, string? State, string Country, string? TaxNumber, string CurrencyCode, string TimeZone, bool IsHeadOffice);
}

namespace MedicalERP.Application.Warehouses.Dtos
{
    public sealed record WarehouseDto(Guid Id, Guid CompanyId, Guid StoreId, string Name, string Code, WarehouseType WarehouseType, string? Address, bool IsDefault, bool IsActive);
    public sealed record CreateWarehouseRequest(Guid StoreId, string Name, string Code, WarehouseType WarehouseType, string? Address, bool IsDefault);
    public sealed record UpdateWarehouseRequest(string Name, WarehouseType WarehouseType, string? Address, bool IsDefault);
}

namespace MedicalERP.Application.Identity.Dtos
{
    public sealed record LoginRequest(string Email, string Password);
    public sealed record RefreshTokenRequest(string RefreshToken);
    public sealed record RevokeRefreshTokenRequest(string RefreshToken, string Reason);
    public sealed record ForgotPasswordRequest(string Email);
    public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);
    public sealed record TokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, Guid UserId, Guid? CompanyId, Guid? DefaultStoreId, IReadOnlyCollection<string> Permissions);
    public sealed record UserDto(Guid Id, Guid? CompanyId, string Email, string FirstName, string LastName, bool IsPlatformAdmin, bool IsActive, IReadOnlyCollection<Guid> StoreIds, IReadOnlyCollection<string> Roles);
    public sealed record CreateUserRequest(string Email, string Password, string FirstName, string LastName, IReadOnlyCollection<string> Roles, IReadOnlyCollection<Guid> StoreIds);
    public sealed record UpdateUserRequest(string FirstName, string LastName, bool IsActive);
    public sealed record AssignStoresRequest(Guid UserId, IReadOnlyCollection<Guid> StoreIds, Guid? DefaultStoreId);
    public sealed record AssignRolesRequest(Guid UserId, IReadOnlyCollection<string> Roles);
    public sealed record RoleDto(Guid Id, Guid? CompanyId, string Name, string? Description, bool IsSystemRole, bool IsActive, IReadOnlyCollection<string> Permissions);
    public sealed record CreateRoleRequest(string Name, string? Description);
    public sealed record UpdateRoleRequest(string? Description, bool IsActive);
    public sealed record ManageRolePermissionsRequest(IReadOnlyCollection<string> Permissions);
}


