using MedicalERP.Domain.Companies;

namespace MedicalERP.Domain.Interfaces;

public interface ICompanyRepository
{
    IQueryable<Company> Query();
    Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Company company, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
