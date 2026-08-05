using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MedicalERP.Domain.DTOs
{
    public sealed class CategoryDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public Guid? ParentCategoryId { get; set; }

        public string? ParentCategoryName { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }

    public sealed class CreateCategoryDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Parent Category")]
        public Guid? ParentCategoryId { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }
    }

    public sealed class UpdateCategoryDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Parent Category")]
        public Guid? ParentCategoryId { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
