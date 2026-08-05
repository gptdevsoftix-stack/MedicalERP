using MedicalERP.Domain.Common;
using MedicalERP.Domain.Enums;

namespace MedicalERP.Domain.Interfaces;

public interface ICatalogMasterRepository
{
    Task<List<CompanyEntity>> GetAllAsync(
        CatalogMasterType masterType,
        Guid? companyId,
        string? search,
        CancellationToken cancellationToken = default);

    Task<CompanyEntity?> GetByIdAsync(
        CatalogMasterType masterType,
        Guid id,
        Guid? companyId,
        CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(
        CatalogMasterType masterType,
        string code,
        Guid companyId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        CatalogMasterType masterType,
        CompanyEntity entity,
        CancellationToken cancellationToken = default);

    void Update(CompanyEntity entity);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
