using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Common;
using MedicalERP.Application.Identity.Dtos;
using MedicalERP.Application.Permissions;
using MedicalERP.Web.Authorization;
using MedicalERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalERP.Web.Controllers;

[Route("api/users")]
public sealed class UsersController(IIdentityService identity, IStoreService stores, ICompanyService companies) : Controller
{
    [HttpGet("/Users")]
    public async Task<IActionResult> Index([FromQuery] QueryParameters query, CancellationToken ct)
    {
        ViewBag.PaginationRouteValues = new Dictionary<string, object?> { };
        return View(await identity.GetUsersAsync(query, ct));
    }

    [HttpGet("/Users/Create")]
    public async Task<IActionResult> CreatePage(CancellationToken ct) => View("Create", await Populate(new UserFormViewModel(), ct));

    [HttpPost("/Users/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePage(UserFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Create", await Populate(model, ct));
        await identity.CreateUserAsync(model.ToCreate(), ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/Users/Edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct) => View(await Populate(UserFormViewModel.From(await identity.GetUserByIdAsync(id, ct)), ct));

    [HttpPost("/Users/Edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UserFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(await Populate(model, ct));
        await identity.UpdateUserAsync(id, model.ToUpdate(), ct);
        await identity.AssignRolesAsync(new AssignRolesRequest(id, model.SelectedRoles), ct);
        await identity.AssignStoresAsync(new AssignStoresRequest(id, model.SelectedStoreIds, model.SelectedStoreIds.FirstOrDefault()), ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/Users/Delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) { await identity.SetUserActiveAsync(id, false, ct); return RedirectToAction(nameof(Index)); }

    private async Task<UserFormViewModel> Populate(UserFormViewModel model, CancellationToken ct)
    {
        try { model.Roles = (await identity.GetRolesAsync(ct)).Select(x => new SelectListItem(x.Name, x.Name, model.SelectedRoles.Contains(x.Name))).ToArray(); } catch { model.Roles = []; }
        try { model.Stores = (await stores.GetAsync(new QueryParameters { PageSize = 500 }, ct)).Items.Select(x => new SelectListItem($"{x.Name} ({x.Code})", x.Id.ToString(), model.SelectedStoreIds.Contains(x.Id))).ToArray(); } catch { model.Stores = []; }
        try { model.Companies = (await companies.GetAsync(new QueryParameters { PageSize = 500 }, ct)).Items.Select(x => new SelectListItem(x.Name, x.Id.ToString(), model.CompanyContextId == x.Id)).ToArray(); } catch { model.Companies = []; }
        return model;
    }

    [HttpGet, HasPermission(Permissions.Users.View)] public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> Get([FromQuery] QueryParameters query, CancellationToken ct) => Ok(ApiResponse<PagedResult<UserDto>>.Ok(await identity.GetUsersAsync(query, ct)));
    [HttpGet("{id:guid}"), HasPermission(Permissions.Users.View)] public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id, CancellationToken ct) => Ok(ApiResponse<UserDto>.Ok(await identity.GetUserByIdAsync(id, ct)));
    [HttpPost, HasPermission(Permissions.Users.Create)] public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserRequest request, CancellationToken ct) => Ok(ApiResponse<UserDto>.Ok(await identity.CreateUserAsync(request, ct)));
    [HttpPut("{id:guid}"), HasPermission(Permissions.Users.Update)] public async Task<ActionResult<ApiResponse<UserDto>>> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct) => Ok(ApiResponse<UserDto>.Ok(await identity.UpdateUserAsync(id, request, ct)));
    [HttpDelete("{id:guid}"), HasPermission(Permissions.Users.Delete)] public async Task<ActionResult<ApiResponse<object>>> DeleteApi(Guid id, CancellationToken ct) { await identity.SetUserActiveAsync(id, false, ct); return Ok(ApiResponse<object>.Ok(new { })); }
    [HttpPost("assign-stores"), HasPermission(Permissions.Users.AssignStores)] public async Task<ActionResult<ApiResponse<object>>> AssignStores([FromBody] AssignStoresRequest request, CancellationToken ct) { await identity.AssignStoresAsync(request, ct); return Ok(ApiResponse<object>.Ok(new { })); }
    [HttpPost("assign-roles"), HasPermission(Permissions.Users.AssignRoles)] public async Task<ActionResult<ApiResponse<object>>> AssignRoles([FromBody] AssignRolesRequest request, CancellationToken ct) { await identity.AssignRolesAsync(request, ct); return Ok(ApiResponse<object>.Ok(new { })); }
}

