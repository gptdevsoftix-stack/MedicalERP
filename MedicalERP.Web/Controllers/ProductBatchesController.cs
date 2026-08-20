using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Inventory.Dtos;
using MedicalERP.Application.Permissions;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalERP.Web.Controllers;

[Authorize]
public sealed class ProductBatchesController : Controller
{
    private readonly IProductBatchService _service;

    public ProductBatchesController(IProductBatchService service)
    {
        _service = service;
    }

    [HttpGet]
    [HasPermission(Permissions.Inventory.View)]
    public async Task<IActionResult> Index(
        Guid? productId,
        Guid? warehouseId,
        string? search,
        bool? isActive,
        DateOnly? expiringBefore,
        int page = 1,
        int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var filter = new ProductBatchFilterDto
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            Search = search,
            IsActive = isActive,
            ExpiringBefore = expiringBefore,
            Page = page,
            PageSize = pageSize
        };

        await LoadDropdownsAsync(productId, warehouseId, cancellationToken);
        SetViewState(filter);

        var records = await _service.GetAsync(filter, cancellationToken);
        return View(records);
    }

    [HttpGet]
    [HasPermission(Permissions.Inventory.Adjust)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new ProductBatchFormDto
        {
            ReceivedAt = DateTimeOffset.Now,
            IsActive = true
        };

        await LoadDropdownsAsync(model.ProductId, model.WarehouseId, cancellationToken);
        ViewBag.CompanyContextId = GetCompanyContextId();

        return View(model);
    }

    [HttpPost]
    [HasPermission(Permissions.Inventory.Adjust)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ProductBatchFormDto model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(model.ProductId, model.WarehouseId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();
            return View(model);
        }

        try
        {
            await _service.CreateAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Product batch created successfully.";

            return RedirectToAction(nameof(Index), GetCompanyRouteValues());
        }
        catch (Exception exception)
            when (exception is InvalidOperationException or KeyNotFoundException or UnauthorizedAccessException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadDropdownsAsync(model.ProductId, model.WarehouseId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();
            return View(model);
        }
    }

    [HttpGet]
    [HasPermission(Permissions.Inventory.Adjust)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var model = await _service.GetFormByIdAsync(id, cancellationToken);

        if (model is null)
        {
            return NotFound();
        }

        await LoadDropdownsAsync(model.ProductId, model.WarehouseId, cancellationToken);
        ViewBag.CompanyContextId = GetCompanyContextId();

        return View(model);
    }

    [HttpPost]
    [HasPermission(Permissions.Inventory.Adjust)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        ProductBatchFormDto model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(model.ProductId, model.WarehouseId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();
            return View(model);
        }

        try
        {
            await _service.UpdateAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Product batch updated successfully.";

            return RedirectToAction(nameof(Index), GetCompanyRouteValues());
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception exception)
            when (exception is InvalidOperationException or UnauthorizedAccessException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadDropdownsAsync(model.ProductId, model.WarehouseId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();
            return View(model);
        }
    }

    [HttpGet]
    [HasPermission(Permissions.Inventory.Adjust)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var model = await _service.GetByIdAsync(id, cancellationToken);

        if (model is null)
        {
            return NotFound();
        }

        ViewBag.CompanyContextId = GetCompanyContextId();
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [HasPermission(Permissions.Inventory.Adjust)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _service.DeactivateAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Product batch deactivated successfully.";

            return RedirectToAction(nameof(Index), GetCompanyRouteValues());
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private async Task LoadDropdownsAsync(
        Guid? selectedProductId,
        Guid? selectedWarehouseId,
        CancellationToken cancellationToken)
    {
        ViewBag.Products = new SelectList(
            await _service.GetProductLookupsAsync(cancellationToken),
            "Id",
            "Name",
            selectedProductId);

        ViewBag.Warehouses = new SelectList(
            await _service.GetWarehouseLookupsAsync(cancellationToken),
            "Id",
            "Name",
            selectedWarehouseId);
    }

    private void SetViewState(ProductBatchFilterDto filter)
    {
        ViewBag.ProductId = filter.ProductId;
        ViewBag.WarehouseId = filter.WarehouseId;
        ViewBag.Search = filter.Search;
        ViewBag.IsActive = filter.IsActive;
        ViewBag.ExpiringBefore = filter.ExpiringBefore;
        ViewBag.CompanyContextId = GetCompanyContextId();
        ViewBag.PaginationRouteValues = new Dictionary<string, object?>
        {
            ["productId"] = filter.ProductId,
            ["warehouseId"] = filter.WarehouseId,
            ["search"] = filter.Search,
            ["isActive"] = filter.IsActive,
            ["expiringBefore"] = filter.ExpiringBefore,
            ["companyContextId"] = GetCompanyContextId()
        };
    }

    private object? GetCompanyRouteValues()
    {
        var companyContextId = GetCompanyContextId();
        return string.IsNullOrWhiteSpace(companyContextId)
            ? null
            : new { companyContextId };
    }

    private string? GetCompanyContextId()
    {
        var companyContextId = Request.Query["companyContextId"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(companyContextId) && Request.HasFormContentType)
        {
            companyContextId = Request.Form["companyContextId"].FirstOrDefault();
        }

        return companyContextId;
    }
}
