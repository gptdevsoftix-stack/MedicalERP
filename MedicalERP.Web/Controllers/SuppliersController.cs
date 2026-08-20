using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Permissions;
using MedicalERP.Application.Purchases.Dtos;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalERP.Web.Controllers;

[Authorize]
public sealed class SuppliersController(ISupplierService service) : Controller
{
    [HttpGet, HasPermission(Permissions.Suppliers.View)]
    public async Task<IActionResult> Index(string? search, bool? isActive, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        ViewBag.Search = search; ViewBag.IsActive = isActive; ViewBag.CompanyContextId = Request.Query["companyContextId"].FirstOrDefault();
        ViewBag.PaginationRouteValues = new Dictionary<string, object?> { ["search"] = search, ["isActive"] = isActive, ["companyContextId"] = ViewBag.CompanyContextId };
        return View(await service.GetAsync(search, isActive, page, pageSize, cancellationToken));
    }

    [HttpGet, HasPermission(Permissions.Suppliers.Create)]
    public IActionResult Create() => View(new SupplierFormDto());

    [HttpPost, HasPermission(Permissions.Suppliers.Create), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierFormDto model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        try { var id = await service.CreateAsync(model, cancellationToken); TempData["SuccessMessage"] = "Supplier created and assigned to the current store."; return RedirectToAction(nameof(Edit), new { id }); }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException) { ModelState.AddModelError(string.Empty, ex.Message); return View(model); }
    }

    [HttpGet, HasPermission(Permissions.Suppliers.Update)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var model = await service.GetForEditAsync(id, cancellationToken); return model is null ? NotFound() : View(model);
    }

    [HttpPost, HasPermission(Permissions.Suppliers.Update), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, SupplierFormDto model, CancellationToken cancellationToken)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        try { await service.UpdateAsync(model, cancellationToken); TempData["SuccessMessage"] = "Supplier updated."; return RedirectToAction(nameof(Index)); }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException or KeyNotFoundException) { ModelState.AddModelError(string.Empty, ex.Message); return View(model); }
    }

    [HttpPost, HasPermission(Permissions.Suppliers.Delete), ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        try { await service.DeactivateAsync(id, cancellationToken); TempData["SuccessMessage"] = "Supplier deactivated for the current store."; }
        catch (KeyNotFoundException) { return NotFound(); }
        return RedirectToAction(nameof(Index));
    }
}
