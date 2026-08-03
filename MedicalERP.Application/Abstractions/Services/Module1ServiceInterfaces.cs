using MedicalERP.Application.Common;
using MedicalERP.Application.Companies.Dtos;
using MedicalERP.Application.Identity.Dtos;
using MedicalERP.Application.Stores.Dtos;
using MedicalERP.Application.Warehouses.Dtos;

namespace MedicalERP.Application.Abstractions.Services;

public interface ICompanyService
{
    Task<PagedResult<CompanyDto>> GetAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<CompanyDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CompanyDto> CreateAsync(CreateCompanyRequest request, CancellationToken cancellationToken);
    Task<CompanyDto> UpdateAsync(Guid id, UpdateCompanyRequest request, CancellationToken cancellationToken);
    Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken);
}

public interface IStoreService
{
    Task<PagedResult<StoreDto>> GetAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<StoreDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<StoreDto> CreateAsync(CreateStoreRequest request, CancellationToken cancellationToken);
    Task<StoreDto> UpdateAsync(Guid id, UpdateStoreRequest request, CancellationToken cancellationToken);
    Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken);
}

public interface IWarehouseService
{
    Task<PagedResult<WarehouseDto>> GetAsync(Guid? storeId, QueryParameters query, CancellationToken cancellationToken);
    Task<WarehouseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<WarehouseDto> CreateAsync(CreateWarehouseRequest request, CancellationToken cancellationToken);
    Task<WarehouseDto> UpdateAsync(Guid id, UpdateWarehouseRequest request, CancellationToken cancellationToken);
    Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken);
}

public interface IIdentityService
{
    Task<TokenResponse> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken);
    Task<string> GeneratePasswordResetTokenAsync(ForgotPasswordRequest request, CancellationToken cancellationToken);
    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken);
    Task<PagedResult<UserDto>> GetUsersAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken);
    Task SetUserActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken);
    Task AssignStoresAsync(AssignStoresRequest request, CancellationToken cancellationToken);
    Task AssignRolesAsync(AssignRolesRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken cancellationToken);
    Task<RoleDto> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken);
    Task<RoleDto> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<RoleDto> UpdateRoleAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken);
    Task SetRoleActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken);
    Task ManageRolePermissionsAsync(Guid roleId, ManageRolePermissionsRequest request, CancellationToken cancellationToken);
}

