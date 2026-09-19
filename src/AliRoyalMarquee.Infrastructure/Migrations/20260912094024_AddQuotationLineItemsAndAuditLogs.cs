using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AliRoyalMarquee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuotationLineItemsAndAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "EnquiryQuotations");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "EnquiryQuotations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GrandTotal",
                table: "EnquiryQuotations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviousQuotationId",
                table: "EnquiryQuotations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceChargeAmount",
                table: "EnquiryQuotations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "EnquiryQuotations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "EnquiryQuotations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EntityId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuotationLineItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuotationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationLineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationLineItems_EnquiryQuotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "EnquiryQuotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryQuotations_PreviousQuotationId",
                table: "EnquiryQuotations",
                column: "PreviousQuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryQuotations_QuotationReference_Version",
                table: "EnquiryQuotations",
                columns: new[] { "QuotationReference", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLineItems_QuotationId",
                table: "QuotationLineItems",
                column: "QuotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_EnquiryQuotations_EnquiryQuotations_PreviousQuotationId",
                table: "EnquiryQuotations",
                column: "PreviousQuotationId",
                principalTable: "EnquiryQuotations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnquiryQuotations_EnquiryQuotations_PreviousQuotationId",
                table: "EnquiryQuotations");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "QuotationLineItems");

            migrationBuilder.DropIndex(
                name: "IX_EnquiryQuotations_PreviousQuotationId",
                table: "EnquiryQuotations");

            migrationBuilder.DropIndex(
                name: "IX_EnquiryQuotations_QuotationReference_Version",
                table: "EnquiryQuotations");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "EnquiryQuotations");

            migrationBuilder.DropColumn(
                name: "GrandTotal",
                table: "EnquiryQuotations");

            migrationBuilder.DropColumn(
                name: "PreviousQuotationId",
                table: "EnquiryQuotations");

            migrationBuilder.DropColumn(
                name: "ServiceChargeAmount",
                table: "EnquiryQuotations");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "EnquiryQuotations");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "EnquiryQuotations");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "EnquiryQuotations",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
