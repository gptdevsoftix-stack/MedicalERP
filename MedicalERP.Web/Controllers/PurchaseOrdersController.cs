using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Permissions;
using MedicalERP.Application.Purchases.Dtos;
using MedicalERP.Application.Warehouses.Dtos;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.DTOs;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalERP.Web.Controllers;

[Authorize]
public sealed class PurchaseOrdersController(
    IPurchaseOrderService service,
    ISupplierService supplierService,
    IWarehouseService warehouseService,
    IProductService productService,
    IProductUnitService productUnitService,
    IStoreContext storeContext) : Controller
{
    [HttpGet, HasPermission(Permissions.Purchases.View)]
    public async Task<IActionResult> Index(string? search, OrderStatus? status, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var filter = new PurchaseOrderFilterDto { Search = search, Status = status, Page = page, PageSize = pageSize };
        ViewBag.Search = search; ViewBag.Status = status; ViewBag.CompanyContextId = GetCompanyContextId();
        ViewBag.PaginationRouteValues = new Dictionary<string, object?> { ["search"] = search, ["status"] = status, ["companyContextId"] = GetCompanyContextId() };
        return View(await service.GetAsync(filter, cancellationToken));
    }

    [HttpGet, HasPermission(Permissions.Purchases.Create)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new PurchaseOrderFormDto { OrderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}" };
        await LoadLookupsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, HasPermission(Permissions.Purchases.Create), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PurchaseOrderFormDto model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) { await LoadLookupsAsync(model, cancellationToken); return View(model); }
        try
        {
            var id = await service.CreateAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Purchase order draft created.";
            return RedirectToAction(nameof(Details), new { id, companyContextId = GetCompanyContextId() });
        }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException)
        {
            ModelState.AddModelError(string.Empty, ex.Message); await LoadLookupsAsync(model, cancellationToken); return View(model);
        }
    }

    [HttpGet, HasPermission(Permissions.Purchases.Update)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var model = await service.GetForEditAsync(id, cancellationToken);
        if (model is null) return NotFound();
        if (model.Status != OrderStatus.Draft) return RedirectToAction(nameof(Details), new { id });
        await LoadLookupsAsync(model, cancellationToken); return View(model);
    }

    [HttpPost, HasPermission(Permissions.Purchases.Update), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, PurchaseOrderFormDto model, CancellationToken cancellationToken)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) { await LoadLookupsAsync(model, cancellationToken); return View(model); }
        try
        {
            await service.UpdateAsync(model, cancellationToken); TempData["SuccessMessage"] = "Purchase order draft updated.";
            return RedirectToAction(nameof(Details), new { id, companyContextId = GetCompanyContextId() });
        }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException or KeyNotFoundException)
        {
            ModelState.AddModelError(string.Empty, ex.Message); await LoadLookupsAsync(model, cancellationToken); return View(model);
        }
    }

    [HttpGet, HasPermission(Permissions.Purchases.View)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var model = await service.GetDetailsAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, HasPermission(Permissions.Purchases.Update), ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken) => await Transition(id, service.SubmitAsync, "Purchase order submitted.", cancellationToken);

    [HttpPost, HasPermission(Permissions.Purchases.Approve), ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken) => await Transition(id, service.ApproveAsync, "Purchase order approved.", cancellationToken);

    [HttpPost, HasPermission(Permissions.Purchases.Update), ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken) => await Transition(id, service.CancelAsync, "Purchase order cancelled.", cancellationToken);

    [HttpGet, HasPermission(Permissions.Purchases.View)]
    public async Task<IActionResult> ProductUnits(Guid productId, CancellationToken cancellationToken) => Json(await service.GetProductUnitsAsync(productId, cancellationToken));

    private async Task<IActionResult> Transition(Guid id, Func<Guid, CancellationToken, Task> action, string message, CancellationToken cancellationToken)
    {
        try { await action(id, cancellationToken); TempData["SuccessMessage"] = message; }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException or KeyNotFoundException) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToAction(nameof(Details), new { id, companyContextId = GetCompanyContextId() });
    }

    private async Task LoadLookupsAsync(PurchaseOrderFormDto model, CancellationToken cancellationToken)
    {
        ViewBag.Suppliers = new SelectList(await service.GetSuppliersAsync(cancellationToken), "Id", "Name", model.SupplierId);
        ViewBag.Products = await service.GetProductsAsync(cancellationToken);
        ViewBag.Warehouses = new SelectList(await service.GetWarehousesAsync(cancellationToken), "Id", "Name", model.WarehouseId);
    }

    private string? GetCompanyContextId() => Request.Query["companyContextId"].FirstOrDefault();

    [HttpGet("PurchaseOrders/QuickCreateSupplier")]
    [HasPermission(Permissions.Purchases.Create)]
    public IActionResult QuickCreateSupplier()
    {
        return PartialView("~/Views/PurchaseOrders/_QuickCreateSupplier.cshtml");
    }

    [HttpPost("PurchaseOrders/QuickCreateSupplier")]
    [IgnoreAntiforgeryToken]
    [HasPermission(Permissions.Purchases.Create)]
    public async Task<IActionResult> QuickCreateSupplier(
        [FromBody] QuickCreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = new SupplierFormDto
            {
                Name = request.Name,
                Code = request.Code,
                ContactPerson = request.ContactPerson,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address
            };
            var id = await supplierService.CreateAsync(dto, cancellationToken);
            var displayName = $"{request.Name} ({request.Code})";
            return Ok(ApiResponse<object>.Ok(new { id, name = displayName }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("PurchaseOrders/QuickCreateWarehouse")]
    [HasPermission(Permissions.Purchases.Create)]
    public IActionResult QuickCreateWarehouse()
    {
        return PartialView("~/Views/PurchaseOrders/_QuickCreateWarehouse.cshtml");
    }

    [HttpPost("PurchaseOrders/QuickCreateWarehouse")]
    [IgnoreAntiforgeryToken]
    [HasPermission(Permissions.Purchases.Create)]
    public async Task<IActionResult> QuickCreateWarehouse(
        [FromBody] QuickCreateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var storeId = storeContext.SelectedStoreId
                ?? throw new InvalidOperationException("No store selected.");
            var dto = new CreateWarehouseRequest(
                storeId,
                request.Name,
                request.Code,
                request.WarehouseType,
                request.Address,
                false);
            var result = await warehouseService.CreateAsync(dto, cancellationToken);
            var displayName = $"{request.Name} ({request.Code})";
            return Ok(ApiResponse<object>.Ok(new { id = result.Id, name = displayName }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("PurchaseOrders/QuickCreateProductUnit")]
    [HasPermission(Permissions.Purchases.Create)]
    public async Task<IActionResult> QuickCreateProductUnit(Guid productId, CancellationToken cancellationToken)
    {
        if (productId == Guid.Empty) return BadRequest();
        ViewBag.Units = new SelectList(await productService.GetUnitsAsync(cancellationToken), "Id", "Name");
        return PartialView("~/Views/PurchaseOrders/_QuickCreateProductUnit.cshtml", new ProductUnitFormDto
        {
            ProductId = productId,
            ConversionFactor = 1,
            IsPurchaseUnit = true,
            IsActive = true
        });
    }

    [HttpPost("PurchaseOrders/QuickCreateProductUnit")]
    [IgnoreAntiforgeryToken]
    [HasPermission(Permissions.Purchases.Create)]
    public async Task<IActionResult> QuickCreateProductUnit([FromBody] ProductUnitFormDto request, CancellationToken cancellationToken)
    {
        try
        {
            request.IsPurchaseUnit = true;
            request.IsActive = true;
            var id = await productUnitService.CreateAsync(request, cancellationToken);
            var created = (await productUnitService.GetByProductIdAsync(request.ProductId, cancellationToken)).Single(x => x.Id == id);
            return Ok(ApiResponse<object>.Ok(new { id, name = $"{created.UnitName} x {created.ConversionFactor:0.####}" }));
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}

public sealed class QuickCreateSupplierRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}

public sealed class QuickCreateWarehouseRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public WarehouseType WarehouseType { get; set; } = WarehouseType.Main;
    public string? Address { get; set; }
}
