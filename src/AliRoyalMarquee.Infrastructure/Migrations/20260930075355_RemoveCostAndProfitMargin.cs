using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AliRoyalMarquee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCostAndProfitMargin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfitMarginTarget",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "MenuItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfitMarginTarget",
                table: "Packages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "MenuItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
