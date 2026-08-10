using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeSaleRegisterSessionNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "StockTransactions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "StockDisposals",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "StockCounts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "StockAdjustments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "Sales",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "RegisterSessionId",
                table: "Sales",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "SaleReturns",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "SaleOrders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "Registers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "PurchaseReturns",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "PurchaseOrders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "ProductBatches",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<decimal>(
                name: "SalePrice",
                table: "ProductBatches",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.Sql(
                "ALTER TABLE [ProductBatches] ADD [RowVersionNew] rowversion; " +
                "ALTER TABLE [ProductBatches] DROP COLUMN [RowVersion]; " +
                "EXEC sp_rename N'[ProductBatches].[RowVersionNew]', N'RowVersion', N'COLUMN';");

            migrationBuilder.AlterColumn<decimal>(
                name: "PurchasePrice",
                table: "ProductBatches",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaximumRetailPrice",
                table: "ProductBatches",
                type: "decimal(18,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CostPrice",
                table: "ProductBatches",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "BatchNumber",
                table: "ProductBatches",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "InventoryStocks",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "GoodsReceipts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_SalePayments_PaymentMethodId",
                table: "SalePayments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_CompanyId_StoreId_ExpiryDate",
                table: "ProductBatches",
                columns: new[] { "CompanyId", "StoreId", "ExpiryDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_CompanyId_StoreId_ProductId",
                table: "ProductBatches",
                columns: new[] { "CompanyId", "StoreId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_CompanyId_StoreId_ProductId_WarehouseId_BatchNumber",
                table: "ProductBatches",
                columns: new[] { "CompanyId", "StoreId", "ProductId", "WarehouseId", "BatchNumber" },
                unique: true,
                filter: "[WarehouseId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductBatches_Companies_CompanyId",
                table: "ProductBatches",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalePayments_PaymentMethods_PaymentMethodId",
                table: "SalePayments",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductBatches_Companies_CompanyId",
                table: "ProductBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_SalePayments_PaymentMethods_PaymentMethodId",
                table: "SalePayments");

            migrationBuilder.DropIndex(
                name: "IX_SalePayments_PaymentMethodId",
                table: "SalePayments");

            migrationBuilder.DropIndex(
                name: "IX_ProductBatches_CompanyId_StoreId_ExpiryDate",
                table: "ProductBatches");

            migrationBuilder.DropIndex(
                name: "IX_ProductBatches_CompanyId_StoreId_ProductId",
                table: "ProductBatches");

            migrationBuilder.DropIndex(
                name: "IX_ProductBatches_CompanyId_StoreId_ProductId_WarehouseId_BatchNumber",
                table: "ProductBatches");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "StockTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "StockDisposals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "StockCounts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "StockAdjustments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "Sales",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RegisterSessionId",
                table: "Sales",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "SaleReturns",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "SaleOrders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "Registers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "PurchaseReturns",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "PurchaseOrders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "ProductBatches",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SalePrice",
                table: "ProductBatches",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.Sql(
                "ALTER TABLE [ProductBatches] ADD [RowVersionOld] varbinary(max); " +
                "UPDATE [ProductBatches] SET [RowVersionOld] = [RowVersion]; " +
                "ALTER TABLE [ProductBatches] DROP COLUMN [RowVersion]; " +
                "EXEC sp_rename N'[ProductBatches].[RowVersionOld]', N'RowVersion', N'COLUMN';");

            migrationBuilder.AlterColumn<decimal>(
                name: "PurchasePrice",
                table: "ProductBatches",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaximumRetailPrice",
                table: "ProductBatches",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CostPrice",
                table: "ProductBatches",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<string>(
                name: "BatchNumber",
                table: "ProductBatches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "InventoryStocks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "GoodsReceipts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
