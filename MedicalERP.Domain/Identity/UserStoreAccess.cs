using MedicalERP.Domain.Common;
using MedicalERP.Domain.Companies;

namespace MedicalERP.Domain.Identity;

public sealed class UserStoreAccess : CompanyEntity
{
    public Guid UserId { get; set; }
    public Guid StoreId { get; set; }
    public bool IsDefaultStore { get; set; }
    public Company? Company { get; set; }
    public Store? Store { get; set; }
}
