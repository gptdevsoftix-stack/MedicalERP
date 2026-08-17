using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Enums;
using MedicalERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Seeding;

public static class DemoSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        var company = await db.Companies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Code == "DEMO", cancellationToken);

        if (company is null)
        {
            company = new Company
            {
                Name = "Demo Pharmacy Inc.",
                Code = "DEMO",
                LegalName = "Demo Pharmacy Inc.",
                Email = "demo@medicalerp.local",
                Phone = "+1-555-0100",
                Address = "100 Main Street",
                City = "Springfield",
                State = "IL",
                Country = "US",
                CurrencyCode = "USD",
                TimeZone = "UTC",
                SubscriptionStatus = SubscriptionStatus.Active,
                SubscriptionStartsAt = DateTime.UtcNow,
                SubscriptionEndsAt = DateTime.UtcNow.AddYears(1)
            };
            db.Companies.Add(company);
            await db.SaveChangesAsync(cancellationToken);
        }

        var store = await db.Stores
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.CompanyId == company.Id && x.Code == "DEMO-STORE", cancellationToken);

        if (store is null)
        {
            store = new Store
            {
                CompanyId = company.Id,
                Name = "Demo Store",
                Code = "DEMO-STORE",
                Email = "store@medicalerp.local",
                Phone = "+1-555-0101",
                Address = "100 Main Street",
                City = "Springfield",
                State = "IL",
                Country = "US",
                CurrencyCode = "USD",
                TimeZone = "UTC",
                IsHeadOffice = true
            };
            db.Stores.Add(store);
            await db.SaveChangesAsync(cancellationToken);
        }

        var warehouse = await db.Warehouses
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.CompanyId == company.Id && x.StoreId == store.Id && x.Code == "DEMO-WH", cancellationToken);

        if (warehouse is null)
        {
            warehouse = new Warehouse
            {
                CompanyId = company.Id,
                StoreId = store.Id,
                Name = "Demo Warehouse",
                Code = "DEMO-WH",
                WarehouseType = WarehouseType.Main,
                Address = "100 Main Street",
                IsDefault = true
            };
            db.Warehouses.Add(warehouse);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
