using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;

namespace AliRoyalMarquee.Infrastructure.Pdf
{
    public class BookingInvoiceDocument : IDocument
    {
        private readonly AliRoyalMarquee.Application.Bookings.DTOs.BookingDetailDto _booking;
        private static readonly string MainColor = "#5C0A1E"; // Dark Red
        private static readonly string TextColor = "#1A2E44"; // Dark Blue
        private static readonly string GrayText = "#666666"; // Gray
        private static readonly string BackgroundGray = "#F5F5F5"; // Light Gray

        public BookingInvoiceDocument(AliRoyalMarquee.Application.Bookings.DTOs.BookingDetailDto booking)
        {
            _booking = booking;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial).FontColor(Colors.Black));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        void ComposeHeader(IContainer container)
        {
            container.PaddingBottom(15).Row(row =>
            {
                row.RelativeItem(2).Row(colRow =>
                {
                    colRow.AutoItem().Width(80).Height(80).Background(Colors.Grey.Lighten3).AlignCenter().AlignMiddle().Text("Logo").FontColor(Colors.Grey.Darken1);
                    colRow.RelativeItem().PaddingLeft(15).Column(col =>
                    {
                        col.Item().Text("ALI ROYAL").FontSize(24).Bold().FontColor(MainColor).LineHeight(1.1f);
                        col.Item().Text("MARQUEE").FontSize(24).Bold().FontColor(MainColor).LineHeight(1.1f);
                        col.Item().PaddingTop(5).Text("Grand Ballroom & Executive Enclosures").FontSize(10).FontColor(GrayText);
                        col.Item().Text("123 Canal Road, Lahore, Punjab | Ph: +92 300 1234567").FontSize(9).FontColor(GrayText);
                    });
                });

                row.RelativeItem(1).AlignRight().Column(col =>
                {
                    col.Item().Text("BOOKING").FontSize(22).Bold().FontColor(TextColor).AlignRight().LineHeight(1.1f);
                    col.Item().Text("INVOICE").FontSize(22).Bold().FontColor(TextColor).AlignRight().LineHeight(1.1f);
                    col.Item().PaddingTop(5).Text(_booking.ReferenceNumber ?? "INV-2026-XXXX").FontSize(12).Bold().FontColor(MainColor).AlignRight();
                    col.Item().Text($"Date of Issue: {DateTime.Now:MMM dd, yyyy}").FontSize(10).AlignRight();
                    col.Item().Text(text =>
                    {
                        text.Span("Status: ").FontSize(10);
                        text.Span((_booking.PaymentStatus ?? "PENDING").ToUpper()).FontSize(10).Bold().FontColor("#F26622"); // Orange
                    });
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingVertical(10).Row(row =>
                {
                    // Client Info
                    row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
                    {
                        col.Item().PaddingBottom(5).Text("CLIENT INFORMATION").FontSize(10).Bold().FontColor(MainColor);
                        col.Item().Row(r => { r.RelativeItem().Text("Name").FontColor(GrayText); r.RelativeItem(2).AlignRight().Text(_booking.CustomerName).Bold(); });
                        col.Item().PaddingTop(2).Row(r => { r.RelativeItem().Text("Contact").FontColor(GrayText); r.RelativeItem(2).AlignRight().Text(_booking.CustomerPhone).Bold(); });
                        col.Item().PaddingTop(2).Row(r => { r.RelativeItem().Text("Booking").FontColor(GrayText); r.RelativeItem(2).AlignRight().Text(_booking.ReferenceNumber).Bold(); });
                    });
                    
                    row.Spacing(10);
                    
                    // Event Details
                    row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
                    {
                        col.Item().PaddingBottom(5).Text("EVENT DETAILS").FontSize(10).Bold().FontColor(MainColor);
                        col.Item().Row(r => { r.RelativeItem().Text("Event Type").FontColor(GrayText); r.RelativeItem(2).AlignRight().Text(_booking.EventTitle ?? "Event").Bold(); });
                        col.Item().PaddingTop(2).Row(r => { r.RelativeItem().Text("Event Date").FontColor(GrayText); r.RelativeItem(2).AlignRight().Text(_booking.DateStr).Bold(); });
                        col.Item().PaddingTop(2).Row(r => { r.RelativeItem().Text("Shift Timing").FontColor(GrayText); r.RelativeItem(2).AlignRight().Text(_booking.Shift).Bold(); });
                        col.Item().PaddingTop(2).Row(r => { r.RelativeItem().Text("Venue").FontColor(GrayText); r.RelativeItem(2).AlignRight().Text(_booking.Hall).Bold(); });
                    });
                    
                    row.Spacing(10);
                    
                    // Capacity Setup
                    row.RelativeItem().Border(2).BorderColor(MainColor).Padding(10).Column(col =>
                    {
                        col.Item().PaddingBottom(5).Text("CAPACITY SETUP").FontSize(10).Bold().FontColor(MainColor);
                        col.Item().Row(r => { r.RelativeItem().Text("Guaranteed Guests").FontColor(GrayText); r.RelativeItem(1).AlignRight().Text($"{_booking.Guests} Pax").Bold().FontColor(MainColor).FontSize(11); });
                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten3);
                        col.Item().PaddingTop(10).Row(r => { r.RelativeItem().Text("Food Buffer (10%)").FontColor(GrayText); r.RelativeItem(1).AlignRight().Text($"{Math.Round(_booking.Guests * 0.1)} Extra").Bold(); });
                        col.Item().Text("(Billed strictly if consumed)").FontSize(8).FontColor(GrayText);
                    });
                });

                column.Item().PaddingVertical(10).Element(ComposeTable);

                column.Item().PaddingTop(10).Row(row =>
                {
                    // Terms & Conditions
                    row.RelativeItem(2).Background(BackgroundGray).Padding(10).Column(col =>
                    {
                        col.Item().PaddingBottom(5).Text("Important Terms & Conditions").Bold();
                        col.Item().Text("• Timing Rule: As per Punjab Govt regulations, food service will close strictly at 9:30 PM and the hall must be vacated by 10:00 PM.").FontSize(9);
                        col.Item().Text("• Guest Count: Final billing is based on the Guaranteed Pax (500) OR actual attendance, whichever is higher. Buffer food is charged at the standard per-head rate if consumed.").FontSize(9);
                        col.Item().Text("• Outside Items: No outside food, fireworks, or aerial firing is permitted on the premises.").FontSize(9);
                        col.Item().Text("• Clearance: 100% of the remaining balance must be cleared before the event.").FontSize(9);
                    });
                    
                    row.Spacing(15);
                    
                    // Totals
                    row.RelativeItem(1).Column(col =>
                    {
                        col.Item().Row(r => { r.RelativeItem().Text("Subtotal").FontColor(GrayText); r.RelativeItem().AlignRight().Text(_booking.TotalAmount.ToString("N0")); });
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten3);
                        col.Item().PaddingTop(5).Row(r => { r.RelativeItem().Text("Service Charge / PRA Tax").FontColor(GrayText).FontSize(9); r.RelativeItem().AlignRight().Text("Included"); });
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten3);
                        col.Item().PaddingTop(5).Row(r => { r.RelativeItem().Text("Grand Total").Bold().FontColor(MainColor).FontSize(11); r.RelativeItem().AlignRight().Text($"PKR {_booking.TotalAmount:N0}").Bold().FontColor(MainColor).FontSize(11); });
                        
                        col.Item().PaddingTop(10).Row(r => { 
                            r.RelativeItem().Text("Advance Paid").FontColor(Colors.Green.Darken2); 
                            r.RelativeItem().AlignRight().Text($"- PKR\n{_booking.PaidAmount:N0}").FontColor(Colors.Green.Darken2); 
                        });
                        
                        col.Item().PaddingTop(10).Background(MainColor).Padding(8).Row(r => { 
                            r.RelativeItem().Text("Balance Due").Bold().FontColor(Colors.White).FontSize(12); 
                            r.RelativeItem().AlignRight().Text($"PKR {(_booking.TotalAmount - _booking.PaidAmount):N0}").Bold().FontColor(Colors.White).FontSize(12); 
                        });
                        
                        col.Item().PaddingTop(5).Text("Refundable Security Deposit: PKR 50,000").FontSize(8).FontColor(GrayText).AlignCenter();
                    });
                });
                
                // Add Signatures
                column.Item().PaddingTop(40).Row(row =>
                {
                    row.RelativeItem().PaddingRight(20).Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        col.Item().PaddingTop(5).Text("Authorized Signature").FontSize(10).FontColor(GrayText).AlignCenter();
                    });
                    row.RelativeItem().PaddingLeft(20).Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        col.Item().PaddingTop(5).Text("Client Signature").FontSize(10).FontColor(GrayText).AlignCenter();
                    });
                });
            });
        }

        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(MainColor).Padding(8).Text("DESCRIPTION").FontColor(Colors.White).FontSize(9).Bold();
                    header.Cell().Background(MainColor).Padding(8).Text("METHOD/STATUS").FontColor(Colors.White).FontSize(9).Bold();
                    header.Cell().Background(MainColor).Padding(8).Text("DATE/REF").FontColor(Colors.White).FontSize(9).Bold();
                    header.Cell().Background(MainColor).Padding(8).AlignRight().Text("AMOUNT (PKR)").FontColor(Colors.White).FontSize(9).Bold();
                });

                // Row 1 (Booking Total)
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8).Column(c => {
                    c.Item().Text("Total Event Booking Charges").Bold();
                    c.Item().Text($"Includes venue ({_booking.Hall}) and requested packages for {_booking.Guests} guests.").FontColor(GrayText).FontSize(9);
                });
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8).Text("-");
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8).Text("-");
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8).AlignRight().Text(_booking.TotalAmount.ToString("N0")).Bold();

                // List Real Payments
                foreach (var payment in _booking.Payments)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8).Column(c => {
                        c.Item().Text("Payment Received").Bold().FontColor(Colors.Green.Darken2);
                    });
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8).Column(c => {
                        c.Item().Text(payment.Method);
                        c.Item().Text(payment.Status).FontSize(8).FontColor(GrayText);
                    });
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8).Column(c => {
                        c.Item().Text(payment.DateStr).FontSize(9);
                        if (!string.IsNullOrEmpty(payment.Reference)) {
                            c.Item().Text($"Ref: {payment.Reference}").FontSize(8).FontColor(GrayText);
                        }
                    });
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8).AlignRight().Text($"-{payment.Amount:N0}").Bold().FontColor(Colors.Green.Darken2);
                }
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text("Ali Royal Marquee Management System").FontSize(8).FontColor(GrayText);
                row.RelativeItem().AlignRight().Text(x =>
                {
                    x.Span("System Generated Invoice | Page ").FontSize(8).FontColor(GrayText);
                    x.CurrentPageNumber().FontSize(8).FontColor(GrayText);
                    x.Span(" of ").FontSize(8).FontColor(GrayText);
                    x.TotalPages().FontSize(8).FontColor(GrayText);
                });
            });
        }
    }
}
