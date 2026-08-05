using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Companies.Dtos;
using MedicalERP.Application.Interfaces;
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
