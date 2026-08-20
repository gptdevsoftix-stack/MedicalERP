using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Permissions;
using MedicalERP.Domain.DTOs;
using MedicalERP.Domain.Enums;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalERP.Web.Controllers;

[Authorize]
public abstract class CatalogMastersController : Controller
{
    private readonly ICatalogMasterService _service;

    protected CatalogMastersController(ICatalogMasterService service)
    {
        _service = service;
    }

    protected abstract CatalogMasterType MasterType { get; }
    protected abstract string Title { get; }
    protected abstract string SingularTitle { get; }

    [HttpGet]
    [HasPermission(Permissions.Products.View)]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        SetViewData(search);

        var result = await _service.GetAllPagedAsync(
            MasterType,
            search,
            page,
            pageSize,
            cancellationToken);

        ViewBag.PaginationRouteValues = new Dictionary<string, object?>
        {
            ["search"] = search,
            ["companyContextId"] = GetCompanyContextId()
        };

        return View("~/Views/CatalogMasters/Index.cshtml", result);
    }

    [HttpGet]
    [HasPermission(Permissions.Products.View)]
    public async Task<IActionResult> Details(
        Guid id,
        CancellationToken cancellationToken)
    {
        SetViewData();

        var record = await _service.GetByIdAsync(
            MasterType,
            id,
            cancellationToken);

        return record is null
            ? NotFound()
            : View("~/Views/CatalogMasters/Details.cshtml", record);
    }

    [HttpGet]
    [HasPermission(Permissions.Products.Create)]
    public IActionResult Create()
    {
        SetViewData();

        return View(
            "~/Views/CatalogMasters/Create.cshtml",
            new CatalogMasterFormDto { MasterType = MasterType });
    }

    [HttpPost]
    [HasPermission(Permissions.Products.Create)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CatalogMasterFormDto request,
        CancellationToken cancellationToken)
    {
        request.MasterType = MasterType;

        if (!ModelState.IsValid)
        {
            SetViewData();
            return View("~/Views/CatalogMasters/Create.cshtml", request);
        }

        try
        {
            await _service.CreateAsync(request, cancellationToken);
            TempData["SuccessMessage"] = $"{SingularTitle} created successfully.";

            return RedirectToAction(nameof(Index), GetCompanyRouteValues());
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            SetViewData();

            return View("~/Views/CatalogMasters/Create.cshtml", request);
        }
    }

    [HttpGet]
    [HasPermission(Permissions.Products.Update)]
    public async Task<IActionResult> Edit(
        Guid id,
        CancellationToken cancellationToken)
    {
        SetViewData();

        var record = await _service.GetForEditAsync(
            MasterType,
            id,
            cancellationToken);

        return record is null
            ? NotFound()
            : View("~/Views/CatalogMasters/Edit.cshtml", record);
    }

    [HttpPost]
    [HasPermission(Permissions.Products.Update)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        CatalogMasterFormDto request,
        CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest();

        request.MasterType = MasterType;

        if (!ModelState.IsValid)
        {
            SetViewData();
            return View("~/Views/CatalogMasters/Edit.cshtml", request);
        }

        try
        {
            await _service.UpdateAsync(request, cancellationToken);
            TempData["SuccessMessage"] = $"{SingularTitle} updated successfully.";

            return RedirectToAction(nameof(Index), GetCompanyRouteValues());
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            SetViewData();

            return View("~/Views/CatalogMasters/Edit.cshtml", request);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    [HasPermission(Permissions.Products.Delete)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        SetViewData();

        var record = await _service.GetByIdAsync(
            MasterType,
            id,
            cancellationToken);

        return record is null
            ? NotFound()
            : View("~/Views/CatalogMasters/Delete.cshtml", record);
    }

    [HttpPost, ActionName("Delete")]
    [HasPermission(Permissions.Products.Delete)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _service.DeactivateAsync(MasterType, id, cancellationToken);
            TempData["SuccessMessage"] = $"{SingularTitle} deactivated successfully.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index), GetCompanyRouteValues());
    }

    private void SetViewData(string? search = null)
    {
        ViewBag.Search = search;
        ViewBag.Title = Title;
        ViewBag.SingularTitle = SingularTitle;
        ViewBag.MasterType = MasterType;
        ViewBag.CompanyContextId = GetCompanyContextId();
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

public sealed class ProductBrandsController : CatalogMastersController
{
    public ProductBrandsController(ICatalogMasterService service) : base(service) { }
    protected override CatalogMasterType MasterType => CatalogMasterType.ProductBrand;
    protected override string Title => "Product Brands";
    protected override string SingularTitle => "Product Brand";
}

public sealed class ManufacturersController : CatalogMastersController
{
    public ManufacturersController(ICatalogMasterService service) : base(service) { }
    protected override CatalogMasterType MasterType => CatalogMasterType.Manufacturer;
    protected override string Title => "Manufacturers";
    protected override string SingularTitle => "Manufacturer";
}

public sealed class GenericMedicinesController : CatalogMastersController
{
    public GenericMedicinesController(ICatalogMasterService service) : base(service) { }
    protected override CatalogMasterType MasterType => CatalogMasterType.GenericMedicine;
    protected override string Title => "Generic Medicines";
    protected override string SingularTitle => "Generic Medicine";
}

public sealed class DosageFormsController : CatalogMastersController
{
    public DosageFormsController(ICatalogMasterService service) : base(service) { }
    protected override CatalogMasterType MasterType => CatalogMasterType.DosageForm;
    protected override string Title => "Dosage Forms";
    protected override string SingularTitle => "Dosage Form";
}

public sealed class StrengthsController : CatalogMastersController
{
    public StrengthsController(ICatalogMasterService service) : base(service) { }
    protected override CatalogMasterType MasterType => CatalogMasterType.Strength;
    protected override string Title => "Strengths";
    protected override string SingularTitle => "Strength";
}

public sealed class UnitsController : CatalogMastersController
{
    public UnitsController(ICatalogMasterService service) : base(service) { }
    protected override CatalogMasterType MasterType => CatalogMasterType.Unit;
    protected override string Title => "Units";
    protected override string SingularTitle => "Unit";
}
