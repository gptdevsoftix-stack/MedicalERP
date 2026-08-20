using MedicalERP.Domain.Catalog;

namespace MedicalERP.Domain.Interfaces;

public interface IProductBarcodeRepository
{
    Task<IReadOnlyCollection<ProductBarcode>> GetAsync(
        Guid companyId,
        Guid? productId,
        string? search,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Guid companyId,
        Guid? productId,
        string? search,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProductBarcode>> GetPagedAsync(
        Guid companyId,
        Guid? productId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ProductBarcode?> GetByIdAsync(
        Guid id,
        Guid companyId,
        bool tracking = false,
        CancellationToken cancellationToken = default);

    Task<bool> BarcodeExistsAsync(
        string barcode,
        Guid companyId,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProductBarcode>> GetPrimaryBarcodesAsync(
        Guid productId,
        Guid companyId,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProductBarcode barcode,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
