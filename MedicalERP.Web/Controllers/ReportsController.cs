using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Permissions;
using MedicalERP.Application.Reports.Dtos;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Web.Controllers;

[Authorize]
public sealed class ReportsController(
    IReportService service,
    ICompanyContext companyContext,
    IStoreContext storeContext,
    ApplicationDbContext db) : Controller
{
    [HttpGet, HasPermission(Permissions.Reports.View)]
    public async Task<IActionResult> Sales(SalesReportFilterDto filter, CancellationToken cancellationToken)
    {
        ViewBag.CompanyContextId = GetCompanyContextId();
        ViewBag.StoreContextId = storeContext.SelectedStoreId?.ToString();
        var companyId = companyContext.RequireCompanyId();
        ViewBag.CompanyName = await db.Companies.AsNoTracking()
            .Where(x => x.Id == companyId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? "Selected company";

        if (storeContext.SelectedStoreId is Guid storeId)
        {
            ViewBag.StoreName = await db.Stores.AsNoTracking()
                .Where(x => x.CompanyId == companyId && x.Id == storeId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync(cancellationToken) ?? "Selected store";
        }
        return View(await service.GetSalesReportAsync(filter, cancellationToken));
    }

    private string? GetCompanyContextId() => Request.Query["companyContextId"].FirstOrDefault();
}
