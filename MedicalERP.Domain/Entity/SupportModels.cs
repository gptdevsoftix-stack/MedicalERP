using MedicalERP.Domain.Common;
using MedicalERP.Domain.Enums;

namespace MedicalERP.Domain.Support;

public sealed class PaymentMethod : CompanyEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public PaymentMethodType MethodType { get; set; }
    public bool RequiresReference { get; set; }
}

public sealed class TaxRate : CompanyEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public bool IsInclusive { get; set; }
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
}

public sealed class ReasonCode : CompanyEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DocumentType AppliesTo { get; set; }
}

public sealed class NumberSequence : StoreEntity
{
    public DocumentType DocumentType { get; set; }
    public string Prefix { get; set; } = string.Empty;
    public long NextNumber { get; set; } = 1;
    public int Padding { get; set; } = 6;
    public int? ResetYear { get; set; }
}

public sealed class ExpenseCategory : CompanyEntity { public string Name { get; set; } = string.Empty; public string Code { get; set; } = string.Empty; }

public sealed class Expense : StoreEntity
{
    public Guid ExpenseCategoryId { get; set; }
    public Guid? RegisterSessionId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public string ExpenseNumber { get; set; } = string.Empty;
    public DateTimeOffset ExpenseDate { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
}

public sealed class SupplierLedgerEntry : StoreEntity
{
    public Guid SupplierId { get; set; }
    public LedgerEntryType EntryType { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTimeOffset EntryDate { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal BalanceAfter { get; set; }
}

public sealed class CustomerLedgerEntry : StoreEntity
{
    public Guid CustomerId { get; set; }
    public LedgerEntryType EntryType { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTimeOffset EntryDate { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal BalanceAfter { get; set; }
}

public sealed class StoreSetting : StoreEntity
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
}

public sealed class AuditLog : StoreEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? IpAddress { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
}
