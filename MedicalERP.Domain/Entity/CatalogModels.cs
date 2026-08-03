using MedicalERP.Domain.Common;
using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Enums;

namespace MedicalERP.Domain.Catalog;

public sealed class Category : CompanyEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> Children { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
}

public sealed class ProductBrand : CompanyEntity { public string Name { get; set; } = string.Empty; public string Code { get; set; } = string.Empty; }
public sealed class Manufacturer : CompanyEntity { public string Name { get; set; } = string.Empty; public string Code { get; set; } = string.Empty; public string? LicenseNumber { get; set; } }
public sealed class GenericMedicine : CompanyEntity { public string Name { get; set; } = string.Empty; public string? Description { get; set; } }
public sealed class DosageForm : CompanyEntity { public string Name { get; set; } = string.Empty; public string Code { get; set; } = string.Empty; }
public sealed class Strength : CompanyEntity { public string Name { get; set; } = string.Empty; public decimal? Value { get; set; } public string? MeasurementUnit { get; set; } }
public sealed class Unit : CompanyEntity { public string Name { get; set; } = string.Empty; public string Symbol { get; set; } = string.Empty; public bool AllowsDecimal { get; set; } }

public sealed class Product : CompanyEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? ProductBrandId { get; set; }
    public Guid? ManufacturerId { get; set; }
    public Guid? GenericMedicineId { get; set; }
    public Guid? DosageFormId { get; set; }
    public Guid? StrengthId { get; set; }
    public Guid BaseUnitId { get; set; }
    public ProductType ProductType { get; set; }
    public bool IsMedicine { get; set; }
    public bool RequiresPrescription { get; set; }
    public bool IsControlledDrug { get; set; }
    public bool TrackBatch { get; set; } = true;
    public bool TrackExpiry { get; set; } = true;
    public bool AllowDiscount { get; set; } = true;
    public bool AllowNegativeStock { get; set; }
    public string? RegulatoryNumber { get; set; }
    public Category Category { get; set; } = null!;
    public ProductBrand? Brand { get; set; }
    public Manufacturer? Manufacturer { get; set; }
    public GenericMedicine? GenericMedicine { get; set; }
    public DosageForm? DosageForm { get; set; }
    public Strength? Strength { get; set; }
    public Unit BaseUnit { get; set; } = null!;
    public ICollection<ProductUnit> Units { get; set; } = [];
    public ICollection<ProductBarcode> Barcodes { get; set; } = [];
}

public sealed class ProductUnit : CompanyEntity
{
    public Guid ProductId { get; set; }
    public Guid UnitId { get; set; }
    public decimal ConversionFactor { get; set; } = 1;
    public bool IsBaseUnit { get; set; }
    public bool IsPurchaseUnit { get; set; }
    public bool IsSaleUnit { get; set; }
    public Product Product { get; set; } = null!;
    public Unit Unit { get; set; } = null!;
}

public sealed class ProductBarcode : CompanyEntity
{
    public Guid ProductId { get; set; }
    public Guid? ProductUnitId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public Product Product { get; set; } = null!;
    public ProductUnit? ProductUnit { get; set; }
}

public sealed class StoreProduct : StoreEntity
{
    public Guid ProductId { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? WholesalePrice { get; set; }
    public decimal? MinimumSalePrice { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal ReorderQuantity { get; set; }
    public bool IsAvailableForSale { get; set; } = true;
    public Product Product { get; set; } = null!;
    public Store? Store { get; set; }
}

public sealed class ProductPriceHistory : StoreEntity
{
    public Guid ProductId { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public DateTimeOffset EffectiveAt { get; set; }
    public string? Reason { get; set; }
}
