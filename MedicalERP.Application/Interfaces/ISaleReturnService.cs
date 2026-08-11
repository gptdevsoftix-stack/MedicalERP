using MedicalERP.Application.Common;
using MedicalERP.Application.Sales.Dtos;

namespace MedicalERP.Application.Interfaces;

public interface ISaleReturnService
{
    Task<PagedResult<SaleReturnListDto>> GetAsync(SaleReturnFilterDto filter, CancellationToken cancellationToken = default);
    Task<SaleReturnFormDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SaleReturnFormDto?> GetForReturnAsync(Guid saleId, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(SaleReturnFormDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SaleLookupDto>> GetWarehousesAsync(CancellationToken cancellationToken = default);
}
