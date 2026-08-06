using MedicalERP.Domain.DTOs;

namespace MedicalERP.Application.Interfaces;

public interface IProductBarcodeService
{
    Task<IReadOnlyCollection<ProductBarcodeListDto>> GetAsync(
        Guid? productId,
        string? search,
        CancellationToken cancellationToken = default);

    Task<ProductBarcodeFormDto?> GetFormByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        ProductBarcodeFormDto request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        ProductBarcodeFormDto request,
        CancellationToken cancellationToken = default);

    Task DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
