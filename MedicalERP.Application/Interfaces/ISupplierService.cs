using MedicalERP.Application.Common;
using MedicalERP.Application.Purchases.Dtos;

namespace MedicalERP.Application.Interfaces;

public interface ISupplierService
{
    Task<PagedResult<SupplierListDto>> GetAsync(string? search, bool? isActive, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<SupplierFormDto?> GetForEditAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(SupplierFormDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(SupplierFormDto request, CancellationToken cancellationToken = default);
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}
