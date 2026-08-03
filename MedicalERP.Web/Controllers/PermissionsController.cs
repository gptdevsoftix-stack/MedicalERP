using MedicalERP.Application.Common;
using MedicalERP.Application.Permissions;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalERP.Web.Controllers;

[ApiController]
[Route("api/permissions")]
public sealed class PermissionsController : ControllerBase
{
    [HttpGet, HasPermission(Permissions.Roles.View)] public ActionResult<ApiResponse<IReadOnlyCollection<string>>> Get() => Ok(ApiResponse<IReadOnlyCollection<string>>.Ok(Permissions.All));
}

