using MedicalERP.Application.Interfaces;
using MedicalERP.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalERP.Web.Controllers;

[Authorize]
public sealed class ProductUnitsController : Controller
{
    private readonly IProductUnitService _service;
    private readonly IProductService _productService;

    public ProductUnitsController(
        IProductUnitService service,
        IProductService productService)
    {
        _service = service;
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        Guid? productId,
        CancellationToken cancellationToken)
    {
        ViewBag.CompanyContextId = GetCompanyContextId();

        if (!productId.HasValue || productId.Value == Guid.Empty)
        {
            ViewBag.Products = await _productService.GetAllAsync(
                null,
                null,
                null,
                true,
                cancellationToken);

            return View(Array.Empty<ProductUnitListDto>());
        }

        var product = await _productService.GetByIdAsync(
            productId.Value,
            cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        var records = await _service.GetByProductIdAsync(
            productId.Value,
            cancellationToken);

        ViewBag.ProductId = productId.Value;
        ViewBag.ProductName = product.Name;

        return View(records);
    }

    [HttpGet]
    public async Task<IActionResult> Create(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var model = new ProductUnitFormDto
        {
            ProductId = productId,
            ConversionFactor = 1,
            IsPurchaseUnit = true,
            IsSaleUnit = true,
            IsActive = true
        };

        await LoadUnitsAsync(null, cancellationToken);
        ViewBag.CompanyContextId = GetCompanyContextId();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ProductUnitFormDto model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadUnitsAsync(model.UnitId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();

            return View(model);
        }

        try
        {
            await _service.CreateAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Product unit created successfully.";

            return RedirectToAction(nameof(Index), GetProductRouteValues(model.ProductId));
        }
        catch (Exception exception)
            when (exception is InvalidOperationException or KeyNotFoundException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadUnitsAsync(model.UnitId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        Guid id,
        CancellationToken cancellationToken)
    {
        var model = await _service.GetFormByIdAsync(id, cancellationToken);

        if (model is null)
        {
            return NotFound();
        }

        await LoadUnitsAsync(model.UnitId, cancellationToken);
        ViewBag.CompanyContextId = GetCompanyContextId();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        ProductUnitFormDto model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await LoadUnitsAsync(model.UnitId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();

            return View(model);
        }

        try
        {
            await _service.UpdateAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Product unit updated successfully.";

            return RedirectToAction(nameof(Index), GetProductRouteValues(model.ProductId));
        }
        catch (Exception exception)
            when (exception is InvalidOperationException or KeyNotFoundException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadUnitsAsync(model.UnitId, cancellationToken);
            ViewBag.CompanyContextId = GetCompanyContextId();

            return View(model);
        }
    }

    [HttpGet]
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

    [HttpPost]
    [ActionName("Delete")]
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

        try
        {
            await _service.DeleteAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Product unit deleted successfully.";

            return RedirectToAction(nameof(Index), GetProductRouteValues(model.ProductId));
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;

            return RedirectToAction(nameof(Index), GetProductRouteValues(model.ProductId));
        }
    }

    private async Task LoadUnitsAsync(
        Guid? selectedId,
        CancellationToken cancellationToken)
    {
        var units = await _productService.GetUnitsAsync(cancellationToken);

        ViewBag.Units = new SelectList(
            units,
            "Id",
            "Name",
            selectedId);
    }

    private object GetProductRouteValues(Guid productId)
    {
        var companyContextId = GetCompanyContextId();

        return string.IsNullOrWhiteSpace(companyContextId)
            ? new { productId }
            : new { productId, companyContextId };
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
