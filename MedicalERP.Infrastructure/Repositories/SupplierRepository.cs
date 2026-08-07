using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Purchases;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class SupplierRepository(ApplicationDbContext context) : ISupplierRepository
{
    private IQueryable<Supplier> Query(Guid companyId, Guid storeId)
    {
        return context.Suppliers.Where(x => x.CompanyId == companyId && context.SupplierStores.Any(ss => ss.SupplierId == x.Id && ss.CompanyId == companyId && ss.StoreId == storeId && ss.IsActive));
    }

    public Task<int> CountAsync(Guid companyId, Guid storeId, string? search, bool? isActive, CancellationToken cancellationToken = default) => ApplyFilters(Query(companyId, storeId), search, isActive).CountAsync(cancellationToken);

    public async Task<IReadOnlyList<Supplier>> GetAsync(Guid companyId, Guid storeId, string? search, bool? isActive, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(Query(companyId, storeId), search, isActive).OrderBy(x => x.Name).Skip(skip).Take(take).AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<Supplier?> GetByIdAsync(Guid id, Guid companyId, Guid storeId, bool tracking, CancellationToken cancellationToken = default)
    {
        IQueryable<Supplier> query = Query(companyId, storeId).Include(x => x.Stores);
        if (!tracking) query = query.AsNoTracking();
        return query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(Guid companyId, string code, Guid? excludedId, CancellationToken cancellationToken = default) => context.Suppliers.AnyAsync(x => x.CompanyId == companyId && x.Code == code && (!excludedId.HasValue || x.Id != excludedId.Value), cancellationToken);

    public async Task AddAsync(Supplier supplier, SupplierStore storeAccess, CancellationToken cancellationToken = default)
    {
        await context.Suppliers.AddAsync(supplier, cancellationToken);
        await context.SupplierStores.AddAsync(storeAccess, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);

    private static IQueryable<Supplier> ApplyFilters(IQueryable<Supplier> query, string? search, bool? isActive)
    {
        if (isActive.HasValue) query = query.Where(x => x.IsActive == isActive.Value);
        if (!string.IsNullOrWhiteSpace(search)) { var value = search.Trim(); query = query.Where(x => x.Name.Contains(value) || x.Code.Contains(value) || (x.ContactPerson != null && x.ContactPerson.Contains(value)) || (x.Phone != null && x.Phone.Contains(value))); }
        return query;
    }
}
