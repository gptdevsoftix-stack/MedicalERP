using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MedicalERP.Domain.DTOs
{
    public sealed class ProductUnitListDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string UnitName { get; set; } = string.Empty;

        public string UnitSymbol { get; set; } = string.Empty;

        public decimal ConversionFactor { get; set; }

        public bool IsBaseUnit { get; set; }

        public bool IsPurchaseUnit { get; set; }

        public bool IsSaleUnit { get; set; }

        public bool IsActive { get; set; }
    }

    public sealed class ProductUnitFormDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [Display(Name = "Unit")]
        public Guid UnitId { get; set; }

        [Range(
            0.0001,
            999999999,
            ErrorMessage = "Conversion factor must be greater than zero.")]
        [Display(Name = "Conversion Factor")]
        public decimal ConversionFactor { get; set; } = 1;

        [Display(Name = "Base Unit")]
        public bool IsBaseUnit { get; set; }

        [Display(Name = "Purchase Unit")]
        public bool IsPurchaseUnit { get; set; }

        [Display(Name = "Sale Unit")]
        public bool IsSaleUnit { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
