using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Interfaces;
using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.DTOs;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalERP.Application.Services
{
    public sealed class ProductUnitService : IProductUnitService
    {
        private readonly IProductUnitRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly ICatalogMasterRepository _catalogMasterRepository;
        private readonly ICompanyContext _companyContext;

        public ProductUnitService(
            IProductUnitRepository repository,
            IProductRepository productRepository,
            ICatalogMasterRepository catalogMasterRepository,
            ICompanyContext companyContext)
        {
            _repository = repository;
            _productRepository = productRepository;
            _catalogMasterRepository = catalogMasterRepository;
            _companyContext = companyContext;
        }

        public async Task<IReadOnlyCollection<ProductUnitListDto>>
            GetByProductIdAsync(
                Guid productId,
                CancellationToken cancellationToken = default)
        {
            var companyId = GetRequiredCompanyId();

            await EnsureProductExistsAsync(
                productId,
                companyId,
                cancellationToken);

            var records = await _repository.GetByProductIdAsync(
                productId,
                companyId,
                false,
                cancellationToken);

            return records
                .Select(x => new ProductUnitListDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    UnitName = x.Unit.Name,
                    UnitSymbol = x.Unit.Symbol,
                    ConversionFactor = x.ConversionFactor,
                    IsBaseUnit = x.IsBaseUnit,
                    IsPurchaseUnit = x.IsPurchaseUnit,
                    IsSaleUnit = x.IsSaleUnit,
                    IsActive = x.IsActive
                })
                .ToList();
        }

        public async Task<ProductUnitFormDto?> GetFormByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var record = await _repository.GetByIdAsync(
                id,
                GetRequiredCompanyId(),
                false,
                cancellationToken);

            if (record is null)
            {
                return null;
            }

            return new ProductUnitFormDto
            {
                Id = record.Id,
                ProductId = record.ProductId,
                UnitId = record.UnitId,
                ConversionFactor = record.ConversionFactor,
                IsBaseUnit = record.IsBaseUnit,
                IsPurchaseUnit = record.IsPurchaseUnit,
                IsSaleUnit = record.IsSaleUnit,
                IsActive = record.IsActive
            };
        }

        public async Task<Guid> CreateAsync(
            ProductUnitFormDto request,
            CancellationToken cancellationToken = default)
        {
            var companyId = GetRequiredCompanyId();

            await ValidateAsync(
                request,
                companyId,
                null,
                cancellationToken);

            var productUnit = new ProductUnit
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                ProductId = request.ProductId,
                UnitId = request.UnitId,
                ConversionFactor = request.IsBaseUnit
                    ? 1
                    : request.ConversionFactor,
                IsBaseUnit = request.IsBaseUnit,
                IsPurchaseUnit = request.IsPurchaseUnit,
                IsSaleUnit = request.IsSaleUnit,
                IsActive = true
            };

            await _repository.AddAsync(
                productUnit,
                cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);

            return productUnit.Id;
        }

        public async Task UpdateAsync(
            ProductUnitFormDto request,
            CancellationToken cancellationToken = default)
        {
            var companyId = GetRequiredCompanyId();

            var productUnit = await _repository.GetByIdAsync(
                request.Id,
                companyId,
                true,
                cancellationToken)
                ?? throw new KeyNotFoundException(
                    "Product unit was not found.");

            if (productUnit.ProductId != request.ProductId)
            {
                throw new InvalidOperationException(
                    "The product of an existing product unit cannot be changed.");
            }

            await ValidateAsync(
                request,
                companyId,
                request.Id,
                cancellationToken);

            productUnit.UnitId = request.UnitId;
            productUnit.ConversionFactor = request.IsBaseUnit
                ? 1
                : request.ConversionFactor;

            productUnit.IsBaseUnit = request.IsBaseUnit;
            productUnit.IsPurchaseUnit = request.IsPurchaseUnit;
            productUnit.IsSaleUnit = request.IsSaleUnit;
            productUnit.IsActive = request.IsActive;

            await _repository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var productUnit = await _repository.GetByIdAsync(
                id,
                GetRequiredCompanyId(),
                true,
                cancellationToken)
                ?? throw new KeyNotFoundException(
                    "Product unit was not found.");

            if (productUnit.IsBaseUnit)
            {
                throw new InvalidOperationException(
                    "The base unit cannot be deleted.");
            }

            _repository.Remove(productUnit);

            await _repository.SaveChangesAsync(cancellationToken);
        }

        private async Task ValidateAsync(
            ProductUnitFormDto request,
            Guid companyId,
            Guid? excludedId,
            CancellationToken cancellationToken)
        {
            await EnsureProductExistsAsync(
                request.ProductId,
                companyId,
                cancellationToken);

            var unit = await _catalogMasterRepository.GetByIdAsync(
                CatalogMasterType.Unit,
                request.UnitId,
                companyId,
                cancellationToken);

            if (unit is null || !unit.IsActive)
            {
                throw new KeyNotFoundException("Unit was not found.");
            }

            if (request.ConversionFactor <= 0)
            {
                throw new InvalidOperationException(
                    "Conversion factor must be greater than zero.");
            }

            var duplicateExists = await _repository.ExistsAsync(
                request.ProductId,
                request.UnitId,
                companyId,
                excludedId,
                cancellationToken);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "This unit is already assigned to the product.");
            }

            if (request.IsBaseUnit)
            {
                var existingBaseUnit =
                    await _repository.GetBaseUnitAsync(
                        request.ProductId,
                        companyId,
                        excludedId,
                        cancellationToken);

                if (existingBaseUnit is not null)
                {
                    throw new InvalidOperationException(
                        "This product already has a base unit.");
                }
            }
        }

        private async Task EnsureProductExistsAsync(
            Guid productId,
            Guid companyId,
            CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(
                productId,
                companyId,
                false,
                cancellationToken);

            if (product is null)
            {
                throw new KeyNotFoundException(
                "Product was not found.");
            }
        }

        private Guid GetRequiredCompanyId()
        {
            return _companyContext.CompanyId
                ?? throw new UnauthorizedAccessException(
                    "Company context is missing.");
        }
    }
}
