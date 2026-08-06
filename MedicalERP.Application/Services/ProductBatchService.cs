using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Inventory.Dtos;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Inventory;

namespace MedicalERP.Application.Services;

public sealed class ProductBatchService : IProductBatchService
{
    private readonly IProductBatchRepository _repository;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICompanyContext _companyContext;
    private readonly IStoreContext _storeContext;

    public ProductBatchService(
        IProductBatchRepository repository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        ICompanyContext companyContext,
        IStoreContext storeContext)
    {
        _repository = repository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _companyContext = companyContext;
        _storeContext = storeContext;
    }

    public async Task<PagedResult<ProductBatchListDto>> GetAsync(
        ProductBatchFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();
        var storeId = await GetRequiredStoreIdAsync(cancellationToken);
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 5, 100);

        var total = await _repository.CountAsync(
            companyId,
            storeId,
            filter.ProductId,
            filter.WarehouseId,
            filter.Search,
            filter.IsActive,
            filter.ExpiringBefore,
            cancellationToken);

        var records = await _repository.GetAsync(
            companyId,
            storeId,
            filter.ProductId,
            filter.WarehouseId,
            filter.Search,
            filter.IsActive,
            filter.ExpiringBefore,
            (page - 1) * pageSize,
            pageSize,
            cancellationToken);
        var items = records.Select(MapList).ToList();

        return new PagedResult<ProductBatchListDto>(items, page, pageSize, total);
    }

    public async Task<ProductBatchFormDto?> GetFormByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record = await GetRecordAsync(id, false, cancellationToken);

        return record is null ? null : new ProductBatchFormDto
        {
            Id = record.Id,
            ProductId = record.ProductId,
            WarehouseId = record.WarehouseId,
            BatchNumber = record.BatchNumber,
            ManufacturingDate = record.ManufacturingDate,
            ExpiryDate = record.ExpiryDate,
            PurchasePrice = record.PurchasePrice,
            CostPrice = record.CostPrice,
            SalePrice = record.SalePrice,
            MaximumRetailPrice = record.MaximumRetailPrice,
            ReceivedAt = record.ReceivedAt,
            IsActive = record.IsActive
        };
    }

    public async Task<ProductBatchListDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record = await GetRecordAsync(id, false, cancellationToken);
        return record is null ? null : MapList(record);
    }

    public async Task<Guid> CreateAsync(
        ProductBatchFormDto request,
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();
        var storeId = await GetRequiredStoreIdAsync(cancellationToken);

        await ValidateAsync(request, companyId, storeId, null, cancellationToken);

        var record = new ProductBatch
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            StoreId = storeId,
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            BatchNumber = request.BatchNumber.Trim(),
            ManufacturingDate = request.ManufacturingDate,
            ExpiryDate = request.ExpiryDate,
            PurchasePrice = request.PurchasePrice,
            CostPrice = request.CostPrice,
            SalePrice = request.SalePrice,
            MaximumRetailPrice = request.MaximumRetailPrice,
            ReceivedAt = request.ReceivedAt,
            IsActive = true
        };

        await _repository.AddAsync(record, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return record.Id;
    }

    public async Task UpdateAsync(
        ProductBatchFormDto request,
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();
        var storeId = await GetRequiredStoreIdAsync(cancellationToken);
        var record = await _repository.GetByIdAsync(
            request.Id,
            companyId,
            storeId,
            true,
            cancellationToken)
            ?? throw new KeyNotFoundException("Product batch was not found.");

        await ValidateAsync(request, companyId, storeId, request.Id, cancellationToken);

        record.ProductId = request.ProductId;
        record.WarehouseId = request.WarehouseId;
        record.BatchNumber = request.BatchNumber.Trim();
        record.ManufacturingDate = request.ManufacturingDate;
        record.ExpiryDate = request.ExpiryDate;
        record.PurchasePrice = request.PurchasePrice;
        record.CostPrice = request.CostPrice;
        record.SalePrice = request.SalePrice;
        record.MaximumRetailPrice = request.MaximumRetailPrice;
        record.ReceivedAt = request.ReceivedAt;
        record.IsActive = request.IsActive;

        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record = await GetRecordAsync(id, true, cancellationToken)
            ?? throw new KeyNotFoundException("Product batch was not found.");

        record.IsActive = false;

        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProductBatchLookupDto>> GetProductLookupsAsync(
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();
        var products = await _productRepository.GetAllAsync(companyId, null, null, null, true, cancellationToken);

        return products
            .Where(x => x.TrackBatch)
            .OrderBy(x => x.Name)
            .Select(x => new ProductBatchLookupDto
            {
                Id = x.Id,
                Name = $"{x.Name} ({x.Code})"
            })
            .ToArray();
    }

    public async Task<IReadOnlyCollection<ProductBatchLookupDto>> GetWarehouseLookupsAsync(
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();
        var storeId = await GetRequiredStoreIdAsync(cancellationToken);

        var warehouses = await _warehouseRepository.GetActiveByStoreAsync(companyId, storeId, cancellationToken);

        return warehouses
            .Select(x => new ProductBatchLookupDto
            {
                Id = x.Id,
                Name = $"{x.Name} ({x.Code})"
            })
            .ToArray();
    }

    private async Task<ProductBatch?> GetRecordAsync(
        Guid id,
        bool tracking,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(
            id,
            GetRequiredCompanyId(),
            await GetRequiredStoreIdAsync(cancellationToken),
            tracking,
            cancellationToken);
    }

    private async Task ValidateAsync(
        ProductBatchFormDto request,
        Guid companyId,
        Guid storeId,
        Guid? excludedId,
        CancellationToken cancellationToken)
    {
        if (request.ProductId == Guid.Empty)
        {
            throw new InvalidOperationException("Product is required.");
        }

        if (string.IsNullOrWhiteSpace(request.BatchNumber))
        {
            throw new InvalidOperationException("Batch number is required.");
        }

        if (request.ManufacturingDate.HasValue &&
            request.ExpiryDate.HasValue &&
            request.ManufacturingDate.Value > request.ExpiryDate.Value)
        {
            throw new InvalidOperationException("Manufacturing date cannot be after expiry date.");
        }

        if (request.ExpiryDate.HasValue && request.ExpiryDate.Value < DateOnly.FromDateTime(DateTime.Today))
        {
            throw new InvalidOperationException("Expired batches cannot be created or reactivated.");
        }

        if (request.MaximumRetailPrice.HasValue && request.MaximumRetailPrice.Value < request.SalePrice)
        {
            throw new InvalidOperationException("Maximum retail price cannot be less than sale price.");
        }

        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            companyId,
            true,
            cancellationToken);

        if (product is null || !product.IsActive)
        {
            throw new KeyNotFoundException("Product was not found.");
        }

        if (!product.TrackBatch)
        {
            throw new InvalidOperationException("Selected product is not configured for batch tracking.");
        }

        if ((product.IsMedicine || product.TrackExpiry) && !request.ExpiryDate.HasValue)
        {
            throw new InvalidOperationException("Expiry date is required for medical or expiry-tracked products.");
        }

        if (request.WarehouseId.HasValue)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId.Value, cancellationToken);
            if (warehouse is null ||
                warehouse.CompanyId != companyId ||
                warehouse.StoreId != storeId ||
                !warehouse.IsActive)
            {
                throw new KeyNotFoundException("Warehouse was not found for the selected store.");
            }
        }

        if (await _repository.ExistsAsync(
                companyId,
                storeId,
                request.ProductId,
                request.WarehouseId,
                request.BatchNumber,
                excludedId,
                cancellationToken))
        {
            throw new InvalidOperationException("Batch number already exists for this product and warehouse.");
        }
    }

    private Guid GetRequiredCompanyId()
    {
        return _companyContext.CompanyId
            ?? throw new UnauthorizedAccessException("Company context is missing.");
    }

    private async Task<Guid> GetRequiredStoreIdAsync(CancellationToken cancellationToken)
    {
        var storeId = _storeContext.RequireSelectedStoreId();
        await _storeContext.EnsureStoreAccessAsync(storeId, cancellationToken);
        return storeId;
    }

    private static ProductBatchListDto MapList(ProductBatch x)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        return new ProductBatchListDto
        {
            Id = x.Id,
            ProductId = x.ProductId,
            ProductName = x.Product.Name,
            ProductCode = x.Product.Code,
            WarehouseId = x.WarehouseId,
            WarehouseName = x.Warehouse?.Name,
            WarehouseCode = x.Warehouse?.Code,
            BatchNumber = x.BatchNumber,
            ManufacturingDate = x.ManufacturingDate,
            ExpiryDate = x.ExpiryDate,
            PurchasePrice = x.PurchasePrice,
            CostPrice = x.CostPrice,
            SalePrice = x.SalePrice,
            MaximumRetailPrice = x.MaximumRetailPrice,
            ReceivedAt = x.ReceivedAt,
            IsExpired = x.ExpiryDate.HasValue && x.ExpiryDate.Value < today,
            IsActive = x.IsActive
        };
    }
}
