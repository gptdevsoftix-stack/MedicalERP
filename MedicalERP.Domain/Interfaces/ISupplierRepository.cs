using MedicalERP.Domain.Purchases;

namespace MedicalERP.Domain.Interfaces;

public interface ISupplierRepository
{
    Task<int> CountAsync(Guid companyId, Guid storeId, string? search, bool? isActive, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Supplier>> GetAsync(Guid companyId, Guid storeId, string? search, bool? isActive, int skip, int take, CancellationToken cancellationToken = default);
    Task<Supplier?> GetByIdAsync(Guid id, Guid companyId, Guid storeId, bool tracking, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(Guid companyId, string code, Guid? excludedId, CancellationToken cancellationToken = default);
    Task AddAsync(Supplier supplier, SupplierStore storeAccess, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
