using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Permissions;
using MedicalERP.Application.Sales.Dtos;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalERP.Web.Controllers;

[Authorize]
public sealed class CustomersController(ICustomerService service) : Controller
{
    [HttpGet, HasPermission(Permissions.Customers.View)]
    public async Task<IActionResult> Index(string? search, bool? isActive, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        ViewBag.Search = search; ViewBag.IsActive = isActive; ViewBag.CompanyContextId = Request.Query["companyContextId"].FirstOrDefault();
        ViewBag.PaginationRouteValues = new Dictionary<string, object?> { ["search"] = search, ["isActive"] = isActive, ["companyContextId"] = ViewBag.CompanyContextId };
        return View(await service.GetAsync(search, isActive, page, pageSize, cancellationToken));
    }

    [HttpGet, HasPermission(Permissions.Customers.Create)]
    public IActionResult Create() => View(new CustomerFormDto());

    [HttpPost, HasPermission(Permissions.Customers.Create), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerFormDto model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        try { var id = await service.CreateAsync(model, cancellationToken); TempData["SuccessMessage"] = "Customer created."; return RedirectToAction(nameof(Edit), new { id }); }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException) { ModelState.AddModelError(string.Empty, ex.Message); return View(model); }
    }

    [HttpGet, HasPermission(Permissions.Customers.Update)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var model = await service.GetForEditAsync(id, cancellationToken); return model is null ? NotFound() : View(model);
    }

    [HttpPost, HasPermission(Permissions.Customers.Update), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CustomerFormDto model, CancellationToken cancellationToken)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        try { await service.UpdateAsync(model, cancellationToken); TempData["SuccessMessage"] = "Customer updated."; return RedirectToAction(nameof(Index)); }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException or KeyNotFoundException) { ModelState.AddModelError(string.Empty, ex.Message); return View(model); }
    }

    [HttpPost, HasPermission(Permissions.Customers.Update), ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        try { await service.DeactivateAsync(id, cancellationToken); TempData["SuccessMessage"] = "Customer deactivated."; }
        catch (KeyNotFoundException) { return NotFound(); }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet, HasPermission(Permissions.Customers.Create)]
    public IActionResult QuickCreate() => PartialView("_QuickCreate", new CustomerFormDto());

    [HttpPost, HasPermission(Permissions.Customers.Create)]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> QuickCreate(CustomerFormDto model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return Json(new { success = false, message = "Validation error.", errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()) });
        }
        try
        {
            var id = await service.CreateAsync(model, cancellationToken);
            return Json(new { success = true, id, name = $"{model.Name} ({model.Code})" });
        }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
