using System.Security.Claims;
using MedicalERP.Application.Permissions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace MedicalERP.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static readonly string[] Roles = ["PlatformSuperAdmin", "Admin", "CompanyOwner", "CompanyAdmin", "RegionalManager", "StoreManager", "Pharmacist", "Cashier", "InventoryManager", "PurchaseManager", "Accountant", "Auditor"];

    private static readonly IReadOnlyDictionary<string, string[]> RolePermissions = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        ["PlatformSuperAdmin"] = Permissions.All,
        ["Admin"] = Permissions.All,
        ["CompanyOwner"] = Permissions.All,
        ["CompanyAdmin"] = Permissions.All,
        ["RegionalManager"] =
        [
            Permissions.Stores.View,
            Permissions.Users.View,
            Permissions.Categories.View,
            Permissions.Products.View,
            Permissions.Products.ManagePrices,
            Permissions.Inventory.View,
            Permissions.Inventory.ViewCost,
            Permissions.Inventory.ViewExpiry,
            Permissions.Suppliers.View,
            Permissions.Purchases.View,
            Permissions.Sales.View,
            Permissions.Customers.View,
            Permissions.Prescriptions.View,
            Permissions.Expenses.View,
            Permissions.Registers.View,
            Permissions.Registers.ViewClosing,
            Permissions.Reports.View,
            Permissions.Reports.ViewProfit,
            Permissions.Reports.ViewCost,
            Permissions.Reports.Export,
            Permissions.AuditLogs.View
        ],
        ["StoreManager"] =
        [
            Permissions.Stores.View,
            Permissions.Users.View,
            Permissions.Users.AssignStores,
            Permissions.Categories.View,
            Permissions.Products.View,
            Permissions.Products.ManagePrices,
            Permissions.Inventory.View,
            Permissions.Inventory.Adjust,
            Permissions.Inventory.Count,
            Permissions.Inventory.Dispose,
            Permissions.Inventory.ViewCost,
            Permissions.Inventory.ViewExpiry,
            Permissions.Suppliers.View,
            Permissions.Purchases.View,
            Permissions.Purchases.Create,
            Permissions.Purchases.Update,
            Permissions.Purchases.Receive,
            Permissions.Sales.View,
            Permissions.Sales.Create,
            Permissions.Sales.Hold,
            Permissions.Sales.Discount,
            Permissions.Sales.Void,
            Permissions.Sales.Return,
            Permissions.Sales.Refund,
            Permissions.Customers.View,
            Permissions.Customers.Create,
            Permissions.Customers.Update,
            Permissions.Prescriptions.View,
            Permissions.Prescriptions.Create,
            Permissions.Prescriptions.Approve,
            Permissions.Expenses.View,
            Permissions.Expenses.Create,
            Permissions.Registers.View,
            Permissions.Registers.Open,
            Permissions.Registers.Close,
            Permissions.Registers.ViewClosing,
            Permissions.Reports.View,
            Permissions.Reports.ViewProfit,
            Permissions.Reports.ViewCost,
            Permissions.Reports.Export
        ],
        ["Pharmacist"] =
        [
            Permissions.Products.View,
            Permissions.Inventory.View,
            Permissions.Inventory.ViewExpiry,
            Permissions.Sales.View,
            Permissions.Sales.Create,
            Permissions.Sales.Hold,
            Permissions.Sales.Return,
            Permissions.Customers.View,
            Permissions.Customers.Create,
            Permissions.Customers.Update,
            Permissions.Prescriptions.View,
            Permissions.Prescriptions.Create,
            Permissions.Prescriptions.Approve,
            Permissions.Registers.View
        ],
        ["Cashier"] =
        [
            Permissions.Products.View,
            Permissions.Sales.View,
            Permissions.Sales.Create,
            Permissions.Sales.Hold,
            Permissions.Sales.Discount,
            Permissions.Sales.Return,
            Permissions.Customers.View,
            Permissions.Customers.Create,
            Permissions.Prescriptions.View,
            Permissions.Registers.View,
            Permissions.Registers.Open,
            Permissions.Registers.Close
        ],
        ["InventoryManager"] =
        [
            Permissions.Categories.View,
            Permissions.Categories.Create,
            Permissions.Categories.Update,
            Permissions.Products.View,
            Permissions.Products.Create,
            Permissions.Products.Update,
            Permissions.Inventory.View,
            Permissions.Inventory.Adjust,
            Permissions.Inventory.Count,
            Permissions.Inventory.Dispose,
            Permissions.Inventory.ViewCost,
            Permissions.Inventory.ViewExpiry,
            Permissions.Suppliers.View,
            Permissions.Purchases.View,
            Permissions.Purchases.Receive,
            Permissions.Reports.View,
            Permissions.Reports.ViewCost
        ],
        ["PurchaseManager"] =
        [
            Permissions.Products.View,
            Permissions.Inventory.View,
            Permissions.Inventory.ViewCost,
            Permissions.Suppliers.View,
            Permissions.Suppliers.Create,
            Permissions.Suppliers.Update,
            Permissions.Purchases.View,
            Permissions.Purchases.Create,
            Permissions.Purchases.Update,
            Permissions.Purchases.Approve,
            Permissions.Purchases.Receive,
            Permissions.Purchases.Return,
            Permissions.Reports.View,
            Permissions.Reports.ViewCost
        ],
        ["Accountant"] =
        [
            Permissions.Suppliers.View,
            Permissions.Purchases.View,
            Permissions.Sales.View,
            Permissions.Expenses.View,
            Permissions.Expenses.Create,
            Permissions.Expenses.Approve,
            Permissions.Registers.ViewClosing,
            Permissions.Reports.View,
            Permissions.Reports.ViewProfit,
            Permissions.Reports.ViewCost,
            Permissions.Reports.Export
        ],
        ["Auditor"] =
        [
            Permissions.Companies.View,
            Permissions.Stores.View,
            Permissions.Users.View,
            Permissions.Roles.View,
            Permissions.Categories.View,
            Permissions.Products.View,
            Permissions.Inventory.View,
            Permissions.Inventory.ViewCost,
            Permissions.Inventory.ViewExpiry,
            Permissions.Suppliers.View,
            Permissions.Purchases.View,
            Permissions.Sales.View,
            Permissions.Customers.View,
            Permissions.Prescriptions.View,
            Permissions.Expenses.View,
            Permissions.Registers.View,
            Permissions.Registers.ViewClosing,
            Permissions.Reports.View,
            Permissions.Reports.ViewProfit,
            Permissions.Reports.ViewCost,
            Permissions.Reports.Export,
            Permissions.AuditLogs.View
        ]
    };

    public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        foreach (var roleName in Roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                role = new ApplicationRole { Name = roleName, Description = roleName, IsSystemRole = true, IsActive = true };
                var created = await roleManager.CreateAsync(role);
                if (!created.Succeeded) throw new InvalidOperationException(string.Join("; ", created.Errors.Select(x => x.Description)));
            }
            else
            {
                var needsUpdate = false;
                if (!role.IsSystemRole) { role.IsSystemRole = true; needsUpdate = true; }
                if (!role.IsActive) { role.IsActive = true; needsUpdate = true; }
                if (string.IsNullOrWhiteSpace(role.Description)) { role.Description = roleName; needsUpdate = true; }
                if (needsUpdate)
                {
                    var updated = await roleManager.UpdateAsync(role);
                    if (!updated.Succeeded) throw new InvalidOperationException(string.Join("; ", updated.Errors.Select(x => x.Description)));
                }
            }

            await SeedRolePermissionClaimsAsync(roleManager, role, RolePermissions.GetValueOrDefault(roleName, []));
        }

        var email = configuration["Seed:PlatformAdmin:Email"];
        var password = configuration["Seed:PlatformAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = configuration["Seed:PlatformAdmin:FirstName"] ?? "Platform",
                LastName = configuration["Seed:PlatformAdmin:LastName"] ?? "Admin",
                IsPlatformAdmin = true,
                IsActive = true,
                CompanyId = null,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }
        else
        {
            var needsUpdate = false;
            if (!admin.IsPlatformAdmin) { admin.IsPlatformAdmin = true; needsUpdate = true; }
            if (!admin.IsActive) { admin.IsActive = true; needsUpdate = true; }
            if (!admin.EmailConfirmed) { admin.EmailConfirmed = true; needsUpdate = true; }
            if (admin.CompanyId is not null) { admin.CompanyId = null; needsUpdate = true; }
            if (needsUpdate)
            {
                var updated = await userManager.UpdateAsync(admin);
                if (!updated.Succeeded) throw new InvalidOperationException(string.Join("; ", updated.Errors.Select(x => x.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(admin, "PlatformSuperAdmin"))
        {
            var result = await userManager.AddToRoleAsync(admin, "PlatformSuperAdmin");
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }
    }

    private static async Task SeedRolePermissionClaimsAsync(RoleManager<ApplicationRole> roleManager, ApplicationRole role, IEnumerable<string> permissions)
    {
        var existing = await roleManager.GetClaimsAsync(role);
        var existingPermissions = existing
            .Where(x => x.Type == PermissionClaimTypes.Permission)
            .Select(x => x.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var permission in permissions.Distinct(StringComparer.OrdinalIgnoreCase).Except(existingPermissions, StringComparer.OrdinalIgnoreCase))
        {
            var result = await roleManager.AddClaimAsync(role, new Claim(PermissionClaimTypes.Permission, permission));
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }
    }
}
