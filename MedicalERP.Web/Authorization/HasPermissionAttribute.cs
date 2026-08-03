using Microsoft.AspNetCore.Authorization;

namespace MedicalERP.Web.Authorization;

public sealed class HasPermissionAttribute(string permission) : AuthorizeAttribute("Permission:" + permission);

