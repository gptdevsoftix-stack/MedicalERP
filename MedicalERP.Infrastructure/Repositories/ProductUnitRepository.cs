using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalERP.Infrastructure.Repositories
{
    public sealed class ProductUnitRepository : IProductUnitRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductUnitRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<ProductUnit>>
            GetByProductIdAsync(
                Guid productId,
                Guid companyId,
                bool tracking = false,
                CancellationToken cancellationToken = default)
        {
            IQueryable<ProductUnit> query = _context.ProductUnits
                .Include(x => x.Product)
                .Include(x => x.Unit)
                .Where(x =>
                    x.ProductId == productId &&
                    x.CompanyId == companyId);

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            return await query
                .OrderByDescending(x => x.IsBaseUnit)
                .ThenBy(x => x.Unit.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductUnit?> GetByIdAsync(
            Guid id,
            Guid companyId,
            bool tracking = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<ProductUnit> query = _context.ProductUnits
                .Include(x => x.Product)
                .Include(x => x.Unit)
                .Where(x =>
                    x.Id == id &&
                    x.CompanyId == companyId);

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            return await query.SingleOrDefaultAsync(cancellationToken);
        }

        public Task<bool> ExistsAsync(
            Guid productId,
            Guid unitId,
            Guid companyId,
            Guid? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            return _context.ProductUnits.AnyAsync(
                x =>
                    x.CompanyId == companyId &&
                    x.ProductId == productId &&
                    x.UnitId == unitId &&
                    (!excludedId.HasValue || x.Id != excludedId.Value),
                cancellationToken);
        }

        public Task<ProductUnit?> GetBaseUnitAsync(
            Guid productId,
            Guid companyId,
            Guid? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            return _context.ProductUnits
                .FirstOrDefaultAsync(
                    x =>
                        x.CompanyId == companyId &&
                        x.ProductId == productId &&
                        x.IsBaseUnit &&
                        (!excludedId.HasValue || x.Id != excludedId.Value),
                    cancellationToken);
        }

        public async Task AddAsync(
            ProductUnit productUnit,
            CancellationToken cancellationToken = default)
        {
            await _context.ProductUnits.AddAsync(
                productUnit,
                cancellationToken);
        }

        public void Remove(ProductUnit productUnit)
        {
            _context.ProductUnits.Remove(productUnit);
        }

        public Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
