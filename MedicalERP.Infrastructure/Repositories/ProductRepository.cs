using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalERP.Infrastructure.Repositories
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync(
            Guid companyId,
            string? search,
            Guid? categoryId,
            bool? isMedicine,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.Manufacturer)
                .Include(x => x.BaseUnit)
                .Where(x => x.CompanyId == companyId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var value = search.Trim();

                query = query.Where(x =>
                    x.Name.Contains(value) ||
                    x.Code.Contains(value) ||
                    (x.RegulatoryNumber != null &&
                     x.RegulatoryNumber.Contains(value)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x =>
                    x.CategoryId == categoryId.Value);
            }

            if (isMedicine.HasValue)
            {
                query = query.Where(x =>
                    x.IsMedicine == isMedicine.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(x =>
                    x.IsActive == isActive.Value);
            }

            return await query
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public Task<Product?> GetByIdAsync(
            Guid id,
            Guid companyId,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Product> query = _context.Products
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.Manufacturer)
                .Include(x => x.GenericMedicine)
                .Include(x => x.DosageForm)
                .Include(x => x.Strength)
                .Include(x => x.BaseUnit);

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query.FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.CompanyId == companyId,
                cancellationToken);
        }

        public Task<bool> CodeExistsAsync(
            string code,
            Guid companyId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            return _context.Products.AnyAsync(
                x => x.CompanyId == companyId &&
                     x.Code == code &&
                     (!excludeId.HasValue ||
                      x.Id != excludeId.Value),
                cancellationToken);
        }

        public Task<bool> RelatedEntityExistsAsync<TEntity>(
            Guid id,
            Guid companyId,
            CancellationToken cancellationToken = default)
            where TEntity : CompanyEntity
        {
            return _context.Set<TEntity>().AnyAsync(
                x => x.Id == id &&
                     x.CompanyId == companyId &&
                     x.IsActive,
                cancellationToken);
        }

        public Task<List<TEntity>> GetActiveLookupsAsync<TEntity>(
            Guid companyId,
            CancellationToken cancellationToken = default)
            where TEntity : CompanyEntity
        {
            return _context.Set<TEntity>()
                .AsNoTracking()
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(
                product,
                cancellationToken);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
