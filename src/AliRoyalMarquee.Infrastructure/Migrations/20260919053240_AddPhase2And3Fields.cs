using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AliRoyalMarquee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhase2And3Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AddonId",
                table: "QuotationLineItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PackageId",
                table: "QuotationLineItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StaffMemberId",
                table: "EventStaff",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PackageId",
                table: "Bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLineItems_AddonId",
                table: "QuotationLineItems",
                column: "AddonId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLineItems_PackageId",
                table: "QuotationLineItems",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_EventStaff_StaffMemberId",
                table: "EventStaff",
                column: "StaffMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PackageId",
                table: "Bookings",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Packages_PackageId",
                table: "Bookings",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventStaff_StaffMembers_StaffMemberId",
                table: "EventStaff",
                column: "StaffMemberId",
                principalTable: "StaffMembers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLineItems_Addons_AddonId",
                table: "QuotationLineItems",
                column: "AddonId",
                principalTable: "Addons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLineItems_Packages_PackageId",
                table: "QuotationLineItems",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Packages_PackageId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_EventStaff_StaffMembers_StaffMemberId",
                table: "EventStaff");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLineItems_Addons_AddonId",
                table: "QuotationLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLineItems_Packages_PackageId",
                table: "QuotationLineItems");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLineItems_AddonId",
                table: "QuotationLineItems");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLineItems_PackageId",
                table: "QuotationLineItems");

            migrationBuilder.DropIndex(
                name: "IX_EventStaff_StaffMemberId",
                table: "EventStaff");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PackageId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AddonId",
                table: "QuotationLineItems");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "QuotationLineItems");

            migrationBuilder.DropColumn(
                name: "StaffMemberId",
                table: "EventStaff");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "Bookings");
        }
    }
}
