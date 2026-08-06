using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class StoreProductRepository : IStoreProductRepository
{
    private readonly ApplicationDbContext _context;

    public StoreProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<StoreProduct>> GetAsync(
        Guid companyId,
        Guid? storeId,
        Guid? productId,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _context.StoreProducts
            .AsNoTracking()
            .Include(x => x.Store)
            .Include(x => x.Product)
            .Where(x => x.CompanyId == companyId);

        if (storeId.HasValue)
        {
            query = query.Where(x => x.StoreId == storeId.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(x => x.ProductId == productId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();

            query = query.Where(x =>
                x.Product.Name.Contains(value) ||
                x.Product.Code.Contains(value) ||
                (x.Store != null && x.Store.Name.Contains(value)));
        }

        return await query
            .OrderBy(x => x.Store == null ? string.Empty : x.Store.Name)
            .ThenBy(x => x.Product.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<StoreProduct?> GetByIdAsync(
        Guid id,
        Guid companyId,
        bool tracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<StoreProduct> query = _context.StoreProducts
            .Include(x => x.Store)
            .Include(x => x.Product);

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(
            x => x.Id == id && x.CompanyId == companyId,
            cancellationToken);
    }

    public Task<bool> ExistsAsync(
        Guid storeId,
        Guid productId,
        Guid companyId,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        return _context.StoreProducts.AnyAsync(
            x => x.CompanyId == companyId &&
                 x.StoreId == storeId &&
                 x.ProductId == productId &&
                 (!excludedId.HasValue || x.Id != excludedId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        StoreProduct storeProduct,
        CancellationToken cancellationToken = default)
    {
        await _context.StoreProducts.AddAsync(storeProduct, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
