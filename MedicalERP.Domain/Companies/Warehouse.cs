using MedicalERP.Domain.Common;
using MedicalERP.Domain.Enums;

namespace MedicalERP.Domain.Companies;

public sealed class Warehouse : StoreEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public WarehouseType WarehouseType { get; set; } = WarehouseType.Main;
    public string? Address { get; set; }
    public bool IsDefault { get; set; }
    public Company? Company { get; set; }
    public Store? Store { get; set; }
}
