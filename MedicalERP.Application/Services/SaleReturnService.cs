using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Sales.Dtos;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Inventory;
using MedicalERP.Domain.Sales;
using MedicalERP.Domain.Support;

namespace MedicalERP.Application.Services;

public sealed class SaleReturnService(
    ISaleReturnRepository repository,
    ICompanyContext companyContext,
    IStoreContext storeContext) : ISaleReturnService
{
    public async Task<PagedResult<SaleReturnListDto>> GetAsync(SaleReturnFilterDto filter, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany();
        var storeId = await RequireStoreAsync(cancellationToken);
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 10, 100);
        int? status = filter.Status.HasValue ? (int)filter.Status.Value : null;
        var total = await repository.CountAsync(companyId, storeId, filter.Search, status, cancellationToken);
        var rows = await repository.GetAsync(companyId, storeId, filter.Search, status, (page - 1) * pageSize, pageSize, cancellationToken);
        return new PagedResult<SaleReturnListDto>(rows.Select(MapList).ToList(), page, pageSize, total);
    }

    public async Task<SaleReturnFormDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var saleReturn = await repository.GetByIdAsync(id, RequireCompany(), await RequireStoreAsync(cancellationToken), false, cancellationToken);
        return saleReturn is null ? null : await MapReturnAsync(saleReturn, cancellationToken);
    }

    public async Task<SaleReturnFormDto?> GetForReturnAsync(Guid saleId, CancellationToken cancellationToken = default)
    {
        var sale = await repository.GetSaleForReturnAsync(saleId, RequireCompany(), await RequireStoreAsync(cancellationToken), false, cancellationToken);
        if (sale is null) return null;
        if (sale.Status is SaleStatus.Cancelled or SaleStatus.Draft or SaleStatus.Held)
            throw new InvalidOperationException("Only confirmed sales can be returned.");

        var items = sale.Items
            .Where(x => x.Quantity - x.ReturnedQuantity > 0)
            .Select(x =>
            {
                var batch = x.Batches.FirstOrDefault();
                return new SaleReturnItemFormDto
                {
                    SaleItemId = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    ProductBatchId = batch?.ProductBatchId,
                    BatchNumber = batch?.ProductBatch.BatchNumber ?? string.Empty,
                    ConversionFactor = x.ConversionFactor,
                    UnitPrice = x.UnitPrice,
                    AvailableQuantity = x.Quantity - x.ReturnedQuantity,
                    ReturnToStock = true
                };
            })
            .ToList();

        if (items.Count == 0)
            throw new InvalidOperationException("This sale has no items left to return.");

        return new SaleReturnFormDto
        {
            SaleId = sale.Id,
            InvoiceNumber = sale.InvoiceNumber,
            WarehouseId = sale.WarehouseId,
            ReturnNumber = $"RET-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
            ReturnDate = DateTimeOffset.Now,
            Items = items
        };
    }

    public async Task<Guid> CreateAsync(SaleReturnFormDto request, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany();
        var storeId = await RequireStoreAsync(cancellationToken);

        var sale = await repository.GetSaleForReturnAsync(request.SaleId, companyId, storeId, true, cancellationToken)
            ?? throw new KeyNotFoundException("Sale was not found.");
        if (sale.Status is SaleStatus.Cancelled or SaleStatus.Draft or SaleStatus.Held)
            throw new InvalidOperationException("Only confirmed sales can be returned.");
        if (request.ReturnDate == default) throw new InvalidOperationException("Return date is required.");
        if (string.IsNullOrWhiteSpace(request.ReturnNumber)) throw new InvalidOperationException("Return number is required.");
        if (await repository.ReturnNumberExistsAsync(companyId, storeId, request.ReturnNumber.Trim(), null, cancellationToken))
            throw new InvalidOperationException("Return number already exists in this store.");
        if (request.WarehouseId.HasValue && !(await repository.GetWarehousesAsync(companyId, storeId, cancellationToken)).Any(x => x.Id == request.WarehouseId.Value))
            throw new InvalidOperationException("Warehouse was not found in this store.");
        if (request.Items.Count == 0) throw new InvalidOperationException("Add at least one return line.");

        var available = sale.Items.ToDictionary(x => x.Id, x => x.Quantity - x.ReturnedQuantity);

        var saleReturn = new SaleReturn
        {
            Id = Guid.NewGuid(), CompanyId = companyId, StoreId = storeId,
            SaleId = sale.Id, WarehouseId = request.WarehouseId,
            ReturnNumber = request.ReturnNumber.Trim(), ReturnDate = request.ReturnDate,
            Status = ReturnStatus.Posted,
            Reason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim()
        };

        decimal subtotal = 0, tax = 0;
        var returnedByItem = new Dictionary<Guid, decimal>();
        foreach (var line in request.Items)
        {
            var saleItem = sale.Items.FirstOrDefault(x => x.Id == line.SaleItemId)
                ?? throw new InvalidOperationException("One or more return lines reference a sale item that was not found.");
            if (line.Quantity <= 0) throw new InvalidOperationException($"Return quantity must be greater than zero for '{saleItem.Product.Name}'.");
            var alreadyReturned = returnedByItem.GetValueOrDefault(saleItem.Id);
            if (line.Quantity + alreadyReturned > available[saleItem.Id])
                throw new InvalidOperationException($"Cannot return more than {available[saleItem.Id]:0.####} for '{saleItem.Product.Name}'.");
            if (line.UnitPrice < 0 || line.TaxAmount < 0) throw new InvalidOperationException("Line amounts cannot be negative.");

            var conversionFactor = saleItem.ConversionFactor;
            var baseQuantity = line.Quantity * conversionFactor;
            var lineTotal = line.Quantity * line.UnitPrice + line.TaxAmount;
            subtotal += line.Quantity * line.UnitPrice;
            tax += line.TaxAmount;

            saleReturn.Items.Add(new SaleReturnItem
            {
                Id = Guid.NewGuid(), CompanyId = companyId, StoreId = storeId,
                SaleReturnId = saleReturn.Id, SaleItemId = saleItem.Id, ProductId = line.ProductId,
                ProductBatchId = line.ProductBatchId, Quantity = line.Quantity, BaseQuantity = baseQuantity,
                UnitPrice = line.UnitPrice, TaxAmount = line.TaxAmount, LineTotal = lineTotal,
                ReturnToStock = line.ReturnToStock
            });

            if (line.ReturnToStock)
                await RestoreStockAsync(companyId, storeId, saleReturn, line, baseQuantity, cancellationToken);

            saleItem.ReturnedQuantity += line.Quantity;
            returnedByItem[saleItem.Id] = alreadyReturned + line.Quantity;
        }

        saleReturn.Subtotal = subtotal;
        saleReturn.TaxAmount = tax;
        saleReturn.RefundAmount = subtotal + tax;

        UpdateSaleReturnStatus(sale);

        await repository.AddAsync(saleReturn, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return saleReturn.Id;
    }

    public async Task<IReadOnlyList<SaleLookupDto>> GetWarehousesAsync(CancellationToken cancellationToken = default)
    {
        var records = await repository.GetWarehousesAsync(RequireCompany(), await RequireStoreAsync(cancellationToken), cancellationToken);
        return records.Select(x => new SaleLookupDto { Id = x.Id, Name = $"{x.Name} ({x.Code})" }).ToArray();
    }

    private async Task RestoreStockAsync(Guid companyId, Guid storeId, SaleReturn saleReturn, SaleReturnItemFormDto line, decimal baseQuantity, CancellationToken cancellationToken)
    {
        var stock = await repository.GetInventoryStockAsync(companyId, storeId, line.ProductId, saleReturn.WarehouseId, null, cancellationToken);
        if (stock is null)
        {
            stock = new InventoryStock
            {
                Id = Guid.NewGuid(), CompanyId = companyId, StoreId = storeId,
                ProductId = line.ProductId, WarehouseId = saleReturn.WarehouseId, ProductBatchId = null,
                QuantityOnHand = 0, ReservedQuantity = 0
            };
            await repository.AddInventoryStockAsync(stock, cancellationToken);
        }

        stock.QuantityOnHand += baseQuantity;

        await repository.AddStockTransactionAsync(new StockTransaction
        {
            Id = Guid.NewGuid(), CompanyId = companyId, StoreId = storeId,
            ProductId = line.ProductId, WarehouseId = saleReturn.WarehouseId, ProductBatchId = null,
            TransactionType = StockTransactionType.SaleReturn,
            ReferenceType = DocumentType.SaleReturn,
            ReferenceId = saleReturn.Id, ReferenceNumber = saleReturn.ReturnNumber,
            QuantityIn = baseQuantity, QuantityOut = 0,
            BalanceAfter = stock.QuantityOnHand,
            UnitCost = line.UnitPrice,
            TransactionAt = saleReturn.ReturnDate,
            Notes = "Stock restored from sale return."
        }, cancellationToken);
    }

    private static void UpdateSaleReturnStatus(Sale sale)
    {
        var fullyReturned = sale.Items.Count > 0 && sale.Items.All(x => x.ReturnedQuantity >= x.Quantity);
        sale.Status = fullyReturned ? SaleStatus.Returned : SaleStatus.PartiallyReturned;
        sale.PaymentStatus = fullyReturned ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
    }

    private static SaleReturnListDto MapList(SaleReturn x) => new()
    {
        Id = x.Id, ReturnNumber = x.ReturnNumber, InvoiceNumber = x.Sale?.InvoiceNumber,
        ReturnDate = x.ReturnDate, Status = x.Status, RefundAmount = x.RefundAmount, ItemCount = x.Items.Count
    };

    private async Task<SaleReturnFormDto> MapReturnAsync(SaleReturn x, CancellationToken cancellationToken)
    {
        var warehouses = (await repository.GetWarehousesAsync(x.CompanyId, x.StoreId, cancellationToken)).ToDictionary(w => w.Id, w => w.Name);
        return new SaleReturnFormDto
        {
            Id = x.Id, SaleId = x.SaleId, InvoiceNumber = x.Sale?.InvoiceNumber ?? string.Empty,
            WarehouseId = x.WarehouseId,
            WarehouseName = x.WarehouseId.HasValue && warehouses.TryGetValue(x.WarehouseId.Value, out var name) ? name : null,
            ReturnNumber = x.ReturnNumber, ReturnDate = x.ReturnDate, Status = x.Status,
            Subtotal = x.Subtotal, TaxAmount = x.TaxAmount, RefundAmount = x.RefundAmount, Reason = x.Reason,
            Items = x.Items.Select(i => new SaleReturnItemFormDto
            {
                SaleItemId = i.SaleItemId, ProductId = i.ProductId, ProductName = i.Product?.Name,
                ProductBatchId = i.ProductBatchId, BatchNumber = i.ProductBatch?.BatchNumber,
                ConversionFactor = 1, UnitPrice = i.UnitPrice, Quantity = i.Quantity,
                AvailableQuantity = i.Quantity, TaxAmount = i.TaxAmount, ReturnToStock = i.ReturnToStock
            }).ToList()
        };
    }

    private Guid RequireCompany() => companyContext.CompanyId ?? throw new UnauthorizedAccessException("Company context is missing.");
    private async Task<Guid> RequireStoreAsync(CancellationToken cancellationToken) { var id = storeContext.RequireSelectedStoreId(); await storeContext.EnsureStoreAccessAsync(id, cancellationToken); return id; }
}
