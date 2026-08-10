using System.ComponentModel.DataAnnotations;
using MedicalERP.Domain.Enums;

namespace MedicalERP.Application.Sales.Dtos;

public sealed class SaleListDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public DateTimeOffset SaleDate { get; set; }
    public SaleStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public int ItemCount { get; set; }
}

public sealed class SaleFormDto
{
    public Guid Id { get; set; }

    [Display(Name = "Customer")]
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }

    [Display(Name = "Warehouse")]
    public Guid? WarehouseId { get; set; }

    [Display(Name = "Register Session")]
    public Guid? RegisterSessionId { get; set; }
    public string? RegisterSessionName { get; set; }

    [Required, StringLength(40)]
    [Display(Name = "Invoice Number")]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Sale Date")]
    public DateTimeOffset SaleDate { get; set; } = DateTimeOffset.Now;

    [Display(Name = "Payment Method")]
    public Guid? PaymentMethodId { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Amount Paid")]
    public decimal PaidAmount { get; set; }

    [StringLength(80)]
    [Display(Name = "Payment Reference")]
    public string? PaymentReference { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public SaleStatus Status { get; set; } = SaleStatus.Confirmed;
    public PaymentStatus PaymentStatus { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ItemDiscount { get; set; }
    public decimal InvoiceDiscount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal RoundOffAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal ChangeAmount { get; set; }
    public decimal DueAmount { get; set; }
    public List<SaleItemFormDto> Items { get; set; } = [];
    public List<SalePaymentFormDto> Payments { get; set; } = [];
}

public sealed class SaleItemFormDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public Guid ProductUnitId { get; set; }
    public string? ProductUnitName { get; set; }
    public decimal ConversionFactor { get; set; }
    public decimal AvailableStock { get; set; }

    [Range(typeof(decimal), "0.0001", "999999999")]
    public decimal Quantity { get; set; }

    [Range(0, 999999999)]
    public decimal UnitPrice { get; set; }

    [Range(0, 999999999)]
    public decimal DiscountAmount { get; set; }

    [Range(0, 999999999)]
    public decimal TaxAmount { get; set; }

    public decimal NetAmount { get; set; }
}

public sealed class SalePaymentFormDto
{
    public Guid Id { get; set; }
    public Guid PaymentMethodId { get; set; }
    public string? PaymentMethodName { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
}

public sealed class SaleFilterDto
{
    public string? Search { get; set; }
    public SaleStatus? Status { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public sealed class SaleLookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class SaleProductLookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal AvailableStock { get; set; }
    public decimal SalePrice { get; set; }
}
