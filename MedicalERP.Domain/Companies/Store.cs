using MedicalERP.Domain.Common;

namespace MedicalERP.Domain.Companies;

public sealed class Store : CompanyEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string Country { get; set; } = "US";
    public string? TaxNumber { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string TimeZone { get; set; } = "UTC";
    public bool IsHeadOffice { get; set; }
    public Company? Company { get; set; }
    public ICollection<Warehouse> Warehouses { get; set; } = [];
}
