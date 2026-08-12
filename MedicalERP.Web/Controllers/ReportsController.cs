using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Permissions;
using MedicalERP.Application.Reports.Dtos;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalERP.Web.Controllers;

[Authorize]
public sealed class ReportsController(IReportService service) : Controller
{
    [HttpGet, HasPermission(Permissions.Reports.View)]
    public async Task<IActionResult> Sales(SalesReportFilterDto filter, CancellationToken cancellationToken)
    {
        ViewBag.CompanyContextId = GetCompanyContextId();
        return View(await service.GetSalesReportAsync(filter, cancellationToken));
    }

    private string? GetCompanyContextId() => Request.Query["companyContextId"].FirstOrDefault();
}
