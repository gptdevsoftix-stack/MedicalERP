using System.Diagnostics;
using MedicalERP.Application.Interfaces;
using MedicalERP.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalERP.Web.Controllers;

public sealed class HomeController(IReportService reportService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewBag.CompanyContextId = Request.Query["companyContextId"].FirstOrDefault();
        return View(await reportService.GetDashboardAsync(cancellationToken));
    }

    public IActionResult Privacy() => View();
    public IActionResult AccessDenied() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
