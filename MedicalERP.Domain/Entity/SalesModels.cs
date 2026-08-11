using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Inventory;
using MedicalERP.Domain.Support;

namespace MedicalERP.Domain.Sales;

public sealed class Customer : CompanyEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public decimal CreditLimit { get; set; }
    public int CreditDays { get; set; }
}

public sealed class SaleOrder : StoreEntity
{
    public Guid? CustomerId { get; set; }
    public Guid? WarehouseId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTimeOffset OrderDate { get; set; }
    public DateTimeOffset? RequiredDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string? Notes { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<SaleOrderItem> Items { get; set; } = [];
}

public sealed class SaleOrderItem : StoreEntity
{
    public Guid SaleOrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductUnitId { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal FulfilledQuantity { get; set; }
    public decimal ConversionFactor { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public SaleOrder SaleOrder { get; set; } = null!;
}

public sealed class Sale : StoreEntity
{
    public Guid? WarehouseId { get; set; }
    public Guid? RegisterSessionId { get; set; }
    public Guid? SaleOrderId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? PrescriptionId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTimeOffset SaleDate { get; set; }
    public SaleStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ItemDiscount { get; set; }
    public decimal InvoiceDiscount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal RoundOffAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public decimal DueAmount { get; set; }
    public string? Notes { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<SaleItem> Items { get; set; } = [];
    public ICollection<SalePayment> Payments { get; set; } = [];
}

public sealed class SaleItem : StoreEntity
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductUnitId { get; set; }
    public Guid? PrescriptionItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal BaseQuantity { get; set; }
    public decimal ConversionFactor { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal ReturnedQuantity { get; set; }
    public Sale Sale { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ICollection<SaleItemBatch> Batches { get; set; } = [];
}

public sealed class SaleItemBatch : StoreEntity
{
    public Guid SaleItemId { get; set; }
    public Guid ProductBatchId { get; set; }
    public decimal BaseQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public SaleItem SaleItem { get; set; } = null!;
    public ProductBatch ProductBatch { get; set; } = null!;
}

public sealed class SalePayment : StoreEntity
{
    public Guid SaleId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset PaidAt { get; set; }
    public string? ReferenceNumber { get; set; }
    public Sale Sale { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
}

public sealed class SaleReturn : StoreEntity
{
    public Guid SaleId { get; set; }
    public Guid? WarehouseId { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTimeOffset ReturnDate { get; set; }
    public ReturnStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal RefundAmount { get; set; }
    public string? Reason { get; set; }
    public Sale? Sale { get; set; }
    public ICollection<SaleReturnItem> Items { get; set; } = [];
}

public sealed class SaleReturnItem : StoreEntity
{
    public Guid SaleReturnId { get; set; }
    public Guid SaleItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductBatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal BaseQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public bool ReturnToStock { get; set; }
    public SaleReturn SaleReturn { get; set; } = null!;
    public Product? Product { get; set; }
    public ProductBatch? ProductBatch { get; set; }
}
