namespace MedicalERP.Application.Permissions;

public static class PermissionClaimTypes { public const string Permission = "Permission"; }

public static class Permissions
{
    public static readonly string[] All =
    [
        Companies.View, Companies.Create, Companies.Update, Companies.Activate, Companies.Suspend,
        Stores.View, Stores.Create, Stores.Update, Stores.Activate, Stores.AssignUsers,
        Users.View, Users.Create, Users.Update, Users.Delete, Users.AssignRoles, Users.AssignStores,
        Roles.View, Roles.Create, Roles.Update, Roles.Delete, Roles.ManagePermissions,
        Categories.View, Categories.Create, Categories.Update, Categories.Delete,
        Products.View, Products.Create, Products.Update, Products.Delete, Products.ManagePrices,
        Inventory.View, Inventory.Adjust, Inventory.Count, Inventory.Dispose, Inventory.ViewCost, Inventory.ViewExpiry,
        Suppliers.View, Suppliers.Create, Suppliers.Update, Suppliers.Delete,
        Purchases.View, Purchases.Create, Purchases.Update, Purchases.Approve, Purchases.Receive, Purchases.Return,
        Sales.View, Sales.Create, Sales.Hold, Sales.Discount, Sales.Void, Sales.Return, Sales.Refund,
        Customers.View, Customers.Create, Customers.Update,
        Prescriptions.View, Prescriptions.Create, Prescriptions.Approve,
        Expenses.View, Expenses.Create, Expenses.Approve,
        Registers.View, Registers.Open, Registers.Close, Registers.ViewClosing,
        Reports.View, Reports.ViewProfit, Reports.ViewCost, Reports.Export,
        AuditLogs.View, Settings.Manage
    ];
    public static class Companies { public const string View = "Companies.View"; public const string Create = "Companies.Create"; public const string Update = "Companies.Update"; public const string Activate = "Companies.Activate"; public const string Suspend = "Companies.Suspend"; }
    public static class Stores { public const string View = "Stores.View"; public const string Create = "Stores.Create"; public const string Update = "Stores.Update"; public const string Activate = "Stores.Activate"; public const string AssignUsers = "Stores.AssignUsers"; }
    public static class Users { public const string View = "Users.View"; public const string Create = "Users.Create"; public const string Update = "Users.Update"; public const string Delete = "Users.Delete"; public const string AssignRoles = "Users.AssignRoles"; public const string AssignStores = "Users.AssignStores"; }
    public static class Roles { public const string View = "Roles.View"; public const string Create = "Roles.Create"; public const string Update = "Roles.Update"; public const string Delete = "Roles.Delete"; public const string ManagePermissions = "Roles.ManagePermissions"; }
    public static class Categories { public const string View = "Categories.View"; public const string Create = "Categories.Create"; public const string Update = "Categories.Update"; public const string Delete = "Categories.Delete"; }
    public static class Products { public const string View = "Products.View"; public const string Create = "Products.Create"; public const string Update = "Products.Update"; public const string Delete = "Products.Delete"; public const string ManagePrices = "Products.ManagePrices"; }
    public static class Inventory { public const string View = "Inventory.View"; public const string Adjust = "Inventory.Adjust"; public const string Count = "Inventory.Count"; public const string Dispose = "Inventory.Dispose"; public const string ViewCost = "Inventory.ViewCost"; public const string ViewExpiry = "Inventory.ViewExpiry"; }
    public static class Suppliers { public const string View = "Suppliers.View"; public const string Create = "Suppliers.Create"; public const string Update = "Suppliers.Update"; public const string Delete = "Suppliers.Delete"; }
    public static class Purchases { public const string View = "Purchases.View"; public const string Create = "Purchases.Create"; public const string Update = "Purchases.Update"; public const string Approve = "Purchases.Approve"; public const string Receive = "Purchases.Receive"; public const string Return = "Purchases.Return"; }
    public static class Sales { public const string View = "Sales.View"; public const string Create = "Sales.Create"; public const string Hold = "Sales.Hold"; public const string Discount = "Sales.Discount"; public const string Void = "Sales.Void"; public const string Return = "Sales.Return"; public const string Refund = "Sales.Refund"; }
    public static class Customers { public const string View = "Customers.View"; public const string Create = "Customers.Create"; public const string Update = "Customers.Update"; }
    public static class Prescriptions { public const string View = "Prescriptions.View"; public const string Create = "Prescriptions.Create"; public const string Approve = "Prescriptions.Approve"; }
    public static class Expenses { public const string View = "Expenses.View"; public const string Create = "Expenses.Create"; public const string Approve = "Expenses.Approve"; }
    public static class Registers { public const string View = "Registers.View"; public const string Open = "Registers.Open"; public const string Close = "Registers.Close"; public const string ViewClosing = "Registers.ViewClosing"; }
    public static class Reports { public const string View = "Reports.View"; public const string ViewProfit = "Reports.ViewProfit"; public const string ViewCost = "Reports.ViewCost"; public const string Export = "Reports.Export"; }
    public static class AuditLogs { public const string View = "AuditLogs.View"; }
    public static class Settings { public const string Manage = "Settings.Manage"; }
}
