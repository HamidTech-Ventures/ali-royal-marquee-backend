using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AliRoyalMarquee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OverhaulEnquiries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredEndTime",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "PreferredStartTime",
                table: "Enquiries");

            migrationBuilder.RenameColumn(
                name: "TaxAmount",
                table: "EnquiryQuotations",
                newName: "TokenMoney");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Enquiries",
                newName: "Shift");

            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "QuotationLineItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AdvancePayment",
                table: "EnquiryQuotations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PRATaxAmount",
                table: "EnquiryQuotations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BufferCapacity",
                table: "Enquiries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PartitionRequired",
                table: "Enquiries",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "QuotationLineItems");

            migrationBuilder.DropColumn(
                name: "AdvancePayment",
                table: "EnquiryQuotations");

            migrationBuilder.DropColumn(
                name: "PRATaxAmount",
                table: "EnquiryQuotations");

            migrationBuilder.DropColumn(
                name: "BufferCapacity",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "PartitionRequired",
                table: "Enquiries");

            migrationBuilder.RenameColumn(
                name: "TokenMoney",
                table: "EnquiryQuotations",
                newName: "TaxAmount");

            migrationBuilder.RenameColumn(
                name: "Shift",
                table: "Enquiries",
                newName: "Priority");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "PreferredEndTime",
                table: "Enquiries",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "PreferredStartTime",
                table: "Enquiries",
                type: "time without time zone",
                nullable: true);
        }
    }
}
