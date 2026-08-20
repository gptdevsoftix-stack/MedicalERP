using MedicalERP.Application.Common;
using MedicalERP.Domain.DTOs;
using MedicalERP.Domain.Enums;

namespace MedicalERP.Application.Interfaces;

public interface ICatalogMasterService
{
    Task<List<CatalogMasterDto>> GetAllAsync(
        CatalogMasterType masterType,
        string? search,
        CancellationToken cancellationToken = default);

    Task<PagedResult<CatalogMasterDto>> GetAllPagedAsync(
        CatalogMasterType masterType,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<CatalogMasterDto?> GetByIdAsync(
        CatalogMasterType masterType,
        Guid id,
        CancellationToken cancellationToken = default);

    Task<CatalogMasterFormDto?> GetForEditAsync(
        CatalogMasterType masterType,
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        CatalogMasterFormDto request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        CatalogMasterFormDto request,
        CancellationToken cancellationToken = default);

    Task DeactivateAsync(
        CatalogMasterType masterType,
        Guid id,
        CancellationToken cancellationToken = default);
}
