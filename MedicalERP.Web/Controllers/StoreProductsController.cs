using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Permissions;
using MedicalERP.Domain.DTOs;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalERP.Web.Controllers;

[Authorize]
public sealed class StoreProductsController : Controller
{
    private readonly IStoreProductService _service;
    private readonly IProductService _productService;
    private readonly IStoreService _storeService;

    public StoreProductsController(
        IStoreProductService service,
        IProductService productService,
        IStoreService storeService)
    {
        _service = service;
        _productService = productService;
        _storeService = storeService;
    }

    [HttpGet]
    [HasPermission(Permissions.Products.View)]
    public async Task<IActionResult> Index(
        Guid? storeId,
        Guid? productId,
        string? search,
        CancellationToken cancellationToken)
    {
        ViewBag.StoreId = storeId;
        ViewBag.ProductId = productId;
        ViewBag.Search = search;
        ViewBag.CompanyContextId = GetCompanyContextId();

        await LoadDropdownsAsync(storeId, productId, cancellationToken);

        var records = await _service.GetAsync(
            storeId,
            productId,
            search,
            cancellationToken);

        return View(records);
    }

    [HttpGet]
    [HasPermission(Permissions.Products.Create)]
    public async Task<IActionResult> Create(
        Guid? storeId,
        Guid? productId,
        CancellationToken cancellationToken)
    {
        var model = new StoreProductFormDto
        {
            StoreId = storeId ?? Guid.Empty,
            ProductId = productId ?? Guid.Empty,
            IsAvailableForSale = true,
            IsActive = true
        };

        await LoadDropdownsAsync(model.StoreId, model.ProductId, cancellationToken);
        ViewBag.CompanyContextId = GetCompanyContextId();

        return View(model);
    }

    [HttpPost]
    [HasPermission(Permissions.Products.Create)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        StoreProductFormDto model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(model.StoreId, model.ProductId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();
            return View(model);
        }

        try
        {
            await _service.CreateAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Store product created successfully.";

            return RedirectToAction(nameof(Index), GetFilterRouteValues(model.StoreId, model.ProductId));
        }
        catch (Exception exception)
            when (exception is InvalidOperationException or KeyNotFoundException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadDropdownsAsync(model.StoreId, model.ProductId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();
            return View(model);
        }
    }

    [HttpGet]
    [HasPermission(Permissions.Products.Update)]
    public async Task<IActionResult> Edit(
        Guid id,
        CancellationToken cancellationToken)
    {
        var model = await _service.GetFormByIdAsync(id, cancellationToken);

        if (model is null)
        {
            return NotFound();
        }

        await LoadDropdownsAsync(model.StoreId, model.ProductId, cancellationToken);
        ViewBag.CompanyContextId = GetCompanyContextId();

        return View(model);
    }

    [HttpPost]
    [HasPermission(Permissions.Products.Update)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        StoreProductFormDto model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(model.StoreId, model.ProductId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();
            return View(model);
        }

        try
        {
            await _service.UpdateAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Store product updated successfully.";

            return RedirectToAction(nameof(Index), GetFilterRouteValues(model.StoreId, model.ProductId));
        }
        catch (Exception exception)
            when (exception is InvalidOperationException or KeyNotFoundException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadDropdownsAsync(model.StoreId, model.ProductId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();
            return View(model);
        }
    }

    [HttpGet]
    [HasPermission(Permissions.Products.Delete)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var model = await _service.GetFormByIdAsync(id, cancellationToken);

        if (model is null)
        {
            return NotFound();
        }

        ViewBag.CompanyContextId = GetCompanyContextId();
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [HasPermission(Permissions.Products.Delete)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        Guid id,
        CancellationToken cancellationToken)
    {
        var model = await _service.GetFormByIdAsync(id, cancellationToken);

        if (model is null)
        {
            return NotFound();
        }

        await _service.DeactivateAsync(id, cancellationToken);
        TempData["SuccessMessage"] = "Store product deactivated successfully.";

        return RedirectToAction(nameof(Index), GetFilterRouteValues(model.StoreId, model.ProductId));
    }

    private async Task LoadDropdownsAsync(
        Guid? selectedStoreId,
        Guid? selectedProductId,
        CancellationToken cancellationToken)
    {
        var stores = await _storeService.GetAsync(
            new QueryParameters { PageSize = 500 },
            cancellationToken);

        ViewBag.Stores = new SelectList(
            stores.Items.Where(x => x.IsActive),
            "Id",
            "Name",
            selectedStoreId);

        var products = await _productService.GetAllAsync(
            null,
            null,
            null,
            true,
            cancellationToken);

        ViewBag.Products = new SelectList(
            products,
            "Id",
            "Name",
            selectedProductId);
    }

    private object? GetFilterRouteValues(Guid? storeId, Guid? productId)
    {
        var companyContextId = GetCompanyContextId();

        return new
        {
            storeId = storeId == Guid.Empty ? null : storeId,
            productId = productId == Guid.Empty ? null : productId,
            companyContextId = string.IsNullOrWhiteSpace(companyContextId)
                ? null
                : companyContextId
        };
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
