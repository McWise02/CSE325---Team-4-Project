using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipe_and_Meal_Tracker.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToMealEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "MealEntries",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MealEntries_UserId",
                table: "MealEntries",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealEntries_AspNetUsers_UserId",
                table: "MealEntries",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealEntries_AspNetUsers_UserId",
                table: "MealEntries");

            migrationBuilder.DropIndex(
                name: "IX_MealEntries_UserId",
                table: "MealEntries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MealEntries");
        }
    }
}
