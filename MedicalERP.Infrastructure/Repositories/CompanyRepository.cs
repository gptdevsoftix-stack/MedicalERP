using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class CompanyRepository(ApplicationDbContext context) : ICompanyRepository
{
    public IQueryable<Company> Query() => context.Companies.AsNoTracking();
    public Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => context.Companies.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task AddAsync(Company company, CancellationToken cancellationToken) => await context.Companies.AddAsync(company, cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}

