using MedicalERP.Application.Common;
using MedicalERP.Application.Sales.Dtos;
using MedicalERP.Domain.Enums;

namespace MedicalERP.Application.Reports.Dtos;

public sealed class SalesSummaryDto
{
    public int SalesCount { get; set; }
    public decimal Revenue { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Paid { get; set; }
    public decimal Due { get; set; }
    public decimal Cost { get; set; }
    public decimal ItemsQuantity { get; set; }
    public decimal GrossProfit => Revenue - Cost;
    public decimal MarginPercent => Revenue == 0 ? 0 : GrossProfit / Revenue * 100;
    public decimal AverageSaleValue => SalesCount == 0 ? 0 : Revenue / SalesCount;
}

public sealed class SalesByDayDto
{
    public DateTimeOffset Day { get; set; }
    public int SalesCount { get; set; }
    public decimal Revenue { get; set; }
    public decimal Discount { get; set; }
}

public sealed class TopProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Revenue { get; set; }
}

public sealed class TopCustomerDto
{
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int SalesCount { get; set; }
    public decimal Revenue { get; set; }
}

public sealed class PaymentMethodSummaryDto
{
    public string PaymentMethodName { get; set; } = string.Empty;
    public int PaymentCount { get; set; }
    public decimal Amount { get; set; }
}

public sealed class LowStockDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal AvailableQuantity { get; set; }
    public decimal ReorderLevel { get; set; }
}

public sealed class SalesReportFilterDto
{
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
    public string? Search { get; set; }
    public SaleStatus? Status { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public sealed class SalesReportDto
{
    public SalesReportFilterDto Filter { get; set; } = new();
    public SalesSummaryDto Summary { get; set; } = new();
    public IReadOnlyList<SalesByDayDto> ByDay { get; set; } = [];
    public IReadOnlyList<TopProductDto> TopProducts { get; set; } = [];
    public IReadOnlyList<TopCustomerDto> TopCustomers { get; set; } = [];
    public IReadOnlyList<PaymentMethodSummaryDto> PaymentMethodMix { get; set; } = [];
    public PagedResult<SaleListDto> Sales { get; set; } = new([], 1, 25, 0);
}

public sealed class DashboardDto
{
    public bool HasContext { get; set; }
    public SalesSummaryDto Today { get; set; } = new();
    public SalesSummaryDto ThisMonth { get; set; } = new();
    public IReadOnlyList<SalesByDayDto> SalesLast14Days { get; set; } = [];
    public IReadOnlyList<PaymentMethodSummaryDto> PaymentMethodMix { get; set; } = [];
    public IReadOnlyList<TopProductDto> TopProducts { get; set; } = [];
    public IReadOnlyList<LowStockDto> LowStock { get; set; } = [];
    public IReadOnlyList<SaleListDto> RecentSales { get; set; } = [];
}
