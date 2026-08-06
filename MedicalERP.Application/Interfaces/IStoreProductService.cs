using MedicalERP.Domain.DTOs;

namespace MedicalERP.Application.Interfaces;

public interface IStoreProductService
{
    Task<IReadOnlyCollection<StoreProductListDto>> GetAsync(
        Guid? storeId,
        Guid? productId,
        string? search,
        CancellationToken cancellationToken = default);

    Task<StoreProductFormDto?> GetFormByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        StoreProductFormDto request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        StoreProductFormDto request,
        CancellationToken cancellationToken = default);

    Task DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
