using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipientItems_Items_ItemId",
                table: "AllotmentLetterRecipientItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Battalions_BattalionId",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Ranges_RangeId",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Unions_UnionId",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Upazilas_UpazilaId",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Zilas_ZilaId",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelItemIssues_Issues_OriginalIssueId",
                table: "PersonnelItemIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelItemIssues_Receives_ReceiveId",
                table: "PersonnelItemIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Items_ItemId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Stores_DestinationStoreId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Stores_SourceStoreId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Stores_StoreId",
                table: "StockMovements");

            migrationBuilder.AddColumn<int>(
                name: "DestinationStoreId1",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemId1",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceStoreId1",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreId1",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginalIssueId1",
                table: "PersonnelItemIssues",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiveId1",
                table: "PersonnelItemIssues",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientType",
                table: "AllotmentLetterRecipients",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientName",
                table: "AllotmentLetterRecipients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AllotmentLetterId1",
                table: "AllotmentLetterRecipients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BattalionId1",
                table: "AllotmentLetterRecipients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RangeId1",
                table: "AllotmentLetterRecipients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnionId1",
                table: "AllotmentLetterRecipients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpazilaId1",
                table: "AllotmentLetterRecipients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ZilaId1",
                table: "AllotmentLetterRecipients",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "IssuedQuantity",
                table: "AllotmentLetterRecipientItems",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AllottedQuantity",
                table: "AllotmentLetterRecipientItems",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "AllotmentLetterRecipientId1",
                table: "AllotmentLetterRecipientItems",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientTitleBn",
                table: "AllotmentLetterDistributions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientTitle",
                table: "AllotmentLetterDistributions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "PurposeBn",
                table: "AllotmentLetterDistributions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Purpose",
                table: "AllotmentLetterDistributions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AddressBn",
                table: "AllotmentLetterDistributions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "AllotmentLetterDistributions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AllotmentLetterId1",
                table: "AllotmentLetterDistributions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_DestinationStoreId1",
                table: "StockMovements",
                column: "DestinationStoreId1");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ItemId",
                table: "StockMovements",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ItemId1",
                table: "StockMovements",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_MovementDate",
                table: "StockMovements",
                column: "MovementDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ReferenceType_ReferenceId",
                table: "StockMovements",
                columns: new[] { "ReferenceType", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_SourceStoreId1",
                table: "StockMovements",
                column: "SourceStoreId1");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_StoreId1",
                table: "StockMovements",
                column: "StoreId1");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_OriginalIssueId1",
                table: "PersonnelItemIssues",
                column: "OriginalIssueId1");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_ReceiveId1",
                table: "PersonnelItemIssues",
                column: "ReceiveId1");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_AllotmentLetterId1",
                table: "AllotmentLetterRecipients",
                column: "AllotmentLetterId1");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_BattalionId1",
                table: "AllotmentLetterRecipients",
                column: "BattalionId1");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_RangeId1",
                table: "AllotmentLetterRecipients",
                column: "RangeId1");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_RecipientType",
                table: "AllotmentLetterRecipients",
                column: "RecipientType");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_UnionId1",
                table: "AllotmentLetterRecipients",
                column: "UnionId1");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_UpazilaId1",
                table: "AllotmentLetterRecipients",
                column: "UpazilaId1");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_ZilaId1",
                table: "AllotmentLetterRecipients",
                column: "ZilaId1");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipientItems_AllotmentLetterRecipientId1",
                table: "AllotmentLetterRecipientItems",
                column: "AllotmentLetterRecipientId1");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterDistributions_AllotmentLetterId1",
                table: "AllotmentLetterDistributions",
                column: "AllotmentLetterId1");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterDistributions_SerialNo",
                table: "AllotmentLetterDistributions",
                column: "SerialNo");

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterDistributions_AllotmentLetters_AllotmentLetterId1",
                table: "AllotmentLetterDistributions",
                column: "AllotmentLetterId1",
                principalTable: "AllotmentLetters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipientItems_AllotmentLetterRecipients_AllotmentLetterRecipientId1",
                table: "AllotmentLetterRecipientItems",
                column: "AllotmentLetterRecipientId1",
                principalTable: "AllotmentLetterRecipients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipientItems_Items_ItemId",
                table: "AllotmentLetterRecipientItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_AllotmentLetters_AllotmentLetterId1",
                table: "AllotmentLetterRecipients",
                column: "AllotmentLetterId1",
                principalTable: "AllotmentLetters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Battalions_BattalionId",
                table: "AllotmentLetterRecipients",
                column: "BattalionId",
                principalTable: "Battalions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Battalions_BattalionId1",
                table: "AllotmentLetterRecipients",
                column: "BattalionId1",
                principalTable: "Battalions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Ranges_RangeId",
                table: "AllotmentLetterRecipients",
                column: "RangeId",
                principalTable: "Ranges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Ranges_RangeId1",
                table: "AllotmentLetterRecipients",
                column: "RangeId1",
                principalTable: "Ranges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Unions_UnionId",
                table: "AllotmentLetterRecipients",
                column: "UnionId",
                principalTable: "Unions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Unions_UnionId1",
                table: "AllotmentLetterRecipients",
                column: "UnionId1",
                principalTable: "Unions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Upazilas_UpazilaId",
                table: "AllotmentLetterRecipients",
                column: "UpazilaId",
                principalTable: "Upazilas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Upazilas_UpazilaId1",
                table: "AllotmentLetterRecipients",
                column: "UpazilaId1",
                principalTable: "Upazilas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Zilas_ZilaId",
                table: "AllotmentLetterRecipients",
                column: "ZilaId",
                principalTable: "Zilas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Zilas_ZilaId1",
                table: "AllotmentLetterRecipients",
                column: "ZilaId1",
                principalTable: "Zilas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelItemIssues_Issues_OriginalIssueId",
                table: "PersonnelItemIssues",
                column: "OriginalIssueId",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelItemIssues_Issues_OriginalIssueId1",
                table: "PersonnelItemIssues",
                column: "OriginalIssueId1",
                principalTable: "Issues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelItemIssues_Receives_ReceiveId",
                table: "PersonnelItemIssues",
                column: "ReceiveId",
                principalTable: "Receives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelItemIssues_Receives_ReceiveId1",
                table: "PersonnelItemIssues",
                column: "ReceiveId1",
                principalTable: "Receives",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Items_ItemId",
                table: "StockMovements",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Items_ItemId1",
                table: "StockMovements",
                column: "ItemId1",
                principalTable: "Items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Stores_DestinationStoreId",
                table: "StockMovements",
                column: "DestinationStoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Stores_DestinationStoreId1",
                table: "StockMovements",
                column: "DestinationStoreId1",
                principalTable: "Stores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Stores_SourceStoreId",
                table: "StockMovements",
                column: "SourceStoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Stores_SourceStoreId1",
                table: "StockMovements",
                column: "SourceStoreId1",
                principalTable: "Stores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Stores_StoreId",
                table: "StockMovements",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Stores_StoreId1",
                table: "StockMovements",
                column: "StoreId1",
                principalTable: "Stores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterDistributions_AllotmentLetters_AllotmentLetterId1",
                table: "AllotmentLetterDistributions");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipientItems_AllotmentLetterRecipients_AllotmentLetterRecipientId1",
                table: "AllotmentLetterRecipientItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipientItems_Items_ItemId",
                table: "AllotmentLetterRecipientItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_AllotmentLetters_AllotmentLetterId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Battalions_BattalionId",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Battalions_BattalionId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Ranges_RangeId",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Ranges_RangeId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Unions_UnionId",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Unions_UnionId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Upazilas_UpazilaId",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Upazilas_UpazilaId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Zilas_ZilaId",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_AllotmentLetterRecipients_Zilas_ZilaId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelItemIssues_Issues_OriginalIssueId",
                table: "PersonnelItemIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelItemIssues_Issues_OriginalIssueId1",
                table: "PersonnelItemIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelItemIssues_Receives_ReceiveId",
                table: "PersonnelItemIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelItemIssues_Receives_ReceiveId1",
                table: "PersonnelItemIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Items_ItemId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Items_ItemId1",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Stores_DestinationStoreId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Stores_DestinationStoreId1",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Stores_SourceStoreId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Stores_SourceStoreId1",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Stores_StoreId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Stores_StoreId1",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_DestinationStoreId1",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_ItemId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_ItemId1",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_MovementDate",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_ReferenceType_ReferenceId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_SourceStoreId1",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_StoreId1",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_PersonnelItemIssues_OriginalIssueId1",
                table: "PersonnelItemIssues");

            migrationBuilder.DropIndex(
                name: "IX_PersonnelItemIssues_ReceiveId1",
                table: "PersonnelItemIssues");

            migrationBuilder.DropIndex(
                name: "IX_AllotmentLetterRecipients_AllotmentLetterId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropIndex(
                name: "IX_AllotmentLetterRecipients_BattalionId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropIndex(
                name: "IX_AllotmentLetterRecipients_RangeId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropIndex(
                name: "IX_AllotmentLetterRecipients_RecipientType",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropIndex(
                name: "IX_AllotmentLetterRecipients_UnionId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropIndex(
                name: "IX_AllotmentLetterRecipients_UpazilaId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropIndex(
                name: "IX_AllotmentLetterRecipients_ZilaId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropIndex(
                name: "IX_AllotmentLetterRecipientItems_AllotmentLetterRecipientId1",
                table: "AllotmentLetterRecipientItems");

            migrationBuilder.DropIndex(
                name: "IX_AllotmentLetterDistributions_AllotmentLetterId1",
                table: "AllotmentLetterDistributions");

            migrationBuilder.DropIndex(
                name: "IX_AllotmentLetterDistributions_SerialNo",
                table: "AllotmentLetterDistributions");

            migrationBuilder.DropColumn(
                name: "DestinationStoreId1",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ItemId1",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "SourceStoreId1",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "StoreId1",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "OriginalIssueId1",
                table: "PersonnelItemIssues");

            migrationBuilder.DropColumn(
                name: "ReceiveId1",
                table: "PersonnelItemIssues");

            migrationBuilder.DropColumn(
                name: "AllotmentLetterId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropColumn(
                name: "BattalionId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropColumn(
                name: "RangeId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropColumn(
                name: "UnionId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropColumn(
                name: "UpazilaId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropColumn(
                name: "ZilaId1",
                table: "AllotmentLetterRecipients");

            migrationBuilder.DropColumn(
                name: "AllotmentLetterRecipientId1",
                table: "AllotmentLetterRecipientItems");

            migrationBuilder.DropColumn(
                name: "AllotmentLetterId1",
                table: "AllotmentLetterDistributions");

            migrationBuilder.AlterColumn<string>(
                name: "RecipientType",
                table: "AllotmentLetterRecipients",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientName",
                table: "AllotmentLetterRecipients",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<decimal>(
                name: "IssuedQuantity",
                table: "AllotmentLetterRecipientItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AllottedQuantity",
                table: "AllotmentLetterRecipientItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<string>(
                name: "RecipientTitleBn",
                table: "AllotmentLetterDistributions",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientTitle",
                table: "AllotmentLetterDistributions",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "PurposeBn",
                table: "AllotmentLetterDistributions",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Purpose",
                table: "AllotmentLetterDistributions",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AddressBn",
                table: "AllotmentLetterDistributions",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "AllotmentLetterDistributions",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipientItems_Items_ItemId",
                table: "AllotmentLetterRecipientItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Battalions_BattalionId",
                table: "AllotmentLetterRecipients",
                column: "BattalionId",
                principalTable: "Battalions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Ranges_RangeId",
                table: "AllotmentLetterRecipients",
                column: "RangeId",
                principalTable: "Ranges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Unions_UnionId",
                table: "AllotmentLetterRecipients",
                column: "UnionId",
                principalTable: "Unions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Upazilas_UpazilaId",
                table: "AllotmentLetterRecipients",
                column: "UpazilaId",
                principalTable: "Upazilas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllotmentLetterRecipients_Zilas_ZilaId",
                table: "AllotmentLetterRecipients",
                column: "ZilaId",
                principalTable: "Zilas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelItemIssues_Issues_OriginalIssueId",
                table: "PersonnelItemIssues",
                column: "OriginalIssueId",
                principalTable: "Issues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelItemIssues_Receives_ReceiveId",
                table: "PersonnelItemIssues",
                column: "ReceiveId",
                principalTable: "Receives",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Items_ItemId",
                table: "StockMovements",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Stores_DestinationStoreId",
                table: "StockMovements",
                column: "DestinationStoreId",
                principalTable: "Stores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Stores_SourceStoreId",
                table: "StockMovements",
                column: "SourceStoreId",
                principalTable: "Stores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Stores_StoreId",
                table: "StockMovements",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id");
        }
    }
}
