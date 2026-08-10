using System.Globalization;
using System.Reflection;
using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Permissions;
using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Inventory;
using MedicalERP.Domain.Support;
using MedicalERP.Infrastructure.Persistence;
using MedicalERP.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Web.Controllers;

[Authorize]
public sealed class InventoryCrudController : Controller
{
    private static readonly IReadOnlyDictionary<string, InventoryDefinition> Definitions =
        new Dictionary<string, InventoryDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["ProductBatches"] = new("Product Batches", typeof(ProductBatch), Permissions.Inventory.Adjust,
                ["StoreId", "ProductId", "WarehouseId", "BatchNumber", "ManufacturingDate", "ExpiryDate", "PurchasePrice", "CostPrice", "SalePrice", "MaximumRetailPrice", "ReceivedAt"]),
            ["InventoryStocks"] = new("Inventory Stock", typeof(InventoryStock), Permissions.Inventory.Adjust,
                ["StoreId", "ProductId", "WarehouseId", "ProductBatchId", "QuantityOnHand", "ReservedQuantity"]),
            ["StockTransactions"] = new("Stock Transactions", typeof(StockTransaction), Permissions.Inventory.Adjust,
                ["StoreId", "ProductId", "TransactionType", "TransactionAt", "QuantityIn", "QuantityOut", "BalanceAfter", "ReferenceNumber", "WarehouseId", "ProductBatchId", "ReferenceType", "ReferenceId", "UnitCost", "Notes"]),
            ["StockAdjustments"] = new("Stock Adjustments", typeof(StockAdjustment), Permissions.Inventory.Adjust,
                ["StoreId", "WarehouseId", "AdjustmentNumber", "AdjustmentType", "AdjustmentDate", "ReasonCodeId", "Notes", "IsPosted"]),
            ["StockAdjustmentItems"] = new("Stock Adjustment Items", typeof(StockAdjustmentItem), Permissions.Inventory.Adjust,
                ["StoreId", "StockAdjustmentId", "ProductId", "ProductBatchId", "Quantity", "UnitCost"]),
            ["StockCounts"] = new("Stock Counts", typeof(StockCount), Permissions.Inventory.Count,
                ["StoreId", "WarehouseId", "CountNumber", "CountDate", "Status", "Notes"]),
            ["StockCountItems"] = new("Stock Count Items", typeof(StockCountItem), Permissions.Inventory.Count,
                ["StoreId", "StockCountId", "ProductId", "ProductBatchId", "SystemQuantity", "CountedQuantity"]),
            ["StockDisposals"] = new("Stock Disposals", typeof(StockDisposal), Permissions.Inventory.Dispose,
                ["StoreId", "WarehouseId", "DisposalNumber", "DisposalDate", "ReasonCodeId", "ApprovedByUserId", "Notes", "IsPosted"]),
            ["StockDisposalItems"] = new("Stock Disposal Items", typeof(StockDisposalItem), Permissions.Inventory.Dispose,
                ["StoreId", "StockDisposalId", "ProductId", "ProductBatchId", "Quantity", "UnitCost"]),
            ["ReasonCodes"] = new("Reason Codes", typeof(ReasonCode), Permissions.Inventory.Adjust,
                ["Code", "Name", "AppliesTo"]),
            ["NumberSequences"] = new("Number Sequences", typeof(NumberSequence), Permissions.Inventory.Adjust,
                ["StoreId", "DocumentType", "Prefix", "NextNumber", "Padding", "ResetYear"])
        };

    private readonly ApplicationDbContext _db;
    private readonly ICompanyContext _companyContext;

    public InventoryCrudController(ApplicationDbContext db, ICompanyContext companyContext)
    {
        _db = db;
        _companyContext = companyContext;
    }

    [HttpGet]
    [HasPermission(Permissions.Inventory.View)]
    public async Task<IActionResult> Index(
        string type = "ProductBatches",
        Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetDefinition(type, out var definition))
        {
            return NotFound();
        }

        ViewBag.CategoryId = categoryId;
        var isInventoryStocks = definition.EntityType == typeof(InventoryStock);
        var isStockTransactions = definition.EntityType == typeof(StockTransaction);
        await PrepareViewAsync(type, definition, cancellationToken);
        var companyId = _companyContext.RequireCompanyId();

        if (isInventoryStocks)
        {
            ViewBag.ProductCategories = await LoadProductCategoriesAsync(companyId, cancellationToken);
            ViewBag.Categories = await LoadCategoryOptionsAsync(companyId, cancellationToken);
        }

        if (isInventoryStocks || isStockTransactions)
        {
            ViewBag.ProductNames = await _db.Products.AsNoTracking()
                .Where(product => product.CompanyId == companyId)
                .ToDictionaryAsync(product => product.Id, product => product.Name, cancellationToken);
        }

        if (isStockTransactions)
        {
            ViewBag.StoreNames = await _db.Stores.AsNoTracking()
                .Where(store => store.CompanyId == companyId)
                .ToDictionaryAsync(store => store.Id, store => store.Name, cancellationToken);
        }

        var records = await Query(definition.EntityType).ToListAsync(cancellationToken);
        var query = records
            .OfType<CompanyEntity>()
            .Where(x => x.CompanyId == companyId);

        if (isInventoryStocks && categoryId.HasValue)
        {
            var categoryProductIds = await _db.Products.AsNoTracking()
                .Where(product => product.CompanyId == companyId && product.CategoryId == categoryId.Value)
                .Select(product => product.Id)
                .ToHashSetAsync(cancellationToken);

            query = query.Where(x => x is InventoryStock stock && categoryProductIds.Contains(stock.ProductId));
        }

        var model = query
            .OrderByDescending(x => x.CreatedAt)
            .Take(500)
            .Cast<object>()
            .ToArray();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(string type, CancellationToken cancellationToken)
    {
        if (!TryGetDefinition(type, out var definition))
        {
            return NotFound();
        }

        if (!User.HasClaim(PermissionClaimTypes.Permission, definition.WritePermission))
        {
            return Forbid();
        }

        var record = Activator.CreateInstance(definition.EntityType);
        if (record is null)
        {
            return NotFound();
        }

        SetCreateDefaults(record, definition);
        await PrepareViewAsync(type, definition, cancellationToken);
        ViewBag.Mode = "Create";
        return View("Form", record);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string type, IFormCollection form, CancellationToken cancellationToken)
    {
        if (!TryGetDefinition(type, out var definition))
        {
            return NotFound();
        }

        if (!User.HasClaim(PermissionClaimTypes.Permission, definition.WritePermission))
        {
            return Forbid();
        }

        var record = Activator.CreateInstance(definition.EntityType);
        if (record is null)
        {
            return NotFound();
        }

        SetCreateDefaults(record, definition);
        ApplyForm(record, definition, form);

        if (!ModelState.IsValid)
        {
            await PrepareViewAsync(type, definition, cancellationToken);
            ViewBag.Mode = "Create";
            return View("Form", record);
        }

        _db.Add(record);
        await _db.SaveChangesAsync(cancellationToken);
        TempData["SuccessMessage"] = $"{definition.Title} record created.";

        return RedirectToAction(nameof(Index), GetRouteValues(type));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string type, Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetDefinition(type, out var definition))
        {
            return NotFound();
        }

        if (!User.HasClaim(PermissionClaimTypes.Permission, definition.WritePermission))
        {
            return Forbid();
        }

        var record = await FindCompanyRecordAsync(definition.EntityType, id, cancellationToken);
        if (record is null)
        {
            return NotFound();
        }

        await PrepareViewAsync(type, definition, cancellationToken);
        ViewBag.Mode = "Edit";
        return View("Form", record);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string type, Guid id, IFormCollection form, CancellationToken cancellationToken)
    {
        if (!TryGetDefinition(type, out var definition))
        {
            return NotFound();
        }

        if (!User.HasClaim(PermissionClaimTypes.Permission, definition.WritePermission))
        {
            return Forbid();
        }

        var record = await FindCompanyRecordAsync(definition.EntityType, id, cancellationToken);
        if (record is null)
        {
            return NotFound();
        }

        ApplyForm(record, definition, form);

        if (!ModelState.IsValid)
        {
            await PrepareViewAsync(type, definition, cancellationToken);
            ViewBag.Mode = "Edit";
            return View("Form", record);
        }

        await _db.SaveChangesAsync(cancellationToken);
        TempData["SuccessMessage"] = $"{definition.Title} record updated.";

        return RedirectToAction(nameof(Index), GetRouteValues(type));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string type, Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetDefinition(type, out var definition))
        {
            return NotFound();
        }

        if (!User.HasClaim(PermissionClaimTypes.Permission, definition.WritePermission))
        {
            return Forbid();
        }

        var record = await FindCompanyRecordAsync(definition.EntityType, id, cancellationToken);
        if (record is null)
        {
            return NotFound();
        }

        await PrepareViewAsync(type, definition, cancellationToken);
        return View(record);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string type, Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetDefinition(type, out var definition))
        {
            return NotFound();
        }

        if (!User.HasClaim(PermissionClaimTypes.Permission, definition.WritePermission))
        {
            return Forbid();
        }

        var record = await FindCompanyRecordAsync(definition.EntityType, id, cancellationToken);
        if (record is null)
        {
            return NotFound();
        }

        ((BaseEntity)record).IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        TempData["SuccessMessage"] = $"{definition.Title} record deactivated.";

        return RedirectToAction(nameof(Index), GetRouteValues(type));
    }

    private async Task PrepareViewAsync(string type, InventoryDefinition definition, CancellationToken cancellationToken)
    {
        ViewBag.TypeKey = type;
        ViewBag.Title = definition.Title;
        ViewBag.Fields = definition.Fields;
        ViewBag.FieldLabels = definition.Fields.ToDictionary(x => x, ToLabel);
        ViewBag.EntityType = definition.EntityType;
        ViewBag.CompanyContextId = GetCompanyContextId();
        ViewBag.Definitions = Definitions.Select(x => new SelectListItem(x.Value.Title, x.Key, x.Key.Equals(type, StringComparison.OrdinalIgnoreCase))).ToArray();
        var options = await LoadOptionsAsync(cancellationToken);
        ViewBag.Options = options;
        ViewBag.OptionMaps = options.ToDictionary(
            x => x.Key,
            x => x.Value
                .Where(item => !string.IsNullOrWhiteSpace(item.Value))
                .GroupBy(item => item.Value)
                .ToDictionary(group => group.Key!, group => group.First().Text),
            StringComparer.OrdinalIgnoreCase);
    }

    private async Task<Dictionary<string, List<SelectListItem>>> LoadOptionsAsync(CancellationToken cancellationToken)
    {
        var companyId = _companyContext.RequireCompanyId();
        var options = new Dictionary<string, List<SelectListItem>>(StringComparer.OrdinalIgnoreCase)
        {
            ["StoreId"] = await _db.Stores.AsNoTracking()
                .Where(x => x.CompanyId == companyId && x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem($"{x.Name} ({x.Code})", x.Id.ToString()))
                .ToListAsync(cancellationToken),
            ["ProductId"] = await _db.Products.AsNoTracking()
                .Where(x => x.CompanyId == companyId && x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToListAsync(cancellationToken),
            ["WarehouseId"] = await _db.Warehouses.AsNoTracking()
                .Where(x => x.CompanyId == companyId && x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem($"{x.Name} ({x.Code})", x.Id.ToString()))
                .ToListAsync(cancellationToken),
            ["ProductBatchId"] = await _db.ProductBatches.AsNoTracking()
                .Where(x => x.CompanyId == companyId && x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SelectListItem(x.BatchNumber, x.Id.ToString()))
                .ToListAsync(cancellationToken),
            ["ReasonCodeId"] = await _db.ReasonCodes.AsNoTracking()
                .Where(x => x.CompanyId == companyId && x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem($"{x.Name} ({x.Code})", x.Id.ToString()))
                .ToListAsync(cancellationToken),
            ["StockAdjustmentId"] = await _db.StockAdjustments.AsNoTracking()
                .Where(x => x.CompanyId == companyId && x.IsActive)
                .OrderByDescending(x => x.AdjustmentDate)
                .Select(x => new SelectListItem(x.AdjustmentNumber, x.Id.ToString()))
                .ToListAsync(cancellationToken),
            ["StockCountId"] = await _db.StockCounts.AsNoTracking()
                .Where(x => x.CompanyId == companyId && x.IsActive)
                .OrderByDescending(x => x.CountDate)
                .Select(x => new SelectListItem(x.CountNumber, x.Id.ToString()))
                .ToListAsync(cancellationToken),
            ["StockDisposalId"] = await _db.StockDisposals.AsNoTracking()
                .Where(x => x.CompanyId == companyId && x.IsActive)
                .OrderByDescending(x => x.DisposalDate)
                .Select(x => new SelectListItem(x.DisposalNumber, x.Id.ToString()))
                .ToListAsync(cancellationToken),
            ["TransactionType"] = EnumOptions<StockTransactionType>(),
            ["ReferenceType"] = EnumOptions<DocumentType>(),
            ["AdjustmentType"] = EnumOptions<AdjustmentType>(),
            ["Status"] = EnumOptions<StockCountStatus>()
        };
        options["AppliesTo"] = options["ReferenceType"];
        options["DocumentType"] = options["ReferenceType"];

        return options;
    }

    private async Task<Dictionary<Guid, string>> LoadProductCategoriesAsync(
        Guid companyId,
        CancellationToken cancellationToken)
    {
        return await _db.Products.AsNoTracking()
            .Where(product => product.CompanyId == companyId)
            .Join(
                _db.Categories.AsNoTracking().Where(category => category.CompanyId == companyId),
                product => product.CategoryId,
                category => category.Id,
                (product, category) => new { product.Id, category.Name })
            .ToDictionaryAsync(x => x.Id, x => x.Name, cancellationToken);
    }

    private async Task<List<SelectListItem>> LoadCategoryOptionsAsync(
        Guid companyId,
        CancellationToken cancellationToken)
    {
        return await _db.Categories.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync(cancellationToken);
    }

    private static List<SelectListItem> EnumOptions<TEnum>()
        where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>()
            .Select(x => new SelectListItem(ToLabel(x.ToString()), x.ToString()))
            .ToList();
    }

    private void SetCreateDefaults(object record, InventoryDefinition definition)
    {
        var now = DateTimeOffset.Now;
        ((CompanyEntity)record).CompanyId = _companyContext.RequireCompanyId();
        ((BaseEntity)record).IsActive = true;

        foreach (var field in definition.Fields)
        {
            var property = definition.EntityType.GetProperty(field);
            if (property?.PropertyType == typeof(DateTimeOffset))
            {
                property.SetValue(record, now);
            }
        }
    }

    private void ApplyForm(object record, InventoryDefinition definition, IFormCollection form)
    {
        ((CompanyEntity)record).CompanyId = _companyContext.RequireCompanyId();

        foreach (var field in definition.Fields)
        {
            var property = definition.EntityType.GetProperty(field);
            if (property is null || !property.CanWrite)
            {
                continue;
            }

            try
            {
                property.SetValue(record, ConvertValue(form, field, property.PropertyType));
            }
            catch (Exception exception) when (exception is FormatException or InvalidCastException or ArgumentException)
            {
                ModelState.AddModelError(field, $"{ToLabel(field)} has an invalid value.");
            }
        }

        ((BaseEntity)record).IsActive = form.ContainsKey("IsActive");
    }

    private static object? ConvertValue(IFormCollection form, string field, Type propertyType)
    {
        if (propertyType == typeof(bool))
        {
            return form.ContainsKey(field);
        }

        var nullableType = Nullable.GetUnderlyingType(propertyType);
        var targetType = nullableType ?? propertyType;
        var value = form[field].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(value))
        {
            if (nullableType is not null)
            {
                return null;
            }

            if (targetType == typeof(string))
            {
                return string.Empty;
            }

            if (targetType == typeof(DateTimeOffset))
            {
                return DateTimeOffset.Now;
            }
        }

        if (targetType == typeof(string))
        {
            return value?.Trim() ?? string.Empty;
        }

        if (targetType == typeof(Guid))
        {
            return Guid.Parse(value ?? string.Empty);
        }

        if (targetType == typeof(decimal))
        {
            return decimal.Parse(value ?? "0", NumberStyles.Number, CultureInfo.InvariantCulture);
        }

        if (targetType == typeof(DateOnly))
        {
            return DateOnly.Parse(value ?? string.Empty, CultureInfo.InvariantCulture);
        }

        if (targetType == typeof(DateTimeOffset))
        {
            if (DateTimeOffset.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var offset))
            {
                return offset;
            }

            return DateTimeOffset.Parse(value ?? string.Empty, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
        }

        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, value ?? string.Empty);
        }

        return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
    }

    private async Task<object?> FindCompanyRecordAsync(Type entityType, Guid id, CancellationToken cancellationToken)
    {
        var record = await _db.FindAsync(entityType, [id], cancellationToken);
        if (record is not CompanyEntity companyEntity || companyEntity.CompanyId != _companyContext.RequireCompanyId())
        {
            return null;
        }

        return record;
    }

    private IQueryable<object> Query(Type entityType)
    {
        var method = typeof(DbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.Name == nameof(DbContext.Set) && x.IsGenericMethod && x.GetParameters().Length == 0);
        var set = method.MakeGenericMethod(entityType).Invoke(_db, null);
        return ((IQueryable)set!).Cast<object>();
    }

    private bool TryGetDefinition(string type, out InventoryDefinition definition)
    {
        return Definitions.TryGetValue(type, out definition!);
    }

    private object GetRouteValues(string type)
    {
        var companyContextId = GetCompanyContextId();
        return new
        {
            type,
            companyContextId = string.IsNullOrWhiteSpace(companyContextId) ? null : companyContextId
        };
    }

    private string? GetCompanyContextId()
    {
        var companyContextId = Request.Query["companyContextId"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(companyContextId) && Request.HasFormContentType)
        {
            companyContextId = Request.Form["companyContextId"].FirstOrDefault();
        }

        return companyContextId;
    }

    private static string ToLabel(string value)
    {
        return string.Concat(value.Select((character, index) =>
            index > 0 && char.IsUpper(character) ? " " + character : character.ToString()));
    }

    private sealed record InventoryDefinition(string Title, Type EntityType, string WritePermission, string[] Fields);
}
