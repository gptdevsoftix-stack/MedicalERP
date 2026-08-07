using MedicalERP.Domain.Purchases;

namespace MedicalERP.Domain.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<int> CountAsync(Guid companyId, Guid storeId, string? search, int? status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PurchaseOrder>> GetAsync(Guid companyId, Guid storeId, string? search, int? status, int skip, int take, CancellationToken cancellationToken = default);
    Task<PurchaseOrder?> GetByIdAsync(Guid id, Guid companyId, Guid storeId, bool tracking, CancellationToken cancellationToken = default);
    Task<bool> OrderNumberExistsAsync(Guid companyId, Guid storeId, string orderNumber, Guid? excludedId, CancellationToken cancellationToken = default);
    void RemoveItems(IEnumerable<PurchaseOrderItem> items);
    Task<IReadOnlyList<Supplier>> GetSuppliersAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MedicalERP.Domain.Catalog.Product>> GetProductsAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MedicalERP.Domain.Catalog.ProductUnit>> GetProductUnitsAsync(Guid productId, Guid companyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MedicalERP.Domain.Companies.Warehouse>> GetWarehousesAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default);
    Task AddAsync(PurchaseOrder order, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
