using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Common;
using MedicalERP.Application.Companies.Dtos;
using MedicalERP.Application.Permissions;
using MedicalERP.Web.Authorization;
using MedicalERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MedicalERP.Web.Controllers;

[Route("api/companies")]
public sealed class CompaniesController(ICompanyService service) : Controller
{
    [HttpGet("/Companies")]
    public async Task<IActionResult> Index([FromQuery] QueryParameters query, CancellationToken ct)
    {
        ViewBag.Query = query;
        ViewBag.PaginationRouteValues = new Dictionary<string, object?>
        {
            ["search"] = query.Search
        };
        try
        {
            return View(await service.GetAsync(query, ct));
        }
        catch (UnauthorizedAccessException ex)
        {
            ViewBag.Warning = ex.Message;
            return View(new PagedResult<CompanyDto>([], query.Page, query.PageSize, 0));
        }
    }

    [Authorize]
    [HttpGet("/Companies/GetAllCompanies")]
    public async Task<IActionResult> GetAllCompanies(CancellationToken ct)
    {
        var result = await service.GetAsync(new QueryParameters { PageSize = 500 }, ct);
        return Ok(result.Items.Where(x => x.IsActive).Select(x => new
        {
            companyId = x.Id,
            companyName = x.Name,
            companyCode = x.Code
        }));
    }

    [HttpGet("/Companies/Create")]
    public IActionResult CreatePage() => View("Create", new CompanyFormViewModel());

    [HttpPost("/Companies/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePage(CompanyFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Create", model);
        await service.CreateAsync(model.ToCreate(), ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/Companies/Edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct) => View(CompanyFormViewModel.From(await service.GetByIdAsync(id, ct)));

    [HttpPost("/Companies/Edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CompanyFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        await service.UpdateAsync(id, model.ToUpdate(), ct);
        await service.SetActiveAsync(id, model.IsActive, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/Companies/Delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.SetActiveAsync(id, false, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet, HasPermission(Permissions.Companies.View)] public async Task<ActionResult<ApiResponse<PagedResult<CompanyDto>>>> Get([FromQuery] QueryParameters query, CancellationToken ct) => Ok(ApiResponse<PagedResult<CompanyDto>>.Ok(await service.GetAsync(query, ct)));
    [HttpGet("{id:guid}"), HasPermission(Permissions.Companies.View)] public async Task<ActionResult<ApiResponse<CompanyDto>>> GetById(Guid id, CancellationToken ct) => Ok(ApiResponse<CompanyDto>.Ok(await service.GetByIdAsync(id, ct)));
    [HttpPost, HasPermission(Permissions.Companies.Create)] public async Task<ActionResult<ApiResponse<CompanyDto>>> Create([FromBody] CreateCompanyRequest request, CancellationToken ct) => Ok(ApiResponse<CompanyDto>.Ok(await service.CreateAsync(request, ct)));
    [HttpPut("{id:guid}"), HasPermission(Permissions.Companies.Update)] public async Task<ActionResult<ApiResponse<CompanyDto>>> Update(Guid id, [FromBody] UpdateCompanyRequest request, CancellationToken ct) => Ok(ApiResponse<CompanyDto>.Ok(await service.UpdateAsync(id, request, ct)));
    [HttpPost("{id:guid}/activate"), HasPermission(Permissions.Companies.Activate)] public async Task<ActionResult<ApiResponse<object>>> Activate(Guid id, CancellationToken ct) { await service.SetActiveAsync(id, true, ct); return Ok(ApiResponse<object>.Ok(new { })); }
    [HttpPost("{id:guid}/suspend"), HasPermission(Permissions.Companies.Suspend)] public async Task<ActionResult<ApiResponse<object>>> Suspend(Guid id, CancellationToken ct) { await service.SetActiveAsync(id, false, ct); return Ok(ApiResponse<object>.Ok(new { })); }
}


