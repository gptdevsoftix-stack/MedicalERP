using MedicalERP.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalERP.Domain.Interfaces
{
    public interface IProductUnitRepository
    {
        Task<IReadOnlyCollection<ProductUnit>> GetByProductIdAsync(
            Guid productId,
            Guid companyId,
            bool tracking = false,
            CancellationToken cancellationToken = default);

        Task<ProductUnit?> GetByIdAsync(
            Guid id,
            Guid companyId,
            bool tracking = false,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            Guid productId,
            Guid unitId,
            Guid companyId,
            Guid? excludedId = null,
            CancellationToken cancellationToken = default);

        Task<ProductUnit?> GetBaseUnitAsync(
            Guid productId,
            Guid companyId,
            Guid? excludedId = null,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProductUnit productUnit,
            CancellationToken cancellationToken = default);

        void Remove(ProductUnit productUnit);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
