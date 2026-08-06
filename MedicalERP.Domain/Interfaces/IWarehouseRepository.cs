using MedicalERP.Domain.Companies;

namespace MedicalERP.Domain.Interfaces;

public interface IWarehouseRepository
{
    IQueryable<Warehouse> Query();
    Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Warehouse>> GetActiveByStoreAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken);
    Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
