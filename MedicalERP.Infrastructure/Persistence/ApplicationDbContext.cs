using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Identity;
using MedicalERP.Domain.Inventory;
using MedicalERP.Domain.Operations;
using MedicalERP.Domain.Purchases;
using MedicalERP.Domain.Sales;
using MedicalERP.Domain.Support;
using MedicalERP.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Persistence;

public sealed partial class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly ICompanyContext? _companyContext;
    private readonly ICurrentUserService? _currentUser;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICompanyContext? companyContext = null, ICurrentUserService? currentUser = null) : base(options)
    {
        _companyContext = companyContext;
        _currentUser = currentUser;
    }

    public Guid? CurrentCompanyId => _companyContext?.CompanyId;
    public bool IsPlatformAdmin => _currentUser?.IsPlatformAdmin == true;

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<UserStoreAccess> UserStoreAccesses => Set<UserStoreAccess>();
    public DbSet<UserCompanyAccess> UserCompanyAccesses => Set<UserCompanyAccess>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


    // Catalog
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductBrand> ProductBrands => Set<ProductBrand>();
    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<GenericMedicine> GenericMedicines => Set<GenericMedicine>();
    public DbSet<DosageForm> DosageForms => Set<DosageForm>();
    public DbSet<Strength> Strengths => Set<Strength>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<ProductUnit> ProductUnits => Set<ProductUnit>();
    public DbSet<ProductBarcode> ProductBarcodes => Set<ProductBarcode>();
    public DbSet<StoreProduct> StoreProducts => Set<StoreProduct>();


    // Inventory
    public DbSet<ProductBatch> ProductBatches => Set<ProductBatch>();
    public DbSet<InventoryStock> InventoryStocks => Set<InventoryStock>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<StockAdjustmentItem> StockAdjustmentItems =>
        Set<StockAdjustmentItem>();

    // Purchasing
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierStore> SupplierStores => Set<SupplierStore>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems =>
        Set<PurchaseOrderItem>();
    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
    public DbSet<GoodsReceiptItem> GoodsReceiptItems =>
        Set<GoodsReceiptItem>();
    public DbSet<PurchaseInvoice> PurchaseInvoices =>
        Set<PurchaseInvoice>();
    public DbSet<SupplierPayment> SupplierPayments =>
        Set<SupplierPayment>();
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<PurchaseReturnItem> PurchaseReturnItems =>
        Set<PurchaseReturnItem>();
    // Sales
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<SaleOrder> SaleOrders => Set<SaleOrder>();
    public DbSet<SaleOrderItem> SaleOrderItems => Set<SaleOrderItem>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<SaleItemBatch> SaleItemBatches => Set<SaleItemBatch>();
    public DbSet<SalePayment> SalePayments => Set<SalePayment>();
    public DbSet<SaleReturn> SaleReturns => Set<SaleReturn>();
    public DbSet<SaleReturnItem> SaleReturnItems =>
        Set<SaleReturnItem>();

    // Registers
    public DbSet<Register> Registers => Set<Register>();
    public DbSet<RegisterSession> RegisterSessions =>
        Set<RegisterSession>();
    public DbSet<CashMovement> CashMovements => Set<CashMovement>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();








    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        foreach (var fk in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) fk.DeleteBehavior = DeleteBehavior.Restrict;
        builder.Entity<Store>().HasQueryFilter(x => CurrentCompanyId == null || x.CompanyId == CurrentCompanyId);
        builder.Entity<Warehouse>().HasQueryFilter(x => CurrentCompanyId == null || x.CompanyId == CurrentCompanyId);
        builder.Entity<UserStoreAccess>().HasQueryFilter(x => CurrentCompanyId == null || x.CompanyId == CurrentCompanyId);
        builder.Entity<UserCompanyAccess>().HasQueryFilter(x => CurrentCompanyId == null || x.CompanyId == CurrentCompanyId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedByUserId = _currentUser?.UserId;
            }
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedByUserId = _currentUser?.UserId;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
