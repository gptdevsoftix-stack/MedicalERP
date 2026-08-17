using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Common;
using MedicalERP.Application.Identity.Dtos;
using MedicalERP.Application.Permissions;
using MedicalERP.Web.Authorization;
using MedicalERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalERP.Web.Controllers;

[Route("api/roles")]
public sealed class RolesController(IIdentityService identity, ICompanyService companies) : Controller
{
    [HttpGet("/Roles")]
    public async Task<IActionResult> Index(CancellationToken ct) => View(await identity.GetRolesAsync(ct));

    [HttpGet("/Roles/Create")]
    public async Task<IActionResult> CreatePage(CancellationToken ct) => View("Create", await Populate(new RoleFormViewModel(), ct));

    [HttpPost("/Roles/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePage(RoleFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Create", await Populate(model, ct));
        var role = await identity.CreateRoleAsync(model.ToCreate(), ct);
        await identity.ManageRolePermissionsAsync(role.Id, new ManageRolePermissionsRequest(model.SelectedPermissions), ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/Roles/Edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct) => View(await Populate(RoleFormViewModel.From(await identity.GetRoleByIdAsync(id, ct)), ct));

    [HttpPost("/Roles/Edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, RoleFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(await Populate(model, ct));
        await identity.UpdateRoleAsync(id, model.ToUpdate(), ct);
        await identity.ManageRolePermissionsAsync(id, new ManageRolePermissionsRequest(model.SelectedPermissions), ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/Roles/Delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) { await identity.SetRoleActiveAsync(id, false, ct); return RedirectToAction(nameof(Index)); }

    private async Task<RoleFormViewModel> Populate(RoleFormViewModel model, CancellationToken ct)
    {
        model.Permissions = Permissions.All.Select(x => new SelectListItem(x, x)).ToArray();
        try { model.Companies = (await companies.GetAsync(new QueryParameters { PageSize = 500 }, ct)).Items.Select(x => new SelectListItem(x.Name, x.Id.ToString(), model.CompanyContextId == x.Id)).ToArray(); } catch { model.Companies = []; }
        return model;
    }

    [HttpGet, HasPermission(Permissions.Roles.View)] public async Task<ActionResult<ApiResponse<IReadOnlyList<RoleDto>>>> Get(CancellationToken ct) => Ok(ApiResponse<IReadOnlyList<RoleDto>>.Ok(await identity.GetRolesAsync(ct)));
    [HttpGet("{id:guid}"), HasPermission(Permissions.Roles.View)] public async Task<ActionResult<ApiResponse<RoleDto>>> GetById(Guid id, CancellationToken ct) => Ok(ApiResponse<RoleDto>.Ok(await identity.GetRoleByIdAsync(id, ct)));
    [HttpPost, HasPermission(Permissions.Roles.Create)] public async Task<ActionResult<ApiResponse<RoleDto>>> Create([FromBody] CreateRoleRequest request, CancellationToken ct) => Ok(ApiResponse<RoleDto>.Ok(await identity.CreateRoleAsync(request, ct)));
    [HttpPut("{id:guid}"), HasPermission(Permissions.Roles.Update)] public async Task<ActionResult<ApiResponse<RoleDto>>> Update(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken ct) => Ok(ApiResponse<RoleDto>.Ok(await identity.UpdateRoleAsync(id, request, ct)));
    [HttpDelete("{id:guid}"), HasPermission(Permissions.Roles.Delete)] public async Task<ActionResult<ApiResponse<object>>> DeleteApi(Guid id, CancellationToken ct) { await identity.SetRoleActiveAsync(id, false, ct); return Ok(ApiResponse<object>.Ok(new { })); }
    [HttpPost("{id:guid}/permissions"), HasPermission(Permissions.Roles.ManagePermissions)] public async Task<ActionResult<ApiResponse<object>>> ManagePermissions(Guid id, [FromBody] ManageRolePermissionsRequest request, CancellationToken ct) { await identity.ManageRolePermissionsAsync(id, request, ct); return Ok(ApiResponse<object>.Ok(new { })); }
}

