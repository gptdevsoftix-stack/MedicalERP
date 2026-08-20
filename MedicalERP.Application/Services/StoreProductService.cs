using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.DTOs;
using MedicalERP.Domain.Interfaces;

namespace MedicalERP.Application.Services;

public sealed class StoreProductService : IStoreProductService
{
    private readonly IStoreProductRepository _repository;
    private readonly IProductRepository _productRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ICompanyContext _companyContext;

    public StoreProductService(
        IStoreProductRepository repository,
        IProductRepository productRepository,
        IStoreRepository storeRepository,
        ICompanyContext companyContext)
    {
        _repository = repository;
        _productRepository = productRepository;
        _storeRepository = storeRepository;
        _companyContext = companyContext;
    }

    public async Task<IReadOnlyCollection<StoreProductListDto>> GetAsync(
        Guid? storeId,
        Guid? productId,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();
        var records = await _repository.GetAsync(
            companyId,
            storeId,
            productId,
            search,
            cancellationToken);

        return records.Select(MapList).ToList();
    }

    public async Task<PagedResult<StoreProductListDto>> GetPagedAsync(
        Guid? storeId,
        Guid? productId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();

        var totalCount = await _repository.CountAsync(
            companyId, storeId, productId, search, cancellationToken);

        var records = await _repository.GetPagedAsync(
            companyId, storeId, productId, search, page, pageSize, cancellationToken);

        var items = records.Select(MapList).ToList();

        return new PagedResult<StoreProductListDto>(items, page, pageSize, totalCount);
    }

    public async Task<StoreProductFormDto?> GetFormByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record = await _repository.GetByIdAsync(
            id,
            GetRequiredCompanyId(),
            false,
            cancellationToken);

        return record is null ? null : new StoreProductFormDto
        {
            Id = record.Id,
            StoreId = record.StoreId,
            ProductId = record.ProductId,
            SalePrice = record.SalePrice,
            WholesalePrice = record.WholesalePrice,
            MinimumSalePrice = record.MinimumSalePrice,
            ReorderLevel = record.ReorderLevel,
            ReorderQuantity = record.ReorderQuantity,
            IsAvailableForSale = record.IsAvailableForSale,
            IsActive = record.IsActive
        };
    }

    public async Task<Guid> CreateAsync(
        StoreProductFormDto request,
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();

        await ValidateAsync(
            request,
            companyId,
            null,
            cancellationToken);

        var storeProduct = new StoreProduct
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            StoreId = request.StoreId,
            ProductId = request.ProductId,
            SalePrice = request.SalePrice,
            WholesalePrice = request.WholesalePrice,
            MinimumSalePrice = request.MinimumSalePrice,
            ReorderLevel = request.ReorderLevel,
            ReorderQuantity = request.ReorderQuantity,
            IsAvailableForSale = request.IsAvailableForSale,
            IsActive = true
        };

        await _repository.AddAsync(storeProduct, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return storeProduct.Id;
    }

    public async Task UpdateAsync(
        StoreProductFormDto request,
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();
        var record = await _repository.GetByIdAsync(
            request.Id,
            companyId,
            true,
            cancellationToken)
            ?? throw new KeyNotFoundException(
                "Store product was not found.");

        await ValidateAsync(
            request,
            companyId,
            request.Id,
            cancellationToken);

        record.StoreId = request.StoreId;
        record.ProductId = request.ProductId;
        record.SalePrice = request.SalePrice;
        record.WholesalePrice = request.WholesalePrice;
        record.MinimumSalePrice = request.MinimumSalePrice;
        record.ReorderLevel = request.ReorderLevel;
        record.ReorderQuantity = request.ReorderQuantity;
        record.IsAvailableForSale = request.IsAvailableForSale;
        record.IsActive = request.IsActive;

        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record = await _repository.GetByIdAsync(
            id,
            GetRequiredCompanyId(),
            true,
            cancellationToken)
            ?? throw new KeyNotFoundException(
                "Store product was not found.");

        record.IsActive = false;
        record.IsAvailableForSale = false;

        await _repository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateAsync(
        StoreProductFormDto request,
        Guid companyId,
        Guid? excludedId,
        CancellationToken cancellationToken)
    {
        if (request.StoreId == Guid.Empty)
        {
            throw new InvalidOperationException("Store is required.");
        }

        if (request.ProductId == Guid.Empty)
        {
            throw new InvalidOperationException("Product is required.");
        }

        var store = await _storeRepository.GetByIdAsync(
            request.StoreId,
            cancellationToken);

        if (store is null ||
            store.CompanyId != companyId ||
            !store.IsActive)
        {
            throw new KeyNotFoundException("Store was not found.");
        }

        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            companyId,
            false,
            cancellationToken);

        if (product is null || !product.IsActive)
        {
            throw new KeyNotFoundException("Product was not found.");
        }

        if (request.MinimumSalePrice.HasValue &&
            request.MinimumSalePrice.Value > request.SalePrice)
        {
            throw new InvalidOperationException(
                "Minimum sale price cannot be greater than sale price.");
        }

        if (request.WholesalePrice.HasValue &&
            request.WholesalePrice.Value < 0)
        {
            throw new InvalidOperationException(
                "Wholesale price cannot be negative.");
        }

        if (request.ReorderLevel < 0 || request.ReorderQuantity < 0)
        {
            throw new InvalidOperationException(
                "Reorder values cannot be negative.");
        }

        if (await _repository.ExistsAsync(
                request.StoreId,
                request.ProductId,
                companyId,
                excludedId,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "This product is already configured for the selected store.");
        }
    }

    private Guid GetRequiredCompanyId()
    {
        return _companyContext.CompanyId
            ?? throw new UnauthorizedAccessException(
                "Company context is missing.");
    }

    private static StoreProductListDto MapList(StoreProduct record)
    {
        return new StoreProductListDto
        {
            Id = record.Id,
            StoreId = record.StoreId,
            StoreName = record.Store?.Name ?? string.Empty,
            ProductId = record.ProductId,
            ProductName = record.Product.Name,
            ProductCode = record.Product.Code,
            SalePrice = record.SalePrice,
            WholesalePrice = record.WholesalePrice,
            MinimumSalePrice = record.MinimumSalePrice,
            ReorderLevel = record.ReorderLevel,
            ReorderQuantity = record.ReorderQuantity,
            IsAvailableForSale = record.IsAvailableForSale,
            IsActive = record.IsActive
        };
    }
}
