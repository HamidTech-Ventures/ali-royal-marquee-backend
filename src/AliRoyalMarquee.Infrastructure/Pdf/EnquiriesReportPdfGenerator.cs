using AliRoyalMarquee.Application.Enquiries.Interfaces;
using AliRoyalMarquee.Application.Enquiries.DTOs;
using AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryStats;
using AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryLifecycle;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace AliRoyalMarquee.Infrastructure.Pdf;

public class EnquiriesReportPdfGenerator : IEnquiriesReportPdfGenerator
{
    public byte[] GeneratePdf(EnquiryStatsDto stats, EnquiryLifecycleDto lifecycle, List<EnquiryDto> enquiries, string appliedFilters)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial));

                page.Header().Element(c => ComposeHeader(c, appliedFilters));
                page.Content().Element(c => ComposeContent(c, stats, lifecycle, enquiries));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, string appliedFilters)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("Ali Royal Marquee - Enquiries Report").FontSize(18).SemiBold().FontColor(Colors.Blue.Darken2);
                column.Item().Text($"Filters Applied: {appliedFilters}");
            });
            row.ConstantItem(100).Height(40).Placeholder(); // Placeholder for Logo
        });
    }

    private void ComposeContent(IContainer container, EnquiryStatsDto stats, EnquiryLifecycleDto lifecycle, List<EnquiryDto> enquiries)
    {
        container.PaddingVertical(1, Unit.Centimetre).Column(column =>
        {
            column.Spacing(10);
            
            // Stats Row
            column.Item().Row(row =>
            {
                row.RelativeItem().Text($"Total: {stats.TotalEnquiries}").SemiBold();
                row.RelativeItem().Text($"Hot Leads: {stats.HotLeads}").SemiBold();
                row.RelativeItem().Text($"Conv. Rate: {stats.ConversionRate}%").SemiBold();
                row.RelativeItem().Text($"Pipeline: {stats.EstimatedPipelineValue:C}").SemiBold();
            });

            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            // Table
            column.Item().Element(c => ComposeTable(c, enquiries));
        });
    }

    private void ComposeTable(IContainer container, List<EnquiryDto> enquiries)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(80); // Ref
                columns.RelativeColumn(2);  // Customer
                columns.RelativeColumn(2);  // Event
                columns.RelativeColumn();   // Date
                columns.RelativeColumn();   // Status
                columns.RelativeColumn();   // Priority
            });

            table.Header(header =>
            {
                header.Cell().Text("Ref #").SemiBold();
                header.Cell().Text("Customer").SemiBold();
                header.Cell().Text("Event").SemiBold();
                header.Cell().Text("Date").SemiBold();
                header.Cell().Text("Status").SemiBold();
                header.Cell().Text("Priority").SemiBold();

                header.Cell().ColumnSpan(6).PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Black);
            });

            foreach (var enq in enquiries)
            {
                table.Cell().Text(enq.ReferenceNumber);
                table.Cell().Text(enq.CustomerName);
                table.Cell().Text(enq.EventName);
                table.Cell().Text(enq.PreferredDate.ToString("dd MMM yyyy"));
                table.Cell().Text(enq.Status.ToString());
                table.Cell().Text(enq.Priority.ToString());
            }
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
