using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.DTOs;
using MedicalERP.Domain.Interfaces;


namespace MedicalERP.Application.Services
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly ICompanyContext _companyContext;

        public CategoryService(
            ICategoryRepository repository,
            ICompanyContext companyContext)
        {
            _repository = repository;
            _companyContext = companyContext;
        }

        public async Task<List<CategoryDto>> GetAllAsync(
            string? search,
            CancellationToken cancellationToken = default)
        {
            var companyId = _companyContext.CompanyId;

            var categories = await _repository.GetAllAsync(
                companyId,
                search,
                cancellationToken);

            return categories.Select(MapToDto).ToList();
        }

        public async Task<PagedResult<CategoryDto>> GetAllPagedAsync(
            string? search,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var companyId = _companyContext.CompanyId;

            var totalCount = await _repository.CountAsync(
                companyId, search, cancellationToken);

            var categories = await _repository.GetPagedAsync(
                companyId, search, page, pageSize, cancellationToken);

            var items = categories.Select(MapToDto).ToList();

            return new PagedResult<CategoryDto>(items, page, pageSize, totalCount);
        }

        public async Task<CategoryDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var companyId = _companyContext.CompanyId;

            var category = await _repository.GetByIdAsync(
                id,
                companyId,
                cancellationToken);

            return category is null ? null : MapToDto(category);
        }

        public async Task<UpdateCategoryDto?> GetForEditAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var companyId = _companyContext.CompanyId;

            var category = await _repository.GetByIdAsync(
                id,
                companyId,
                cancellationToken);

            if (category is null)
                return null;

            return new UpdateCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Code = category.Code,
                ParentCategoryId = category.ParentCategoryId,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive
            };
        }

        public async Task<Guid> CreateAsync(
            CreateCategoryDto request,
            CancellationToken cancellationToken = default)
        {
            var companyId = _companyContext.RequireCompanyId();
            var code = request.Code.Trim().ToUpperInvariant();

            if (await _repository.CodeExistsAsync(
                    code,
                    companyId,
                    cancellationToken: cancellationToken))
            {
                throw new InvalidOperationException(
                    "A category with this code already exists.");
            }

            await ValidateParentAsync(
                request.ParentCategoryId,
                companyId,
                cancellationToken);

            var category = new Category
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                Name = request.Name.Trim(),
                Code = code,
                ParentCategoryId = request.ParentCategoryId,
                DisplayOrder = request.DisplayOrder,
                IsActive = true
            };

            await _repository.AddAsync(category, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return category.Id;
        }

        public async Task UpdateAsync(
            UpdateCategoryDto request,
            CancellationToken cancellationToken = default)
        {
            var companyId = _companyContext.RequireCompanyId();

            var category = await _repository.GetByIdAsync(
                request.Id,
                companyId,
                cancellationToken)
                ?? throw new KeyNotFoundException("Category not found.");

            if (request.ParentCategoryId == request.Id)
            {
                throw new InvalidOperationException(
                    "A category cannot be its own parent.");
            }

            var code = request.Code.Trim().ToUpperInvariant();

            if (await _repository.CodeExistsAsync(
                    code,
                    companyId,
                    request.Id,
                    cancellationToken))
            {
                throw new InvalidOperationException(
                    "A category with this code already exists.");
            }

            await ValidateParentAsync(
                request.ParentCategoryId,
                companyId,
                cancellationToken);

            category.Name = request.Name.Trim();
            category.Code = code;
            category.ParentCategoryId = request.ParentCategoryId;
            category.DisplayOrder = request.DisplayOrder;
            category.IsActive = request.IsActive;

            _repository.Update(category);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var companyId = _companyContext.RequireCompanyId();

            var category = await _repository.GetByIdAsync(
                id,
                companyId,
                cancellationToken)
                ?? throw new KeyNotFoundException("Category not found.");

            if (await _repository.HasChildrenAsync(
                    id,
                    companyId,
                    cancellationToken))
            {
                throw new InvalidOperationException(
                    "Deactivate the child categories first.");
            }

            category.IsActive = false;

            _repository.Update(category);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        private async Task ValidateParentAsync(
            Guid? parentCategoryId,
            Guid companyId,
            CancellationToken cancellationToken)
        {
            if (!parentCategoryId.HasValue)
                return;

            var parent = await _repository.GetByIdAsync(
                parentCategoryId.Value,
                companyId,
                cancellationToken);

            if (parent is null || !parent.IsActive)
            {
                throw new InvalidOperationException(
                    "The selected parent category is invalid.");
            }
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Code = category.Code,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive
            };
        }
    }
}
