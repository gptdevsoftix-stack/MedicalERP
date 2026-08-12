using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Inventory;
using MedicalERP.Domain.Operations;
using MedicalERP.Domain.Sales;
using MedicalERP.Domain.Support;

namespace MedicalERP.Domain.Interfaces;

public interface ISaleRepository
{
    Task<int> CountAsync(Guid companyId, Guid storeId, string? search, int? status, int? paymentStatus, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Sale>> GetAsync(Guid companyId, Guid storeId, string? search, int? status, int? paymentStatus, DateTimeOffset? from, DateTimeOffset? to, int skip, int take, CancellationToken cancellationToken = default);
    Task<Sale?> GetByIdAsync(Guid id, Guid companyId, Guid storeId, bool tracking, CancellationToken cancellationToken = default);
    Task<bool> IsPaidAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> InvoiceNumberExistsAsync(Guid companyId, Guid storeId, string invoiceNumber, Guid? excludedId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> GetCustomersAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SaleProductLookupData>> GetProductsAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductUnit>> GetProductUnitsAsync(Guid productId, Guid companyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Warehouse>> GetWarehousesAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentMethod>> GetPaymentMethodsAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task AddPaymentMethodAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegisterSession>> GetRegisterSessionsAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default);
    Task<RegisterSession?> GetOpenRegisterSessionAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default);
    Task<Register?> GetDefaultRegisterAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default);
    Task AddRegisterAsync(Register register, CancellationToken cancellationToken = default);
    Task AddRegisterSessionAsync(RegisterSession session, CancellationToken cancellationToken = default);
    Task<decimal> GetCostPriceAsync(Guid companyId, Guid storeId, Guid productId, Guid? warehouseId, CancellationToken cancellationToken = default);
    Task<InventoryStock?> GetInventoryStockAsync(Guid companyId, Guid storeId, Guid productId, Guid? warehouseId, Guid? productBatchId, CancellationToken cancellationToken = default);
    Task AddInventoryStockAsync(InventoryStock stock, CancellationToken cancellationToken = default);
    Task AddStockTransactionAsync(StockTransaction transaction, CancellationToken cancellationToken = default);
    Task AddSalePaymentAsync(SalePayment payment, CancellationToken cancellationToken = default);
    Task AddAsync(Sale sale, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed record SaleProductLookupData(
    Guid Id,
    string Name,
    decimal AvailableStock,
    decimal SalePrice,
    bool AllowNegativeStock);
