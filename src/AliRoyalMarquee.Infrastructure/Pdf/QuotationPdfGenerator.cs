using AliRoyalMarquee.Application.Enquiries.Interfaces;
using AliRoyalMarquee.Application.Enquiries.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AliRoyalMarquee.Infrastructure.Pdf;

public class QuotationPdfGenerator : IQuotationPdfGenerator
{
    public byte[] GeneratePdf(EnquiryDetailDto enquiry, EnquiryQuotationDto quotation)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                page.Header().Element(c => ComposeHeader(c, enquiry, quotation));
                page.Content().Element(c => ComposeContent(c, enquiry, quotation));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, EnquiryDetailDto enquiry, EnquiryQuotationDto quotation)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("Ali Royal Marquee").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                column.Item().Text("123 Wedding Boulevard, Celebration City");
                column.Item().Text("Phone: +92 300 1234567 | Email: info@aliroyalmarquee.com");
            });

            row.ConstantItem(100).Height(50).Placeholder(); // Placeholder for Logo
        });
    }

    private void ComposeContent(IContainer container, EnquiryDetailDto enquiry, EnquiryQuotationDto quotation)
    {
        container.PaddingVertical(1, Unit.Centimetre).Column(column =>
        {
            column.Spacing(5);

            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Prepared For:").SemiBold();
                    col.Item().Text(enquiry.CustomerName);
                    col.Item().Text(enquiry.CustomerPhone);
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text($"Quotation #: {quotation.QuotationReference}").SemiBold();
                    col.Item().Text($"Date: {quotation.CreatedAt:dd MMM yyyy}");
                    col.Item().Text($"Valid Until: {quotation.ValidUntil:dd MMM yyyy}");
                    col.Item().Text($"Event: {enquiry.EventName} - {enquiry.PreferredDate:dd MMM yyyy}");
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            column.Item().Element(c => ComposeTable(c, quotation));

            if (!string.IsNullOrWhiteSpace(quotation.Notes))
            {
                column.Item().PaddingTop(15).Text("Notes & Terms:").SemiBold();
                column.Item().Text(quotation.Notes);
            }
        });
    }

    private void ComposeTable(IContainer container, EnquiryQuotationDto quotation)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(30); // #
                columns.RelativeColumn(3);  // Description
                columns.RelativeColumn();   // Qty
                columns.RelativeColumn();   // Unit Price
                columns.RelativeColumn();   // Total
            });

            table.Header(header =>
            {
                header.Cell().Text("#").SemiBold();
                header.Cell().Text("Description").SemiBold();
                header.Cell().AlignRight().Text("Qty").SemiBold();
                header.Cell().AlignRight().Text("Unit Price").SemiBold();
                header.Cell().AlignRight().Text("Total").SemiBold();

                header.Cell().ColumnSpan(5).PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Black);
            });

            // Currently EnquiryQuotationDto in GetEnquiryById doesn't return LineItems yet, 
            // so we will just display a summary row for now until we fully wire it.
            // But structurally this is how QuestPDF handles the table.
            table.Cell().Text("1");
            table.Cell().Text("Event Charges (Summary)");
            table.Cell().AlignRight().Text("1");
            table.Cell().AlignRight().Text($"{quotation.Amount:N2}");
            table.Cell().AlignRight().Text($"{quotation.Amount:N2}");

            table.Cell().ColumnSpan(5).PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            table.Cell().ColumnSpan(4).AlignRight().Text("Grand Total:").SemiBold();
            table.Cell().AlignRight().Text($"{quotation.Amount:N2}").SemiBold();
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("Page ");
            x.CurrentPageNumber();
            x.Span(" of ");
            x.TotalPages();
        });
    }
}
