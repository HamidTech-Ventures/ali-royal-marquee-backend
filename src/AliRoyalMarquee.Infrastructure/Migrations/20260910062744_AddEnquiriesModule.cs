using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AliRoyalMarquee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEnquiriesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AlternativeDate",
                table: "Enquiries",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToId",
                table: "Enquiries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Enquiries",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedValue",
                table: "Enquiries",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LostReason",
                table: "Enquiries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Enquiries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PreferredEndTime",
                table: "Enquiries",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PreferredStartTime",
                table: "Enquiries",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreferredVenueId",
                table: "Enquiries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Enquiries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Enquiries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EnquiryActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EnquiryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    PerformedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnquiryActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnquiryActivities_Enquiries_EnquiryId",
                        column: x => x.EnquiryId,
                        principalTable: "Enquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnquiryActivities_Users_PerformedById",
                        column: x => x.PerformedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnquiryFollowUps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EnquiryId = table.Column<Guid>(type: "uuid", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AssignedToId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Result = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnquiryFollowUps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnquiryFollowUps_Enquiries_EnquiryId",
                        column: x => x.EnquiryId,
                        principalTable: "Enquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnquiryFollowUps_Users_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EnquiryFollowUps_Users_CompletedById",
                        column: x => x.CompletedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnquiryQuotations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EnquiryId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuotationReference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnquiryQuotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnquiryQuotations_Enquiries_EnquiryId",
                        column: x => x.EnquiryId,
                        principalTable: "Enquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnquiryQuotations_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_AssignedToId",
                table: "Enquiries",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_PreferredVenueId",
                table: "Enquiries",
                column: "PreferredVenueId");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryActivities_EnquiryId",
                table: "EnquiryActivities",
                column: "EnquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryActivities_PerformedById",
                table: "EnquiryActivities",
                column: "PerformedById");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryFollowUps_AssignedToId",
                table: "EnquiryFollowUps",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryFollowUps_CompletedById",
                table: "EnquiryFollowUps",
                column: "CompletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryFollowUps_EnquiryId",
                table: "EnquiryFollowUps",
                column: "EnquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryQuotations_CreatorId",
                table: "EnquiryQuotations",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryQuotations_EnquiryId",
                table: "EnquiryQuotations",
                column: "EnquiryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enquiries_Users_AssignedToId",
                table: "Enquiries",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Enquiries_Venues_PreferredVenueId",
                table: "Enquiries",
                column: "PreferredVenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enquiries_Users_AssignedToId",
                table: "Enquiries");

            migrationBuilder.DropForeignKey(
                name: "FK_Enquiries_Venues_PreferredVenueId",
                table: "Enquiries");

            migrationBuilder.DropTable(
                name: "EnquiryActivities");

            migrationBuilder.DropTable(
                name: "EnquiryFollowUps");

            migrationBuilder.DropTable(
                name: "EnquiryQuotations");

            migrationBuilder.DropIndex(
                name: "IX_Enquiries_AssignedToId",
                table: "Enquiries");

            migrationBuilder.DropIndex(
                name: "IX_Enquiries_PreferredVenueId",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "AlternativeDate",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "EstimatedValue",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "LostReason",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "PreferredEndTime",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "PreferredStartTime",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "PreferredVenueId",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Enquiries");
        }
    }
}
