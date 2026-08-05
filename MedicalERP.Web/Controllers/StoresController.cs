using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Common;
using MedicalERP.Application.Permissions;
using MedicalERP.Application.Stores.Dtos;
using MedicalERP.Web.Authorization;
using MedicalERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalERP.Web.Controllers;

[Route("api/stores")]
public sealed class StoresController(IStoreService service, ICompanyService companies) : Controller
{
    [HttpGet("/Stores")]
    public async Task<IActionResult> Index([FromQuery] QueryParameters query, CancellationToken ct)
    {
        ViewBag.Query = query;
        return View(await service.GetAsync(query, ct));
    }

    [HttpGet("/Stores/Create")]
    public async Task<IActionResult> CreatePage(CancellationToken ct) => View("Create", await PopulateCompanies(new StoreFormViewModel(), ct));

    [HttpPost("/Stores/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePage(StoreFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Create", await PopulateCompanies(model, ct));
        await service.CreateAsync(model.ToCreate(), ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/Stores/Edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct) => View(await PopulateCompanies(StoreFormViewModel.From(await service.GetByIdAsync(id, ct)), ct));

    [HttpPost("/Stores/Edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, StoreFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(await PopulateCompanies(model, ct));
        await service.UpdateAsync(id, model.ToUpdate(), ct);
        await service.SetActiveAsync(id, model.IsActive, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/Stores/Delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) { await service.SetActiveAsync(id, false, ct); return RedirectToAction(nameof(Index)); }

    private async Task<StoreFormViewModel> PopulateCompanies(StoreFormViewModel model, CancellationToken ct)
    {
        try
        {
            var list = await companies.GetAsync(new QueryParameters { PageSize = 500 }, ct);
            model.Companies = list.Items.Select(x => new SelectListItem(x.Name, x.Id.ToString(), model.CompanyContextId == x.Id)).ToArray();
        }
        catch { model.Companies = []; }
        return model;
    }

    [HttpGet, HasPermission(Permissions.Stores.View)] public async Task<ActionResult<ApiResponse<PagedResult<StoreDto>>>> Get([FromQuery] QueryParameters query, CancellationToken ct) => Ok(ApiResponse<PagedResult<StoreDto>>.Ok(await service.GetAsync(query, ct)));
    [HttpGet("{id:guid}"), HasPermission(Permissions.Stores.View)] public async Task<ActionResult<ApiResponse<StoreDto>>> GetById(Guid id, CancellationToken ct) => Ok(ApiResponse<StoreDto>.Ok(await service.GetByIdAsync(id, ct)));
    [HttpPost, HasPermission(Permissions.Stores.Create)] public async Task<ActionResult<ApiResponse<StoreDto>>> Create([FromBody] CreateStoreRequest request, CancellationToken ct) => Ok(ApiResponse<StoreDto>.Ok(await service.CreateAsync(request, ct)));
    [HttpPut("{id:guid}"), HasPermission(Permissions.Stores.Update)] public async Task<ActionResult<ApiResponse<StoreDto>>> Update(Guid id, [FromBody] UpdateStoreRequest request, CancellationToken ct) => Ok(ApiResponse<StoreDto>.Ok(await service.UpdateAsync(id, request, ct)));
    [HttpPost("{id:guid}/activate"), HasPermission(Permissions.Stores.Activate)] public async Task<ActionResult<ApiResponse<object>>> Activate(Guid id, CancellationToken ct) { await service.SetActiveAsync(id, true, ct); return Ok(ApiResponse<object>.Ok(new { })); }
    [HttpPost("{id:guid}/deactivate"), HasPermission(Permissions.Stores.Activate)] public async Task<ActionResult<ApiResponse<object>>> Deactivate(Guid id, CancellationToken ct) { await service.SetActiveAsync(id, false, ct); return Ok(ApiResponse<object>.Ok(new { })); }
}

