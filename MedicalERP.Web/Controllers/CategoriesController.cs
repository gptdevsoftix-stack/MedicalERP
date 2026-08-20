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
public sealed class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [HasPermission(Permissions.Categories.View)]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        ViewBag.Search = search;

        var result = await _categoryService.GetAllPagedAsync(
            search,
            page,
            pageSize,
            cancellationToken);

        ViewBag.PaginationRouteValues = new Dictionary<string, object?>
        {
            ["search"] = search
        };

        return View(result);
    }

    [HttpGet]
    [HasPermission(Permissions.Categories.View)]
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
    [HasPermission(Permissions.Categories.Create)]
    public async Task<IActionResult> Create(
        CancellationToken cancellationToken)
    {
        await LoadParentCategoriesAsync(null, null, cancellationToken);
        return View(new CreateCategoryDto());
    }

    [HttpPost]
    [HasPermission(Permissions.Categories.Create)]
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
    [HasPermission(Permissions.Categories.Update)]
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
    [HasPermission(Permissions.Categories.Update)]
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
    [HasPermission(Permissions.Categories.Delete)]
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
    [HasPermission(Permissions.Categories.Delete)]
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

    [HttpGet("/api/categories")]
    [HasPermission(Permissions.Categories.View)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<CategoryDto>>>> Get(
        string? search,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(search, cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<CategoryDto>>.Ok(categories));
    }

    [HttpGet("/api/categories/{id:guid}")]
    [HasPermission(Permissions.Categories.View)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);

        return category is null
            ? NotFound(ApiResponse<CategoryDto>.Fail("Category not found."))
            : Ok(ApiResponse<CategoryDto>.Ok(category));
    }

    [HttpPost("/api/categories")]
    [HasPermission(Permissions.Categories.Create)]
    public async Task<ActionResult<ApiResponse<object>>> CreateApi(
        [FromBody] CreateCategoryDto request,
        CancellationToken cancellationToken)
    {
        var id = await _categoryService.CreateAsync(request, cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { id }));
    }

    [HttpPut("/api/categories/{id:guid}")]
    [HasPermission(Permissions.Categories.Update)]
    public async Task<ActionResult<ApiResponse<object>>> UpdateApi(
        Guid id,
        [FromBody] UpdateCategoryDto request,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        await _categoryService.UpdateAsync(request, cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { }));
    }

    [HttpDelete("/api/categories/{id:guid}")]
    [HasPermission(Permissions.Categories.Delete)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteApi(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _categoryService.DeactivateAsync(id, cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { }));
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
