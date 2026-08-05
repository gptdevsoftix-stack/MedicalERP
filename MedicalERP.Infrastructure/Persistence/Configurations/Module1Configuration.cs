using MedicalERP.Domain.Catalog;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Identity;
using MedicalERP.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalERP.Infrastructure.Persistence.Configurations;

internal static class ConfigurationHelpers
{
    public static void ConfigureBase<T>(this EntityTypeBuilder<T> b) where T : BaseEntity
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.RowVersion).IsRowVersion();
        b.Property(x => x.CreatedAt).IsRequired();
    }
}

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> b)
    {
        b.ConfigureBase();
        b.HasIndex(x => x.Code).IsUnique();
        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.Property(x => x.Code).IsRequired().HasMaxLength(40);
        b.Property(x => x.LegalName).HasMaxLength(200);
        b.Property(x => x.Email).HasMaxLength(256);
        b.Property(x => x.Phone).HasMaxLength(40);
        b.Property(x => x.Country).HasMaxLength(80);
        b.Property(x => x.CurrencyCode).HasMaxLength(3);
        b.Property(x => x.TimeZone).HasMaxLength(100);
    }
}

public sealed class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> b)
    {
        b.ConfigureBase();
        b.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.Property(x => x.Code).IsRequired().HasMaxLength(40);
        b.Property(x => x.Email).HasMaxLength(256);
        b.Property(x => x.CurrencyCode).HasMaxLength(3);
        b.Property(x => x.TimeZone).HasMaxLength(100);
        b.HasOne(x => x.Company).WithMany(x => x.Stores).HasForeignKey(x => x.CompanyId);
    }
}

public sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> b)
    {
        b.ConfigureBase();
        b.HasIndex(x => new { x.CompanyId, x.StoreId, x.Code }).IsUnique();
        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.Property(x => x.Code).IsRequired().HasMaxLength(40);
        b.HasOne(x => x.Company).WithMany(x => x.Warehouses).HasForeignKey(x => x.CompanyId);
        b.HasOne(x => x.Store).WithMany(x => x.Warehouses).HasForeignKey(x => x.StoreId);
    }
}

public sealed class UserStoreAccessConfiguration : IEntityTypeConfiguration<UserStoreAccess>
{
    public void Configure(EntityTypeBuilder<UserStoreAccess> b)
    {
        b.ConfigureBase();
        b.HasIndex(x => new { x.UserId, x.StoreId }).IsUnique();
        b.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        b.HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
    }
}

public sealed class UserCompanyAccessConfiguration : IEntityTypeConfiguration<UserCompanyAccess>
{
    public void Configure(EntityTypeBuilder<UserCompanyAccess> b)
    {
        b.ConfigureBase();
        b.HasIndex(x => new { x.UserId, x.CompanyId }).IsUnique();
        b.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
    }
}

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ConfigureBase();
        b.HasIndex(x => x.TokenHash).IsUnique();
        b.HasIndex(x => x.UserId);
        b.Property(x => x.TokenHash).IsRequired().HasMaxLength(128);
        b.Property(x => x.CreatedByIp).HasMaxLength(64);
        b.Property(x => x.RevokedByIp).HasMaxLength(64);
        b.Property(x => x.ReasonRevoked).HasMaxLength(250);
    }
}

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> b)
    {
        b.Property(x => x.FirstName).HasMaxLength(80);
        b.Property(x => x.LastName).HasMaxLength(80);
        b.HasIndex(x => x.CompanyId);
    }
}

public sealed class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> b)
    {
        b.Property(x => x.Description).HasMaxLength(250);
        b.HasIndex(x => new { x.CompanyId, x.NormalizedName }).IsUnique();
    }
}

public sealed class CategoryConfiguration
    : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.Code
        }).IsUnique();

        builder.HasOne(x => x.ParentCategory)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ProductBrandConfiguration
    : IEntityTypeConfiguration<ProductBrand>
{
    public void Configure(EntityTypeBuilder<ProductBrand> builder)
    {
        builder.ToTable("ProductBrands");
        builder.ConfigureBase();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        builder.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ManufacturerConfiguration
    : IEntityTypeConfiguration<Manufacturer>
{
    public void Configure(EntityTypeBuilder<Manufacturer> builder)
    {
        builder.ToTable("Manufacturers");
        builder.ConfigureBase();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.LicenseNumber).HasMaxLength(80);
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        builder.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class GenericMedicineConfiguration
    : IEntityTypeConfiguration<GenericMedicine>
{
    public void Configure(EntityTypeBuilder<GenericMedicine> builder)
    {
        builder.ToTable("GenericMedicines");
        builder.ConfigureBase();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class DosageFormConfiguration
    : IEntityTypeConfiguration<DosageForm>
{
    public void Configure(EntityTypeBuilder<DosageForm> builder)
    {
        builder.ToTable("DosageForms");
        builder.ConfigureBase();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        builder.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class StrengthConfiguration
    : IEntityTypeConfiguration<Strength>
{
    public void Configure(EntityTypeBuilder<Strength> builder)
    {
        builder.ToTable("Strengths");
        builder.ConfigureBase();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Value).HasColumnType("decimal(18,4)");
        builder.Property(x => x.MeasurementUnit).HasMaxLength(40);
        builder.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class UnitConfiguration
    : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");
        builder.ConfigureBase();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Symbol).HasMaxLength(20).IsRequired();
        builder.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
    }
}
