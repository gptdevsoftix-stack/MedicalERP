using MedicalERP.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalERP.Application.Interfaces
{
    public interface IProductUnitService
    {
        Task<IReadOnlyCollection<ProductUnitListDto>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task<ProductUnitFormDto?> GetFormByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(
            ProductUnitFormDto request,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            ProductUnitFormDto request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
