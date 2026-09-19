using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AliRoyalMarquee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Milestone1Fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OriginalPaymentId",
                table: "Payments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseDate",
                table: "Expenses",
                column: "ExpenseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingDate",
                table: "Bookings",
                column: "BookingDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StartTime",
                table: "Bookings",
                column: "StartTime");

            migrationBuilder.Sql(@"
                ALTER TABLE ""Bookings"" DROP CONSTRAINT ""EX_Booking_Overlap"";
                ALTER TABLE ""Bookings"" 
                ADD CONSTRAINT ""EX_Booking_Overlap"" 
                EXCLUDE USING gist (
                    ""VenueId"" WITH =, 
                    tstzrange(""StartTime"", ""EndTime"") WITH &&
                ) WHERE (""Status"" != 3);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseDate",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Email",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_BookingDate",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_StartTime",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "OriginalPaymentId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Payments");

            migrationBuilder.Sql(@"
                ALTER TABLE ""Bookings"" DROP CONSTRAINT ""EX_Booking_Overlap"";
                ALTER TABLE ""Bookings"" 
                ADD CONSTRAINT ""EX_Booking_Overlap"" 
                EXCLUDE USING gist (
                    ""VenueId"" WITH =, 
                    tstzrange(""StartTime"", ""EndTime"") WITH &&
                );
            ");
        }
    }
}
