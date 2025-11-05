using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLedgerBookManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LedgerBooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LedgerNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BookName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BookType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    TotalPages = table.Column<int>(type: "int", nullable: false),
                    CurrentPageNo = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LedgerBooks_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LedgerBooks_StoreId",
                table: "LedgerBooks",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerBooks_LedgerNo",
                table: "LedgerBooks",
                column: "LedgerNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerBooks_BookType",
                table: "LedgerBooks",
                column: "BookType");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerBooks_IsClosed",
                table: "LedgerBooks",
                column: "IsClosed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LedgerBooks");
        }
    }
}
