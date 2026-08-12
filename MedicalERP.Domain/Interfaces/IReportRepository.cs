using MedicalERP.Domain.Sales;

namespace MedicalERP.Domain.Interfaces;

public sealed record SalesSummaryData(
    int SalesCount,
    decimal Revenue,
    decimal Discount,
    decimal Tax,
    decimal Paid,
    decimal Due,
    decimal Cost,
    decimal ItemsQuantity);

public sealed record SalesByDayData(
    DateTimeOffset Day,
    int SalesCount,
    decimal Revenue,
    decimal Discount);

public sealed record TopProductData(
    Guid ProductId,
    string ProductName,
    decimal Quantity,
    decimal Revenue);

public sealed record TopCustomerData(
    Guid? CustomerId,
    string CustomerName,
    int SalesCount,
    decimal Revenue);

public sealed record PaymentMethodSummaryData(
    string PaymentMethodName,
    int PaymentCount,
    decimal Amount);

public sealed record LowStockData(
    Guid ProductId,
    string ProductName,
    decimal AvailableQuantity,
    decimal ReorderLevel);

public interface IReportRepository
{
    Task<SalesSummaryData> GetSalesSummaryAsync(Guid companyId, Guid storeId, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesByDayData>> GetSalesByDayAsync(Guid companyId, Guid storeId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TopProductData>> GetTopProductsAsync(Guid companyId, Guid storeId, DateTimeOffset? from, DateTimeOffset? to, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TopCustomerData>> GetTopCustomersAsync(Guid companyId, Guid storeId, DateTimeOffset? from, DateTimeOffset? to, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentMethodSummaryData>> GetPaymentMethodSummaryAsync(Guid companyId, Guid storeId, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LowStockData>> GetLowStockAsync(Guid companyId, Guid storeId, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Sale>> GetRecentSalesAsync(Guid companyId, Guid storeId, int take, CancellationToken cancellationToken = default);
}
