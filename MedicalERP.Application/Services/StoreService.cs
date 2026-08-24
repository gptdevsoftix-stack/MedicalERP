using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Stores.Dtos;
using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Interfaces;

namespace MedicalERP.Application.Services;

public sealed class StoreService(IStoreRepository stores, ICompanyContext companyContext, IStoreContext storeContext) : IStoreService
{
    public async Task<PagedResult<StoreDto>> GetAsync(QueryParameters query, CancellationToken cancellationToken)
    {
        var allowedStores = storeContext.AllowedStoreIds;
        var source = stores.Query().Where(x => allowedStores.Count == 0 || allowedStores.Contains(x.Id));
        var companyId = companyContext.CompanyId;
        if (companyId.HasValue) source = source.Where(x => x.CompanyId == companyId.Value);
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
