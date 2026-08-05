using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Warehouses.Dtos;
using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Interfaces;

namespace MedicalERP.Application.Services;

public sealed class WarehouseService(IWarehouseRepository warehouses, ICompanyContext companyContext, IStoreContext storeContext) : IWarehouseService
{
    public async Task<PagedResult<WarehouseDto>> GetAsync(Guid? storeId, QueryParameters query, CancellationToken cancellationToken)
    {
        if (storeId.HasValue) await storeContext.EnsureStoreAccessAsync(storeId.Value, cancellationToken);
        var allowedStores = storeContext.AllowedStoreIds;
        var source = warehouses.Query().Where(x => (!storeId.HasValue || x.StoreId == storeId.Value) && (allowedStores.Count == 0 || allowedStores.Contains(x.StoreId)));
        var total = source.Count();
        var items = source.OrderBy(x => x.Name).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(x => Map(x)).ToList();
        return new PagedResult<WarehouseDto>(items, query.Page, query.PageSize, total);
    }

    public async Task<WarehouseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var warehouse = warehouses.Query().SingleOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Warehouse not found.");
        await storeContext.EnsureStoreAccessAsync(warehouse.StoreId, cancellationToken);
        return Map(warehouse);
    }

    public async Task<WarehouseDto> CreateAsync(CreateWarehouseRequest request, CancellationToken cancellationToken)
    {
        await storeContext.EnsureStoreAccessAsync(request.StoreId, cancellationToken);
        var warehouse = new Warehouse { CompanyId = companyContext.RequireCompanyId(), StoreId = request.StoreId, Name = request.Name, Code = request.Code, WarehouseType = request.WarehouseType, Address = request.Address, IsDefault = request.IsDefault };
        await warehouses.AddAsync(warehouse, cancellationToken);
        await warehouses.SaveChangesAsync(cancellationToken);
        return Map(warehouse);
    }

    public async Task<WarehouseDto> UpdateAsync(Guid id, UpdateWarehouseRequest request, CancellationToken cancellationToken)
    {
        var warehouse = await warehouses.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Warehouse not found.");
        await storeContext.EnsureStoreAccessAsync(warehouse.StoreId, cancellationToken);
        warehouse.Name = request.Name; warehouse.WarehouseType = request.WarehouseType; warehouse.Address = request.Address; warehouse.IsDefault = request.IsDefault;
        await warehouses.SaveChangesAsync(cancellationToken);
        return Map(warehouse);
    }

    public async Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        var warehouse = await warehouses.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Warehouse not found.");
        await storeContext.EnsureStoreAccessAsync(warehouse.StoreId, cancellationToken);
        warehouse.IsActive = isActive;
        await warehouses.SaveChangesAsync(cancellationToken);
    }

    private static WarehouseDto Map(Warehouse x) => new(x.Id, x.CompanyId, x.StoreId, x.Name, x.Code, x.WarehouseType, x.Address, x.IsDefault, x.IsActive);
}
