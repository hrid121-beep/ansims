using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogueFieldsToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CatalogueEntryDate",
                table: "Items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CatalogueLedgerNo",
                table: "Items",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CataloguePageNo",
                table: "Items",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CatalogueRemarks",
                table: "Items",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstReceivedDate",
                table: "Items",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatalogueEntryDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CatalogueLedgerNo",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CataloguePageNo",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CatalogueRemarks",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "FirstReceivedDate",
                table: "Items");
        }
    }
}
