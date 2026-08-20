using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.DTOs;
using MedicalERP.Domain.Interfaces;

namespace MedicalERP.Application.Services;

public sealed class ProductBarcodeService : IProductBarcodeService
{
    private readonly IProductBarcodeRepository _repository;
    private readonly IProductRepository _productRepository;
    private readonly IProductUnitRepository _productUnitRepository;
    private readonly ICompanyContext _companyContext;

    public ProductBarcodeService(
        IProductBarcodeRepository repository,
        IProductRepository productRepository,
        IProductUnitRepository productUnitRepository,
        ICompanyContext companyContext)
    {
        _repository = repository;
        _productRepository = productRepository;
        _productUnitRepository = productUnitRepository;
        _companyContext = companyContext;
    }

    public async Task<IReadOnlyCollection<ProductBarcodeListDto>> GetAsync(
        Guid? productId,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();
        var records = await _repository.GetAsync(
            companyId,
            productId,
            search,
            cancellationToken);

        return records.Select(MapList).ToList();
    }

    public async Task<PagedResult<ProductBarcodeListDto>> GetPagedAsync(
        Guid? productId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();

        var totalCount = await _repository.CountAsync(
            companyId, productId, search, cancellationToken);

        var records = await _repository.GetPagedAsync(
            companyId, productId, search, page, pageSize, cancellationToken);

        var items = records.Select(MapList).ToList();

        return new PagedResult<ProductBarcodeListDto>(items, page, pageSize, totalCount);
    }

    public async Task<ProductBarcodeFormDto?> GetFormByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record = await _repository.GetByIdAsync(
            id,
            GetRequiredCompanyId(),
            false,
            cancellationToken);

        return record is null ? null : new ProductBarcodeFormDto
        {
            Id = record.Id,
            ProductId = record.ProductId,
            ProductUnitId = record.ProductUnitId,
            Barcode = record.Barcode,
            IsPrimary = record.IsPrimary,
            IsActive = record.IsActive
        };
    }

    public async Task<Guid> CreateAsync(
        ProductBarcodeFormDto request,
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();
        var barcodeValue = NormalizeBarcode(request.Barcode);

        await ValidateAsync(
            request,
            companyId,
            barcodeValue,
            null,
            cancellationToken);

        var barcode = new ProductBarcode
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            ProductId = request.ProductId,
            ProductUnitId = request.ProductUnitId,
            Barcode = barcodeValue,
            IsPrimary = request.IsPrimary,
            IsActive = true
        };

        await ClearOtherPrimaryAsync(
            barcode.ProductId,
            companyId,
            null,
            barcode.IsPrimary,
            cancellationToken);

        await _repository.AddAsync(barcode, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return barcode.Id;
    }

    public async Task UpdateAsync(
        ProductBarcodeFormDto request,
        CancellationToken cancellationToken = default)
    {
        var companyId = GetRequiredCompanyId();
        var record = await _repository.GetByIdAsync(
            request.Id,
            companyId,
            true,
            cancellationToken)
            ?? throw new KeyNotFoundException("Barcode was not found.");

        var barcodeValue = NormalizeBarcode(request.Barcode);

        await ValidateAsync(
            request,
            companyId,
            barcodeValue,
            request.Id,
            cancellationToken);

        await ClearOtherPrimaryAsync(
            request.ProductId,
            companyId,
            request.Id,
            request.IsPrimary,
            cancellationToken);

        record.ProductId = request.ProductId;
        record.ProductUnitId = request.ProductUnitId;
        record.Barcode = barcodeValue;
        record.IsPrimary = request.IsPrimary;
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
            ?? throw new KeyNotFoundException("Barcode was not found.");

        record.IsActive = false;
        record.IsPrimary = false;

        await _repository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateAsync(
        ProductBarcodeFormDto request,
        Guid companyId,
        string barcodeValue,
        Guid? excludedId,
        CancellationToken cancellationToken)
    {
        if (request.ProductId == Guid.Empty)
        {
            throw new InvalidOperationException("Product is required.");
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

        if (await _repository.BarcodeExistsAsync(
                barcodeValue,
                companyId,
                excludedId,
                cancellationToken))
        {
            throw new InvalidOperationException("This barcode already exists.");
        }

        if (!request.ProductUnitId.HasValue)
        {
            return;
        }

        var productUnit = await _productUnitRepository.GetByIdAsync(
            request.ProductUnitId.Value,
            companyId,
            false,
            cancellationToken);

        if (productUnit is null ||
            productUnit.ProductId != request.ProductId ||
            !productUnit.IsActive)
        {
            throw new InvalidOperationException(
                "The selected product unit is invalid.");
        }
    }

    private async Task ClearOtherPrimaryAsync(
        Guid productId,
        Guid companyId,
        Guid? excludedId,
        bool shouldClear,
        CancellationToken cancellationToken)
    {
        if (!shouldClear)
        {
            return;
        }

        var primaryBarcodes = await _repository.GetPrimaryBarcodesAsync(
            productId,
            companyId,
            excludedId,
            cancellationToken);

        foreach (var barcode in primaryBarcodes)
        {
            barcode.IsPrimary = false;
        }
    }

    private Guid GetRequiredCompanyId()
    {
        return _companyContext.CompanyId
            ?? throw new UnauthorizedAccessException(
                "Company context is missing.");
    }

    private static ProductBarcodeListDto MapList(ProductBarcode record)
    {
        return new ProductBarcodeListDto
        {
            Id = record.Id,
            ProductId = record.ProductId,
            ProductName = record.Product.Name,
            ProductCode = record.Product.Code,
            ProductUnitId = record.ProductUnitId,
            ProductUnitName = record.ProductUnit is null
                ? null
                : $"{record.ProductUnit.Unit.Name} ({record.ProductUnit.ConversionFactor:0.####})",
            Barcode = record.Barcode,
            IsPrimary = record.IsPrimary,
            IsActive = record.IsActive
        };
    }

    private static string NormalizeBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            throw new InvalidOperationException("Barcode is required.");
        }

        return barcode.Trim();
    }
}
