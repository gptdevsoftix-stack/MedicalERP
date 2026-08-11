using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Inventory;
using MedicalERP.Domain.Sales;
using MedicalERP.Domain.Support;

namespace MedicalERP.Domain.Interfaces;

public interface ISaleReturnRepository
{
    Task<int> CountAsync(Guid companyId, Guid storeId, string? search, int? status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SaleReturn>> GetAsync(Guid companyId, Guid storeId, string? search, int? status, int skip, int take, CancellationToken cancellationToken = default);
    Task<SaleReturn?> GetByIdAsync(Guid id, Guid companyId, Guid storeId, bool tracking, CancellationToken cancellationToken = default);
    Task<Sale?> GetSaleForReturnAsync(Guid saleId, Guid companyId, Guid storeId, bool tracking, CancellationToken cancellationToken = default);
    Task<bool> ReturnNumberExistsAsync(Guid companyId, Guid storeId, string returnNumber, Guid? excludedId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Warehouse>> GetWarehousesAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default);
    Task<InventoryStock?> GetInventoryStockAsync(Guid companyId, Guid storeId, Guid productId, Guid? warehouseId, Guid? productBatchId, CancellationToken cancellationToken = default);
    Task AddInventoryStockAsync(InventoryStock stock, CancellationToken cancellationToken = default);
    Task AddStockTransactionAsync(StockTransaction transaction, CancellationToken cancellationToken = default);
    Task AddAsync(SaleReturn saleReturn, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
