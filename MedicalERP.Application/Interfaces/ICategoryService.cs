using MedicalERP.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalERP.Application.Interfaces
{

    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync(
            string? search,
            CancellationToken cancellationToken = default);

        Task<CategoryDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<UpdateCategoryDto?> GetForEditAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(
            CreateCategoryDto request,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            UpdateCategoryDto request,
            CancellationToken cancellationToken = default);

        Task DeactivateAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
