using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AliRoyalMarquee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEnquirySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "PreferredStartTime",
                table: "Enquiries",
                type: "time without time zone",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "interval",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "PreferredEndTime",
                table: "Enquiries",
                type: "time without time zone",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "interval",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "PreferredDate",
                table: "Enquiries",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "AlternativeDate",
                table: "Enquiries",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "PreferredStartTime",
                table: "Enquiries",
                type: "interval",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "PreferredEndTime",
                table: "Enquiries",
                type: "interval",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PreferredDate",
                table: "Enquiries",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AlternativeDate",
                table: "Enquiries",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
