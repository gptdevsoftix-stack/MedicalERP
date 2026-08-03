using MedicalERP.Domain.Common;
using MedicalERP.Domain.Companies;

namespace MedicalERP.Domain.Identity;

public sealed class UserCompanyAccess : CompanyEntity
{
    public Guid UserId { get; set; }
    public bool IsDefaultCompany { get; set; }
    public Company? Company { get; set; }
}
