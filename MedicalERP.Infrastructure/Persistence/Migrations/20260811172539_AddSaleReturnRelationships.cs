using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleReturnRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ProductBatchId",
                table: "SaleReturnItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.Sql("UPDATE SaleReturnItems SET ProductBatchId = NULL WHERE ProductBatchId = '00000000-0000-0000-0000-000000000000'");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturns_SaleId",
                table: "SaleReturns",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturnItems_ProductBatchId",
                table: "SaleReturnItems",
                column: "ProductBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturnItems_ProductId",
                table: "SaleReturnItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleReturnItems_ProductBatches_ProductBatchId",
                table: "SaleReturnItems",
                column: "ProductBatchId",
                principalTable: "ProductBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleReturnItems_Products_ProductId",
                table: "SaleReturnItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleReturns_Sales_SaleId",
                table: "SaleReturns",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturnItems_ProductBatches_ProductBatchId",
                table: "SaleReturnItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturnItems_Products_ProductId",
                table: "SaleReturnItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturns_Sales_SaleId",
                table: "SaleReturns");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturns_SaleId",
                table: "SaleReturns");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturnItems_ProductBatchId",
                table: "SaleReturnItems");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturnItems_ProductId",
                table: "SaleReturnItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductBatchId",
                table: "SaleReturnItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
