using MedicalERP.Application.Common;
using MedicalERP.Application.Sales.Dtos;

namespace MedicalERP.Application.Interfaces;

public interface ISaleService
{
    Task<PagedResult<SaleListDto>> GetAsync(SaleFilterDto filter, CancellationToken cancellationToken = default);
    Task<SaleFormDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(SaleFormDto request, CancellationToken cancellationToken = default);
    Task<Guid> EnsureOpenRegisterSessionAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SaleLookupDto>> GetCustomersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SaleProductLookupDto>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SaleLookupDto>> GetProductUnitsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SaleLookupDto>> GetWarehousesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SaleLookupDto>> GetPaymentMethodsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SaleLookupDto>> GetRegisterSessionsAsync(CancellationToken cancellationToken = default);
}
