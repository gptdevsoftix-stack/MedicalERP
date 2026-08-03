using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Enums;

namespace MedicalERP.Domain.Inventory;

public sealed class ProductBatch : StoreEntity
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateOnly? ManufacturingDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? MaximumRetailPrice { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
}

public sealed class InventoryStock : StoreEntity
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? ProductBatchId { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity => QuantityOnHand - ReservedQuantity;
    public Product Product { get; set; } = null!;
    public ProductBatch? ProductBatch { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
}

public sealed class StockTransaction : StoreEntity
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? ProductBatchId { get; set; }
    public StockTransactionType TransactionType { get; set; }
    public DocumentType ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    public decimal QuantityIn { get; set; }
    public decimal QuantityOut { get; set; }
    public decimal BalanceAfter { get; set; }
    public decimal UnitCost { get; set; }
    public DateTimeOffset TransactionAt { get; set; }
    public string? Notes { get; set; }
}

public sealed class StockAdjustment : StoreEntity
{
    public Guid WarehouseId { get; set; }
    public string AdjustmentNumber { get; set; } = string.Empty;
    public AdjustmentType AdjustmentType { get; set; }
    public DateTimeOffset AdjustmentDate { get; set; }
    public Guid ReasonCodeId { get; set; }
    public string? Notes { get; set; }
    public bool IsPosted { get; set; }
    public ICollection<StockAdjustmentItem> Items { get; set; } = [];
}

public sealed class StockAdjustmentItem : StoreEntity
{
    public Guid StockAdjustmentId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductBatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public StockAdjustment StockAdjustment { get; set; } = null!;
}

public sealed class StockCount : StoreEntity
{
    public Guid WarehouseId { get; set; }
    public string CountNumber { get; set; } = string.Empty;
    public DateTimeOffset CountDate { get; set; }
    public StockCountStatus Status { get; set; }
    public string? Notes { get; set; }
    public ICollection<StockCountItem> Items { get; set; } = [];
}

public sealed class StockCountItem : StoreEntity
{
    public Guid StockCountId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductBatchId { get; set; }
    public decimal SystemQuantity { get; set; }
    public decimal CountedQuantity { get; set; }
    public decimal VarianceQuantity => CountedQuantity - SystemQuantity;
    public StockCount StockCount { get; set; } = null!;
}

public sealed class StockDisposal : StoreEntity
{
    public Guid WarehouseId { get; set; }
    public string DisposalNumber { get; set; } = string.Empty;
    public DateTimeOffset DisposalDate { get; set; }
    public Guid ReasonCodeId { get; set; }
    public string? ApprovedByUserId { get; set; }
    public string? Notes { get; set; }
    public bool IsPosted { get; set; }
    public ICollection<StockDisposalItem> Items { get; set; } = [];
}

public sealed class StockDisposalItem : StoreEntity
{
    public Guid StockDisposalId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductBatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public StockDisposal StockDisposal { get; set; } = null!;
}
