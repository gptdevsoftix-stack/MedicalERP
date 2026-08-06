using System.ComponentModel.DataAnnotations;

namespace MedicalERP.Domain.DTOs;

public sealed class StoreProductListDto
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public decimal SalePrice { get; set; }
    public decimal? WholesalePrice { get; set; }
    public decimal? MinimumSalePrice { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal ReorderQuantity { get; set; }
    public bool IsAvailableForSale { get; set; }
    public bool IsActive { get; set; }
}

public sealed class StoreProductFormDto
{
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "Store")]
    public Guid StoreId { get; set; }

    [Required]
    [Display(Name = "Product")]
    public Guid ProductId { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Sale Price")]
    public decimal SalePrice { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Wholesale Price")]
    public decimal? WholesalePrice { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Minimum Sale Price")]
    public decimal? MinimumSalePrice { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Reorder Level")]
    public decimal ReorderLevel { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Reorder Quantity")]
    public decimal ReorderQuantity { get; set; }

    [Display(Name = "Available For Sale")]
    public bool IsAvailableForSale { get; set; } = true;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
