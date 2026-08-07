using System.ComponentModel.DataAnnotations;
using MedicalERP.Domain.Enums;

namespace MedicalERP.Application.Purchases.Dtos;

public sealed class PurchaseOrderListDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string? WarehouseName { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public DateTimeOffset? ExpectedDeliveryDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal GrandTotal { get; set; }
    public int ItemCount { get; set; }
}

public sealed class PurchaseOrderFormDto
{
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "Supplier")]
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;

    [Display(Name = "Warehouse")]
    public Guid? WarehouseId { get; set; }

    [Required, StringLength(40)]
    [Display(Name = "Order Number")]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Order Date")]
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

    [Display(Name = "Expected Delivery")]
    public DateTimeOffset? ExpectedDeliveryDate { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Other Charges")]
    public decimal OtherCharges { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public List<PurchaseOrderItemFormDto> Items { get; set; } = [];
}

public sealed class PurchaseOrderItemFormDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid ProductUnitId { get; set; }
    public string ProductUnitName { get; set; } = string.Empty;
    public decimal ConversionFactor { get; set; }

    [Range(typeof(decimal), "0.0001", "999999999")]
    public decimal OrderedQuantity { get; set; }

    [Range(0, 999999999)]
    public decimal FreeQuantity { get; set; }

    [Range(0, 999999999)]
    public decimal UnitPrice { get; set; }

    [Range(0, 999999999)]
    public decimal DiscountAmount { get; set; }

    [Range(0, 999999999)]
    public decimal TaxAmount { get; set; }
}

public sealed class PurchaseOrderFilterDto
{
    public string? Search { get; set; }
    public OrderStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public sealed class PurchaseLookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
