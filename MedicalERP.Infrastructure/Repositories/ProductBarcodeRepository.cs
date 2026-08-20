using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class ProductBarcodeRepository : IProductBarcodeRepository
{
    private readonly ApplicationDbContext _context;

    public ProductBarcodeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<ProductBarcode>> GetAsync(
        Guid companyId,
        Guid? productId,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(companyId, productId, search);

        return await query
            .OrderBy(x => x.Product.Name)
            .ThenByDescending(x => x.IsPrimary)
            .ThenBy(x => x.Barcode)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(
        Guid companyId,
        Guid? productId,
        string? search,
        CancellationToken cancellationToken = default)
    {
        return BuildQuery(companyId, productId, search)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProductBarcode>> GetPagedAsync(
        Guid companyId,
        Guid? productId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(companyId, productId, search);

        return await query
            .OrderBy(x => x.Product.Name)
            .ThenByDescending(x => x.IsPrimary)
            .ThenBy(x => x.Barcode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<ProductBarcode> BuildQuery(
        Guid companyId,
        Guid? productId,
        string? search)
    {
        var query = _context.ProductBarcodes
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.ProductUnit)
                .ThenInclude(x => x!.Unit)
            .Where(x => x.CompanyId == companyId);

        if (productId.HasValue)
        {
            query = query.Where(x => x.ProductId == productId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();

            query = query.Where(x =>
                x.Barcode.Contains(value) ||
                x.Product.Name.Contains(value) ||
                x.Product.Code.Contains(value));
        }

        return query;
    }

    public Task<ProductBarcode?> GetByIdAsync(
        Guid id,
        Guid companyId,
        bool tracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ProductBarcode> query = _context.ProductBarcodes
            .Include(x => x.Product)
            .Include(x => x.ProductUnit)
                .ThenInclude(x => x!.Unit);

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(
            x => x.Id == id && x.CompanyId == companyId,
            cancellationToken);
    }

    public Task<bool> BarcodeExistsAsync(
        string barcode,
        Guid companyId,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        return _context.ProductBarcodes.AnyAsync(
            x => x.CompanyId == companyId &&
                 x.Barcode == barcode &&
                 (!excludedId.HasValue || x.Id != excludedId.Value),
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProductBarcode>> GetPrimaryBarcodesAsync(
        Guid productId,
        Guid companyId,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProductBarcodes
            .Where(x => x.CompanyId == companyId &&
                        x.ProductId == productId &&
                        x.IsPrimary &&
                        (!excludedId.HasValue || x.Id != excludedId.Value))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        ProductBarcode barcode,
        CancellationToken cancellationToken = default)
    {
        await _context.ProductBarcodes.AddAsync(barcode, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
