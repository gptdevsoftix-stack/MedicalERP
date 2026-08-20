using MedicalERP.Application.Sales.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedicalERP.Web.Models;

public static class SaleReceiptPdf
{
    static SaleReceiptPdf() => QuestPDF.Settings.License = LicenseType.Community;

    public static byte[] Build(SaleFormDto model, string companyName = "MEDICALERP PHARMACY")
    {
        var discount = model.ItemDiscount + model.InvoiceDiscount;
        var pageHeight = Math.Max(150, 118 + model.Items.Count * 9);
        return Document.Create(document => document.Page(page =>
        {
            page.Size(80, pageHeight, Unit.Millimetre);
            page.Margin(4, Unit.Millimetre);
            page.DefaultTextStyle(x => x.FontFamily("Courier New").FontSize(8).FontColor(Colors.Black));
            page.Content().Column(col =>
            {
                col.Spacing(2);
                col.Item().AlignCenter().Text(companyName.ToUpperInvariant()).FontFamily("Times New Roman").FontSize(15).Bold();
                col.Item().AlignCenter().Text("(Pharmacy)").FontFamily("Times New Roman").FontSize(11).Bold();
                col.Item().AlignCenter().Text("Quality medicines & healthcare services").FontSize(6.5f);
                col.Item().Background(Colors.Black).PaddingVertical(1).AlignCenter().Text("SALE RECEIPT").FontColor(Colors.White).Bold();

                col.Item().Row(row => { row.RelativeItem().Text($"Bill No: {model.InvoiceNumber}").Bold(); row.AutoItem().Text("Original").Bold(); });
                col.Item().Row(row => { row.RelativeItem().Text("Date & Time:").Bold(); row.AutoItem().Text(model.SaleDate.ToString("dd-MMM-yyyy HH:mm")); });
                col.Item().Text($"Customer: {model.CustomerName ?? "Cash Sale Walk-in Customer"}");

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(c => { c.RelativeColumn(2.7f); c.RelativeColumn(.65f); c.RelativeColumn(1); c.RelativeColumn(.8f); c.RelativeColumn(1.1f); });
                    table.Header(h =>
                    {
                        h.Cell().Element(Header).Text("Description"); h.Cell().Element(Header).AlignRight().Text("Qty");
                        h.Cell().Element(Header).AlignRight().Text("Price"); h.Cell().Element(Header).AlignRight().Text("Disc"); h.Cell().Element(Header).AlignRight().Text("Total");
                    });
                    foreach (var item in model.Items)
                    {
                        table.Cell().PaddingVertical(1).Text(item.ProductName ?? "-");
                        table.Cell().PaddingVertical(1).AlignRight().Text(item.Quantity.ToString("0.##"));
                        table.Cell().PaddingVertical(1).AlignRight().Text(item.UnitPrice.ToString("N2"));
                        table.Cell().PaddingVertical(1).AlignRight().Text(item.DiscountAmount.ToString("N2"));
                        table.Cell().PaddingVertical(1).AlignRight().Text(item.NetAmount.ToString("N2"));
                    }
                });
                col.Item().LineHorizontal(.7f);
                col.Item().Row(row => { row.RelativeItem().Text($"{model.Items.Count} item(s)").Bold(); row.AutoItem().Text($"Item Disc:  {model.ItemDiscount:N2}"); });
                col.Item().LineHorizontal(.7f);

                col.Item().AlignRight().Width(150).Column(totals =>
                {
                    TotalRow(totals, "Gross Total:", model.Subtotal);
                    if (model.InvoiceDiscount > 0) TotalRow(totals, "(-) Invoice Disc:", model.InvoiceDiscount);
                    if (model.TaxAmount > 0) TotalRow(totals, "Tax:", model.TaxAmount);
                    TotalRow(totals, "Net Total:", model.GrandTotal, true);
                    TotalRow(totals, "Cash Received:", model.PaidAmount);
                    TotalRow(totals, "Cash Back:", model.ChangeAmount);
                    if (model.DueAmount > 0) TotalRow(totals, "Balance Due:", model.DueAmount, true);
                });

                col.Item().BorderBottom(1.5f).PaddingVertical(2).Row(row =>
                {
                    row.RelativeItem().Text("You Saved").FontFamily("Arial").FontSize(15);
                    row.AutoItem().Text(discount.ToString("N2")).FontFamily("Arial").FontSize(16).Bold();
                });
                col.Item().Row(row => { row.RelativeItem().Text("User: Cashier").FontSize(7); row.AutoItem().Text($"Counter: {model.RegisterSessionName ?? "Pharmacy"}").FontSize(7); });
                col.Item().AlignCenter().PaddingTop(2).Width(125).Height(35).Row(barcode => DrawBarcode(barcode, model.InvoiceNumber));
                col.Item().AlignCenter().Text(model.InvoiceNumber).FontSize(7).LetterSpacing(1);
                col.Item().AlignCenter().Text("* No Return or Exchange Without Receipt.\n* Exchange within 3 days of purchase.\n* No Cash Refunds.").FontFamily("Times New Roman").FontSize(6.5f);
                col.Item().BorderBottom(1).PaddingBottom(1).AlignCenter().Text("THANK YOU FOR YOUR VISIT").FontFamily("Times New Roman").FontSize(9).Bold().Italic().Underline();
                col.Item().AlignCenter().Text("Powered by MedicalERP").FontSize(6);
            });
        })).GeneratePdf();
    }

    private static IContainer Header(IContainer c) => c.BorderBottom(.7f).PaddingVertical(1).DefaultTextStyle(x => x.FontFamily("Times New Roman").FontSize(7).Bold());

    private static void TotalRow(ColumnDescriptor col, string label, decimal amount, bool bold = false) => col.Item().Row(row =>
    {
        var left = row.RelativeItem().Text(label).FontFamily("Times New Roman");
        var right = row.ConstantItem(55).AlignRight().Text(amount.ToString("N2")).FontFamily("Times New Roman");
        if (bold) { left.Bold().FontSize(9); right.Bold().FontSize(9); }
    });

    private static void DrawBarcode(RowDescriptor row, string value)
    {
        var bits = string.Concat(value.Select(ch => Convert.ToString(ch, 2).PadLeft(8, '0')));
        foreach (var bit in "101" + bits + "101")
            row.RelativeItem().Background(bit == '1' ? Colors.Black : Colors.White);
    }
}
