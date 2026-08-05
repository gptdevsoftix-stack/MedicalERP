using MedicalERP.Application.Common;
using MedicalERP.Application.Warehouses.Dtos;

namespace MedicalERP.Application.Interfaces;

public interface IWarehouseService
{
    Task<PagedResult<WarehouseDto>> GetAsync(Guid? storeId, QueryParameters query, CancellationToken cancellationToken);
    Task<WarehouseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<WarehouseDto> CreateAsync(CreateWarehouseRequest request, CancellationToken cancellationToken);
    Task<WarehouseDto> UpdateAsync(Guid id, UpdateWarehouseRequest request, CancellationToken cancellationToken);
    Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken);
}
