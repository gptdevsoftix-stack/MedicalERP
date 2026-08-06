using MedicalERP.Domain.Inventory;

namespace MedicalERP.Domain.Interfaces;

public interface IProductBatchRepository
{
    IQueryable<ProductBatch> Query();

    Task<IReadOnlyList<ProductBatch>> GetAsync(
        Guid companyId,
        Guid storeId,
        Guid? productId,
        Guid? warehouseId,
        string? search,
        bool? isActive,
        DateOnly? expiringBefore,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Guid companyId,
        Guid storeId,
        Guid? productId,
        Guid? warehouseId,
        string? search,
        bool? isActive,
        DateOnly? expiringBefore,
        CancellationToken cancellationToken = default);

    Task<ProductBatch?> GetByIdAsync(
        Guid id,
        Guid companyId,
        Guid storeId,
        bool tracking = false,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid companyId,
        Guid storeId,
        Guid productId,
        Guid? warehouseId,
        string batchNumber,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProductBatch productBatch,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
