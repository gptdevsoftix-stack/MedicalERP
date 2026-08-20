using MedicalERP.Domain.Catalog;

namespace MedicalERP.Domain.Interfaces;

public interface IStoreProductRepository
{
    Task<IReadOnlyCollection<StoreProduct>> GetAsync(
        Guid companyId,
        Guid? storeId,
        Guid? productId,
        string? search,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Guid companyId,
        Guid? storeId,
        Guid? productId,
        string? search,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StoreProduct>> GetPagedAsync(
        Guid companyId,
        Guid? storeId,
        Guid? productId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<StoreProduct?> GetByIdAsync(
        Guid id,
        Guid companyId,
        bool tracking = false,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid storeId,
        Guid productId,
        Guid companyId,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        StoreProduct storeProduct,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
