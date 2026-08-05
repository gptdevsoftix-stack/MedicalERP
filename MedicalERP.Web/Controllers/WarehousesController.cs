using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Common;
using MedicalERP.Application.Permissions;
using MedicalERP.Application.Warehouses.Dtos;
using MedicalERP.Web.Authorization;
using MedicalERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalERP.Web.Controllers;

[Route("api/warehouses")]
public sealed class WarehousesController(IWarehouseService service, IStoreService stores) : Controller
{
    [HttpGet("/Warehouses")]
    public async Task<IActionResult> Index([FromQuery] Guid? storeId, [FromQuery] QueryParameters query, CancellationToken ct)
    {
        ViewBag.Query = query;
        return View(await service.GetAsync(storeId, query, ct));
    }

    [HttpGet("/Warehouses/Create")]
    public async Task<IActionResult> CreatePage(CancellationToken ct) => View("Create", await PopulateStores(new WarehouseFormViewModel(), ct));

    [HttpPost("/Warehouses/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePage(WarehouseFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Create", await PopulateStores(model, ct));
        await service.CreateAsync(model.ToCreate(), ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/Warehouses/Edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct) => View(await PopulateStores(WarehouseFormViewModel.From(await service.GetByIdAsync(id, ct)), ct));

    [HttpPost("/Warehouses/Edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, WarehouseFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(await PopulateStores(model, ct));
        await service.UpdateAsync(id, model.ToUpdate(), ct);
        await service.SetActiveAsync(id, model.IsActive, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/Warehouses/Delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) { await service.SetActiveAsync(id, false, ct); return RedirectToAction(nameof(Index)); }

    private async Task<WarehouseFormViewModel> PopulateStores(WarehouseFormViewModel model, CancellationToken ct)
    {
        try
        {
            var list = await stores.GetAsync(new QueryParameters { PageSize = 500 }, ct);
            model.Stores = list.Items.Select(x => new SelectListItem($"{x.Name} ({x.Code})", x.Id.ToString(), model.StoreId == x.Id)).ToArray();
        }
        catch { model.Stores = []; }
        return model;
    }

    [HttpGet, HasPermission(Permissions.Stores.View)] public async Task<ActionResult<ApiResponse<PagedResult<WarehouseDto>>>> Get([FromQuery] Guid? storeId, [FromQuery] QueryParameters query, CancellationToken ct) => Ok(ApiResponse<PagedResult<WarehouseDto>>.Ok(await service.GetAsync(storeId, query, ct)));
    [HttpGet("{id:guid}"), HasPermission(Permissions.Stores.View)] public async Task<ActionResult<ApiResponse<WarehouseDto>>> GetById(Guid id, CancellationToken ct) => Ok(ApiResponse<WarehouseDto>.Ok(await service.GetByIdAsync(id, ct)));
    [HttpPost, HasPermission(Permissions.Stores.Create)] public async Task<ActionResult<ApiResponse<WarehouseDto>>> Create([FromBody] CreateWarehouseRequest request, CancellationToken ct) => Ok(ApiResponse<WarehouseDto>.Ok(await service.CreateAsync(request, ct)));
    [HttpPut("{id:guid}"), HasPermission(Permissions.Stores.Update)] public async Task<ActionResult<ApiResponse<WarehouseDto>>> Update(Guid id, [FromBody] UpdateWarehouseRequest request, CancellationToken ct) => Ok(ApiResponse<WarehouseDto>.Ok(await service.UpdateAsync(id, request, ct)));
    [HttpPost("{id:guid}/deactivate"), HasPermission(Permissions.Stores.Update)] public async Task<ActionResult<ApiResponse<object>>> Deactivate(Guid id, CancellationToken ct) { await service.SetActiveAsync(id, false, ct); return Ok(ApiResponse<object>.Ok(new { })); }
}

