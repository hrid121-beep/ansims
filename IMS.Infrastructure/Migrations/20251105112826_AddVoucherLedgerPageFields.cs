using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherLedgerPageFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VoucherDate",
                table: "Receives",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherDocumentPath",
                table: "Receives",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VoucherGeneratedDate",
                table: "Receives",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherNo",
                table: "Receives",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherNumber",
                table: "Receives",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherQRCode",
                table: "Receives",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LedgerNo",
                table: "ReceiveItems",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageNo",
                table: "ReceiveItems",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PartiallyUsableQuantity",
                table: "ReceiveItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnusableQuantity",
                table: "ReceiveItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UsableQuantity",
                table: "ReceiveItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LedgerNo",
                table: "IssueItems",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageNo",
                table: "IssueItems",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PartiallyUsableQuantity",
                table: "IssueItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnusableQuantity",
                table: "IssueItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UsableQuantity",
                table: "IssueItems",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoucherDate",
                table: "Receives");

            migrationBuilder.DropColumn(
                name: "VoucherDocumentPath",
                table: "Receives");

            migrationBuilder.DropColumn(
                name: "VoucherGeneratedDate",
                table: "Receives");

            migrationBuilder.DropColumn(
                name: "VoucherNo",
                table: "Receives");

            migrationBuilder.DropColumn(
                name: "VoucherNumber",
                table: "Receives");

            migrationBuilder.DropColumn(
                name: "VoucherQRCode",
                table: "Receives");

            migrationBuilder.DropColumn(
                name: "LedgerNo",
                table: "ReceiveItems");

            migrationBuilder.DropColumn(
                name: "PageNo",
                table: "ReceiveItems");

            migrationBuilder.DropColumn(
                name: "PartiallyUsableQuantity",
                table: "ReceiveItems");

            migrationBuilder.DropColumn(
                name: "UnusableQuantity",
                table: "ReceiveItems");

            migrationBuilder.DropColumn(
                name: "UsableQuantity",
                table: "ReceiveItems");

            migrationBuilder.DropColumn(
                name: "LedgerNo",
                table: "IssueItems");

            migrationBuilder.DropColumn(
                name: "PageNo",
                table: "IssueItems");

            migrationBuilder.DropColumn(
                name: "PartiallyUsableQuantity",
                table: "IssueItems");

            migrationBuilder.DropColumn(
                name: "UnusableQuantity",
                table: "IssueItems");

            migrationBuilder.DropColumn(
                name: "UsableQuantity",
                table: "IssueItems");
        }
    }
}
