using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCriticalStockIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreItems_Items_ItemId",
                table: "StoreItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreItems_Stores_StoreId",
                table: "StoreItems");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "StoreItems",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreItems_Items_ItemId",
                table: "StoreItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreItems_Stores_StoreId",
                table: "StoreItems",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreItems_Items_ItemId",
                table: "StoreItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreItems_Stores_StoreId",
                table: "StoreItems");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "StoreItems");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreItems_Items_ItemId",
                table: "StoreItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreItems_Stores_StoreId",
                table: "StoreItems",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
