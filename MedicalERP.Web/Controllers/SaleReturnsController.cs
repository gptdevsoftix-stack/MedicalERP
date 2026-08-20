using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Permissions;
using MedicalERP.Application.Sales.Dtos;
using MedicalERP.Domain.Enums;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalERP.Web.Controllers;

[Authorize]
public sealed class SaleReturnsController(ISaleReturnService service) : Controller
{
    [HttpGet, HasPermission(Permissions.Sales.Return)]
    public async Task<IActionResult> Index(string? search, ReturnStatus? status, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var filter = new SaleReturnFilterDto { Search = search, Status = status, Page = page, PageSize = pageSize };
        ViewBag.Search = search; ViewBag.Status = status; ViewBag.CompanyContextId = GetCompanyContextId();
        ViewBag.PaginationRouteValues = new Dictionary<string, object?> { ["search"] = search, ["status"] = status, ["companyContextId"] = GetCompanyContextId() };
        return View(await service.GetAsync(filter, cancellationToken));
    }

    [HttpGet, HasPermission(Permissions.Sales.Return)]
    public async Task<IActionResult> Create(Guid saleId, CancellationToken cancellationToken)
    {
        try
        {
            var model = await service.GetForReturnAsync(saleId, cancellationToken);
            if (model is null) return NotFound();
            await LoadLookupsAsync(model, cancellationToken);
            return View(model);
        }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Details", "Sales", new { id = saleId, companyContextId = GetCompanyContextId() });
        }
    }

    [HttpPost, HasPermission(Permissions.Sales.Return), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleReturnFormDto model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return await ReloadFormAsync(model, cancellationToken);
        try
        {
            var id = await service.CreateAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Return posted and stock restored.";
            return RedirectToAction(nameof(Details), new { id, companyContextId = GetCompanyContextId() });
        }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException or KeyNotFoundException)
        {
            ModelState.AddModelError(string.Empty, ex.Message); return await ReloadFormAsync(model, cancellationToken);
        }
    }

    [HttpGet, HasPermission(Permissions.Sales.View)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var model = await service.GetDetailsAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    private async Task<IActionResult> ReloadFormAsync(SaleReturnFormDto posted, CancellationToken cancellationToken)
    {
        try
        {
            var fresh = await service.GetForReturnAsync(posted.SaleId, cancellationToken);
            if (fresh is null) return NotFound();
            fresh.ReturnNumber = posted.ReturnNumber;
            fresh.ReturnDate = posted.ReturnDate;
            fresh.WarehouseId = posted.WarehouseId;
            fresh.Reason = posted.Reason;
            foreach (var line in posted.Items)
            {
                var target = fresh.Items.FirstOrDefault(x => x.SaleItemId == line.SaleItemId);
                if (target is null) continue;
                target.Quantity = line.Quantity;
                target.TaxAmount = line.TaxAmount;
                target.ReturnToStock = line.ReturnToStock;
            }
            await LoadLookupsAsync(fresh, cancellationToken);
            return View(fresh);
        }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(posted);
        }
    }

    private async Task LoadLookupsAsync(SaleReturnFormDto model, CancellationToken cancellationToken)
    {
        ViewBag.Warehouses = new SelectList(await service.GetWarehousesAsync(cancellationToken), "Id", "Name", model.WarehouseId);
    }

    private string? GetCompanyContextId() => Request.Query["companyContextId"].FirstOrDefault();
}
