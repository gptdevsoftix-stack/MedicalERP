using MedicalERP.Application.Interfaces;
using MedicalERP.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalERP.Web.Controllers;

[Authorize]
public sealed class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? search,
        CancellationToken cancellationToken)
    {
        ViewBag.Search = search;

        var categories = await _categoryService.GetAllAsync(
            search,
            cancellationToken);

        return View(categories);
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(
            id,
            cancellationToken);

        return category is null ? NotFound() : View(category);
    }

    [HttpGet]
    public async Task<IActionResult> Create(
        CancellationToken cancellationToken)
    {
        await LoadParentCategoriesAsync(null, null, cancellationToken);
        return View(new CreateCategoryDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateCategoryDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadParentCategoriesAsync(
                request.ParentCategoryId,
                null,
                cancellationToken);

            return View(request);
        }

        try
        {
            await _categoryService.CreateAsync(
                request,
                cancellationToken);

            TempData["SuccessMessage"] =
                "Category created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);

            await LoadParentCategoriesAsync(
                request.ParentCategoryId,
                null,
                cancellationToken);

            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetForEditAsync(
            id,
            cancellationToken);

        if (category is null)
            return NotFound();

        await LoadParentCategoriesAsync(
            category.ParentCategoryId,
            id,
            cancellationToken);

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        UpdateCategoryDto request,
        CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await LoadParentCategoriesAsync(
                request.ParentCategoryId,
                id,
                cancellationToken);

            return View(request);
        }

        try
        {
            await _categoryService.UpdateAsync(
                request,
                cancellationToken);

            TempData["SuccessMessage"] =
                "Category updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);

            await LoadParentCategoriesAsync(
                request.ParentCategoryId,
                id,
                cancellationToken);

            return View(request);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(
            id,
            cancellationToken);

        return category is null ? NotFound() : View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _categoryService.DeactivateAsync(
                id,
                cancellationToken);

            TempData["SuccessMessage"] =
                "Category deactivated successfully.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadParentCategoriesAsync(
        Guid? selectedId,
        Guid? excludeId,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(
            null,
            cancellationToken);

        ViewBag.ParentCategories = categories
            .Where(x =>
                x.IsActive &&
                (!excludeId.HasValue || x.Id != excludeId.Value))
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
                Selected = x.Id == selectedId
            })
            .ToList();
    }
}