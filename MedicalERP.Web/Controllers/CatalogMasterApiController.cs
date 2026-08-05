using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Permissions;
using MedicalERP.Domain.DTOs;
using MedicalERP.Domain.Enums;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalERP.Web.Controllers;

[Route("api/catalog-masters")]
public sealed class CatalogMasterApiController(ICatalogMasterService service) : ControllerBase
{
    [HttpGet("{masterType}")]
    [HasPermission(Permissions.Products.View)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<CatalogMasterDto>>>> Get(
        CatalogMasterType masterType,
        string? search,
        CancellationToken cancellationToken)
    {
        var records = await service.GetAllAsync(masterType, search, cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<CatalogMasterDto>>.Ok(records));
    }

    [HttpGet("{masterType}/{id:guid}")]
    [HasPermission(Permissions.Products.View)]
    public async Task<ActionResult<ApiResponse<CatalogMasterDto>>> GetById(
        CatalogMasterType masterType,
        Guid id,
        CancellationToken cancellationToken)
    {
        var record = await service.GetByIdAsync(masterType, id, cancellationToken);

        return record is null
            ? NotFound(ApiResponse<CatalogMasterDto>.Fail("Record not found."))
            : Ok(ApiResponse<CatalogMasterDto>.Ok(record));
    }

    [HttpPost("{masterType}")]
    [HasPermission(Permissions.Products.Create)]
    public async Task<ActionResult<ApiResponse<object>>> Create(
        CatalogMasterType masterType,
        [FromBody] CatalogMasterFormDto request,
        CancellationToken cancellationToken)
    {
        request.MasterType = masterType;
        var id = await service.CreateAsync(request, cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { id }));
    }

    [HttpPut("{masterType}/{id:guid}")]
    [HasPermission(Permissions.Products.Update)]
    public async Task<ActionResult<ApiResponse<object>>> Update(
        CatalogMasterType masterType,
        Guid id,
        [FromBody] CatalogMasterFormDto request,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        request.MasterType = masterType;
        await service.UpdateAsync(request, cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { }));
    }

    [HttpDelete("{masterType}/{id:guid}")]
    [HasPermission(Permissions.Products.Delete)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        CatalogMasterType masterType,
        Guid id,
        CancellationToken cancellationToken)
    {
        await service.DeactivateAsync(masterType, id, cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { }));
    }
}
