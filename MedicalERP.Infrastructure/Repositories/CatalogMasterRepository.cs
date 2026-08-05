using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories;

public sealed class CatalogMasterRepository : ICatalogMasterRepository
{
    private readonly ApplicationDbContext _context;

    public CatalogMasterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CompanyEntity>> GetAllAsync(
        CatalogMasterType masterType,
        Guid? companyId,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = Query(masterType)
            .AsNoTracking()
            .Where(x => !companyId.HasValue || x.CompanyId == companyId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();
            query = query.Where(x => EF.Property<string>(x, "Name").Contains(value));
        }

        return await query
            .OrderBy(x => EF.Property<string>(x, "Name"))
            .ToListAsync(cancellationToken);
    }

    public Task<CompanyEntity?> GetByIdAsync(
        CatalogMasterType masterType,
        Guid id,
        Guid? companyId,
        CancellationToken cancellationToken = default)
    {
        return Query(masterType)
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     (!companyId.HasValue || x.CompanyId == companyId.Value),
                cancellationToken);
    }

    public Task<bool> CodeExistsAsync(
        CatalogMasterType masterType,
        string code,
        Guid companyId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();

        return Query(masterType).AnyAsync(
            x => x.CompanyId == companyId &&
                 EF.Property<string>(x, "Code") == normalizedCode &&
                 (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        CatalogMasterType masterType,
        CompanyEntity entity,
        CancellationToken cancellationToken = default)
    {
        switch (masterType)
        {
            case CatalogMasterType.ProductBrand:
                await _context.ProductBrands.AddAsync((ProductBrand)entity, cancellationToken);
                break;
            case CatalogMasterType.Manufacturer:
                await _context.Manufacturers.AddAsync((Manufacturer)entity, cancellationToken);
                break;
            case CatalogMasterType.GenericMedicine:
                await _context.GenericMedicines.AddAsync((GenericMedicine)entity, cancellationToken);
                break;
            case CatalogMasterType.DosageForm:
                await _context.DosageForms.AddAsync((DosageForm)entity, cancellationToken);
                break;
            case CatalogMasterType.Strength:
                await _context.Strengths.AddAsync((Strength)entity, cancellationToken);
                break;
            case CatalogMasterType.Unit:
                await _context.Units.AddAsync((Unit)entity, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(masterType), masterType, null);
        }
    }

    public void Update(CompanyEntity entity)
    {
        _context.Update(entity);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<CompanyEntity> Query(CatalogMasterType masterType)
    {
        return masterType switch
        {
            CatalogMasterType.ProductBrand => _context.ProductBrands,
            CatalogMasterType.Manufacturer => _context.Manufacturers,
            CatalogMasterType.GenericMedicine => _context.GenericMedicines,
            CatalogMasterType.DosageForm => _context.DosageForms,
            CatalogMasterType.Strength => _context.Strengths,
            CatalogMasterType.Unit => _context.Units,
            _ => throw new ArgumentOutOfRangeException(nameof(masterType), masterType, null)
        };
    }
}
