using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Inventory;
using MedicalERP.Domain.Operations;
using MedicalERP.Domain.Purchases;
using MedicalERP.Domain.Support;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Persistence;

public partial class ApplicationDbContext
{
    public DbSet<ProductPriceHistory> ProductPriceHistories => Set<ProductPriceHistory>();
    public DbSet<StockCount> StockCounts => Set<StockCount>();
    public DbSet<StockCountItem> StockCountItems => Set<StockCountItem>();
    public DbSet<StockDisposal> StockDisposals => Set<StockDisposal>();
    public DbSet<StockDisposalItem> StockDisposalItems => Set<StockDisposalItem>();
    public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems => Set<PurchaseInvoiceItem>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();
    public DbSet<ReasonCode> ReasonCodes => Set<ReasonCode>();
    public DbSet<NumberSequence> NumberSequences => Set<NumberSequence>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<SupplierLedgerEntry> SupplierLedgerEntries => Set<SupplierLedgerEntry>();
    public DbSet<CustomerLedgerEntry> CustomerLedgerEntries => Set<CustomerLedgerEntry>();
    public DbSet<StoreSetting> StoreSettings => Set<StoreSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
}
