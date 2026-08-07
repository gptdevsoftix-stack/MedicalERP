using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Inventory;
using MedicalERP.Domain.Purchases;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class PurchaseOrderRepository(ApplicationDbContext context) : IPurchaseOrderRepository
{
    private IQueryable<PurchaseOrder> Query(Guid companyId, Guid storeId)
    {
        return context.PurchaseOrders
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId)
            .Include(x => x.Supplier)
            .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units).ThenInclude(x => x.Unit)
            .Include(x => x.GoodsReceipts);
    }

    public Task<int> CountAsync(Guid companyId, Guid storeId, string? search, int? status, CancellationToken cancellationToken = default)
    {
        return ApplyFilters(context.PurchaseOrders.Where(x => x.CompanyId == companyId && x.StoreId == storeId), search, status).CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PurchaseOrder>> GetAsync(Guid companyId, Guid storeId, string? search, int? status, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(Query(companyId, storeId), search, status)
            .OrderByDescending(x => x.OrderDate).ThenByDescending(x => x.OrderNumber)
            .Skip(skip).Take(take).AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<PurchaseOrder?> GetByIdAsync(Guid id, Guid companyId, Guid storeId, bool tracking, CancellationToken cancellationToken = default)
    {
        IQueryable<PurchaseOrder> query = Query(companyId, storeId);
        if (!tracking) query = query.AsNoTracking();
        return query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> OrderNumberExistsAsync(Guid companyId, Guid storeId, string orderNumber, Guid? excludedId, CancellationToken cancellationToken = default)
    {
        return context.PurchaseOrders.AnyAsync(x => x.CompanyId == companyId && x.StoreId == storeId && x.OrderNumber == orderNumber && (!excludedId.HasValue || x.Id != excludedId.Value), cancellationToken);
    }

    public void RemoveItems(IEnumerable<PurchaseOrderItem> items) => context.PurchaseOrderItems.RemoveRange(items);

    public async Task<IReadOnlyList<Supplier>> GetSuppliersAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default)
    {
        return await context.Suppliers.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && context.SupplierStores.Any(ss => ss.SupplierId == x.Id && ss.CompanyId == companyId && ss.StoreId == storeId && ss.IsActive))
            .OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await context.Products.AsNoTracking().Where(x => x.CompanyId == companyId && x.IsActive).OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductUnit>> GetProductUnitsAsync(Guid productId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return await context.ProductUnits.AsNoTracking().Include(x => x.Unit)
            .Where(x => x.CompanyId == companyId && x.ProductId == productId && x.IsActive && (x.IsPurchaseUnit || x.IsBaseUnit))
            .OrderByDescending(x => x.IsBaseUnit).ThenBy(x => x.Unit.Name).ToListAsync(cancellationToken);
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

    public async Task AddAsync(PurchaseOrder order, CancellationToken cancellationToken = default) => await context.PurchaseOrders.AddAsync(order, cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);

    private static IQueryable<PurchaseOrder> ApplyFilters(IQueryable<PurchaseOrder> query, string? search, int? status)
    {
        if (status.HasValue) query = query.Where(x => (int)x.Status == status.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();
            query = query.Where(x => x.OrderNumber.Contains(value) || x.Supplier.Name.Contains(value));
        }
        return query;
    }
}
