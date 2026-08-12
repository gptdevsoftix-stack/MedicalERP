using System.Linq.Expressions;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Sales;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class ReportRepository(ApplicationDbContext context) : IReportRepository
{
    public async Task<SalesSummaryData> GetSalesSummaryAsync(Guid companyId, Guid storeId, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken = default)
    {
        var sales = ActiveSales(companyId, storeId, from, to);
        var count = await sales.CountAsync(cancellationToken);

        var totals = await sales.GroupBy(_ => 1)
            .Select(g => new
            {
                Revenue = g.Sum(x => x.GrandTotal),
                Discount = g.Sum(x => x.ItemDiscount + x.InvoiceDiscount),
                Tax = g.Sum(x => x.TaxAmount),
                Paid = g.Sum(x => x.PaidAmount),
                Due = g.Sum(x => x.DueAmount)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var items = ApplyDateRange(context.SaleItems.Where(x => x.CompanyId == companyId && x.StoreId == storeId), from, to, x => x.Sale.SaleDate)
            .Where(x => x.Sale.Status != SaleStatus.Draft && x.Sale.Status != SaleStatus.Cancelled);

        var itemsQuantity = await items.SumAsync(x => x.Quantity, cancellationToken);
        var cost = await items.SumAsync(x => x.CostPrice * x.BaseQuantity, cancellationToken);

        return new SalesSummaryData(
            count,
            totals?.Revenue ?? 0,
            totals?.Discount ?? 0,
            totals?.Tax ?? 0,
            totals?.Paid ?? 0,
            totals?.Due ?? 0,
            cost,
            itemsQuantity);
    }

    public async Task<IReadOnlyList<SalesByDayData>> GetSalesByDayAsync(Guid companyId, Guid storeId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        var rows = await ActiveSales(companyId, storeId, from, to)
            .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month, x.SaleDate.Day })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.Day,
                Count = g.Count(),
                Revenue = g.Sum(x => x.GrandTotal),
                Discount = g.Sum(x => x.ItemDiscount + x.InvoiceDiscount)
            })
            .OrderBy(g => g.Year).ThenBy(g => g.Month).ThenBy(g => g.Day)
            .ToListAsync(cancellationToken);

        return rows
            .Select(g => new SalesByDayData(new DateTimeOffset(g.Year, g.Month, g.Day, 0, 0, 0, TimeSpan.Zero), g.Count, g.Revenue, g.Discount))
            .ToArray();
    }

    public async Task<IReadOnlyList<TopProductData>> GetTopProductsAsync(Guid companyId, Guid storeId, DateTimeOffset? from, DateTimeOffset? to, int take, CancellationToken cancellationToken = default)
    {
        var rows = await ApplyDateRange(context.SaleItems.Where(x => x.CompanyId == companyId && x.StoreId == storeId), from, to, x => x.Sale.SaleDate)
            .Where(x => x.Sale.Status != SaleStatus.Draft && x.Sale.Status != SaleStatus.Cancelled)
            .GroupBy(x => new { x.ProductId, x.Product.Name })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.Name,
                Quantity = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.NetAmount)
            })
            .OrderByDescending(x => x.Revenue)
            .Take(take)
            .ToListAsync(cancellationToken);

        return rows.Select(x => new TopProductData(x.ProductId, x.Name, x.Quantity, x.Revenue)).ToArray();
    }

    public async Task<IReadOnlyList<TopCustomerData>> GetTopCustomersAsync(Guid companyId, Guid storeId, DateTimeOffset? from, DateTimeOffset? to, int take, CancellationToken cancellationToken = default)
    {
        var rows = await ActiveSales(companyId, storeId, from, to)
            .GroupBy(x => x.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                Count = g.Count(),
                Revenue = g.Sum(x => x.GrandTotal)
            })
            .OrderByDescending(x => x.Revenue)
            .Take(take)
            .ToListAsync(cancellationToken);

        var customerIds = rows.Where(x => x.CustomerId.HasValue).Select(x => x.CustomerId!.Value).ToArray();
        var names = await context.Customers.AsNoTracking()
            .Where(x => customerIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name, cancellationToken);

        return rows
            .Select(x => new TopCustomerData(
                x.CustomerId,
                x.CustomerId.HasValue ? names.GetValueOrDefault(x.CustomerId.Value) ?? "Customer" : "Walk-in customer",
                x.Count,
                x.Revenue))
            .ToArray();
    }

    public async Task<IReadOnlyList<PaymentMethodSummaryData>> GetPaymentMethodSummaryAsync(Guid companyId, Guid storeId, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken = default)
    {
        var rows = await ApplyDateRange(context.SalePayments.Where(x => x.CompanyId == companyId && x.StoreId == storeId), from, to, x => x.Sale.SaleDate)
            .Where(x => x.Sale.Status != SaleStatus.Draft && x.Sale.Status != SaleStatus.Cancelled)
            .GroupBy(x => x.PaymentMethod.Name)
            .Select(g => new
            {
                Name = g.Key,
                Count = g.Count(),
                Amount = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.Amount)
            .ToListAsync(cancellationToken);

        return rows.Select(x => new PaymentMethodSummaryData(x.Name, x.Count, x.Amount)).ToArray();
    }

    public async Task<IReadOnlyList<LowStockData>> GetLowStockAsync(Guid companyId, Guid storeId, int take, CancellationToken cancellationToken = default)
    {
        var available = await context.InventoryStocks.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId)
            .GroupBy(x => x.ProductId)
            .Select(g => new { ProductId = g.Key, Available = g.Sum(x => x.QuantityOnHand - x.ReservedQuantity) })
            .ToListAsync(cancellationToken);

        var reorderLevels = await context.StoreProducts.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId)
            .ToDictionaryAsync(x => x.ProductId, x => x.ReorderLevel, cancellationToken);

        var productIds = available.Select(x => x.ProductId).ToArray();
        var productNames = await context.Products.AsNoTracking()
            .Where(x => productIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name, cancellationToken);

        return available
            .Where(x => x.Available <= reorderLevels.GetValueOrDefault(x.ProductId))
            .Select(x => new LowStockData(
                x.ProductId,
                productNames.GetValueOrDefault(x.ProductId) ?? "Product",
                x.Available,
                reorderLevels.GetValueOrDefault(x.ProductId)))
            .OrderBy(x => x.AvailableQuantity)
            .Take(take)
            .ToArray();
    }

    public async Task<IReadOnlyList<Sale>> GetRecentSalesAsync(Guid companyId, Guid storeId, int take, CancellationToken cancellationToken = default)
    {
        return await context.Sales.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.StoreId == storeId)
            .Include(x => x.Customer)
            .Include(x => x.Items)
            .OrderByDescending(x => x.SaleDate)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Sale> ActiveSales(Guid companyId, Guid storeId, DateTimeOffset? from, DateTimeOffset? to)
    {
        return ApplyDateRange(context.Sales.Where(x => x.CompanyId == companyId && x.StoreId == storeId), from, to, x => x.SaleDate)
            .Where(x => x.Status != SaleStatus.Draft && x.Status != SaleStatus.Cancelled);
    }

    private static IQueryable<T> ApplyDateRange<T>(IQueryable<T> query, DateTimeOffset? from, DateTimeOffset? to, Expression<Func<T, DateTimeOffset>> dateSelector)
    {
        var selector = dateSelector.Parameters[0];
        if (from.HasValue) query = query.Where(BuildDatePredicate(dateSelector, selector, from.Value, lessThan: false));
        if (to.HasValue) query = query.Where(BuildDatePredicate(dateSelector, selector, to.Value, lessThan: true));
        return query;
    }

    private static Expression<Func<T, bool>> BuildDatePredicate<T>(Expression<Func<T, DateTimeOffset>> dateSelector, ParameterExpression selector, DateTimeOffset value, bool lessThan)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var body = new ReplaceParameterVisitor(selector, parameter).Visit(dateSelector.Body);
        var comparison = lessThan
            ? Expression.LessThan(body, Expression.Constant(value))
            : Expression.GreaterThanOrEqual(body, Expression.Constant(value));
        return Expression.Lambda<Func<T, bool>>(comparison, parameter);
    }

    private sealed class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public ReplaceParameterVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node) => node == _from ? _to : base.VisitParameter(node);
    }
}
