using MedicalERP.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalERP.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(
            Guid companyId,
            string? search,
            CancellationToken cancellationToken = default);

        Task<Category?> GetByIdAsync(
            Guid id,
            Guid companyId,
            CancellationToken cancellationToken = default);

        Task<bool> CodeExistsAsync(
            string code,
            Guid companyId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<bool> HasChildrenAsync(
            Guid categoryId,
            Guid companyId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Category category,
            CancellationToken cancellationToken = default);

        void Update(Category category);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
