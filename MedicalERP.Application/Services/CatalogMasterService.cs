using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Interfaces;
using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.DTOs;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Interfaces;

namespace MedicalERP.Application.Services;

public sealed class CatalogMasterService : ICatalogMasterService
{
    private readonly ICatalogMasterRepository _repository;
    private readonly ICompanyContext _companyContext;

    public CatalogMasterService(
        ICatalogMasterRepository repository,
        ICompanyContext companyContext)
    {
        _repository = repository;
        _companyContext = companyContext;
    }

    public async Task<List<CatalogMasterDto>> GetAllAsync(
        CatalogMasterType masterType,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var companyId = _companyContext.CompanyId;
        var entities = await _repository.GetAllAsync(
            masterType,
            companyId,
            search,
            cancellationToken);

        return entities.Select(x => MapToDto(masterType, x)).ToList();
    }

    public async Task<CatalogMasterDto?> GetByIdAsync(
        CatalogMasterType masterType,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(
            masterType,
            id,
            _companyContext.CompanyId,
            cancellationToken);

        return entity is null ? null : MapToDto(masterType, entity);
    }

    public async Task<CatalogMasterFormDto?> GetForEditAsync(
        CatalogMasterType masterType,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(
            masterType,
            id,
            _companyContext.CompanyId,
            cancellationToken);

        return entity is null ? null : MapToFormDto(masterType, entity);
    }

    public async Task<Guid> CreateAsync(
        CatalogMasterFormDto request,
        CancellationToken cancellationToken = default)
    {
        var companyId = _companyContext.RequireCompanyId();
        Validate(request);

        if (RequiresCode(request.MasterType))
        {
            var code = NormalizeRequired(request.Code);

            if (await _repository.CodeExistsAsync(
                    request.MasterType,
                    code,
                    companyId,
                    cancellationToken: cancellationToken))
            {
                throw new InvalidOperationException("A record with this code already exists.");
            }
        }

        var entity = CreateEntity(request, companyId);

        await _repository.AddAsync(request.MasterType, entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task UpdateAsync(
        CatalogMasterFormDto request,
        CancellationToken cancellationToken = default)
    {
        var companyId = _companyContext.RequireCompanyId();
        Validate(request);

        var entity = await _repository.GetByIdAsync(
            request.MasterType,
            request.Id,
            companyId,
            cancellationToken)
            ?? throw new KeyNotFoundException("Record not found.");

        if (RequiresCode(request.MasterType))
        {
            var code = NormalizeRequired(request.Code);

            if (await _repository.CodeExistsAsync(
                    request.MasterType,
                    code,
                    companyId,
                    request.Id,
                    cancellationToken))
            {
                throw new InvalidOperationException("A record with this code already exists.");
            }
        }

        Apply(request, entity);
        _repository.Update(entity);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        CatalogMasterType masterType,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var companyId = _companyContext.RequireCompanyId();
        var entity = await _repository.GetByIdAsync(
            masterType,
            id,
            companyId,
            cancellationToken)
            ?? throw new KeyNotFoundException("Record not found.");

        entity.IsActive = false;
        _repository.Update(entity);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    private static CompanyEntity CreateEntity(CatalogMasterFormDto request, Guid companyId)
    {
        CompanyEntity entity = request.MasterType switch
        {
            CatalogMasterType.ProductBrand => new ProductBrand(),
            CatalogMasterType.Manufacturer => new Manufacturer(),
            CatalogMasterType.GenericMedicine => new GenericMedicine(),
            CatalogMasterType.DosageForm => new DosageForm(),
            CatalogMasterType.Strength => new Strength(),
            CatalogMasterType.Unit => new Unit(),
            _ => throw new ArgumentOutOfRangeException(nameof(request.MasterType), request.MasterType, null)
        };

        entity.Id = Guid.NewGuid();
        entity.CompanyId = companyId;
        Apply(request, entity);
        entity.IsActive = true;

        return entity;
    }

    private static void Apply(CatalogMasterFormDto request, CompanyEntity entity)
    {
        switch (entity)
        {
            case ProductBrand productBrand:
                productBrand.Name = request.Name.Trim();
                productBrand.Code = NormalizeRequired(request.Code);
                break;
            case Manufacturer manufacturer:
                manufacturer.Name = request.Name.Trim();
                manufacturer.Code = NormalizeRequired(request.Code);
                manufacturer.LicenseNumber = NormalizeOptional(request.LicenseNumber);
                break;
            case GenericMedicine genericMedicine:
                genericMedicine.Name = request.Name.Trim();
                genericMedicine.Description = NormalizeOptional(request.Description);
                break;
            case DosageForm dosageForm:
                dosageForm.Name = request.Name.Trim();
                dosageForm.Code = NormalizeRequired(request.Code);
                break;
            case Strength strength:
                strength.Name = request.Name.Trim();
                strength.Value = request.Value;
                strength.MeasurementUnit = NormalizeOptional(request.MeasurementUnit);
                break;
            case Unit unit:
                unit.Name = request.Name.Trim();
                unit.Symbol = NormalizeRequired(request.Symbol);
                unit.AllowsDecimal = request.AllowsDecimal;
                break;
        }

        entity.IsActive = request.IsActive;
    }

    private static void Validate(CatalogMasterFormDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new InvalidOperationException("Name is required.");

        if (RequiresCode(request.MasterType) && string.IsNullOrWhiteSpace(request.Code))
            throw new InvalidOperationException("Code is required.");

        if (request.MasterType == CatalogMasterType.Unit && string.IsNullOrWhiteSpace(request.Symbol))
            throw new InvalidOperationException("Symbol is required.");
    }

    private static bool RequiresCode(CatalogMasterType masterType)
    {
        return masterType is CatalogMasterType.ProductBrand
            or CatalogMasterType.Manufacturer
            or CatalogMasterType.DosageForm;
    }

    private static string NormalizeRequired(string? value)
    {
        return value?.Trim().ToUpperInvariant()
            ?? throw new InvalidOperationException("Required value is missing.");
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static CatalogMasterDto MapToDto(CatalogMasterType masterType, CompanyEntity entity)
    {
        var form = MapToFormDto(masterType, entity);

        return new CatalogMasterDto
        {
            Id = form.Id,
            MasterType = form.MasterType,
            Name = form.Name,
            Code = form.Code,
            Description = form.Description,
            LicenseNumber = form.LicenseNumber,
            Value = form.Value,
            MeasurementUnit = form.MeasurementUnit,
            Symbol = form.Symbol,
            AllowsDecimal = form.AllowsDecimal,
            IsActive = form.IsActive
        };
    }

    private static CatalogMasterFormDto MapToFormDto(CatalogMasterType masterType, CompanyEntity entity)
    {
        var dto = new CatalogMasterFormDto
        {
            Id = entity.Id,
            MasterType = masterType,
            IsActive = entity.IsActive
        };

        switch (entity)
        {
            case ProductBrand productBrand:
                dto.Name = productBrand.Name;
                dto.Code = productBrand.Code;
                break;
            case Manufacturer manufacturer:
                dto.Name = manufacturer.Name;
                dto.Code = manufacturer.Code;
                dto.LicenseNumber = manufacturer.LicenseNumber;
                break;
            case GenericMedicine genericMedicine:
                dto.Name = genericMedicine.Name;
                dto.Description = genericMedicine.Description;
                break;
            case DosageForm dosageForm:
                dto.Name = dosageForm.Name;
                dto.Code = dosageForm.Code;
                break;
            case Strength strength:
                dto.Name = strength.Name;
                dto.Value = strength.Value;
                dto.MeasurementUnit = strength.MeasurementUnit;
                break;
            case Unit unit:
                dto.Name = unit.Name;
                dto.Symbol = unit.Symbol;
                dto.AllowsDecimal = unit.AllowsDecimal;
                break;
        }

        return dto;
    }
}
