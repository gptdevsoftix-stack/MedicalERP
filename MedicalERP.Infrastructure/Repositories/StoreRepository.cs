using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class StoreRepository(ApplicationDbContext context) : IStoreRepository
{
    public IQueryable<Store> Query() => context.Stores.AsNoTracking();
    public Task<Store?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => context.Stores.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task AddAsync(Store store, CancellationToken cancellationToken) => await context.Stores.AddAsync(store, cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}

