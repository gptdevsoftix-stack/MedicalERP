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
public sealed class ProductBarcodesController : Controller
{
    private readonly IProductBarcodeService _service;
    private readonly IProductService _productService;
    private readonly IProductUnitService _productUnitService;

    public ProductBarcodesController(
        IProductBarcodeService service,
        IProductService productService,
        IProductUnitService productUnitService)
    {
        _service = service;
        _productService = productService;
        _productUnitService = productUnitService;
    }

    [HttpGet]
    [HasPermission(Permissions.Products.View)]
    public async Task<IActionResult> Index(
        Guid? productId,
        string? search,
        CancellationToken cancellationToken)
    {
        ViewBag.ProductId = productId;
        ViewBag.Search = search;
        ViewBag.CompanyContextId = GetCompanyContextId();

        await LoadProductsAsync(productId, cancellationToken);

        var records = await _service.GetAsync(
            productId,
            search,
            cancellationToken);

        return View(records);
    }

    [HttpGet]
    [HasPermission(Permissions.Products.Create)]
    public async Task<IActionResult> Create(
        Guid? productId,
        CancellationToken cancellationToken)
    {
        var model = new ProductBarcodeFormDto
        {
            ProductId = productId ?? Guid.Empty,
            IsActive = true
        };

        await LoadDropdownsAsync(model.ProductId, model.ProductUnitId, cancellationToken);
        ViewBag.CompanyContextId = GetCompanyContextId();

        return View(model);
    }

    [HttpPost]
    [HasPermission(Permissions.Products.Create)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ProductBarcodeFormDto model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(model.ProductId, model.ProductUnitId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();
            return View(model);
        }

        try
        {
            await _service.CreateAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Barcode created successfully.";

            return RedirectToAction(nameof(Index), GetProductRouteValues(model.ProductId));
        }
        catch (Exception exception)
            when (exception is InvalidOperationException or KeyNotFoundException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadDropdownsAsync(model.ProductId, model.ProductUnitId, cancellationToken);
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

        await LoadDropdownsAsync(model.ProductId, model.ProductUnitId, cancellationToken);
        ViewBag.CompanyContextId = GetCompanyContextId();

        return View(model);
    }

    [HttpPost]
    [HasPermission(Permissions.Products.Update)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        ProductBarcodeFormDto model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(model.ProductId, model.ProductUnitId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();
            return View(model);
        }

        try
        {
            await _service.UpdateAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Barcode updated successfully.";

            return RedirectToAction(nameof(Index), GetProductRouteValues(model.ProductId));
        }
        catch (Exception exception)
            when (exception is InvalidOperationException or KeyNotFoundException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadDropdownsAsync(model.ProductId, model.ProductUnitId, cancellationToken);
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
        TempData["SuccessMessage"] = "Barcode deactivated successfully.";

        return RedirectToAction(nameof(Index), GetProductRouteValues(model.ProductId));
    }

    private async Task LoadDropdownsAsync(
        Guid productId,
        Guid? selectedProductUnitId,
        CancellationToken cancellationToken)
    {
        await LoadProductsAsync(productId == Guid.Empty ? null : productId, cancellationToken);

        var units = productId == Guid.Empty
            ? Array.Empty<ProductUnitListDto>()
            : await _productUnitService.GetByProductIdAsync(productId, cancellationToken);

        ViewBag.ProductUnits = new SelectList(
            units,
            "Id",
            "UnitName",
            selectedProductUnitId);
    }

    private async Task LoadProductsAsync(
        Guid? selectedProductId,
        CancellationToken cancellationToken)
    {
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

    private object? GetProductRouteValues(Guid? productId)
    {
        var companyContextId = GetCompanyContextId();

        if (productId.HasValue && productId.Value != Guid.Empty)
        {
            return string.IsNullOrWhiteSpace(companyContextId)
                ? new { productId }
                : new { productId, companyContextId };
        }

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
