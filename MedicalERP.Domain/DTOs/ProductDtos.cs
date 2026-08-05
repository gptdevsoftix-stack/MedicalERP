using MedicalERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MedicalERP.Domain.DTOs
{
    public sealed class ProductListDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string? BrandName { get; set; }

        public string? ManufacturerName { get; set; }

        public string BaseUnitName { get; set; } = string.Empty;

        public ProductType ProductType { get; set; }

        public bool IsMedicine { get; set; }

        public bool IsActive { get; set; }
    }

    public sealed class ProductDetailsDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? BrandName { get; set; }

        public string? ManufacturerName { get; set; }

        public string? GenericMedicineName { get; set; }

        public string? DosageFormName { get; set; }

        public string? StrengthName { get; set; }

        public string BaseUnitName { get; set; } = string.Empty;

        public ProductType ProductType { get; set; }

        public bool IsMedicine { get; set; }

        public bool RequiresPrescription { get; set; }

        public bool IsControlledDrug { get; set; }

        public bool TrackBatch { get; set; }

        public bool TrackExpiry { get; set; }

        public bool AllowDiscount { get; set; }

        public bool AllowNegativeStock { get; set; }

        public string? RegulatoryNumber { get; set; }

        public bool IsActive { get; set; }
    }

    public sealed class ProductFormDto
    {
        public Guid Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        [Display(Name = "Product brand")]
        public Guid? ProductBrandId { get; set; }

        [Display(Name = "Manufacturer")]
        public Guid? ManufacturerId { get; set; }

        [Display(Name = "Generic medicine")]
        public Guid? GenericMedicineId { get; set; }

        [Display(Name = "Dosage form")]
        public Guid? DosageFormId { get; set; }

        [Display(Name = "Strength")]
        public Guid? StrengthId { get; set; }

        [Required(ErrorMessage = "Base unit is required.")]
        [Display(Name = "Base unit")]
        public Guid BaseUnitId { get; set; }

        [Display(Name = "Product type")]
        public ProductType ProductType { get; set; }

        [Display(Name = "Medicine")]
        public bool IsMedicine { get; set; }

        [Display(Name = "Requires prescription")]
        public bool RequiresPrescription { get; set; }

        [Display(Name = "Controlled drug")]
        public bool IsControlledDrug { get; set; }

        [Display(Name = "Track batches")]
        public bool TrackBatch { get; set; } = true;

        [Display(Name = "Track expiry")]
        public bool TrackExpiry { get; set; } = true;

        [Display(Name = "Allow discount")]
        public bool AllowDiscount { get; set; } = true;

        [Display(Name = "Allow negative stock")]
        public bool AllowNegativeStock { get; set; }

        [StringLength(100)]
        [Display(Name = "Regulatory number")]
        public string? RegulatoryNumber { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class ProductLookupDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
