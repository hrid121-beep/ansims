using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLedgerBookIdToIssueAndReceiveItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LedgerBookId",
                table: "ReceiveItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LedgerBookId",
                table: "IssueItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiveItems_LedgerBookId",
                table: "ReceiveItems",
                column: "LedgerBookId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueItems_LedgerBookId",
                table: "IssueItems",
                column: "LedgerBookId");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueItems_LedgerBooks_LedgerBookId",
                table: "IssueItems",
                column: "LedgerBookId",
                principalTable: "LedgerBooks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiveItems_LedgerBooks_LedgerBookId",
                table: "ReceiveItems",
                column: "LedgerBookId",
                principalTable: "LedgerBooks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueItems_LedgerBooks_LedgerBookId",
                table: "IssueItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiveItems_LedgerBooks_LedgerBookId",
                table: "ReceiveItems");

            migrationBuilder.DropIndex(
                name: "IX_ReceiveItems_LedgerBookId",
                table: "ReceiveItems");

            migrationBuilder.DropIndex(
                name: "IX_IssueItems_LedgerBookId",
                table: "IssueItems");

            migrationBuilder.DropColumn(
                name: "LedgerBookId",
                table: "ReceiveItems");

            migrationBuilder.DropColumn(
                name: "LedgerBookId",
                table: "IssueItems");
        }
    }
}
