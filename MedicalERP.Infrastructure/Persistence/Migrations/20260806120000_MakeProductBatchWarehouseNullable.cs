using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalERP.Infrastructure.Persistence.Migrations;

public partial class MakeProductBatchWarehouseNullable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "WarehouseId",
            table: "ProductBatches",
            type: "uniqueidentifier",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "WarehouseId",
            table: "ProductBatches",
            type: "uniqueidentifier",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier",
            oldNullable: true);
    }
}
