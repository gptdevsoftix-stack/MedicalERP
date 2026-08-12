using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Inventory;
using MedicalERP.Domain.Operations;
using MedicalERP.Domain.Sales;
using MedicalERP.Domain.Support;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class SaleRepository(ApplicationDbContext context) : ISaleRepository
{
    private IQueryable<Sale> Query(Guid companyId, Guid storeId)
    {
        return context.Sales
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId)
            .Include(x => x.Customer)
            .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units).ThenInclude(x => x.Unit)
            .Include(x => x.Items).ThenInclude(x => x.Batches).ThenInclude(x => x.ProductBatch)
            .Include(x => x.Payments).ThenInclude(x => x.PaymentMethod);
    }

    public Task<int> CountAsync(Guid companyId, Guid storeId, string? search, int? status, int? paymentStatus, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken = default)
    {
        return ApplyFilters(context.Sales.Where(x => x.CompanyId == companyId && x.StoreId == storeId), search, status, paymentStatus, from, to).CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetAsync(Guid companyId, Guid storeId, string? search, int? status, int? paymentStatus, DateTimeOffset? from, DateTimeOffset? to, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(Query(companyId, storeId), search, status, paymentStatus, from, to)
            .OrderByDescending(x => x.SaleDate).ThenByDescending(x => x.InvoiceNumber)
            .Skip(skip).Take(take).AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<Sale?> GetByIdAsync(Guid id, Guid companyId, Guid storeId, bool tracking, CancellationToken cancellationToken = default)
    {
        IQueryable<Sale> query = Query(companyId, storeId);
        if (!tracking) query = query.AsNoTracking();
        return query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> IsPaidAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Sales.AnyAsync(x => x.Id == id && x.PaymentStatus == PaymentStatus.Paid, cancellationToken);
    }

    public Task<bool> InvoiceNumberExistsAsync(Guid companyId, Guid storeId, string invoiceNumber, Guid? excludedId, CancellationToken cancellationToken = default)
    {
        return context.Sales.AnyAsync(x => x.CompanyId == companyId && x.StoreId == storeId && x.InvoiceNumber == invoiceNumber && (!excludedId.HasValue || x.Id != excludedId.Value), cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> GetCustomersAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await context.Customers.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive)
            .OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SaleProductLookupData>> GetProductsAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default)
    {
        var stocks = await context.InventoryStocks.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId)
            .GroupBy(x => x.ProductId)
            .Select(g => new { ProductId = g.Key, Available = g.Sum(x => x.QuantityOnHand - x.ReservedQuantity) })
            .ToDictionaryAsync(x => x.ProductId, x => x.Available, cancellationToken);

        var storePrices = await context.StoreProducts.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId)
            .ToDictionaryAsync(x => x.ProductId, x => x.SalePrice, cancellationToken);

        var products = await context.Products.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && x.ProductType != ProductType.Service)
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name, x.AllowNegativeStock })
            .ToListAsync(cancellationToken);

        return products
            .Where(p => stocks.GetValueOrDefault(p.Id) > 0)
            .Select(p => new SaleProductLookupData(
                p.Id,
                p.Name,
                stocks.GetValueOrDefault(p.Id),
                storePrices.GetValueOrDefault(p.Id),
                p.AllowNegativeStock)).ToArray();
    }

    public async Task<IReadOnlyList<ProductUnit>> GetProductUnitsAsync(Guid productId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return await context.ProductUnits.AsNoTracking().Include(x => x.Unit)
            .Where(x => x.CompanyId == companyId && x.ProductId == productId && x.IsActive && (x.IsSaleUnit || x.IsBaseUnit))
            .OrderByDescending(x => x.IsBaseUnit).ThenBy(x => x.Unit.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Warehouse>> GetWarehousesAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default)
    {
        return await context.Warehouses.AsNoTracking().Where(x => x.CompanyId == companyId && x.StoreId == storeId && x.IsActive).OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentMethod>> GetPaymentMethodsAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await context.PaymentMethods.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive)
            .OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task AddPaymentMethodAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default) =>
        await context.PaymentMethods.AddAsync(paymentMethod, cancellationToken);

    public async Task<IReadOnlyList<RegisterSession>> GetRegisterSessionsAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default)
    {
        return await context.RegisterSessions.AsNoTracking().Include(x => x.Register)
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId && x.Status == RegisterSessionStatus.Open)
            .OrderByDescending(x => x.OpenedAt).ToListAsync(cancellationToken);
    }

    public Task<RegisterSession?> GetOpenRegisterSessionAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default)
    {
        return context.RegisterSessions.Include(x => x.Register)
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId && x.Status == RegisterSessionStatus.Open)
            .OrderByDescending(x => x.OpenedAt).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Register?> GetDefaultRegisterAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default)
    {
        return context.Registers.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId && x.IsEnabled)
            .OrderBy(x => x.Code).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddRegisterAsync(Register register, CancellationToken cancellationToken = default) => await context.Registers.AddAsync(register, cancellationToken);

    public async Task AddRegisterSessionAsync(RegisterSession session, CancellationToken cancellationToken = default) => await context.RegisterSessions.AddAsync(session, cancellationToken);

    public async Task<decimal> GetCostPriceAsync(Guid companyId, Guid storeId, Guid productId, Guid? warehouseId, CancellationToken cancellationToken = default)
    {
        return await context.ProductBatches.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId && x.ProductId == productId && x.WarehouseId == warehouseId && x.IsActive)
            .OrderBy(x => x.ExpiryDate).ThenByDescending(x => x.ReceivedAt)
            .Select(x => x.CostPrice)
            .FirstOrDefaultAsync(cancellationToken);
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

    public async Task AddSalePaymentAsync(SalePayment payment, CancellationToken cancellationToken = default) => await context.SalePayments.AddAsync(payment, cancellationToken);

    public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default) => await context.Sales.AddAsync(sale, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyConflictException("The record was modified by another user. Please reload and try again.", ex);
        }
    }

    private static IQueryable<Sale> ApplyFilters(IQueryable<Sale> query, string? search, int? status, int? paymentStatus, DateTimeOffset? from, DateTimeOffset? to)
    {
        if (status.HasValue) query = query.Where(x => (int)x.Status == status.Value);
        if (paymentStatus.HasValue) query = query.Where(x => (int)x.PaymentStatus == paymentStatus.Value);
        if (from.HasValue) query = query.Where(x => x.SaleDate >= from.Value);
        if (to.HasValue) query = query.Where(x => x.SaleDate < to.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();
            query = query.Where(x => x.InvoiceNumber.Contains(value) || (x.Customer != null && x.Customer.Name.Contains(value)));
        }
        return query;
    }
}
