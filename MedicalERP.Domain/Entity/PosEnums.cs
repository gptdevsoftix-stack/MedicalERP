namespace MedicalERP.Domain.Enums;

public enum ProductType { Medicine = 1, General = 2, Service = 3 }
public enum OrderStatus { Draft = 1, Pending = 2, Approved = 3, PartiallyFulfilled = 4, Fulfilled = 5, Cancelled = 6, Closed = 7 }
public enum PurchaseInvoiceStatus { Draft = 1, Posted = 2, PartiallyPaid = 3, Paid = 4, Cancelled = 5 }
public enum SaleStatus { Draft = 1, Held = 2, Confirmed = 3, PartiallyReturned = 4, Returned = 5, Cancelled = 6 }
public enum PaymentStatus { Unpaid = 1, PartiallyPaid = 2, Paid = 3, Refunded = 4, PartiallyRefunded = 5 }
public enum PaymentMethodType { Cash = 1, Card = 2, BankTransfer = 3, MobileWallet = 4, Credit = 5, Other = 6 }
public enum StockTransactionType { OpeningStock = 1, PurchaseReceipt = 2, PurchaseReturn = 3, Sale = 4, SaleReturn = 5, PositiveAdjustment = 6, NegativeAdjustment = 7, StockCount = 8, Disposal = 9, Reservation = 10, ReservationRelease = 11, Cancellation = 12 }
public enum AdjustmentType { Increase = 1, Decrease = 2 }
public enum StockCountStatus { Draft = 1, InProgress = 2, Completed = 3, Posted = 4, Cancelled = 5 }
public enum RegisterSessionStatus { Open = 1, Closed = 2, Reconciled = 3 }
public enum CashMovementType { CashIn = 1, CashOut = 2, Expense = 3, Refund = 4 }
public enum ReturnStatus { Draft = 1, Approved = 2, Posted = 3, Cancelled = 4 }
public enum PrescriptionStatus { Draft = 1, Verified = 2, PartiallyDispensed = 3, Dispensed = 4, Rejected = 5 }
public enum LedgerEntryType { Invoice = 1, Payment = 2, Return = 3, CreditNote = 4, DebitNote = 5, OpeningBalance = 6, Adjustment = 7 }
public enum DiscountType { Fixed = 1, Percentage = 2 }
public enum DocumentType { PurchaseOrder = 1, GoodsReceipt = 2, PurchaseInvoice = 3, PurchaseReturn = 4, SaleOrder = 5, SaleInvoice = 6, SaleReturn = 7, StockAdjustment = 8, StockCount = 9, StockDisposal = 10 }
