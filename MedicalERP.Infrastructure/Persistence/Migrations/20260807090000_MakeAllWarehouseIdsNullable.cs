using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalERP.Infrastructure.Persistence.Migrations;

public partial class MakeAllWarehouseIdsNullable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        foreach (var table in new[]
        {
            "InventoryStocks",
            "StockAdjustments",
            "StockCounts",
            "StockDisposals",
            "StockTransactions",
            "Registers",
            "GoodsReceipts",
            "PurchaseOrders",
            "PurchaseReturns",
            "Sales",
            "SaleOrders",
            "SaleReturns"
        })
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: table,
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var table in new[]
        {
            "InventoryStocks",
            "StockAdjustments",
            "StockCounts",
            "StockDisposals",
            "StockTransactions",
            "Registers",
            "GoodsReceipts",
            "PurchaseOrders",
            "PurchaseReturns",
            "Sales",
            "SaleOrders",
            "SaleReturns"
        })
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: table,
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
