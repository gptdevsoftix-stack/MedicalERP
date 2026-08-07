using System.ComponentModel.DataAnnotations;

namespace MedicalERP.Application.Sales.Dtos;

public sealed class CustomerListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int CreditDays { get; set; }
    public decimal CreditLimit { get; set; }
    public bool IsActive { get; set; }
}

public sealed class CustomerFormDto
{
    public Guid Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string Code { get; set; } = string.Empty;

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
