using MedicalERP.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MedicalERP.Domain.DTOs;

public sealed class CatalogMasterDto
{
    public Guid Id { get; set; }
    public CatalogMasterType MasterType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? LicenseNumber { get; set; }
    public decimal? Value { get; set; }
    public string? MeasurementUnit { get; set; }
    public string? Symbol { get; set; }
    public bool AllowsDecimal { get; set; }
    public bool IsActive { get; set; }
}

public sealed class CatalogMasterFormDto
{
    public Guid Id { get; set; }
    public CatalogMasterType MasterType { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Code { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(80)]
    [Display(Name = "License Number")]
    public string? LicenseNumber { get; set; }

    public decimal? Value { get; set; }

    [StringLength(40)]
    [Display(Name = "Measurement Unit")]
    public string? MeasurementUnit { get; set; }

    [StringLength(20)]
    public string? Symbol { get; set; }

    [Display(Name = "Allows Decimal")]
    public bool AllowsDecimal { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
