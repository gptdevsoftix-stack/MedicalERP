using MedicalERP.Domain.Common;
using MedicalERP.Domain.Enums;

namespace MedicalERP.Domain.Companies;

public sealed class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? LogoUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string Country { get; set; } = "US";
    public string? TaxNumber { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string TimeZone { get; set; } = "UTC";
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trial;
    public DateTime? SubscriptionStartsAt { get; set; }
    public DateTime? SubscriptionEndsAt { get; set; }
    public ICollection<Store> Stores { get; set; } = [];
    public ICollection<Warehouse> Warehouses { get; set; } = [];
}
