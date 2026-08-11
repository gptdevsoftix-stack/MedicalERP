using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Inventory;
using MedicalERP.Domain.Sales;
using MedicalERP.Domain.Support;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class SaleReturnRepository(ApplicationDbContext context) : ISaleReturnRepository
{
    private IQueryable<SaleReturn> Query(Guid companyId, Guid storeId)
    {
        return context.SaleReturns
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId)
            .Include(x => x.Sale)
            .Include(x => x.Items).ThenInclude(x => x.Product)
            .Include(x => x.Items).ThenInclude(x => x.ProductBatch);
    }

    public Task<int> CountAsync(Guid companyId, Guid storeId, string? search, int? status, CancellationToken cancellationToken = default)
    {
        return ApplyFilters(context.SaleReturns.Where(x => x.CompanyId == companyId && x.StoreId == storeId), search, status).CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SaleReturn>> GetAsync(Guid companyId, Guid storeId, string? search, int? status, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(Query(companyId, storeId), search, status)
            .OrderByDescending(x => x.ReturnDate).ThenByDescending(x => x.ReturnNumber)
            .Skip(skip).Take(take).AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<SaleReturn?> GetByIdAsync(Guid id, Guid companyId, Guid storeId, bool tracking, CancellationToken cancellationToken = default)
    {
        IQueryable<SaleReturn> query = Query(companyId, storeId);
        if (!tracking) query = query.AsNoTracking();
        return query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Sale?> GetSaleForReturnAsync(Guid saleId, Guid companyId, Guid storeId, bool tracking, CancellationToken cancellationToken = default)
    {
        IQueryable<Sale> query = context.Sales
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId)
            .Include(x => x.Items).ThenInclude(x => x.Product)
            .Include(x => x.Items).ThenInclude(x => x.Batches).ThenInclude(x => x.ProductBatch);
        if (!tracking) query = query.AsNoTracking();
        return query.SingleOrDefaultAsync(x => x.Id == saleId, cancellationToken);
    }

    public Task<bool> ReturnNumberExistsAsync(Guid companyId, Guid storeId, string returnNumber, Guid? excludedId, CancellationToken cancellationToken = default)
    {
        return context.SaleReturns.AnyAsync(x => x.CompanyId == companyId && x.StoreId == storeId && x.ReturnNumber == returnNumber && (!excludedId.HasValue || x.Id != excludedId.Value), cancellationToken);
    }

    public async Task<IReadOnlyList<Warehouse>> GetWarehousesAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default)
    {
        return await context.Warehouses.AsNoTracking().Where(x => x.CompanyId == companyId && x.StoreId == storeId && x.IsActive).OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public Task<InventoryStock?> GetInventoryStockAsync(Guid companyId, Guid storeId, Guid productId, Guid? warehouseId, Guid? productBatchId, CancellationToken cancellationToken = default)
    {
        return context.InventoryStocks.SingleOrDefaultAsync(
            x => x.CompanyId == companyId &&
                 x.StoreId == storeId &&
                 x.ProductId == productId &&
                 x.WarehouseId == warehouseId &&
                 x.ProductBatchId == productBatchId,
            cancellationToken);
    }

    public async Task AddInventoryStockAsync(InventoryStock stock, CancellationToken cancellationToken = default) => await context.InventoryStocks.AddAsync(stock, cancellationToken);

    public async Task AddStockTransactionAsync(StockTransaction transaction, CancellationToken cancellationToken = default) => await context.StockTransactions.AddAsync(transaction, cancellationToken);

    public async Task AddAsync(SaleReturn saleReturn, CancellationToken cancellationToken = default) => await context.SaleReturns.AddAsync(saleReturn, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);

    private static IQueryable<SaleReturn> ApplyFilters(IQueryable<SaleReturn> query, string? search, int? status)
    {
        if (status.HasValue) query = query.Where(x => (int)x.Status == status.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();
            query = query.Where(x => x.ReturnNumber.Contains(value) || (x.Sale != null && x.Sale.InvoiceNumber.Contains(value)));
        }
        return query;
    }
}
