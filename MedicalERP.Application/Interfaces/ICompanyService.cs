using MedicalERP.Application.Common;
using MedicalERP.Application.Companies.Dtos;

namespace MedicalERP.Application.Interfaces;

public interface ICompanyService
{
    Task<PagedResult<CompanyDto>> GetAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<CompanyDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CompanyDto> CreateAsync(CreateCompanyRequest request, CancellationToken cancellationToken);
    Task<CompanyDto> UpdateAsync(Guid id, UpdateCompanyRequest request, CancellationToken cancellationToken);
    Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken);
}
