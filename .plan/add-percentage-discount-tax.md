# Plan: Add Percentage-Based Discount & Tax (No Model Changes)

## Context
Currently, the POS sale flow has per-line fixed-amount discount and tax inputs. The user wants **invoice-level percentage-based** discount and tax instead. **No entity/DTO/model changes allowed** — only UI and JavaScript logic changes. The computed amounts will be calculated client-side and submitted through the existing form fields.

## Scope
- **Entity**: `Sale` (POS invoice) only
- **Level**: Invoice-level only (single discount % and tax % for the whole order)
- **Type**: Percentage-based (replace per-line fixed amounts)
- **Constraint**: No model/entity/DTO changes — frontend computes amounts and sends them via existing fields

## How It Works
1. Cashier enters **Discount %** and **Tax %** in the cart footer
2. JavaScript computes:
   - `discountAmount = subtotal × (discountPct / 100)`
   - `taxAmount = (subtotal - discountAmount) × (taxPct / 100)`
   - `grandTotal = subtotal - discountAmount + taxAmount`
3. The computed amounts are distributed evenly across line items into the existing `DiscountAmount` and `TaxAmount` hidden fields before form submission
4. Server-side `SaleService.CreateAsync` remains unchanged — it already reads per-line amounts

---

## Files to Modify (4 files)

### 1. `MedicalERP.Web/Views/Sales/_Form.cshtml`
- Replace per-line "Discount" and "Tax" inputs in the cart line items with invoice-level percentage inputs in the cart footer
- Add two new inputs in the footer (before the totals section):
  - **Discount %**: `<input id="sale-discount-pct" type="number" min="0" max="100" step="0.01" value="0">`
  - **Tax %**: `<input id="sale-tax-pct" type="number" min="0" max="100" step="0.01" value="0">`
- Add hidden inputs to submit computed totals: `InvoiceDiscount` and `TaxAmount` at the invoice level
- Remove the per-line "Discount / Tax" toggle button and `.line-details` panel from the cart row template

### 2. `MedicalERP.Web/Views/Sales/_SaleScripts.cshtml`
- Update `recalc()` function:
  - Compute `subtotal` from `qty × price` for each line
  - Read discount % and tax % from the new footer inputs
  - Compute `discount = subtotal × (pct / 100)`, `tax = (subtotal - discount) × (taxPct / 100)`, `grandTotal = subtotal - discount + tax`
  - Distribute `discountAmount` and `taxAmount` evenly across line items by setting the hidden field values
  - Update all display elements (subtotal, discount, tax, total, due, change)
- Add event listeners on the % inputs to trigger `recalc()`
- Remove per-line discount/tax references from the `add()` function (no more line-details panel)
- Remove the `line-details-toggle` click handler

### 3. `MedicalERP.Web/Views/Sales/Details.cshtml`
- No functional changes needed — it already displays `ItemDiscount`, `TaxAmount`, and `GrandTotal` from the saved model
- Optionally: add "%" labels next to discount/tax amounts if the stored percentages are available (but since we're not storing %, just show the amounts as-is)

### 4. `MedicalERP.Web/Views/Sales/Receipt.cshtml`
- No changes needed — already shows discount and tax amounts from the model

---

## What Stays the Same
- **`SalesModels.cs`** — No changes to `Sale` or `SaleItem` entities
- **`SaleDtos.cs`** — No changes to DTOs
- **`SaleService.cs`** — No changes to the service (it already computes from line amounts)
- **`SaleReceiptPdf.cs`** — No changes (already shows discount/tax amounts)
- **Database** — No migration needed

## Verification
1. `dotnet build MedicalERP.sln` — compiles with no errors
2. Navigate to Sales → Create
3. Add products to cart — no per-line discount/tax inputs
4. Enter Discount % (e.g., 10%) and Tax % (e.g., 5%) in footer
5. Verify totals recalculate in real-time
6. Confirm sale — verify stored `ItemDiscount`, `TaxAmount`, `GrandTotal` are correct
7. Check Details and Receipt views display correct amounts
