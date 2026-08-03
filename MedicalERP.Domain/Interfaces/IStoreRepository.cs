using MedicalERP.Domain.Companies;

namespace MedicalERP.Domain.Interfaces;

public interface IStoreRepository
{
    IQueryable<Store> Query();
    Task<Store?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Store store, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
