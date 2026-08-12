using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Reports.Dtos;
using MedicalERP.Application.Sales.Dtos;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Sales;

namespace MedicalERP.Application.Services;

public sealed class ReportService(
    IReportRepository reportRepository,
    ISaleService saleService,
    ICompanyContext companyContext,
    IStoreContext storeContext) : IReportService
{
    private const int TopProductsCount = 10;
    private const int TopCustomersCount = 10;

    public async Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        Guid companyId;
        Guid storeId;
        try
        {
            companyId = RequireCompany();
            storeId = await RequireStoreAsync(cancellationToken);
        }
        catch (UnauthorizedAccessException)
        {
            return new DashboardDto { HasContext = false };
        }

        var now = DateTimeOffset.Now;
        var todayFrom = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);
        var monthFrom = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);

        var today = await reportRepository.GetSalesSummaryAsync(companyId, storeId, todayFrom, todayFrom.AddDays(1), cancellationToken);
        var month = await reportRepository.GetSalesSummaryAsync(companyId, storeId, monthFrom, monthFrom.AddMonths(1), cancellationToken);
        var byDay = await reportRepository.GetSalesByDayAsync(companyId, storeId, todayFrom.AddDays(-13), todayFrom.AddDays(1), cancellationToken);
        var paymentMix = await reportRepository.GetPaymentMethodSummaryAsync(companyId, storeId, monthFrom, monthFrom.AddMonths(1), cancellationToken);
        var topProducts = await reportRepository.GetTopProductsAsync(companyId, storeId, monthFrom, monthFrom.AddMonths(1), TopProductsCount, cancellationToken);
        var lowStock = await reportRepository.GetLowStockAsync(companyId, storeId, 8, cancellationToken);
        var recentSales = await reportRepository.GetRecentSalesAsync(companyId, storeId, 8, cancellationToken);

        return new DashboardDto
        {
            HasContext = true,
            Today = MapSummary(today),
            ThisMonth = MapSummary(month),
            SalesLast14Days = byDay.Select(MapByDay).ToArray(),
            PaymentMethodMix = paymentMix.Select(MapPayment).ToArray(),
            TopProducts = topProducts.Select(MapProduct).ToArray(),
            LowStock = lowStock.Select(MapLowStock).ToArray(),
            RecentSales = recentSales.Select(MapSaleList).ToArray()
        };
    }

    public async Task<SalesReportDto> GetSalesReportAsync(SalesReportFilterDto filter, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany();
        var storeId = await RequireStoreAsync(cancellationToken);

        var (from, to) = NormalizeRange(filter.From, filter.To);

        var summary = await reportRepository.GetSalesSummaryAsync(companyId, storeId, from, to, cancellationToken);
        var byDay = await reportRepository.GetSalesByDayAsync(companyId, storeId, from, to, cancellationToken);
        var paymentMix = await reportRepository.GetPaymentMethodSummaryAsync(companyId, storeId, from, to, cancellationToken);
        var topProducts = await reportRepository.GetTopProductsAsync(companyId, storeId, from, to, TopProductsCount, cancellationToken);
        var topCustomers = await reportRepository.GetTopCustomersAsync(companyId, storeId, from, to, TopCustomersCount, cancellationToken);
        var sales = await saleService.GetAsync(new SaleFilterDto
        {
            Search = filter.Search,
            Status = filter.Status,
            PaymentStatus = filter.PaymentStatus,
            From = from,
            To = to,
            Page = filter.Page,
            PageSize = filter.PageSize
        }, cancellationToken);

        return new SalesReportDto
        {
            Filter = filter,
            Summary = MapSummary(summary),
            ByDay = byDay.Select(MapByDay).ToArray(),
            PaymentMethodMix = paymentMix.Select(MapPayment).ToArray(),
            TopProducts = topProducts.Select(MapProduct).ToArray(),
            TopCustomers = topCustomers.Select(MapCustomer).ToArray(),
            Sales = sales
        };
    }

    private static (DateTimeOffset From, DateTimeOffset To) NormalizeRange(DateTimeOffset? requestedFrom, DateTimeOffset? requestedTo)
    {
        var now = DateTimeOffset.Now;
        var monthFrom = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);

        DateTimeOffset StartOfDay(DateTimeOffset value) => new(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset);

        var from = requestedFrom.HasValue ? StartOfDay(requestedFrom.Value) : monthFrom;
        var to = requestedTo.HasValue ? StartOfDay(requestedTo.Value).AddDays(1) : monthFrom.AddMonths(1);
        if (to <= from) to = from.AddDays(1);
        return (from, to);
    }

    private static SalesSummaryDto MapSummary(SalesSummaryData data) => new()
    {
        SalesCount = data.SalesCount,
        Revenue = data.Revenue,
        Discount = data.Discount,
        Tax = data.Tax,
        Paid = data.Paid,
        Due = data.Due,
        Cost = data.Cost,
        ItemsQuantity = data.ItemsQuantity
    };

    private static SalesByDayDto MapByDay(SalesByDayData data) => new()
    {
        Day = data.Day,
        SalesCount = data.SalesCount,
        Revenue = data.Revenue,
        Discount = data.Discount
    };

    private static TopProductDto MapProduct(TopProductData data) => new()
    {
        ProductId = data.ProductId,
        ProductName = data.ProductName,
        Quantity = data.Quantity,
        Revenue = data.Revenue
    };

    private static TopCustomerDto MapCustomer(TopCustomerData data) => new()
    {
        CustomerId = data.CustomerId,
        CustomerName = data.CustomerName,
        SalesCount = data.SalesCount,
        Revenue = data.Revenue
    };

    private static PaymentMethodSummaryDto MapPayment(PaymentMethodSummaryData data) => new()
    {
        PaymentMethodName = data.PaymentMethodName,
        PaymentCount = data.PaymentCount,
        Amount = data.Amount
    };

    private static LowStockDto MapLowStock(LowStockData data) => new()
    {
        ProductId = data.ProductId,
        ProductName = data.ProductName,
        AvailableQuantity = data.AvailableQuantity,
        ReorderLevel = data.ReorderLevel
    };

    private static SaleListDto MapSaleList(Sale sale) => new()
    {
        Id = sale.Id,
        InvoiceNumber = sale.InvoiceNumber,
        CustomerName = sale.Customer?.Name,
        SaleDate = sale.SaleDate,
        Status = sale.Status,
        PaymentStatus = sale.PaymentStatus,
        GrandTotal = sale.GrandTotal,
        PaidAmount = sale.PaidAmount,
        ItemCount = sale.Items.Count
    };

    private Guid RequireCompany() => companyContext.CompanyId ?? throw new UnauthorizedAccessException("Company context is missing.");
    private async Task<Guid> RequireStoreAsync(CancellationToken cancellationToken) { var id = storeContext.RequireSelectedStoreId(); await storeContext.EnsureStoreAccessAsync(id, cancellationToken); return id; }
}
