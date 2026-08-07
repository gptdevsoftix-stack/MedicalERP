using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Purchases.Dtos;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Purchases;

namespace MedicalERP.Application.Services;

public sealed class PurchaseOrderService(
    IPurchaseOrderRepository repository,
    ICompanyContext companyContext,
    IStoreContext storeContext,
    ICurrentUserService currentUser) : IPurchaseOrderService
{
    public async Task<PagedResult<PurchaseOrderListDto>> GetAsync(PurchaseOrderFilterDto filter, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany();
        var storeId = await RequireStoreAsync(cancellationToken);
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 10, 100);
        int? status = filter.Status.HasValue ? (int)filter.Status.Value : null;
        var total = await repository.CountAsync(companyId, storeId, filter.Search, status, cancellationToken);
        var rows = await repository.GetAsync(companyId, storeId, filter.Search, status, (page - 1) * pageSize, pageSize, cancellationToken);
        return new PagedResult<PurchaseOrderListDto>(rows.Select(MapList).ToList(), page, pageSize, total);
    }

    public async Task<PurchaseOrderFormDto?> GetForEditAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(id, false, cancellationToken);
        return order is null ? null : MapForm(order);
    }

    public async Task<PurchaseOrderFormDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(id, false, cancellationToken);
        return order is null ? null : MapForm(order);
    }

    public async Task<Guid> CreateAsync(PurchaseOrderFormDto request, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany();
        var storeId = await RequireStoreAsync(cancellationToken);
        await ValidateAsync(request, companyId, storeId, null, cancellationToken);

        var order = new PurchaseOrder
        {
            Id = Guid.NewGuid(), CompanyId = companyId, StoreId = storeId,
            SupplierId = request.SupplierId, WarehouseId = request.WarehouseId,
            OrderNumber = request.OrderNumber.Trim(), OrderDate = request.OrderDate,
            ExpectedDeliveryDate = request.ExpectedDeliveryDate, OtherCharges = request.OtherCharges,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(), Status = OrderStatus.Draft
        };
        ReplaceItems(order, request);
        CalculateTotals(order);
        await repository.AddAsync(order, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return order.Id;
    }

    public async Task UpdateAsync(PurchaseOrderFormDto request, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany();
        var storeId = await RequireStoreAsync(cancellationToken);
        var order = await repository.GetByIdAsync(request.Id, companyId, storeId, true, cancellationToken) ?? throw new KeyNotFoundException("Purchase order was not found.");
        if (order.Status != OrderStatus.Draft) throw new InvalidOperationException("Only draft purchase orders can be edited.");
        await ValidateAsync(request, companyId, storeId, request.Id, cancellationToken);

        order.SupplierId = request.SupplierId; order.WarehouseId = request.WarehouseId;
        order.OrderNumber = request.OrderNumber.Trim(); order.OrderDate = request.OrderDate;
        order.ExpectedDeliveryDate = request.ExpectedDeliveryDate; order.OtherCharges = request.OtherCharges;
        order.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        repository.RemoveItems(order.Items.ToList());
        order.Items.Clear();
        ReplaceItems(order, request);
        CalculateTotals(order);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public Task SubmitAsync(Guid id, CancellationToken cancellationToken = default) => ChangeStatusAsync(id, OrderStatus.Pending, cancellationToken);

    public async Task ApproveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await GetRequiredTrackedOrderAsync(id, cancellationToken);
        if (order.Status != OrderStatus.Pending) throw new InvalidOperationException("Only pending purchase orders can be approved.");
        order.Status = OrderStatus.Approved;
        order.ApprovedByUserId = currentUser.UserId?.ToString();
        order.ApprovedAt = DateTimeOffset.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
    }

    public Task CancelAsync(Guid id, CancellationToken cancellationToken = default) => ChangeStatusAsync(id, OrderStatus.Cancelled, cancellationToken);

    public async Task<IReadOnlyList<PurchaseLookupDto>> GetSuppliersAsync(CancellationToken cancellationToken = default)
    {
        var records = await repository.GetSuppliersAsync(RequireCompany(), await RequireStoreAsync(cancellationToken), cancellationToken);
        return records.Select(x => new PurchaseLookupDto { Id = x.Id, Name = $"{x.Name} ({x.Code})" }).ToArray();
    }

    public async Task<IReadOnlyList<PurchaseLookupDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var records = await repository.GetProductsAsync(RequireCompany(), cancellationToken);
        return records.Select(x => new PurchaseLookupDto { Id = x.Id, Name = $"{x.Name} ({x.Code})" }).ToArray();
    }

    public async Task<IReadOnlyList<PurchaseLookupDto>> GetProductUnitsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var records = await repository.GetProductUnitsAsync(productId, RequireCompany(), cancellationToken);
        return records.Select(x => new PurchaseLookupDto { Id = x.Id, Name = $"{x.Unit.Name} x {x.ConversionFactor:0.####}" }).ToArray();
    }

    public async Task<IReadOnlyList<PurchaseLookupDto>> GetWarehousesAsync(CancellationToken cancellationToken = default)
    {
        var records = await repository.GetWarehousesAsync(RequireCompany(), await RequireStoreAsync(cancellationToken), cancellationToken);
        return records.Select(x => new PurchaseLookupDto { Id = x.Id, Name = $"{x.Name} ({x.Code})" }).ToArray();
    }

    private async Task ChangeStatusAsync(Guid id, OrderStatus target, CancellationToken cancellationToken)
    {
        var order = await GetRequiredTrackedOrderAsync(id, cancellationToken);
        if (target == OrderStatus.Pending && order.Status != OrderStatus.Draft) throw new InvalidOperationException("Only draft purchase orders can be submitted.");
        if (target == OrderStatus.Cancelled && order.Status is OrderStatus.Fulfilled or OrderStatus.Closed or OrderStatus.Cancelled) throw new InvalidOperationException("This purchase order cannot be cancelled.");
        order.Status = target;
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<PurchaseOrder> GetRequiredTrackedOrderAsync(Guid id, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, RequireCompany(), await RequireStoreAsync(cancellationToken), true, cancellationToken) ?? throw new KeyNotFoundException("Purchase order was not found.");
    }

    private async Task<PurchaseOrder?> GetOrderAsync(Guid id, bool tracking, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, RequireCompany(), await RequireStoreAsync(cancellationToken), tracking, cancellationToken);
    }

    private async Task ValidateAsync(PurchaseOrderFormDto request, Guid companyId, Guid storeId, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (request.SupplierId == Guid.Empty || !(await repository.GetSuppliersAsync(companyId, storeId, cancellationToken)).Any(x => x.Id == request.SupplierId)) throw new InvalidOperationException("Select an active supplier assigned to this store.");
        if (string.IsNullOrWhiteSpace(request.OrderNumber)) throw new InvalidOperationException("Order number is required.");
        if (await repository.OrderNumberExistsAsync(companyId, storeId, request.OrderNumber.Trim(), excludedId, cancellationToken)) throw new InvalidOperationException("Order number already exists in this store.");
        if (request.OrderDate == default) throw new InvalidOperationException("Order date is required.");
        if (request.ExpectedDeliveryDate.HasValue && request.ExpectedDeliveryDate.Value < request.OrderDate) throw new InvalidOperationException("Expected delivery cannot be before the order date.");
        if (request.WarehouseId.HasValue && !(await repository.GetWarehousesAsync(companyId, storeId, cancellationToken)).Any(x => x.Id == request.WarehouseId.Value)) throw new InvalidOperationException("Warehouse was not found in this store.");
        if (request.Items.Count == 0) throw new InvalidOperationException("Add at least one product line.");

        var products = await repository.GetProductsAsync(companyId, cancellationToken);
        foreach (var item in request.Items)
        {
            if (item.ProductId == Guid.Empty || item.ProductUnitId == Guid.Empty || item.OrderedQuantity <= 0) throw new InvalidOperationException("Each line needs a product, unit, and quantity greater than zero.");
            if (item.FreeQuantity < 0 || item.UnitPrice < 0 || item.DiscountAmount < 0 || item.TaxAmount < 0) throw new InvalidOperationException("Line amounts cannot be negative.");
            if (!products.Any(x => x.Id == item.ProductId)) throw new InvalidOperationException("One or more selected products are invalid.");
            var units = await repository.GetProductUnitsAsync(item.ProductId, companyId, cancellationToken);
            var unit = units.FirstOrDefault(x => x.Id == item.ProductUnitId) ?? throw new InvalidOperationException("One or more selected product units are invalid.");
            if (unit.ConversionFactor <= 0) throw new InvalidOperationException("Product unit conversion factor must be greater than zero.");
            item.ConversionFactor = unit.ConversionFactor;
        }
    }

    private static void ReplaceItems(PurchaseOrder order, PurchaseOrderFormDto request)
    {
        foreach (var item in request.Items)
        {
            order.Items.Add(new PurchaseOrderItem
            {
                Id = Guid.NewGuid(),
                CompanyId = order.CompanyId, StoreId = order.StoreId, ProductId = item.ProductId, ProductUnitId = item.ProductUnitId,
                OrderedQuantity = item.OrderedQuantity, FreeQuantity = item.FreeQuantity, UnitPrice = item.UnitPrice,
                DiscountAmount = item.DiscountAmount, TaxAmount = item.TaxAmount, ConversionFactor = item.ConversionFactor
            });
        }
    }

    private static void CalculateTotals(PurchaseOrder order)
    {
        order.Subtotal = order.Items.Sum(x => x.OrderedQuantity * x.UnitPrice);
        order.DiscountAmount = order.Items.Sum(x => x.DiscountAmount);
        order.TaxAmount = order.Items.Sum(x => x.TaxAmount);
        foreach (var item in order.Items) item.LineTotal = item.OrderedQuantity * item.UnitPrice - item.DiscountAmount + item.TaxAmount;
        order.GrandTotal = order.Subtotal - order.DiscountAmount + order.TaxAmount + order.OtherCharges;
    }

    private static PurchaseOrderListDto MapList(PurchaseOrder x) => new()
    {
        Id = x.Id, OrderNumber = x.OrderNumber, SupplierName = x.Supplier.Name, OrderDate = x.OrderDate,
        ExpectedDeliveryDate = x.ExpectedDeliveryDate, Status = x.Status, GrandTotal = x.GrandTotal,
        ItemCount = x.Items.Count, WarehouseName = null
    };

    private static PurchaseOrderFormDto MapForm(PurchaseOrder x) => new()
    {
        Id = x.Id, SupplierId = x.SupplierId, SupplierName = x.Supplier.Name, WarehouseId = x.WarehouseId, OrderNumber = x.OrderNumber,
        OrderDate = x.OrderDate, ExpectedDeliveryDate = x.ExpectedDeliveryDate, OtherCharges = x.OtherCharges,
        Notes = x.Notes, Status = x.Status,
        Items = x.Items.Select(i => new PurchaseOrderItemFormDto { Id = i.Id, ProductId = i.ProductId, ProductName = i.Product.Name, ProductUnitId = i.ProductUnitId, ProductUnitName = i.Product.Units.FirstOrDefault(u => u.Id == i.ProductUnitId)?.Unit.Name ?? string.Empty, ConversionFactor = i.ConversionFactor, OrderedQuantity = i.OrderedQuantity, FreeQuantity = i.FreeQuantity, UnitPrice = i.UnitPrice, DiscountAmount = i.DiscountAmount, TaxAmount = i.TaxAmount }).ToList()
    };

    private Guid RequireCompany() => companyContext.CompanyId ?? throw new UnauthorizedAccessException("Company context is missing.");
    private async Task<Guid> RequireStoreAsync(CancellationToken cancellationToken) { var id = storeContext.RequireSelectedStoreId(); await storeContext.EnsureStoreAccessAsync(id, cancellationToken); return id; }
}
