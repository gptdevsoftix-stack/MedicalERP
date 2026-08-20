using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalERP.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(
            Guid companyId,
            string? search,
            Guid? categoryId,
            bool? isMedicine,
            bool? isActive,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            Guid companyId,
            string? search,
            Guid? categoryId,
            bool? isMedicine,
            bool? isActive,
            CancellationToken cancellationToken = default);

        Task<List<Product>> GetPagedAsync(
            Guid companyId,
            string? search,
            Guid? categoryId,
            bool? isMedicine,
            bool? isActive,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Product?> GetByIdAsync(
            Guid id,
            Guid companyId,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default);

        Task<bool> CodeExistsAsync(
            string code,
            Guid companyId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<bool> RelatedEntityExistsAsync<TEntity>(
            Guid id,
            Guid companyId,
            CancellationToken cancellationToken = default)
            where TEntity : CompanyEntity;

        Task<List<TEntity>> GetActiveLookupsAsync<TEntity>(
            Guid companyId,
            CancellationToken cancellationToken = default)
            where TEntity : CompanyEntity;

        Task AddAsync(
            Product product,
            CancellationToken cancellationToken = default);

        void Update(Product product);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
