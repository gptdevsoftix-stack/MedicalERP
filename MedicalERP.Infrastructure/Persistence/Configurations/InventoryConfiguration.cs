using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalERP.Infrastructure.Persistence.Configurations;

public sealed class ProductBatchConfiguration : IEntityTypeConfiguration<ProductBatch>
{
    public void Configure(EntityTypeBuilder<ProductBatch> builder)
    {
        builder.ToTable("ProductBatches");
        builder.ConfigureBase();

        builder.Property(x => x.BatchNumber)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(x => x.PurchasePrice)
            .HasColumnType("decimal(18,4)");

        builder.Property(x => x.CostPrice)
            .HasColumnType("decimal(18,4)");

        builder.Property(x => x.SalePrice)
            .HasColumnType("decimal(18,4)");

        builder.Property(x => x.MaximumRetailPrice)
            .HasColumnType("decimal(18,4)");

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.StoreId,
            x.ProductId,
            x.WarehouseId,
            x.BatchNumber
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.StoreId,
            x.ExpiryDate
        });

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.StoreId,
            x.ProductId
        });

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
