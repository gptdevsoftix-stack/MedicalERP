using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Sales;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class CustomerRepository(ApplicationDbContext context) : ICustomerRepository
{
    private IQueryable<Customer> Query(Guid companyId)
    {
        return context.Customers.Where(x => x.CompanyId == companyId);
    }

    public Task<int> CountAsync(Guid companyId, string? search, bool? isActive, CancellationToken cancellationToken = default)
        => ApplyFilters(Query(companyId), search, isActive).CountAsync(cancellationToken);

    public async Task<IReadOnlyList<Customer>> GetAsync(Guid companyId, string? search, bool? isActive, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(Query(companyId), search, isActive).OrderBy(x => x.Name).Skip(skip).Take(take).AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<Customer?> GetByIdAsync(Guid id, Guid companyId, bool tracking, CancellationToken cancellationToken = default)
    {
        IQueryable<Customer> query = Query(companyId);
        if (!tracking) query = query.AsNoTracking();
        return query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(Guid companyId, string code, Guid? excludedId, CancellationToken cancellationToken = default)
        => context.Customers.AnyAsync(x => x.CompanyId == companyId && x.Code == code && (!excludedId.HasValue || x.Id != excludedId.Value), cancellationToken);

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await context.Customers.AddAsync(customer, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);

    private static IQueryable<Customer> ApplyFilters(IQueryable<Customer> query, string? search, bool? isActive)
    {
        if (isActive.HasValue) query = query.Where(x => x.IsActive == isActive.Value);
        if (!string.IsNullOrWhiteSpace(search)) { var value = search.Trim(); query = query.Where(x => x.Name.Contains(value) || x.Code.Contains(value) || (x.Phone != null && x.Phone.Contains(value)) || (x.Email != null && x.Email.Contains(value))); }
        return query;
    }
}
