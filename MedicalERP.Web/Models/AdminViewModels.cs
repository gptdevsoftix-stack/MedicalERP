using MedicalERP.Application.Companies.Dtos;
using MedicalERP.Application.Identity.Dtos;
using System.ComponentModel.DataAnnotations;
using MedicalERP.Application.Stores.Dtos;
using MedicalERP.Application.Warehouses.Dtos;
using MedicalERP.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalERP.Web.Models;

public sealed class CompanyFormViewModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string Country { get; set; } = "US";
    public string? TaxNumber { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string TimeZone { get; set; } = "UTC";
    public bool IsActive { get; set; } = true;
    public CreateCompanyRequest ToCreate() => new(Name, Code, LegalName, Email, Phone, Address, City, State, Country, TaxNumber, CurrencyCode, TimeZone);
    public UpdateCompanyRequest ToUpdate() => new(Name, LegalName, Email, Phone, Address, City, State, Country, TaxNumber, CurrencyCode, TimeZone);
    public static CompanyFormViewModel From(CompanyDto x) => new() { Id = x.Id, Name = x.Name, Code = x.Code, LegalName = x.LegalName, Email = x.Email, Phone = x.Phone, Address = x.Address, City = x.City, State = x.State, Country = x.Country, TaxNumber = x.TaxNumber, CurrencyCode = x.CurrencyCode, TimeZone = x.TimeZone, IsActive = x.IsActive };
}

public sealed class StoreFormViewModel
{
    public Guid? Id { get; set; }
    public Guid? CompanyContextId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string Country { get; set; } = "US";
    public string? TaxNumber { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string TimeZone { get; set; } = "UTC";
    public bool IsHeadOffice { get; set; }
    public bool IsActive { get; set; } = true;
    public IReadOnlyList<SelectListItem> Companies { get; set; } = [];
    public CreateStoreRequest ToCreate() => new(Name, Code, Email, Phone, Address, City, State, Country, TaxNumber, CurrencyCode, TimeZone, IsHeadOffice);
    public UpdateStoreRequest ToUpdate() => new(Name, Email, Phone, Address, City, State, Country, TaxNumber, CurrencyCode, TimeZone, IsHeadOffice);
    public static StoreFormViewModel From(StoreDto x) => new() { Id = x.Id, CompanyContextId = x.CompanyId, Name = x.Name, Code = x.Code, Email = x.Email, Phone = x.Phone, Address = x.Address, City = x.City, State = x.State, Country = x.Country, TaxNumber = x.TaxNumber, CurrencyCode = x.CurrencyCode, TimeZone = x.TimeZone, IsHeadOffice = x.IsHeadOffice, IsActive = x.IsActive };
}

public sealed class WarehouseFormViewModel
{
    public Guid? Id { get; set; }
    public Guid? CompanyContextId { get; set; }
    public Guid StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public WarehouseType WarehouseType { get; set; } = WarehouseType.Main;
    public string? Address { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public IReadOnlyList<SelectListItem> Stores { get; set; } = [];
    public CreateWarehouseRequest ToCreate() => new(StoreId, Name, Code, WarehouseType, Address, IsDefault);
    public UpdateWarehouseRequest ToUpdate() => new(Name, WarehouseType, Address, IsDefault);
    public static WarehouseFormViewModel From(WarehouseDto x) => new() { Id = x.Id, CompanyContextId = x.CompanyId, StoreId = x.StoreId, Name = x.Name, Code = x.Code, WarehouseType = x.WarehouseType, Address = x.Address, IsDefault = x.IsDefault, IsActive = x.IsActive };
}

public sealed class UserFormViewModel
{
    public Guid? Id { get; set; }
    public Guid? CompanyContextId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<string> SelectedRoles { get; set; } = [];
    public List<Guid> SelectedStoreIds { get; set; } = [];
    public IReadOnlyList<SelectListItem> Companies { get; set; } = [];
    public IReadOnlyList<SelectListItem> Roles { get; set; } = [];
    public IReadOnlyList<SelectListItem> Stores { get; set; } = [];
    public CreateUserRequest ToCreate() => new(Email, Password, FirstName, LastName, SelectedRoles, SelectedStoreIds);
    public UpdateUserRequest ToUpdate() => new(FirstName, LastName, IsActive);
    public static UserFormViewModel From(UserDto x) => new() { Id = x.Id, Email = x.Email, FirstName = x.FirstName, LastName = x.LastName, IsActive = x.IsActive, SelectedRoles = x.Roles.ToList(), SelectedStoreIds = x.StoreIds.ToList() };
}

public sealed class RoleFormViewModel
{
    public Guid? Id { get; set; }
    public Guid? CompanyContextId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public List<string> SelectedPermissions { get; set; } = [];
    public IReadOnlyList<SelectListItem> Companies { get; set; } = [];
    public IReadOnlyList<SelectListItem> Permissions { get; set; } = [];
    public CreateRoleRequest ToCreate() => new(Name, Description);
    public UpdateRoleRequest ToUpdate() => new(Description, IsActive);
    public static RoleFormViewModel From(RoleDto x) => new() { Id = x.Id, Name = x.Name, Description = x.Description, IsActive = x.IsActive, SelectedPermissions = x.Permissions.ToList() };
}

public sealed class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}

public sealed class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
}


