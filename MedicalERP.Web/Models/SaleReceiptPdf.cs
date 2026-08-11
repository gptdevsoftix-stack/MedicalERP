using MedicalERP.Application.Sales.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedicalERP.Web.Models;

public static class SaleReceiptPdf
{
    static SaleReceiptPdf()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static byte[] Build(SaleFormDto model)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5.Portrait());
                page.Margin(18);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Black));
                page.Content().Column(col =>
                {
                    col.Spacing(5);

                    col.Item().Column(header =>
                    {
                        header.Spacing(1);
                        header.Item().AlignCenter().Text("MEDICALERP PHARMACY").FontSize(16).Bold();
                        header.Item().AlignCenter().Text("Multi-company Medical ERP & POS").FontSize(8).FontColor(Colors.Grey.Darken1);
                        header.Item().AlignCenter().Text($"Receipt / Invoice: {model.InvoiceNumber}").FontSize(10).SemiBold().FontColor(Colors.Blue.Darken2);
                    });

                    col.Item().LineHorizontal(0.75f).LineColor(Colors.Grey.Medium);

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Date: {model.SaleDate:yyyy-MM-dd HH:mm}").FontSize(8);
                        row.RelativeItem().AlignRight().Text($"Customer: {model.CustomerName ?? "Walk-in customer"}").FontSize(8);
                    });
                    col.Item().Text($"Register session: {(model.RegisterSessionName ?? "Auto / Open")}").FontSize(8);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(3);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1.4f);
                        });

                        table.Header(h =>
                        {
                            h.Cell().Element(HeaderCellStyle).Text("Item");
                            h.Cell().Element(HeaderCellStyle).AlignRight().Text("Qty");
                            h.Cell().Element(HeaderCellStyle).AlignRight().Text("Price");
                            h.Cell().Element(HeaderCellStyle).AlignRight().Text("Total");
                        });

                        foreach (var item in model.Items)
                        {
                            table.Cell().PaddingVertical(2).Text(item.ProductName ?? "-").FontSize(8);
                            table.Cell().PaddingVertical(2).AlignRight().Text(item.Quantity.ToString("N4")).FontSize(8);
                            table.Cell().PaddingVertical(2).AlignRight().Text($"Rs {item.UnitPrice.ToString("N2")}").FontSize(8);
                            table.Cell().PaddingVertical(2).AlignRight().Text($"Rs {item.NetAmount.ToString("N2")}").FontSize(8);
                        }
                    });

                    col.Item().LineHorizontal(0.75f).LineColor(Colors.Grey.Medium);

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Subtotal").FontSize(9);
                        row.RelativeItem().AlignRight().Text($"Rs {model.Subtotal.ToString("N2")}").FontSize(9);
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Discount").FontSize(9);
                        row.RelativeItem().AlignRight().Text($"- Rs {model.ItemDiscount.ToString("N2")}").FontSize(9);
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Tax").FontSize(9);
                        row.RelativeItem().AlignRight().Text($"Rs {model.TaxAmount.ToString("N2")}").FontSize(9);
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Grand Total").FontSize(12).Bold();
                        row.RelativeItem().AlignRight().Text($"Rs {model.GrandTotal.ToString("N2")}").FontSize(12).Bold();
                    });

                    col.Item().LineHorizontal(0.75f).LineColor(Colors.Grey.Medium);

                    if (model.Payments.Count > 0)
                    {
                        col.Item().Text("Payment").FontSize(9).SemiBold().FontColor(Colors.Grey.Darken2);
                        foreach (var payment in model.Payments)
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text(payment.PaymentMethodName ?? "Payment").FontSize(8);
                                row.RelativeItem().AlignRight().Text($"Rs {payment.Amount.ToString("N2")}").FontSize(8);
                            });
                        }
                    }

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Paid").FontSize(9);
                        row.RelativeItem().AlignRight().Text($"Rs {model.PaidAmount.ToString("N2")}").FontSize(9).SemiBold();
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Change").FontSize(9);
                        row.RelativeItem().AlignRight().Text($"Rs {model.ChangeAmount.ToString("N2")}").FontSize(9);
                    });
                    if (model.DueAmount > 0)
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Balance Due").FontSize(9);
                            row.RelativeItem().AlignRight().Text($"Rs {model.DueAmount.ToString("N2")}").FontSize(9).Bold();
                        });
                    }

                    col.Item().LineHorizontal(0.75f).LineColor(Colors.Grey.Medium);

                    col.Item().AlignCenter().PaddingTop(6).Text("Thank you for your purchase!").FontSize(9).SemiBold();
                    col.Item().AlignCenter().Text("Medicines should be stored as directed on the label.").FontSize(7).FontColor(Colors.Grey.Darken1);
                });
            });
        });

        return document.GeneratePdf();
    }

    private static IContainer HeaderCellStyle(IContainer container) => container
        .Background(Colors.Grey.Lighten3)
        .PaddingVertical(3)
        .PaddingHorizontal(2)
        .DefaultTextStyle(x => x.SemiBold().FontSize(8));
}
