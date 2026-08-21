# MedicalERP User Guide

Version reviewed: 21 August 2026  
Audience: administrators, store managers, purchasing staff, inventory staff, pharmacists, cashiers, accountants, and auditors

## 1. What the system does

MedicalERP is a multi-company, multi-store medical inventory and point-of-sale system. The current web application supports:

- companies, stores, warehouses, users, roles, and permissions;
- categories and medical/general product setup;
- product units, barcodes, store-specific prices, batches, and stock visibility;
- suppliers and purchase orders;
- customers, sales, receipts, payments, and sale returns; and
- sales, revenue, profit, payment, product, and customer reporting.

The menus and buttons shown to each user depend on assigned permissions. A missing menu or an **Access denied** page normally means the user's role does not include the required permission.

## 2. Important operating rules

> **Stock-impact warning:** Approving a purchase order adds its ordered quantity plus free quantity to stock. Creating a sale deducts stock. Posting a sale return restores stock for each line whose **Return to stock** option is selected.

- Always check the selected store in the top bar before entering store-level data or transactions.
- Platform administrators should also check the company context used on company-specific pages.
- Codes, order numbers, invoice numbers, return numbers, and barcodes should be unique in their applicable company/store scope.
- Most delete actions deactivate a record rather than permanently removing it. Historical transactions remain available.
- Only draft purchase orders can be edited.
- A cancelled, draft, or held sale cannot be returned.
- Do not approve a purchase order merely to record authorization: in the current implementation, approval also receives all ordered and free quantities into stock.

## 3. Signing in and selecting a store

1. Open the MedicalERP web address supplied by your administrator.
2. Enter your email address and password.
3. Optionally select **Remember me**, then choose **Sign in**.
4. After login, use **Select Store** in the top navigation bar to choose the store where you are working.
5. Confirm the company name shown at the top-left and the selected store before entering a purchase, sale, return, batch, or stock record.
6. Use the user menu to sign out when finished, especially on a shared terminal.

If no store is available, ask an administrator to assign the user to a store under **Users**.

## 4. Recommended first-time setup

Complete setup in this order so later screens have the required choices:

1. **Companies** — create the legal/business entity.
2. **Stores** — add branches and select the head office where applicable.
3. **Warehouses** — add at least one warehouse and mark the normal receiving warehouse as default.
4. **Roles** — define access by job function.
5. **Users** — create staff accounts and assign roles and stores.
6. **Categories and catalog masters** — prepare categories, brands, manufacturers, generic medicines, dosage forms, strengths, and units.
7. **Products** — create the saleable products and their stock rules.
8. **Product Units** — add packs, boxes, strips, or other conversions when products are bought or sold in multiple units.
9. **Product Barcodes** — attach scannable codes to the appropriate product/unit.
10. **Store Products** — set store-level price, availability, and reorder values.
11. **Suppliers and Customers** — create business contacts.
12. **Product Batches** — enter batch/expiry and pricing data where batches are tracked.

## 5. Organization and security administration

### Companies

Use **Companies** to manage tenant businesses. Record the company name, code, legal and contact information, address, currency, time zone, and subscription details. Use Edit to correct information or change active status. Company codes should be planned before transactions begin.

### Stores

Use **Stores** to create branches belonging to a company. Enter name, code, contact/address details, tax number, currency, and time zone. Select **Is Head Office** only for the appropriate branch. Platform administrators must select the correct **Company Context**.

### Warehouses

Use **Warehouses** to define stock locations for a store. Select the store and record the warehouse name, code, type, address, and whether it is the default. Deactivating a warehouse prevents normal future use but does not erase its history.

### Roles and permissions

Use **Roles** to create or edit a job role. On the permission grid, select only the actions that role needs. Permissions are grouped by area, such as Companies, Stores, Products, Inventory, Purchases, Sales, Reports, Users, and Roles.

The system seeds these standard roles: PlatformSuperAdmin, Admin, CompanyOwner, CompanyAdmin, RegionalManager, StoreManager, Pharmacist, Cashier, InventoryManager, PurchaseManager, Accountant, and Auditor. Their permissions can be reviewed in the role screens. Administrators and company owners have broad access; specialist roles are more restricted.

### Users

To add an employee:

1. Open **Users** and choose **Create**.
2. For a platform administrator, choose the company context.
3. Enter email, initial password, first name, and last name.
4. Select one or more roles.
5. Select every store the employee may use.
6. Save the user.

Edit a user to change their name, roles, stores, or active status. Email is read-only after creation. Deleting from the list deactivates the account.

## 6. Product catalog

### Categories and catalog masters

Use **Catalog > Categories** for the product hierarchy. Use the relevant catalog-master screens or the **+** buttons in the product form to maintain brands, manufacturers, generic medicines, dosage forms, strengths, and units.

When creating a master record, complete the fields applicable to its type. These can include name, code, licence number, description, strength value and measurement unit, or unit symbol and decimal support.

### Products

1. Open **Catalog > Products** and choose **Create**.
2. Enter product name, unique code, type, description, category, and base unit.
3. For medicines, select brand/manufacturer, generic medicine, dosage form, strength, and regulatory number as applicable.
4. Set the operational flags carefully:
   - **Requires Prescription** identifies prescription medicines.
   - **Is Controlled Drug** identifies controlled products.
   - **Track Batch** enables batch-level handling.
   - **Track Expiry** identifies expiry-sensitive products.
   - **Allow Discount** permits discounting.
   - **Allow Negative Stock** determines whether a sale may exceed available stock.
5. Save the product.

The **+** buttons beside lookup fields allow quick creation without leaving the product form.

### Product units

Create alternative buying/selling units for a product, such as tablet, strip, pack, or box. Set the conversion factor relative to the base unit and the applicable purchase/sale settings. Correct conversions are essential because transaction quantities are converted to base stock quantities.

### Product barcodes

Use **Catalog > Product Barcodes** to associate a barcode with a product and, where applicable, a product unit. Search the list by barcode or product. Deactivation preserves previous use while preventing the code from being treated as active.

### Store products and pricing

Use **Catalog > Store Products** to configure a product for a particular store. Set sale price, wholesale price, minimum sale price, reorder level, reorder quantity, and **Is Available For Sale**. Use this screen for branch-specific pricing and replenishment thresholds.

## 7. Inventory

### Product batches

Use **Inventory > Product Batches** for products that need lot and expiry tracking. Select product and warehouse, then enter batch number, manufacturing date, expiry date, purchase price, cost price, sale price, maximum retail price, and received date/time.

The list can be filtered by product, warehouse, active status, expiry cutoff, or search text. Before saving, verify that manufacturing date is before expiry date and that the warehouse matches the physical stock location.

### Inventory stock

Use **Inventory Stock** to see current quantities by product/location. Treat this as the current stock balance view. Filters such as category help narrow the list.

### Stock transactions

Use **Inventory > Stock Transactions** as the movement ledger. Purchases, sales, returns, and supported adjustments create movement records with a reference number and quantity change. Use this screen to investigate why a stock balance changed; avoid editing historical movement data unless your organization has an approved correction procedure.

Several domain capabilities exist for counts, adjustments, disposals, and reason codes, but their menu entries are currently hidden. They should not be treated as available operator workflows unless enabled and tested by the system administrator.

## 8. Suppliers and purchasing

### Suppliers

Use the Suppliers page available in the purchasing workflow to maintain supplier name, code, contact person, email, phone, tax number, address, credit days, and credit limit. A supplier can also be created from the **+** button on a purchase order.

### Create and approve a purchase order

1. Open **Purchase Orders** and choose **New Purchase Order**.
2. Select a supplier. Optionally quick-create one with **+**.
3. Enter a unique order number and order date.
4. Select the receiving warehouse. Optionally enter expected delivery, other charges, and notes.
5. Choose **Add line** and select a product and unit.
6. Enter ordered quantity, free quantity, unit price, discount, and tax for each line.
7. Check subtotal, discount, tax, other charges, and grand total.
8. Choose **Save Draft**.
9. Open the order details and choose **Submit for approval**. Its status changes from Draft to Pending.
10. An authorized approver checks the order and chooses **Approve**.

Purchase order flow: `Draft → Pending → Approved`

A draft can be edited. An eligible order can be cancelled, but fulfilled, closed, or already-cancelled orders cannot be cancelled.

> In this version, **Approve** also adds the full ordered quantity plus free quantity to inventory, marks those quantities as received, and posts a Purchase Receipt stock transaction. Verify supplier, warehouse, unit conversions, quantities, free quantities, prices, and tax before approval.

## 9. Customers and sales

### Customers

Use Customers to record name, code, phone, email, tax number, address, credit days, and credit limit. Customers are company-specific. A customer may be created quickly while making a sale. Leave the customer empty on a sale for a walk-in customer.

### Create a sale

1. Confirm the selected store.
2. Open **Sales** and choose **New Sale**.
3. Select a customer or leave **Walk-in customer**.
4. Check the invoice number and sale date.
5. Select the warehouse. A register session may be selected; if left empty, the system can auto-open one when saving.
6. Search the product catalog and add the required products.
7. For every line, verify product unit, quantity, price, discount, and tax.
8. Review the calculated subtotal and grand total.
9. Select a payment method or choose **No payment now**. Enter amount paid and a reference where appropriate. If using Other, enter the other payment-method name.
10. Save the sale.

The saved sale is confirmed, stock is deducted, and Sale stock transactions are recorded. Payment state is based on the amount received: Unpaid, Partially Paid, or Paid. If the payment method requires payment, the amount cannot exceed the sale total.

From sale details you can:

- choose **Mark as Paid** for an unpaid/part-paid eligible sale;
- choose **Return** if the sale is confirmed or partially returned and you have return permission;
- choose **Receipt** to view the receipt; and
- download/print the PDF receipt from the receipt screen.

### Process a sale return

1. Open the original sale and choose **Return**.
2. Check the generated return number and return date.
3. Select the warehouse and enter an optional reason.
4. Enter a return quantity only for items being returned. It cannot exceed the remaining returnable quantity.
5. Leave **Return to stock** selected for resalable goods. Clear it for damaged, expired, or otherwise non-resalable goods.
6. Review the refund amount and post the return.

The return is posted immediately. Selected stock is restored and a Sale Return stock transaction is created. A partially returned sale becomes **Partially Returned / Partially Refunded**; when all quantities are returned it becomes **Returned / Refunded**.

## 10. Reports

Open **Reports** to use the Sales Report. Filter by From date, To date, invoice/customer search, sale status, and payment status. Shortcut buttons provide the last 7 or 30 days.

The report includes sale count and item quantity; revenue and average sale value; net profit, margin, and expenses; collected and outstanding amounts; daily revenue and payment-method charts; top products and customers; and detailed sales with total and due amount.

Choose **Print** for a printable copy. Profit and cost visibility should be restricted through report permissions according to company policy.

## 11. Daily operating checklist

### Start of day

- Sign in with your own account.
- Confirm company and store.
- Confirm that the required warehouse, products, prices, and stock are available.
- Report missing access rather than sharing another employee's login.

### During the day

- Check product, unit, batch, quantity, and price before saving.
- Use unique document numbers.
- Add customers when credit tracking or a named receipt is needed.
- Use **Return to stock** only when the physical goods are actually returned to usable inventory.

### End of day

- Review the Sales Report for sales, collected amount, and due amount.
- Investigate unexpected balances through Stock Transactions.
- Print or retain reports according to company procedure.
- Sign out.

## 12. Troubleshooting

| Problem | Likely cause | Action |
|---|---|---|
| A menu is missing | Role lacks the View permission | Ask an administrator to review the user's role permissions. |
| Access denied | The specific action permission is missing | Request only the permission required for the job. |
| No store is available | User is not assigned to a store | Edit the user and assign the correct store. |
| A lookup is empty | Required master data is inactive, missing, or belongs to another company/store | Check context and create/reactivate the master record. |
| Duplicate code/document error | The value already exists in its scope | Search the list and use a new unique value. |
| Purchase order cannot be edited | It is no longer Draft | Review its status; do not recreate it without checking stock impact. |
| Sale cannot be returned | It is Draft, Held, Cancelled, or fully returned | Open an eligible confirmed/partially returned sale. |
| Return quantity is rejected | It exceeds the remaining returnable quantity | Check earlier returns and reduce the quantity. |
| Sale fails for insufficient stock | Stock is below the required converted quantity and negative stock is disabled | Correct stock/warehouse/unit, receive stock, or follow an authorized adjustment process. |
| Stock changed unexpectedly | A PO was approved, sale saved, or return posted | Find the document reference in Stock Transactions. |

## 13. Current-scope notes

The codebase contains underlying models or permission names for prescriptions, expenses, registers, audit logs, stock counts, adjustments, disposals, purchase returns, and other advanced operations. They are not all exposed as complete menu-driven workflows in the current user interface. This guide documents the workflows that are visibly implemented and should not be taken as confirmation that every underlying model is production-ready.

Before production rollout, the organization should define its own document-number format, approval authority, warehouse policy, batch/expiry procedure, return/refund policy, permission matrix, backup procedure, and financial reconciliation process.
