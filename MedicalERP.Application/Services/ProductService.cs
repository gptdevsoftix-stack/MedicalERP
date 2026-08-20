using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.DTOs;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Interfaces;

namespace MedicalERP.Application.Services
{
    public sealed class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ICompanyContext _companyContext;

        public ProductService(
            IProductRepository repository,
            ICompanyContext companyContext)
        {
            _repository = repository;
            _companyContext = companyContext;
        }

        public async Task<List<ProductListDto>> GetAllAsync(
            string? search,
            Guid? categoryId,
            bool? isMedicine,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var companyId = _companyContext.CompanyId
                  ?? throw new UnauthorizedAccessException(
                      "Company context is missing.");
            var products = await _repository.GetAllAsync(
                companyId,
                search,
                categoryId,
                isMedicine,
                isActive,
                cancellationToken);

            return products.Select(x => new ProductListDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                CategoryName = x.Category.Name,
                BrandName = x.Brand?.Name,
                ManufacturerName = x.Manufacturer?.Name,
                BaseUnitName = x.BaseUnit.Name,
                ProductType = x.ProductType,
                IsMedicine = x.IsMedicine,
                IsActive = x.IsActive
            }).ToList();
        }

        public async Task<PagedResult<ProductListDto>> GetAllPagedAsync(
            string? search,
            Guid? categoryId,
            bool? isMedicine,
            bool? isActive,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var companyId = _companyContext.CompanyId
                  ?? throw new UnauthorizedAccessException(
                      "Company context is missing.");

            var totalCount = await _repository.CountAsync(
                companyId, search, categoryId, isMedicine, isActive, cancellationToken);

            var products = await _repository.GetPagedAsync(
                companyId, search, categoryId, isMedicine, isActive, page, pageSize, cancellationToken);

            var items = products.Select(x => new ProductListDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                CategoryName = x.Category.Name,
                BrandName = x.Brand?.Name,
                ManufacturerName = x.Manufacturer?.Name,
                BaseUnitName = x.BaseUnit.Name,
                ProductType = x.ProductType,
                IsMedicine = x.IsMedicine,
                IsActive = x.IsActive
            }).ToList();

            return new PagedResult<ProductListDto>(items, page, pageSize, totalCount);
        }

        public async Task<ProductDetailsDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var companyId = _companyContext.CompanyId
                  ?? throw new UnauthorizedAccessException(
                      "Company context is missing.");
            var product = await _repository.GetByIdAsync(
                id,
                companyId,
                true,
                cancellationToken);

            if (product is null)
                return null;

            return new ProductDetailsDto
            {
                Id = product.Id,
                Name = product.Name,
                Code = product.Code,
                Description = product.Description,
                CategoryName = product.Category.Name,
                BrandName = product.Brand?.Name,
                ManufacturerName = product.Manufacturer?.Name,
                GenericMedicineName = product.GenericMedicine?.Name,
                DosageFormName = product.DosageForm?.Name,
                StrengthName = product.Strength?.Name,
                BaseUnitName = product.BaseUnit.Name,
                ProductType = product.ProductType,
                IsMedicine = product.IsMedicine,
                RequiresPrescription = product.RequiresPrescription,
                IsControlledDrug = product.IsControlledDrug,
                TrackBatch = product.TrackBatch,
                TrackExpiry = product.TrackExpiry,
                AllowDiscount = product.AllowDiscount,
                AllowNegativeStock = product.AllowNegativeStock,
                RegulatoryNumber = product.RegulatoryNumber,
                IsActive = product.IsActive
            };
        }

        public async Task<ProductFormDto?> GetForEditAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var companyId = _companyContext.CompanyId
                  ?? throw new UnauthorizedAccessException(
                      "Company context is missing.");
            var product = await _repository.GetByIdAsync(
                id,
                companyId,
                true,
                cancellationToken);

            if (product is null)
                return null;

            return new ProductFormDto
            {
                Id = product.Id,
                Name = product.Name,
                Code = product.Code,
                Description = product.Description,
                CategoryId = product.CategoryId,
                ProductBrandId = product.ProductBrandId,
                ManufacturerId = product.ManufacturerId,
                GenericMedicineId = product.GenericMedicineId,
                DosageFormId = product.DosageFormId,
                StrengthId = product.StrengthId,
                BaseUnitId = product.BaseUnitId,
                ProductType = product.ProductType,
                IsMedicine = product.IsMedicine,
                RequiresPrescription = product.RequiresPrescription,
                IsControlledDrug = product.IsControlledDrug,
                TrackBatch = product.TrackBatch,
                TrackExpiry = product.TrackExpiry,
                AllowDiscount = product.AllowDiscount,
                AllowNegativeStock = product.AllowNegativeStock,
                RegulatoryNumber = product.RegulatoryNumber,
                IsActive = product.IsActive
            };
        }

        public async Task<Guid> CreateAsync(
            ProductFormDto request,
            CancellationToken cancellationToken = default)
        {
            NormalizeProductType(request);

            var companyId = _companyContext.CompanyId
                  ?? throw new UnauthorizedAccessException(
                      "Company context is missing.");

            var code = NormalizeCode(request.Code);

            if (await _repository.CodeExistsAsync(
                    code,
                    companyId,
                    cancellationToken: cancellationToken))
            {
                throw new InvalidOperationException(
                    "A product with this code already exists.");
            }

            await ValidateLookupsAsync(
                request,
                companyId,
                cancellationToken);

            ValidateMedicalFields(request);

            var product = new Product
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                Name = request.Name.Trim(),
                Code = code,
                Description = NormalizeOptional(request.Description),
                CategoryId = request.CategoryId,
                ProductBrandId = request.ProductBrandId,
                ManufacturerId = request.ManufacturerId,
                GenericMedicineId = request.GenericMedicineId,
                DosageFormId = request.DosageFormId,
                StrengthId = request.StrengthId,
                BaseUnitId = request.BaseUnitId,
                ProductType = request.ProductType,
                IsMedicine = request.IsMedicine,
                RequiresPrescription = request.RequiresPrescription,
                IsControlledDrug = request.IsControlledDrug,
                TrackBatch = request.TrackBatch,
                TrackExpiry = request.TrackExpiry,
                AllowDiscount = request.AllowDiscount,
                AllowNegativeStock = request.AllowNegativeStock,
                RegulatoryNumber =
                    NormalizeOptional(request.RegulatoryNumber),
                IsActive = true
            };

            ClearMedicineFieldsWhenNotMedicine(product);

            await _repository.AddAsync(product, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return product.Id;
        }

        public async Task UpdateAsync(
            ProductFormDto request,
            CancellationToken cancellationToken = default)
        {
            NormalizeProductType(request);

            var companyId = _companyContext.CompanyId
                  ?? throw new UnauthorizedAccessException(
                      "Company context is missing.");
            var product = await _repository.GetByIdAsync(
                request.Id,
                companyId,
                false,
                cancellationToken)
                ?? throw new KeyNotFoundException(
                    "Product not found.");

            var code = NormalizeCode(request.Code);

            if (await _repository.CodeExistsAsync(
                    code,
                    companyId,
                    request.Id,
                    cancellationToken))
            {
                throw new InvalidOperationException(
                    "A product with this code already exists.");
            }

            await ValidateLookupsAsync(
                request,
                companyId,
                cancellationToken);

            ValidateMedicalFields(request);

            product.Name = request.Name.Trim();
            product.Code = code;
            product.Description =
                NormalizeOptional(request.Description);
            product.CategoryId = request.CategoryId;
            product.ProductBrandId = request.ProductBrandId;
            product.ManufacturerId = request.ManufacturerId;
            product.GenericMedicineId = request.GenericMedicineId;
            product.DosageFormId = request.DosageFormId;
            product.StrengthId = request.StrengthId;
            product.BaseUnitId = request.BaseUnitId;
            product.ProductType = request.ProductType;
            product.IsMedicine = request.IsMedicine;
            product.RequiresPrescription =
                request.RequiresPrescription;
            product.IsControlledDrug = request.IsControlledDrug;
            product.TrackBatch = request.TrackBatch;
            product.TrackExpiry = request.TrackExpiry;
            product.AllowDiscount = request.AllowDiscount;
            product.AllowNegativeStock =
                request.AllowNegativeStock;
            product.RegulatoryNumber =
                NormalizeOptional(request.RegulatoryNumber);
            product.IsActive = request.IsActive;

            ClearMedicineFieldsWhenNotMedicine(product);

            _repository.Update(product);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var companyId = _companyContext.CompanyId
        ?? throw new UnauthorizedAccessException(
            "Company context is missing.");
            var product = await _repository.GetByIdAsync(
                id,
                companyId,
                false,
                cancellationToken)
                ?? throw new KeyNotFoundException(
                    "Product not found.");

            product.IsActive = false;

            _repository.Update(product);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        public Task<List<ProductLookupDto>> GetCategoriesAsync(
            CancellationToken cancellationToken = default)
        {
            return GetLookupAsync<Category>(
                x => x.Name,
                cancellationToken);
        }

        public Task<List<ProductLookupDto>> GetBrandsAsync(
            CancellationToken cancellationToken = default)
        {
            return GetLookupAsync<ProductBrand>(
                x => x.Name,
                cancellationToken);
        }

        public Task<List<ProductLookupDto>> GetManufacturersAsync(
            CancellationToken cancellationToken = default)
        {
            return GetLookupAsync<Manufacturer>(
                x => x.Name,
                cancellationToken);
        }

        public Task<List<ProductLookupDto>> GetGenericMedicinesAsync(
            CancellationToken cancellationToken = default)
        {
            return GetLookupAsync<GenericMedicine>(
                x => x.Name,
                cancellationToken);
        }

        public Task<List<ProductLookupDto>> GetDosageFormsAsync(
            CancellationToken cancellationToken = default)
        {
            return GetLookupAsync<DosageForm>(
                x => x.Name,
                cancellationToken);
        }

        public Task<List<ProductLookupDto>> GetStrengthsAsync(
            CancellationToken cancellationToken = default)
        {
            return GetLookupAsync<Strength>(
                x => x.Name,
                cancellationToken);
        }

        public Task<List<ProductLookupDto>> GetUnitsAsync(
            CancellationToken cancellationToken = default)
        {
            return GetLookupAsync<Unit>(
                x => $"{x.Name} ({x.Symbol})",
                cancellationToken);
        }

        private async Task<List<ProductLookupDto>> GetLookupAsync<TEntity>(
     Func<TEntity, string> nameSelector,
     CancellationToken cancellationToken)
     where TEntity : CompanyEntity
        {
            var companyId = _companyContext.CompanyId
                ?? throw new UnauthorizedAccessException(
                    "Company context is missing.");

            var records =
                await _repository.GetActiveLookupsAsync<TEntity>(
                    companyId,
                    cancellationToken);

            return records
                .Select(entity => new ProductLookupDto
                {
                    Id = entity.Id,
                    Name = nameSelector(entity)
                })
                .OrderBy(dto => dto.Name)
                .ToList();
        }

        private async Task ValidateLookupsAsync(
            ProductFormDto request,
            Guid companyId,
            CancellationToken cancellationToken)
        {
            await ValidateRequiredAsync<Category>(
                request.CategoryId,
                companyId,
                "The selected category is invalid.",
                cancellationToken);

            await ValidateRequiredAsync<Unit>(
                request.BaseUnitId,
                companyId,
                "The selected base unit is invalid.",
                cancellationToken);

            await ValidateOptionalAsync<ProductBrand>(
                request.ProductBrandId,
                companyId,
                "The selected product brand is invalid.",
                cancellationToken);

            await ValidateOptionalAsync<Manufacturer>(
                request.ManufacturerId,
                companyId,
                "The selected manufacturer is invalid.",
                cancellationToken);

            await ValidateOptionalAsync<GenericMedicine>(
                request.GenericMedicineId,
                companyId,
                "The selected generic medicine is invalid.",
                cancellationToken);

            await ValidateOptionalAsync<DosageForm>(
                request.DosageFormId,
                companyId,
                "The selected dosage form is invalid.",
                cancellationToken);

            await ValidateOptionalAsync<Strength>(
                request.StrengthId,
                companyId,
                "The selected strength is invalid.",
                cancellationToken);
        }

        private async Task ValidateRequiredAsync<TEntity>(
            Guid? id,
            Guid companyId,
            string errorMessage,
            CancellationToken cancellationToken)
        where TEntity : CompanyEntity
        {
            if (!id.HasValue ||
                id.Value == Guid.Empty ||
                !await _repository.RelatedEntityExistsAsync<TEntity>(
                    id.Value,
                    companyId,
                    cancellationToken))
            {
                throw new InvalidOperationException(errorMessage);
            }
        }

        private async Task ValidateOptionalAsync<TEntity>(
            Guid? id,
            Guid companyId,
            string errorMessage,
            CancellationToken cancellationToken)
            where TEntity : CompanyEntity
        {
            if (!id.HasValue)
                return;

            await ValidateRequiredAsync<TEntity>(
                id.Value,
                companyId,
                errorMessage,
                cancellationToken);
        }

        private static void NormalizeProductType(
            ProductFormDto request)
        {
            request.IsMedicine =
                request.ProductType == ProductType.Medicine;
        }

        private static void ValidateMedicalFields(
            ProductFormDto request)
        {
            if (!request.IsMedicine &&
                (request.RequiresPrescription ||
                 request.IsControlledDrug))
            {
                throw new InvalidOperationException(
                    "A non-medicine product cannot require a " +
                    "prescription or be a controlled drug.");
            }

            if (request.IsControlledDrug &&
                !request.RequiresPrescription)
            {
                throw new InvalidOperationException(
                    "A controlled drug must require a prescription.");
            }
        }

        private static void ClearMedicineFieldsWhenNotMedicine(
            Product product)
        {
            if (product.IsMedicine)
                return;

            product.GenericMedicineId = null;
            product.DosageFormId = null;
            product.StrengthId = null;
            product.RequiresPrescription = false;
            product.IsControlledDrug = false;
            product.RegulatoryNumber = null;
        }

        private static string NormalizeCode(string code)
        {
            return code.Trim().ToUpperInvariant();
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}
