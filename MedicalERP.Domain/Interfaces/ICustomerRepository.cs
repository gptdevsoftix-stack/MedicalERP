using MedicalERP.Domain.Sales;

namespace MedicalERP.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<int> CountAsync(Guid companyId, string? search, bool? isActive, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> GetAsync(Guid companyId, string? search, bool? isActive, int skip, int take, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid id, Guid companyId, bool tracking, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(Guid companyId, string code, Guid? excludedId, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
