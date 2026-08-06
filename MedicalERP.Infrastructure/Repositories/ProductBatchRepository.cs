using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Inventory;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class ProductBatchRepository : IProductBatchRepository
{
    private readonly ApplicationDbContext _context;

    public ProductBatchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<ProductBatch> Query()
    {
        return _context.ProductBatches
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Warehouse);
    }

    public async Task<IReadOnlyList<ProductBatch>> GetAsync(
        Guid companyId,
        Guid storeId,
        Guid? productId,
        Guid? warehouseId,
        string? search,
        bool? isActive,
        DateOnly? expiringBefore,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(
                Query().Where(x => x.CompanyId == companyId && x.StoreId == storeId),
                productId,
                warehouseId,
                search,
                isActive,
                expiringBefore)
            .OrderBy(x => x.ExpiryDate == null)
            .ThenBy(x => x.ExpiryDate)
            .ThenBy(x => x.Product.Name)
            .ThenBy(x => x.BatchNumber)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(
        Guid companyId,
        Guid storeId,
        Guid? productId,
        Guid? warehouseId,
        string? search,
        bool? isActive,
        DateOnly? expiringBefore,
        CancellationToken cancellationToken = default)
    {
        return ApplyFilters(
                _context.ProductBatches.Where(x => x.CompanyId == companyId && x.StoreId == storeId),
                productId,
                warehouseId,
                search,
                isActive,
                expiringBefore)
            .CountAsync(cancellationToken);
    }

    public Task<ProductBatch?> GetByIdAsync(
        Guid id,
        Guid companyId,
        Guid storeId,
        bool tracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ProductBatch> query = _context.ProductBatches
            .Include(x => x.Product)
            .Include(x => x.Warehouse);

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(
            x => x.Id == id &&
                 x.CompanyId == companyId &&
                 x.StoreId == storeId,
            cancellationToken);
    }

    public Task<bool> ExistsAsync(
        Guid companyId,
        Guid storeId,
        Guid productId,
        Guid? warehouseId,
        string batchNumber,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = batchNumber.Trim();

        return _context.ProductBatches.AnyAsync(
            x => x.CompanyId == companyId &&
                 x.StoreId == storeId &&
                 x.ProductId == productId &&
                 x.WarehouseId == warehouseId &&
                 x.BatchNumber == normalized &&
                 (!excludedId.HasValue || x.Id != excludedId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        ProductBatch productBatch,
        CancellationToken cancellationToken = default)
    {
        await _context.ProductBatches.AddAsync(productBatch, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<ProductBatch> ApplyFilters(
        IQueryable<ProductBatch> query,
        Guid? productId,
        Guid? warehouseId,
        string? search,
        bool? isActive,
        DateOnly? expiringBefore)
    {
        if (productId.HasValue)
        {
            query = query.Where(x => x.ProductId == productId.Value);
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(x => x.WarehouseId == warehouseId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        if (expiringBefore.HasValue)
        {
            query = query.Where(x => x.ExpiryDate != null && x.ExpiryDate <= expiringBefore.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();
            query = query.Where(x =>
                x.BatchNumber.Contains(value) ||
                x.Product.Name.Contains(value) ||
                x.Product.Code.Contains(value) ||
                (x.Warehouse != null && x.Warehouse.Name.Contains(value)) ||
                (x.Warehouse != null && x.Warehouse.Code.Contains(value)));
        }

        return query;
    }
}
