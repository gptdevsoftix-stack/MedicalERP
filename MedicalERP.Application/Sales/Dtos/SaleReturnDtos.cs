using System.ComponentModel.DataAnnotations;
using MedicalERP.Domain.Enums;

namespace MedicalERP.Application.Sales.Dtos;

public sealed class SaleReturnListDto
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public string? InvoiceNumber { get; set; }
    public DateTimeOffset ReturnDate { get; set; }
    public ReturnStatus Status { get; set; }
    public decimal RefundAmount { get; set; }
    public int ItemCount { get; set; }
}

public sealed class SaleReturnFormDto
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }

    [Required, StringLength(40)]
    [Display(Name = "Return Number")]
    public string ReturnNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Return Date")]
    public DateTimeOffset ReturnDate { get; set; } = DateTimeOffset.Now;

    public ReturnStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal RefundAmount { get; set; }

    [StringLength(1000)]
    public string? Reason { get; set; }

    public List<SaleReturnItemFormDto> Items { get; set; } = [];
}

public sealed class SaleReturnItemFormDto
{
    public Guid SaleItemId { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public Guid? ProductBatchId { get; set; }
    public string? BatchNumber { get; set; }
    public decimal ConversionFactor { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal AvailableQuantity { get; set; }

    [Range(typeof(decimal), "0.0001", "999999999")]
    public decimal Quantity { get; set; }

    [Range(0, 999999999)]
    public decimal TaxAmount { get; set; }

    public bool ReturnToStock { get; set; } = true;
}

public sealed class SaleReturnFilterDto
{
    public string? Search { get; set; }
    public ReturnStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
