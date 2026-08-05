using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Permissions;
using MedicalERP.Domain.DTOs;
using MedicalERP.Domain.Enums;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalERP.Web.Controllers;

[Authorize]
public sealed class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [HasPermission(Permissions.Products.View)]
    public async Task<IActionResult> Index(
        string? search,
        Guid? categoryId,
        bool? isMedicine,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;
        ViewBag.IsMedicine = isMedicine;
        ViewBag.IsActive = isActive;
        ViewBag.CompanyContextId = GetCompanyContextId();

        await LoadCategoryFilterAsync(categoryId, cancellationToken);

        var products = await _productService.GetAllAsync(
            search,
            categoryId,
            isMedicine,
            isActive,
            cancellationToken);

        return View(products);
    }

    [HttpGet]
    [HasPermission(Permissions.Products.View)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        ViewBag.CompanyContextId = GetCompanyContextId();

        var product = await _productService.GetByIdAsync(id, cancellationToken);

        return product is null ? NotFound() : View(product);
    }

    [HttpGet]
    [HasPermission(Permissions.Products.Create)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewBag.CompanyContextId = GetCompanyContextId();

        var model = new ProductFormDto();
        await LoadDropdownsAsync(model, cancellationToken);

        return View(model);
    }

    [HttpPost]
    [HasPermission(Permissions.Products.Create)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CompanyContextId = GetCompanyContextId();
            await LoadDropdownsAsync(request, cancellationToken);
            return View(request);
        }

        try
        {
            var id = await _productService.CreateAsync(request, cancellationToken);
            TempData["SuccessMessage"] = "Product created successfully.";

            return RedirectToAction(nameof(Details), GetRouteValues(id));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            ViewBag.CompanyContextId = GetCompanyContextId();
            await LoadDropdownsAsync(request, cancellationToken);

            return View(request);
        }
    }

    [HttpGet]
    [HasPermission(Permissions.Products.Update)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        ViewBag.CompanyContextId = GetCompanyContextId();

        var product = await _productService.GetForEditAsync(id, cancellationToken);

        if (product is null)
            return NotFound();

        await LoadDropdownsAsync(product, cancellationToken);

        return View(product);
    }

    [HttpPost]
    [HasPermission(Permissions.Products.Update)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        ProductFormDto request,
        CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.CompanyContextId = GetCompanyContextId();
            await LoadDropdownsAsync(request, cancellationToken);
            return View(request);
        }

        try
        {
            await _productService.UpdateAsync(request, cancellationToken);
            TempData["SuccessMessage"] = "Product updated successfully.";

            return RedirectToAction(nameof(Details), GetRouteValues(id));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            ViewBag.CompanyContextId = GetCompanyContextId();
            await LoadDropdownsAsync(request, cancellationToken);

            return View(request);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    [HasPermission(Permissions.Products.Delete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        ViewBag.CompanyContextId = GetCompanyContextId();

        var product = await _productService.GetByIdAsync(id, cancellationToken);

        return product is null ? NotFound() : View(product);
    }

    [HttpPost, ActionName("Delete")]
    [HasPermission(Permissions.Products.Delete)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _productService.DeactivateAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Product deactivated successfully.";

            return RedirectToAction(nameof(Index), GetCompanyRouteValues());
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private async Task LoadDropdownsAsync(ProductFormDto model, CancellationToken cancellationToken)
    {
        ViewBag.Categories = CreateSelectList(
            await _productService.GetCategoriesAsync(cancellationToken),
            model.CategoryId);

        ViewBag.ProductBrands = CreateSelectList(
            await _productService.GetBrandsAsync(cancellationToken),
            model.ProductBrandId);

        ViewBag.Manufacturers = CreateSelectList(
            await _productService.GetManufacturersAsync(cancellationToken),
            model.ManufacturerId);

        ViewBag.GenericMedicines = CreateSelectList(
            await _productService.GetGenericMedicinesAsync(cancellationToken),
            model.GenericMedicineId);

        ViewBag.DosageForms = CreateSelectList(
            await _productService.GetDosageFormsAsync(cancellationToken),
            model.DosageFormId);

        ViewBag.Strengths = CreateSelectList(
            await _productService.GetStrengthsAsync(cancellationToken),
            model.StrengthId);

        ViewBag.Units = CreateSelectList(
            await _productService.GetUnitsAsync(cancellationToken),
            model.BaseUnitId);

        ViewBag.ProductTypes = new SelectList(
            Enum.GetValues<ProductType>()
                .Select(x => new
                {
                    Value = x,
                    Text = x.ToString()
                }),
            "Value",
            "Text",
            model.ProductType);
    }

    private async Task LoadCategoryFilterAsync(Guid? selectedId, CancellationToken cancellationToken)
    {
        ViewBag.Categories = CreateSelectList(
            await _productService.GetCategoriesAsync(cancellationToken),
            selectedId);
    }

    private static List<SelectListItem> CreateSelectList(
        IEnumerable<ProductLookupDto> records,
        Guid? selectedId)
    {
        return records.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name,
            Selected = x.Id == selectedId
        }).ToList();
    }

    private object GetRouteValues(Guid id)
    {
        var companyContextId = GetCompanyContextId();

        return string.IsNullOrWhiteSpace(companyContextId)
            ? new { id }
            : new { id, companyContextId };
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
            companyContextId = Request.Form["companyContextId"].FirstOrDefault();

        return companyContextId;
    }
}
