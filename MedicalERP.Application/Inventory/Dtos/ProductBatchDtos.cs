using System.ComponentModel.DataAnnotations;

namespace MedicalERP.Application.Inventory.Dtos;

public sealed class ProductBatchListDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public Guid? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? WarehouseCode { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateOnly? ManufacturingDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? MaximumRetailPrice { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public bool IsExpired { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ProductBatchFormDto
{
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "Product")]
    public Guid ProductId { get; set; }

    [Display(Name = "Warehouse")]
    public Guid? WarehouseId { get; set; }

    [Required]
    [StringLength(80)]
    [Display(Name = "Batch Number")]
    public string BatchNumber { get; set; } = string.Empty;

    [Display(Name = "Manufacturing Date")]
    public DateOnly? ManufacturingDate { get; set; }

    [Display(Name = "Expiry Date")]
    public DateOnly? ExpiryDate { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Purchase Price")]
    public decimal PurchasePrice { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Cost Price")]
    public decimal CostPrice { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Sale Price")]
    public decimal SalePrice { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Maximum Retail Price")]
    public decimal? MaximumRetailPrice { get; set; }

    [Display(Name = "Received At")]
    public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.Now;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public sealed class ProductBatchFilterDto
{
    public Guid? ProductId { get; set; }
    public Guid? WarehouseId { get; set; }
    public string? Search { get; set; }
    public bool? IsActive { get; set; } = true;
    public DateOnly? ExpiringBefore { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public sealed class ProductBatchLookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
