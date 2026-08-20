using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Repositories
{
    public sealed class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync(
            Guid? companyId,
            string? search,
            CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(companyId, search);

            return await query
                .Include(x => x.ParentCategory)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public Task<int> CountAsync(
            Guid? companyId,
            string? search,
            CancellationToken cancellationToken = default)
        {
            return BuildQuery(companyId, search)
                .CountAsync(cancellationToken);
        }

        public async Task<List<Category>> GetPagedAsync(
            Guid? companyId,
            string? search,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(companyId, search);

            return await query
                .Include(x => x.ParentCategory)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        private IQueryable<Category> BuildQuery(
            Guid? companyId,
            string? search)
        {
            var query = _context.Categories
                .AsNoTracking()
                .Where(x => !companyId.HasValue || x.CompanyId == companyId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var value = search.Trim();

                query = query.Where(x =>
                    x.Name.Contains(value) ||
                    x.Code.Contains(value));
            }

            return query;
        }

        public Task<Category?> GetByIdAsync(
            Guid id,
            Guid? companyId,
            CancellationToken cancellationToken = default)
        {
            return _context.Categories
                .Include(x => x.ParentCategory)
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         (!companyId.HasValue || x.CompanyId == companyId.Value),
                    cancellationToken);
        }

        public Task<bool> CodeExistsAsync(
            string code,
            Guid companyId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var normalizedCode = code.Trim();

            return _context.Categories.AnyAsync(
                x => x.CompanyId == companyId &&
                     x.Code == normalizedCode &&
                     (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
        }

        public Task<bool> HasChildrenAsync(
            Guid categoryId,
            Guid companyId,
            CancellationToken cancellationToken = default)
        {
            return _context.Categories.AnyAsync(
                x => x.CompanyId == companyId &&
                     x.ParentCategoryId == categoryId &&
                     x.IsActive,
                cancellationToken);
        }

        public async Task AddAsync(
            Category category,
            CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddAsync(
                category,
                cancellationToken);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
