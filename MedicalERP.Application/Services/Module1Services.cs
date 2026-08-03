using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Abstractions.Services;
using MedicalERP.Application.Common;
using MedicalERP.Application.Companies.Dtos;
using MedicalERP.Application.Stores.Dtos;
using MedicalERP.Application.Warehouses.Dtos;
using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Interfaces;

namespace MedicalERP.Application.Services;

public sealed class CompanyService(ICompanyRepository companies, ICurrentUserService currentUser) : ICompanyService
{
    public async Task<PagedResult<CompanyDto>> GetAsync(QueryParameters query, CancellationToken cancellationToken)
    {
        if (!currentUser.IsPlatformAdmin) throw new UnauthorizedAccessException("Only platform admins can list companies.");
        var source = companies.Query();
        if (!string.IsNullOrWhiteSpace(query.Search)) source = source.Where(x => x.Name.Contains(query.Search) || x.Code.Contains(query.Search));
        var total = source.Count();
        var items = source.OrderBy(x => x.Name).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(x => Map(x)).ToList();
        return new PagedResult<CompanyDto>(items, query.Page, query.PageSize, total);
    }

    public async Task<CompanyDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var company = companies.Query().SingleOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Company not found.");
        return Map(company);
    }

    public async Task<CompanyDto> CreateAsync(CreateCompanyRequest request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsPlatformAdmin) throw new UnauthorizedAccessException("Only platform admins can create companies.");
        var company = new Company { Name = request.Name, Code = request.Code, LegalName = request.LegalName, Email = request.Email, Phone = request.Phone, Address = request.Address, City = request.City, State = request.State, Country = request.Country, TaxNumber = request.TaxNumber, CurrencyCode = request.CurrencyCode, TimeZone = request.TimeZone };
        await companies.AddAsync(company, cancellationToken);
        await companies.SaveChangesAsync(cancellationToken);
        return Map(company);
    }

    public async Task<CompanyDto> UpdateAsync(Guid id, UpdateCompanyRequest request, CancellationToken cancellationToken)
    {
        var company = await companies.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Company not found.");
        company.Name = request.Name; company.LegalName = request.LegalName; company.Email = request.Email; company.Phone = request.Phone; company.Address = request.Address; company.City = request.City; company.State = request.State; company.Country = request.Country; company.TaxNumber = request.TaxNumber; company.CurrencyCode = request.CurrencyCode; company.TimeZone = request.TimeZone;
        await companies.SaveChangesAsync(cancellationToken);
        return Map(company);
    }

    public async Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        var company = await companies.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Company not found.");
        company.IsActive = isActive;
        await companies.SaveChangesAsync(cancellationToken);
    }

    private static CompanyDto Map(Company x) => new(x.Id, x.Name, x.Code, x.LegalName, x.Email, x.Phone, x.Address, x.City, x.State, x.Country, x.TaxNumber, x.CurrencyCode, x.TimeZone, x.SubscriptionStatus.ToString(), x.IsActive);
}

public sealed class StoreService(IStoreRepository stores, ICompanyContext companyContext, IStoreContext storeContext) : IStoreService
{
    public async Task<PagedResult<StoreDto>> GetAsync(QueryParameters query, CancellationToken cancellationToken)
    {
        var allowedStores = storeContext.AllowedStoreIds;
        var source = stores.Query().Where(x => allowedStores.Count == 0 || allowedStores.Contains(x.Id));
        if (!string.IsNullOrWhiteSpace(query.Search)) source = source.Where(x => x.Name.Contains(query.Search) || x.Code.Contains(query.Search));
        var total = source.Count();
        var items = source.OrderBy(x => x.Name).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(x => Map(x)).ToList();
        return new PagedResult<StoreDto>(items, query.Page, query.PageSize, total);
    }

    public async Task<StoreDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await storeContext.EnsureStoreAccessAsync(id, cancellationToken);
        var store = stores.Query().SingleOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Store not found.");
        return Map(store);
    }

    public async Task<StoreDto> CreateAsync(CreateStoreRequest request, CancellationToken cancellationToken)
    {
        var companyId = companyContext.RequireCompanyId();
        var store = new Store { CompanyId = companyId, Name = request.Name, Code = request.Code, Email = request.Email, Phone = request.Phone, Address = request.Address, City = request.City, State = request.State, Country = request.Country, TaxNumber = request.TaxNumber, CurrencyCode = request.CurrencyCode, TimeZone = request.TimeZone, IsHeadOffice = request.IsHeadOffice };
        await stores.AddAsync(store, cancellationToken);
        await stores.SaveChangesAsync(cancellationToken);
        return Map(store);
    }

    public async Task<StoreDto> UpdateAsync(Guid id, UpdateStoreRequest request, CancellationToken cancellationToken)
    {
        await storeContext.EnsureStoreAccessAsync(id, cancellationToken);
        var store = await stores.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Store not found.");
        store.Name = request.Name; store.Email = request.Email; store.Phone = request.Phone; store.Address = request.Address; store.City = request.City; store.State = request.State; store.Country = request.Country; store.TaxNumber = request.TaxNumber; store.CurrencyCode = request.CurrencyCode; store.TimeZone = request.TimeZone; store.IsHeadOffice = request.IsHeadOffice;
        await stores.SaveChangesAsync(cancellationToken);
        return Map(store);
    }

    public async Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        await storeContext.EnsureStoreAccessAsync(id, cancellationToken);
        var store = await stores.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Store not found.");
        store.IsActive = isActive;
        await stores.SaveChangesAsync(cancellationToken);
    }

    private static StoreDto Map(Store x) => new(x.Id, x.CompanyId, x.Name, x.Code, x.Email, x.Phone, x.Address, x.City, x.State, x.Country, x.TaxNumber, x.CurrencyCode, x.TimeZone, x.IsHeadOffice, x.IsActive);
}

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



