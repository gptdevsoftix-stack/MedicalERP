using System.ComponentModel.DataAnnotations;

namespace MedicalERP.Application.Purchases.Dtos;

public sealed class SupplierListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int CreditDays { get; set; }
    public decimal CreditLimit { get; set; }
    public bool IsActive { get; set; }
}

public sealed class SupplierFormDto
{
    public Guid Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string Code { get; set; } = string.Empty;

    [StringLength(120)]
    public string? ContactPerson { get; set; }

    [EmailAddress, StringLength(256)]
    public string? Email { get; set; }

    [StringLength(40)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(40)]
    public string? TaxNumber { get; set; }

    [Range(0, 3650)]
    public int CreditDays { get; set; }

    [Range(0, 999999999)]
    public decimal CreditLimit { get; set; }

    public bool IsActive { get; set; } = true;
}
