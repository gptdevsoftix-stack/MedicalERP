using MedicalERP.Application.Common;
using MedicalERP.Application.Inventory.Dtos;

namespace MedicalERP.Application.Interfaces;

public interface IProductBatchService
{
    Task<PagedResult<ProductBatchListDto>> GetAsync(
        ProductBatchFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<ProductBatchFormDto?> GetFormByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ProductBatchListDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        ProductBatchFormDto request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        ProductBatchFormDto request,
        CancellationToken cancellationToken = default);

    Task DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProductBatchLookupDto>> GetProductLookupsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProductBatchLookupDto>> GetWarehouseLookupsAsync(
        CancellationToken cancellationToken = default);
}
