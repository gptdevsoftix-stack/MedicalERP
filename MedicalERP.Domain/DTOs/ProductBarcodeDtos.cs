using System.ComponentModel.DataAnnotations;

namespace MedicalERP.Domain.DTOs;

public sealed class ProductBarcodeListDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public Guid? ProductUnitId { get; set; }
    public string? ProductUnitName { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ProductBarcodeFormDto
{
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "Product")]
    public Guid ProductId { get; set; }

    [Display(Name = "Product Unit")]
    public Guid? ProductUnitId { get; set; }

    [Required]
    [StringLength(100)]
    public string Barcode { get; set; } = string.Empty;

    [Display(Name = "Primary")]
    public bool IsPrimary { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
