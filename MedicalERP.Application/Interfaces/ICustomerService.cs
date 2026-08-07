using MedicalERP.Application.Common;
using MedicalERP.Application.Sales.Dtos;

namespace MedicalERP.Application.Interfaces;

public interface ICustomerService
{
    Task<PagedResult<CustomerListDto>> GetAsync(string? search, bool? isActive, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<CustomerFormDto?> GetForEditAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CustomerFormDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(CustomerFormDto request, CancellationToken cancellationToken = default);
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}
