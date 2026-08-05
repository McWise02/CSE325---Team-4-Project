using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipe_and_Meal_Tracker.Migrations
{
    /// <inheritdoc />
    public partial class AddPantryInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PantryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(9,3)", precision: 9, scale: 3, nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    ReorderThreshold = table.Column<decimal>(type: "decimal(9,3)", precision: 9, scale: 3, nullable: false),
                    StorageLocation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    ExpiresOn = table.Column<DateOnly>(type: "date", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PantryItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PantryItems_Name",
                table: "PantryItems",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PantryItems");
        }
    }
}
