using MedicalERP.Application.Common;
using MedicalERP.Application.Purchases.Dtos;

namespace MedicalERP.Application.Interfaces;

public interface IPurchaseOrderService
{
    Task<PagedResult<PurchaseOrderListDto>> GetAsync(PurchaseOrderFilterDto filter, CancellationToken cancellationToken = default);
    Task<PurchaseOrderFormDto?> GetForEditAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PurchaseOrderFormDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(PurchaseOrderFormDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(PurchaseOrderFormDto request, CancellationToken cancellationToken = default);
    Task SubmitAsync(Guid id, CancellationToken cancellationToken = default);
    Task ApproveAsync(Guid id, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PurchaseLookupDto>> GetSuppliersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PurchaseLookupDto>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PurchaseLookupDto>> GetProductUnitsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PurchaseLookupDto>> GetWarehousesAsync(CancellationToken cancellationToken = default);
}
