using MedicalERP.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalERP.Application.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductListDto>> GetAllAsync(
            string? search,
            Guid? categoryId,
            bool? isMedicine,
            bool? isActive,
            CancellationToken cancellationToken = default);

        Task<ProductDetailsDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<ProductFormDto?> GetForEditAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(
            ProductFormDto request,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            ProductFormDto request,
            CancellationToken cancellationToken = default);

        Task DeactivateAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<ProductLookupDto>> GetCategoriesAsync(
            CancellationToken cancellationToken = default);

        Task<List<ProductLookupDto>> GetBrandsAsync(
            CancellationToken cancellationToken = default);

        Task<List<ProductLookupDto>> GetManufacturersAsync(
            CancellationToken cancellationToken = default);

        Task<List<ProductLookupDto>> GetGenericMedicinesAsync(
            CancellationToken cancellationToken = default);

        Task<List<ProductLookupDto>> GetDosageFormsAsync(
            CancellationToken cancellationToken = default);

        Task<List<ProductLookupDto>> GetStrengthsAsync(
            CancellationToken cancellationToken = default);

        Task<List<ProductLookupDto>> GetUnitsAsync(
            CancellationToken cancellationToken = default);
    }
}
