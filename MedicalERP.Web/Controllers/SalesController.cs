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
public sealed class SalesController(ISaleService service) : Controller
{
    [HttpGet, HasPermission(Permissions.Sales.View)]
    public async Task<IActionResult> Index(string? search, SaleStatus? status, PaymentStatus? paymentStatus, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var filter = new SaleFilterDto { Search = search, Status = status, PaymentStatus = paymentStatus, Page = page, PageSize = pageSize };
        ViewBag.Search = search; ViewBag.Status = status; ViewBag.PaymentStatus = paymentStatus; ViewBag.CompanyContextId = GetCompanyContextId();
        ViewBag.PaginationRouteValues = new Dictionary<string, object?> { ["search"] = search, ["status"] = status, ["paymentStatus"] = paymentStatus, ["companyContextId"] = GetCompanyContextId() };
        return View(await service.GetAsync(filter, cancellationToken));
    }

    [HttpGet, HasPermission(Permissions.Sales.Create)]
    public async Task<IActionResult> Create(Guid? customerId, CancellationToken cancellationToken)
    {
        var model = new SaleFormDto
        {
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
            CustomerId = customerId
        };
        var sessionId = await service.EnsureOpenRegisterSessionAsync(cancellationToken);
        model.RegisterSessionId = sessionId;
        await LoadLookupsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, HasPermission(Permissions.Sales.Create), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleFormDto model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) { await LoadLookupsAsync(model, cancellationToken); return View(model); }
        try
        {
            var id = await service.CreateAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Sale confirmed and stock updated.";
            return RedirectToAction(nameof(Receipt), new { id, companyContextId = GetCompanyContextId() });
        }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException)
        {
            ModelState.AddModelError(string.Empty, ex.Message); await LoadLookupsAsync(model, cancellationToken); return View(model);
        }
    }

    [HttpGet, HasPermission(Permissions.Sales.View)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var model = await service.GetDetailsAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, HasPermission(Permissions.Sales.Create), ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkPaid(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await service.MarkAsPaidAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Sale marked as paid.";
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException or UnauthorizedAccessException)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (MedicalERP.Domain.Common.ConcurrencyConflictException)
        {
            TempData["ErrorMessage"] = "This sale was already marked as paid or modified by another user.";
        }

        return RedirectToAction(nameof(Details), new { id, companyContextId = GetCompanyContextId() });
    }

    [HttpGet, HasPermission(Permissions.Sales.View)]
    public async Task<IActionResult> Receipt(Guid id, CancellationToken cancellationToken)
    {
        var model = await service.GetDetailsAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpGet, HasPermission(Permissions.Sales.View)]
    public async Task<IActionResult> ReceiptPdf(Guid id, CancellationToken cancellationToken)
    {
        var model = await service.GetDetailsAsync(id, cancellationToken);
        if (model is null) return NotFound();
        var companyName = User.FindFirst("company_name")?.Value ?? "MEDICALERP PHARMACY";
        var bytes = Models.SaleReceiptPdf.Build(model, companyName);
        return File(bytes, "application/pdf", $"Receipt-{model.InvoiceNumber}.pdf");
    }

    [HttpGet, HasPermission(Permissions.Sales.View)]
    public async Task<IActionResult> ProductUnits(Guid productId, CancellationToken cancellationToken) => Json(await service.GetProductUnitsAsync(productId, cancellationToken));

    private async Task LoadLookupsAsync(SaleFormDto model, CancellationToken cancellationToken)
    {
        ViewBag.Customers = new SelectList(await service.GetCustomersAsync(cancellationToken), "Id", "Name", model.CustomerId);
        ViewBag.Products = await service.GetProductsAsync(cancellationToken);
        ViewBag.Warehouses = new SelectList(await service.GetWarehousesAsync(cancellationToken), "Id", "Name", model.WarehouseId);
        ViewBag.PaymentMethods = new SelectList(await service.GetPaymentMethodsAsync(cancellationToken), "Id", "Name", model.PaymentMethodId);
        ViewBag.RegisterSessions = new SelectList(await service.GetRegisterSessionsAsync(cancellationToken), "Id", "Name", model.RegisterSessionId);
    }

    private string? GetCompanyContextId() => Request.Query["companyContextId"].FirstOrDefault();
}
