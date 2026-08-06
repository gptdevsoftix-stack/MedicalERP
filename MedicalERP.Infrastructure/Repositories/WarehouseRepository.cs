using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class WarehouseRepository(ApplicationDbContext context) : IWarehouseRepository
{
    public IQueryable<Warehouse> Query() => context.Warehouses.AsNoTracking();
    public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => context.Warehouses.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task<IReadOnlyList<Warehouse>> GetActiveByStoreAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken) => await context.Warehouses.AsNoTracking().Where(x => x.CompanyId == companyId && x.StoreId == storeId && x.IsActive).OrderBy(x => x.Name).ToListAsync(cancellationToken);
    public async Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken) => await context.Warehouses.AddAsync(warehouse, cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}

