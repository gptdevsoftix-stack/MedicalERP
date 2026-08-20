using MedicalERP.Application.Common;
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
    private readonly ICategoryService _categoryService;
    private readonly ICatalogMasterService _catalogMasterService;

    public ProductsController(
        IProductService productService,
        ICategoryService categoryService,
        ICatalogMasterService catalogMasterService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _catalogMasterService = catalogMasterService;
    }

    [HttpGet]
    [HasPermission(Permissions.Products.View)]
    public async Task<IActionResult> Index(
        string? search,
        Guid? categoryId,
        bool? isMedicine,
        bool? isActive,
        int page = 1,
        int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;
        ViewBag.IsMedicine = isMedicine;
        ViewBag.IsActive = isActive;
        ViewBag.CompanyContextId = GetCompanyContextId();

        await LoadCategoryFilterAsync(categoryId, cancellationToken);

        var result = await _productService.GetAllPagedAsync(
            search,
            categoryId,
            isMedicine,
            isActive,
            page,
            pageSize,
            cancellationToken);

        ViewBag.PaginationRouteValues = GetPaginationRouteValues(search, categoryId, isMedicine, isActive);

        return View(result);
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

    private Dictionary<string, object?> GetPaginationRouteValues(
        string? search,
        Guid? categoryId,
        bool? isMedicine,
        bool? isActive)
    {
        var companyContextId = GetCompanyContextId();
        return new Dictionary<string, object?>
        {
            ["search"] = search,
            ["categoryId"] = categoryId,
            ["isMedicine"] = isMedicine,
            ["isActive"] = isActive,
            ["companyContextId"] = companyContextId
        };
    }

    [HttpGet("Products/QuickCreate")]
    [HasPermission(Permissions.Products.Create)]
    public IActionResult QuickCreate(CatalogMasterType masterType)
    {
        ViewBag.MasterType = masterType;
        return PartialView("~/Views/Products/_QuickCreateCatalogMaster.cshtml");
    }

    [HttpPost("Products/QuickCreate")]
    [IgnoreAntiforgeryToken]
    [HasPermission(Permissions.Products.Create)]
    public async Task<IActionResult> QuickCreate(
        [FromBody] QuickCreateCatalogMasterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.MasterType == CatalogMasterType.Category)
            {
                var dto = new CreateCategoryDto
                {
                    Name = request.Name,
                    Code = request.Code ?? request.Name
                };
                var id = await _categoryService.CreateAsync(dto, cancellationToken);
                return Ok(ApiResponse<object>.Ok(new { id, name = request.Name }));
            }

            var formDto = new CatalogMasterFormDto
            {
                MasterType = request.MasterType,
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                LicenseNumber = request.LicenseNumber,
                Value = request.Value,
                MeasurementUnit = request.MeasurementUnit,
                Symbol = request.Symbol,
                AllowsDecimal = request.AllowsDecimal
            };
            var resultId = await _catalogMasterService.CreateAsync(formDto, cancellationToken);
            return Ok(ApiResponse<object>.Ok(new { id = resultId, name = request.Name }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    private string? GetCompanyContextId()
    {
        var companyContextId = Request.Query["companyContextId"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(companyContextId) && Request.HasFormContentType)
            companyContextId = Request.Form["companyContextId"].FirstOrDefault();

        return companyContextId;
    }
}

public sealed class QuickCreateCatalogMasterRequest
{
    public CatalogMasterType MasterType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? LicenseNumber { get; set; }
    public decimal? Value { get; set; }
    public string? MeasurementUnit { get; set; }
    public string? Symbol { get; set; }
    public bool AllowsDecimal { get; set; }
}
