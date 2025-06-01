using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNewsManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Initdb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "NewsArticles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticles_CreatedByID",
                table: "NewsArticles",
                column: "CreatedByID");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsArticles_SystemAccounts_CreatedByID",
                table: "NewsArticles",
                column: "CreatedByID",
                principalTable: "SystemAccounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsArticles_SystemAccounts_CreatedByID",
                table: "NewsArticles");

            migrationBuilder.DropIndex(
                name: "IX_NewsArticles_CreatedByID",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "NewsArticles");
        }
    }
}
