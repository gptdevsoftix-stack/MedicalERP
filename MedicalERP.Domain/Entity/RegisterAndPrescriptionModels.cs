using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.Enums;

namespace MedicalERP.Domain.Operations;

public sealed class Register : StoreEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid? WarehouseId { get; set; }
    public bool IsEnabled { get; set; } = true;
}

public sealed class RegisterSession : StoreEntity
{
    public Guid RegisterId { get; set; }
    public string CashierUserId { get; set; } = string.Empty;
    public DateTimeOffset OpenedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public decimal OpeningCash { get; set; }
    public decimal ExpectedCash { get; set; }
    public decimal CountedCash { get; set; }
    public decimal Difference { get; set; }
    public RegisterSessionStatus Status { get; set; }
    public string? ClosingNotes { get; set; }
    public Register Register { get; set; } = null!;
    public ICollection<CashMovement> CashMovements { get; set; } = [];
}

public sealed class CashMovement : StoreEntity
{
    public Guid RegisterSessionId { get; set; }
    public CashMovementType MovementType { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset MovementAt { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
}

public sealed class Doctor : CompanyEntity
{
    public string Name { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
    public string? Specialty { get; set; }
    public string? Phone { get; set; }
}

public sealed class Patient : CompanyEntity
{
    public Guid? CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Allergies { get; set; }
    public string? MedicalNotes { get; set; }
}

public sealed class Prescription : StoreEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public string PrescriptionNumber { get; set; } = string.Empty;
    public DateOnly PrescriptionDate { get; set; }
    public DateOnly? ValidUntil { get; set; }
    public PrescriptionStatus Status { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? VerifiedByUserId { get; set; }
    public DateTimeOffset? VerifiedAt { get; set; }
    public ICollection<PrescriptionItem> Items { get; set; } = [];
}

public sealed class PrescriptionItem : StoreEntity
{
    public Guid PrescriptionId { get; set; }
    public Guid ProductId { get; set; }
    public decimal PrescribedQuantity { get; set; }
    public decimal DispensedQuantity { get; set; }
    public string? DosageInstructions { get; set; }
    public Prescription Prescription { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
