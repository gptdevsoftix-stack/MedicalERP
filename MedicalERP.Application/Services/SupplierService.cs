using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Purchases.Dtos;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Purchases;

namespace MedicalERP.Application.Services;

public sealed class SupplierService(ISupplierRepository repository, ICompanyContext companyContext, IStoreContext storeContext) : ISupplierService
{
    public async Task<PagedResult<SupplierListDto>> GetAsync(string? search, bool? isActive, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany(); var storeId = await RequireStoreAsync(cancellationToken); page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 10, 100);
        var total = await repository.CountAsync(companyId, storeId, search, isActive, cancellationToken);
        var rows = await repository.GetAsync(companyId, storeId, search, isActive, (page - 1) * pageSize, pageSize, cancellationToken);
        return new PagedResult<SupplierListDto>(rows.Select(Map).ToList(), page, pageSize, total);
    }

    public async Task<SupplierFormDto?> GetForEditAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await repository.GetByIdAsync(id, RequireCompany(), await RequireStoreAsync(cancellationToken), false, cancellationToken);
        return row is null ? null : new SupplierFormDto { Id = row.Id, Name = row.Name, Code = row.Code, ContactPerson = row.ContactPerson, Email = row.Email, Phone = row.Phone, Address = row.Address, TaxNumber = row.TaxNumber, CreditDays = row.CreditDays, CreditLimit = row.CreditLimit, IsActive = row.IsActive };
    }

    public async Task<Guid> CreateAsync(SupplierFormDto request, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany(); var storeId = await RequireStoreAsync(cancellationToken); await ValidateAsync(request, companyId, null, cancellationToken);
        var supplier = new Supplier { Id = Guid.NewGuid(), CompanyId = companyId, Name = request.Name.Trim(), Code = request.Code.Trim(), ContactPerson = Clean(request.ContactPerson), Email = Clean(request.Email), Phone = Clean(request.Phone), Address = Clean(request.Address), TaxNumber = Clean(request.TaxNumber), CreditDays = request.CreditDays, CreditLimit = request.CreditLimit, IsActive = true };
        var access = new SupplierStore { Id = Guid.NewGuid(), CompanyId = companyId, StoreId = storeId, SupplierId = supplier.Id, IsPreferred = false, IsActive = true };
        await repository.AddAsync(supplier, access, cancellationToken); await repository.SaveChangesAsync(cancellationToken); return supplier.Id;
    }

    public async Task UpdateAsync(SupplierFormDto request, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany(); var storeId = await RequireStoreAsync(cancellationToken); var supplier = await repository.GetByIdAsync(request.Id, companyId, storeId, true, cancellationToken) ?? throw new KeyNotFoundException("Supplier was not found.");
        await ValidateAsync(request, companyId, request.Id, cancellationToken); supplier.Name = request.Name.Trim(); supplier.Code = request.Code.Trim(); supplier.ContactPerson = Clean(request.ContactPerson); supplier.Email = Clean(request.Email); supplier.Phone = Clean(request.Phone); supplier.Address = Clean(request.Address); supplier.TaxNumber = Clean(request.TaxNumber); supplier.CreditDays = request.CreditDays; supplier.CreditLimit = request.CreditLimit; supplier.IsActive = request.IsActive;
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var storeId = await RequireStoreAsync(cancellationToken);
        var supplier = await repository.GetByIdAsync(id, RequireCompany(), storeId, true, cancellationToken) ?? throw new KeyNotFoundException("Supplier was not found.");
        var access = supplier.Stores.SingleOrDefault(x => x.StoreId == storeId && x.IsActive) ?? throw new KeyNotFoundException("Supplier was not assigned to the current store.");
        access.IsActive = false;
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateAsync(SupplierFormDto request, Guid companyId, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) throw new InvalidOperationException("Supplier name is required.");
        if (string.IsNullOrWhiteSpace(request.Code)) throw new InvalidOperationException("Supplier code is required.");
        if (request.CreditDays < 0 || request.CreditLimit < 0) throw new InvalidOperationException("Credit values cannot be negative.");
        if (await repository.CodeExistsAsync(companyId, request.Code.Trim(), excludedId, cancellationToken)) throw new InvalidOperationException("Supplier code already exists in this company.");
    }

    private Guid RequireCompany() => companyContext.CompanyId ?? throw new UnauthorizedAccessException("Company context is missing.");
    private async Task<Guid> RequireStoreAsync(CancellationToken cancellationToken) { var id = storeContext.RequireSelectedStoreId(); await storeContext.EnsureStoreAccessAsync(id, cancellationToken); return id; }
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static SupplierListDto Map(Supplier x) => new() { Id = x.Id, Name = x.Name, Code = x.Code, ContactPerson = x.ContactPerson, Phone = x.Phone, Email = x.Email, CreditDays = x.CreditDays, CreditLimit = x.CreditLimit, IsActive = x.IsActive };
}
