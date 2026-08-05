using MedicalERP.Application.Common;
using MedicalERP.Application.Stores.Dtos;

namespace MedicalERP.Application.Interfaces;

public interface IStoreService
{
    Task<PagedResult<StoreDto>> GetAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<StoreDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<StoreDto> CreateAsync(CreateStoreRequest request, CancellationToken cancellationToken);
    Task<StoreDto> UpdateAsync(Guid id, UpdateStoreRequest request, CancellationToken cancellationToken);
    Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken);
}
