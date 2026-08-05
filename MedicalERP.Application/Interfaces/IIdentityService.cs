using MedicalERP.Application.Common;
using MedicalERP.Application.Identity.Dtos;

namespace MedicalERP.Application.Interfaces;

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
