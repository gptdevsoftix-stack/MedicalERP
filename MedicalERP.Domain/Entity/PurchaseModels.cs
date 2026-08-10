using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Inventory;

namespace MedicalERP.Domain.Purchases;

public sealed class Supplier : CompanyEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public int CreditDays { get; set; }
    public decimal CreditLimit { get; set; }
    public ICollection<SupplierStore> Stores { get; set; } = [];
}

public sealed class SupplierStore : StoreEntity
{
    public Guid SupplierId { get; set; }
    public bool IsPreferred { get; set; }
    public Supplier Supplier { get; set; } = null!;
}

public sealed class PurchaseOrder : StoreEntity
{
    public Guid SupplierId { get; set; }
    public Guid? WarehouseId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTimeOffset OrderDate { get; set; }
    public DateTimeOffset? ExpectedDeliveryDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal OtherCharges { get; set; }
    public decimal GrandTotal { get; set; }
    public string? Notes { get; set; }
    public string? ApprovedByUserId { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public ICollection<PurchaseOrderItem> Items { get; set; } = [];
    public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = [];
}

public sealed class PurchaseOrderItem : StoreEntity
{
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductUnitId { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal FreeQuantity { get; set; }
    public decimal ConversionFactor { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public sealed class GoodsReceipt : StoreEntity
{
    public Guid SupplierId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public string? SupplierDeliveryNote { get; set; }
    public DateTimeOffset ReceiptDate { get; set; }
    public bool IsPosted { get; set; }
    public string? Notes { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
    public ICollection<GoodsReceiptItem> Items { get; set; } = [];
}

public sealed class GoodsReceiptItem : StoreEntity
{
    public Guid GoodsReceiptId { get; set; }
    public Guid? PurchaseOrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductUnitId { get; set; }
    public Guid ProductBatchId { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal FreeQuantity { get; set; }
    public decimal BaseQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public GoodsReceipt GoodsReceipt { get; set; } = null!;
    public ProductBatch ProductBatch { get; set; } = null!;
}

public sealed class PurchaseInvoice : StoreEntity
{
    public Guid SupplierId { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public Guid? GoodsReceiptId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string SupplierInvoiceNumber { get; set; } = string.Empty;
    public DateTimeOffset InvoiceDate { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public PurchaseInvoiceStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public ICollection<PurchaseInvoiceItem> Items { get; set; } = [];
    public ICollection<SupplierPayment> Payments { get; set; } = [];
}

public sealed class PurchaseInvoiceItem : StoreEntity
{
    public Guid PurchaseInvoiceId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductUnitId { get; set; }
    public Guid? ProductBatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;
}

public sealed class SupplierPayment : StoreEntity
{
    public Guid SupplierId { get; set; }
    public Guid? PurchaseInvoiceId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public DateTimeOffset PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}

public sealed class PurchaseReturn : StoreEntity
{
    public Guid SupplierId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? PurchaseInvoiceId { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTimeOffset ReturnDate { get; set; }
    public ReturnStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string? Reason { get; set; }
    public ICollection<PurchaseReturnItem> Items { get; set; } = [];
}

public sealed class PurchaseReturnItem : StoreEntity
{
    public Guid PurchaseReturnId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductBatchId { get; set; }
    public Guid ProductUnitId { get; set; }
    public decimal Quantity { get; set; }
    public decimal BaseQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public PurchaseReturn PurchaseReturn { get; set; } = null!;
}
