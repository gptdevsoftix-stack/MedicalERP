using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Sales.Dtos;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Sales;

namespace MedicalERP.Application.Services;

public sealed class CustomerService(ICustomerRepository repository, ICompanyContext companyContext) : ICustomerService
{
    public async Task<PagedResult<CustomerListDto>> GetAsync(string? search, bool? isActive, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany(); page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 10, 100);
        var total = await repository.CountAsync(companyId, search, isActive, cancellationToken);
        var rows = await repository.GetAsync(companyId, search, isActive, (page - 1) * pageSize, pageSize, cancellationToken);
        return new PagedResult<CustomerListDto>(rows.Select(Map).ToList(), page, pageSize, total);
    }

    public async Task<CustomerFormDto?> GetForEditAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await repository.GetByIdAsync(id, RequireCompany(), false, cancellationToken);
        return row is null ? null : new CustomerFormDto { Id = row.Id, Name = row.Name, Code = row.Code, Email = row.Email, Phone = row.Phone, Address = row.Address, TaxNumber = row.TaxNumber, CreditDays = row.CreditDays, CreditLimit = row.CreditLimit, IsActive = row.IsActive };
    }

    public async Task<Guid> CreateAsync(CustomerFormDto request, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany(); await ValidateAsync(request, companyId, null, cancellationToken);
        var customer = new Customer { Id = Guid.NewGuid(), CompanyId = companyId, Name = request.Name.Trim(), Code = request.Code.Trim(), Email = Clean(request.Email), Phone = Clean(request.Phone), Address = Clean(request.Address), TaxNumber = Clean(request.TaxNumber), CreditDays = request.CreditDays, CreditLimit = request.CreditLimit, IsActive = true };
        await repository.AddAsync(customer, cancellationToken); await repository.SaveChangesAsync(cancellationToken); return customer.Id;
    }

    public async Task UpdateAsync(CustomerFormDto request, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany(); var customer = await repository.GetByIdAsync(request.Id, companyId, true, cancellationToken) ?? throw new KeyNotFoundException("Customer was not found.");
        await ValidateAsync(request, companyId, request.Id, cancellationToken); customer.Name = request.Name.Trim(); customer.Code = request.Code.Trim(); customer.Email = Clean(request.Email); customer.Phone = Clean(request.Phone); customer.Address = Clean(request.Address); customer.TaxNumber = Clean(request.TaxNumber); customer.CreditDays = request.CreditDays; customer.CreditLimit = request.CreditLimit; customer.IsActive = request.IsActive;
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(id, RequireCompany(), true, cancellationToken) ?? throw new KeyNotFoundException("Customer was not found.");
        customer.IsActive = false;
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateAsync(CustomerFormDto request, Guid companyId, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) throw new InvalidOperationException("Customer name is required.");
        if (string.IsNullOrWhiteSpace(request.Code)) throw new InvalidOperationException("Customer code is required.");
        if (request.CreditDays < 0 || request.CreditLimit < 0) throw new InvalidOperationException("Credit values cannot be negative.");
        if (await repository.CodeExistsAsync(companyId, request.Code.Trim(), excludedId, cancellationToken)) throw new InvalidOperationException("Customer code already exists in this company.");
    }

    private Guid RequireCompany() => companyContext.CompanyId ?? throw new UnauthorizedAccessException("Company context is missing.");
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static CustomerListDto Map(Customer x) => new() { Id = x.Id, Name = x.Name, Code = x.Code, Phone = x.Phone, Email = x.Email, CreditDays = x.CreditDays, CreditLimit = x.CreditLimit, IsActive = x.IsActive };
}
