using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Common;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Sales.Dtos;
using MedicalERP.Domain.Enums;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Domain.Inventory;
using MedicalERP.Domain.Operations;
using MedicalERP.Domain.Sales;
using MedicalERP.Domain.Support;

namespace MedicalERP.Application.Services;

public sealed class SaleService(
    ISaleRepository repository,
    ICompanyContext companyContext,
    IStoreContext storeContext,
    ICurrentUserService currentUser) : ISaleService
{
    private static readonly (string Code, string Name, PaymentMethodType Type, bool RequiresReference)[] StandardPaymentMethods =
    [
        ("CASH", "Cash", PaymentMethodType.Cash, false),
        ("CARD", "Card", PaymentMethodType.Card, true),
        ("ONLINE", "Online Payment", PaymentMethodType.MobileWallet, true),
        ("BANK_TRANSFER", "Bank Transfer", PaymentMethodType.BankTransfer, true),
        ("OTHER", "Other", PaymentMethodType.Other, false)
    ];

    public async Task<PagedResult<SaleListDto>> GetAsync(SaleFilterDto filter, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany();
        var storeId = await RequireStoreAsync(cancellationToken);
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 10, 100);
        int? status = filter.Status.HasValue ? (int)filter.Status.Value : null;
        int? paymentStatus = filter.PaymentStatus.HasValue ? (int)filter.PaymentStatus.Value : null;
        var total = await repository.CountAsync(companyId, storeId, filter.Search, status, paymentStatus, filter.From, filter.To, cancellationToken);
        var rows = await repository.GetAsync(companyId, storeId, filter.Search, status, paymentStatus, filter.From, filter.To, (page - 1) * pageSize, pageSize, cancellationToken);
        return new PagedResult<SaleListDto>(rows.Select(MapList).ToList(), page, pageSize, total);
    }

    public async Task<SaleFormDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await repository.GetByIdAsync(id, RequireCompany(), await RequireStoreAsync(cancellationToken), false, cancellationToken);
        return sale is null ? null : MapForm(sale);
    }

    public async Task<Guid> CreateAsync(SaleFormDto request, CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany();
        var storeId = await RequireStoreAsync(cancellationToken);
        await ValidateAsync(request, companyId, storeId, cancellationToken);

        var sessionId = request.RegisterSessionId ?? await EnsureOpenRegisterSessionAsync(cancellationToken);
        var products = await repository.GetProductsAsync(companyId, storeId, cancellationToken);

        var sale = new Sale
        {
            Id = Guid.NewGuid(), CompanyId = companyId, StoreId = storeId,
            WarehouseId = request.WarehouseId, RegisterSessionId = sessionId,
            CustomerId = request.CustomerId,
            InvoiceNumber = request.InvoiceNumber.Trim(), SaleDate = request.SaleDate,
            Status = SaleStatus.Confirmed, PaymentStatus = PaymentStatus.Unpaid,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
        };

        decimal subtotal = 0, itemDiscount = 0, tax = 0;
        foreach (var line in request.Items)
        {
            var product = products.FirstOrDefault(x => x.Id == line.ProductId)
                ?? throw new InvalidOperationException("One or more selected products are invalid.");
            var baseQuantity = line.Quantity * line.ConversionFactor;
            var lineTotal = line.Quantity * line.UnitPrice - line.DiscountAmount + line.TaxAmount;
            subtotal += line.Quantity * line.UnitPrice;
            itemDiscount += line.DiscountAmount;
            tax += line.TaxAmount;

            var saleItem = new SaleItem
            {
                Id = Guid.NewGuid(), CompanyId = companyId, StoreId = storeId,
                SaleId = sale.Id, ProductId = line.ProductId, ProductUnitId = line.ProductUnitId,
                Quantity = line.Quantity, BaseQuantity = baseQuantity, ConversionFactor = line.ConversionFactor,
                UnitPrice = line.UnitPrice, GrossAmount = line.Quantity * line.UnitPrice,
                DiscountAmount = line.DiscountAmount, TaxAmount = line.TaxAmount, NetAmount = lineTotal
            };
            sale.Items.Add(saleItem);

            if (product.AllowNegativeStock is false)
            {
                var available = await GetAvailableAsync(companyId, storeId, line.ProductId, request.WarehouseId, cancellationToken);
                if (available < baseQuantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for '{product.Name}'. Available: {available:0.####}, required: {baseQuantity:0.####}.");
                }
            }

            await DeductStockAsync(companyId, storeId, sale, saleItem, line, product, cancellationToken);
        }

        sale.Subtotal = subtotal;
        sale.ItemDiscount = itemDiscount;
        sale.TaxAmount = tax;
        sale.GrandTotal = subtotal - itemDiscount + tax;

        await ApplyPaymentAsync(sale, request, cancellationToken);
        await repository.AddAsync(sale, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return sale.Id;
    }

    public async Task<Guid> EnsureOpenRegisterSessionAsync(CancellationToken cancellationToken = default)
    {
        var companyId = RequireCompany();
        var storeId = await RequireStoreAsync(cancellationToken);

        var existing = await repository.GetOpenRegisterSessionAsync(companyId, storeId, cancellationToken);
        if (existing is not null) return existing.Id;

        var register = await repository.GetDefaultRegisterAsync(companyId, storeId, cancellationToken);
        if (register is null)
        {
            register = new Register
            {
                Id = Guid.NewGuid(), CompanyId = companyId, StoreId = storeId,
                Name = "Main Register", Code = "REG-MAIN", IsEnabled = true
            };
            await repository.AddRegisterAsync(register, cancellationToken);
        }

        var session = new RegisterSession
        {
            Id = Guid.NewGuid(), CompanyId = companyId, StoreId = storeId,
            RegisterId = register.Id, CashierUserId = currentUser.UserId?.ToString() ?? string.Empty,
            OpenedAt = DateTimeOffset.Now, OpeningCash = 0, Status = RegisterSessionStatus.Open
        };
        await repository.AddRegisterSessionAsync(session, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return session.Id;
    }

    public async Task<IReadOnlyList<SaleLookupDto>> GetCustomersAsync(CancellationToken cancellationToken = default)
    {
        var records = await repository.GetCustomersAsync(RequireCompany(), cancellationToken);
        return records.Select(x => new SaleLookupDto { Id = x.Id, Name = $"{x.Name} ({x.Code})" }).ToArray();
    }

    public async Task<IReadOnlyList<SaleProductLookupDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var records = await repository.GetProductsAsync(RequireCompany(), await RequireStoreAsync(cancellationToken), cancellationToken);
        return records.Select(x => new SaleProductLookupDto { Id = x.Id, Name = x.Name, AvailableStock = x.AvailableStock, SalePrice = x.SalePrice }).ToArray();
    }

    public async Task<IReadOnlyList<SaleLookupDto>> GetProductUnitsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var records = await repository.GetProductUnitsAsync(productId, RequireCompany(), cancellationToken);
        return records.Select(x => new SaleLookupDto { Id = x.Id, Name = $"{x.Unit.Name} x {x.ConversionFactor:0.####}" }).ToArray();
    }

    public async Task<IReadOnlyList<SaleLookupDto>> GetWarehousesAsync(CancellationToken cancellationToken = default)
    {
        var records = await repository.GetWarehousesAsync(RequireCompany(), await RequireStoreAsync(cancellationToken), cancellationToken);
        return records.Select(x => new SaleLookupDto { Id = x.Id, Name = $"{x.Name} ({x.Code})" }).ToArray();
    }

    public async Task<IReadOnlyList<SaleLookupDto>> GetPaymentMethodsAsync(CancellationToken cancellationToken = default)
    {
        var records = await EnsureStandardPaymentMethodsAsync(RequireCompany(), cancellationToken);
        return records
            .OrderBy(x => PaymentMethodSort(x.Code))
            .ThenBy(x => x.Name)
            .Select(x => new SaleLookupDto { Id = x.Id, Name = x.Name })
            .ToArray();
    }

    public async Task<IReadOnlyList<SaleLookupDto>> GetRegisterSessionsAsync(CancellationToken cancellationToken = default)
    {
        var records = await repository.GetRegisterSessionsAsync(RequireCompany(), await RequireStoreAsync(cancellationToken), cancellationToken);
        return records.Select(x => new SaleLookupDto { Id = x.Id, Name = $"{x.Register?.Name ?? "Register"} (opened {x.OpenedAt:yyyy-MM-dd HH:mm})" }).ToArray();
    }

    private async Task<decimal> GetAvailableAsync(Guid companyId, Guid storeId, Guid productId, Guid? warehouseId, CancellationToken cancellationToken)
    {
        var stock = await repository.GetInventoryStockAsync(companyId, storeId, productId, warehouseId, null, cancellationToken);
        return stock?.AvailableQuantity ?? 0;
    }

    private async Task DeductStockAsync(
        Guid companyId, Guid storeId, Sale sale, SaleItem saleItem, SaleItemFormDto line, SaleProductLookupData product,
        CancellationToken cancellationToken)
    {
        var stock = await repository.GetInventoryStockAsync(companyId, storeId, line.ProductId, sale.WarehouseId, null, cancellationToken);
        if (stock is null)
        {
            if (product.AllowNegativeStock)
            {
                stock = new InventoryStock
                {
                    Id = Guid.NewGuid(), CompanyId = companyId, StoreId = storeId,
                    ProductId = line.ProductId, WarehouseId = sale.WarehouseId, ProductBatchId = null,
                    QuantityOnHand = 0, ReservedQuantity = 0
                };
                await repository.AddInventoryStockAsync(stock, cancellationToken);
            }
            else
            {
                throw new InvalidOperationException($"No stock record found for '{product.Name}'.");
            }
        }

        stock.QuantityOnHand -= saleItem.BaseQuantity;

        var costPrice = await GetCostPriceAsync(companyId, storeId, line.ProductId, sale.WarehouseId, cancellationToken);
        saleItem.CostPrice = costPrice;

        await repository.AddStockTransactionAsync(new StockTransaction
        {
            Id = Guid.NewGuid(), CompanyId = companyId, StoreId = storeId,
            ProductId = line.ProductId, WarehouseId = sale.WarehouseId, ProductBatchId = null,
            TransactionType = StockTransactionType.Sale,
            ReferenceType = DocumentType.SaleInvoice,
            ReferenceId = sale.Id, ReferenceNumber = sale.InvoiceNumber,
            QuantityIn = 0, QuantityOut = saleItem.BaseQuantity,
            BalanceAfter = stock.QuantityOnHand,
            UnitCost = costPrice,
            TransactionAt = sale.SaleDate,
            Notes = "Stock deducted from confirmed sale."
        }, cancellationToken);
    }

    private async Task<decimal> GetCostPriceAsync(Guid companyId, Guid storeId, Guid productId, Guid? warehouseId, CancellationToken cancellationToken)
    {
        return await repository.GetCostPriceAsync(companyId, storeId, productId, warehouseId, cancellationToken);
    }

    private async Task ApplyPaymentAsync(Sale sale, SaleFormDto request, CancellationToken cancellationToken)
    {
        var paid = request.PaidAmount;
        if (paid <= 0)
        {
            sale.PaymentStatus = PaymentStatus.Unpaid;
            sale.PaidAmount = 0; sale.ChangeAmount = 0; sale.DueAmount = sale.GrandTotal;
            return;
        }

        if (request.PaymentMethodId is null || request.PaymentMethodId == Guid.Empty)
            throw new InvalidOperationException("Select a payment method when recording a payment.");

        var paymentMethodId = await ResolvePaymentMethodIdAsync(sale.CompanyId, request, cancellationToken);
        var amount = Math.Min(paid, sale.GrandTotal);
        sale.PaidAmount = amount;
        sale.ChangeAmount = paid > sale.GrandTotal ? paid - sale.GrandTotal : 0;
        sale.DueAmount = sale.GrandTotal - amount;
        sale.PaymentStatus = sale.DueAmount <= 0 ? PaymentStatus.Paid : PaymentStatus.PartiallyPaid;

        sale.Payments.Add(new SalePayment
        {
            Id = Guid.NewGuid(), CompanyId = sale.CompanyId, StoreId = sale.StoreId,
            SaleId = sale.Id, PaymentMethodId = paymentMethodId,
            Amount = amount, PaidAt = sale.SaleDate,
            ReferenceNumber = string.IsNullOrWhiteSpace(request.PaymentReference) ? null : request.PaymentReference.Trim()
        });
    }

    private async Task<Guid> ResolvePaymentMethodIdAsync(Guid companyId, SaleFormDto request, CancellationToken cancellationToken)
    {
        var records = await EnsureStandardPaymentMethodsAsync(companyId, cancellationToken);
        var selected = records.First(x => x.Id == request.PaymentMethodId!.Value);
        if (!IsOtherPaymentMethod(selected) || string.IsNullOrWhiteSpace(request.OtherPaymentMethodName))
        {
            return selected.Id;
        }

        var otherName = request.OtherPaymentMethodName.Trim();
        var existing = records.FirstOrDefault(x => string.Equals(x.Name, otherName, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            return existing.Id;
        }

        var custom = new PaymentMethod
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = otherName,
            Code = $"OTHER-{Guid.NewGuid():N}"[..38],
            MethodType = PaymentMethodType.Other,
            RequiresReference = false,
            IsActive = true
        };
        await repository.AddPaymentMethodAsync(custom, cancellationToken);
        return custom.Id;
    }

    private async Task<IReadOnlyList<PaymentMethod>> EnsureStandardPaymentMethodsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var records = (await repository.GetPaymentMethodsAsync(companyId, cancellationToken)).ToList();
        var existingCodes = records.Select(x => x.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingNames = records.Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var added = false;

        foreach (var method in StandardPaymentMethods)
        {
            if (existingCodes.Contains(method.Code) || existingNames.Contains(method.Name)) continue;
            var paymentMethod = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                Code = method.Code,
                Name = method.Name,
                MethodType = method.Type,
                RequiresReference = method.RequiresReference,
                IsActive = true
            };
            await repository.AddPaymentMethodAsync(paymentMethod, cancellationToken);
            records.Add(paymentMethod);
            existingCodes.Add(method.Code);
            existingNames.Add(method.Name);
            added = true;
        }

        if (added)
        {
            await repository.SaveChangesAsync(cancellationToken);
            records = (await repository.GetPaymentMethodsAsync(companyId, cancellationToken)).ToList();
        }

        return records;
    }

    private static bool IsOtherPaymentMethod(PaymentMethod paymentMethod) =>
        paymentMethod.MethodType == PaymentMethodType.Other ||
        string.Equals(paymentMethod.Code, "OTHER", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(paymentMethod.Name, "Other", StringComparison.OrdinalIgnoreCase);

    private static int PaymentMethodSort(string code)
    {
        var index = Array.FindIndex(StandardPaymentMethods, x => string.Equals(x.Code, code, StringComparison.OrdinalIgnoreCase));
        return index < 0 ? StandardPaymentMethods.Length : index;
    }

    private async Task ValidateAsync(SaleFormDto request, Guid companyId, Guid storeId, CancellationToken cancellationToken)
    {
        if (request.CustomerId.HasValue && !(await repository.GetCustomersAsync(companyId, cancellationToken)).Any(x => x.Id == request.CustomerId.Value))
            throw new InvalidOperationException("Select a valid customer.");
        if (string.IsNullOrWhiteSpace(request.InvoiceNumber)) throw new InvalidOperationException("Invoice number is required.");
        if (await repository.InvoiceNumberExistsAsync(companyId, storeId, request.InvoiceNumber.Trim(), null, cancellationToken)) throw new InvalidOperationException("Invoice number already exists in this store.");
        if (request.SaleDate == default) throw new InvalidOperationException("Sale date is required.");
        if (request.WarehouseId.HasValue && !(await repository.GetWarehousesAsync(companyId, storeId, cancellationToken)).Any(x => x.Id == request.WarehouseId.Value)) throw new InvalidOperationException("Warehouse was not found in this store.");
        if (request.RegisterSessionId.HasValue && request.RegisterSessionId != Guid.Empty && !(await repository.GetRegisterSessionsAsync(companyId, storeId, cancellationToken)).Any(x => x.Id == request.RegisterSessionId.Value)) throw new InvalidOperationException("The selected register session is not open.");
        if (request.PaidAmount < 0) throw new InvalidOperationException("Amount paid cannot be negative.");
        if (request.PaidAmount > 0 && request.PaymentMethodId is null) throw new InvalidOperationException("Select a payment method when recording a payment.");
        var paymentMethods = await EnsureStandardPaymentMethodsAsync(companyId, cancellationToken);
        if (request.PaymentMethodId.HasValue && !paymentMethods.Any(x => x.Id == request.PaymentMethodId.Value)) throw new InvalidOperationException("Payment method was not found.");
        if (request.PaidAmount > 0 && request.PaymentMethodId.HasValue && IsOtherPaymentMethod(paymentMethods.First(x => x.Id == request.PaymentMethodId.Value)) && string.IsNullOrWhiteSpace(request.OtherPaymentMethodName))
            throw new InvalidOperationException("Enter the other payment method name.");
        if (request.Items.Count == 0) throw new InvalidOperationException("Add at least one product line.");

        var products = await repository.GetProductsAsync(companyId, storeId, cancellationToken);
        foreach (var item in request.Items)
        {
            if (item.ProductId == Guid.Empty || item.ProductUnitId == Guid.Empty || item.Quantity <= 0) throw new InvalidOperationException("Each line needs a product, unit, and quantity greater than zero.");
            if (item.UnitPrice < 0 || item.DiscountAmount < 0 || item.TaxAmount < 0) throw new InvalidOperationException("Line amounts cannot be negative.");
            var product = products.FirstOrDefault(x => x.Id == item.ProductId) ?? throw new InvalidOperationException("One or more selected products are invalid.");
            var units = await repository.GetProductUnitsAsync(item.ProductId, companyId, cancellationToken);
            var unit = units.FirstOrDefault(x => x.Id == item.ProductUnitId) ?? throw new InvalidOperationException("One or more selected product units are invalid.");
            if (unit.ConversionFactor <= 0) throw new InvalidOperationException("Product unit conversion factor must be greater than zero.");
            item.ConversionFactor = unit.ConversionFactor;
        }
    }

    private static SaleListDto MapList(Sale x) => new()
    {
        Id = x.Id, InvoiceNumber = x.InvoiceNumber, CustomerName = x.Customer?.Name,
        SaleDate = x.SaleDate, Status = x.Status, PaymentStatus = x.PaymentStatus,
        GrandTotal = x.GrandTotal, PaidAmount = x.PaidAmount, ItemCount = x.Items.Count
    };

    private static SaleFormDto MapForm(Sale x) => new()
    {
        Id = x.Id, CustomerId = x.CustomerId, CustomerName = x.Customer?.Name,
        WarehouseId = x.WarehouseId, RegisterSessionId = x.RegisterSessionId,
        InvoiceNumber = x.InvoiceNumber, SaleDate = x.SaleDate,
        Status = x.Status, PaymentStatus = x.PaymentStatus, Subtotal = x.Subtotal, ItemDiscount = x.ItemDiscount,
        TaxAmount = x.TaxAmount, RoundOffAmount = x.RoundOffAmount, GrandTotal = x.GrandTotal,
        PaidAmount = x.PaidAmount, ChangeAmount = x.ChangeAmount, DueAmount = x.DueAmount,
        Notes = x.Notes,
        Items = x.Items.Select(i => new SaleItemFormDto
        {
            Id = i.Id, ProductId = i.ProductId, ProductName = i.Product.Name,
            ProductUnitId = i.ProductUnitId, ProductUnitName = i.Product.Units.FirstOrDefault(u => u.Id == i.ProductUnitId)?.Unit.Name ?? string.Empty,
            ConversionFactor = i.ConversionFactor, Quantity = i.Quantity, UnitPrice = i.UnitPrice,
            DiscountAmount = i.DiscountAmount, TaxAmount = i.TaxAmount, NetAmount = i.NetAmount
        }).ToList(),
        Payments = x.Payments.Select(p => new SalePaymentFormDto
        {
            Id = p.Id, PaymentMethodId = p.PaymentMethodId, PaymentMethodName = p.PaymentMethod.Name,
            Amount = p.Amount, ReferenceNumber = p.ReferenceNumber
        }).ToList()
    };

    private Guid RequireCompany() => companyContext.CompanyId ?? throw new UnauthorizedAccessException("Company context is missing.");
    private async Task<Guid> RequireStoreAsync(CancellationToken cancellationToken) { var id = storeContext.RequireSelectedStoreId(); await storeContext.EnsureStoreAccessAsync(id, cancellationToken); return id; }
}
