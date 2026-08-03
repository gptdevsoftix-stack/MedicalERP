using Microsoft.AspNetCore.Identity;

namespace MedicalERP.Infrastructure.Identity;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public Guid? CompanyId { get; set; }
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public bool IsActive { get; set; } = true;
}
