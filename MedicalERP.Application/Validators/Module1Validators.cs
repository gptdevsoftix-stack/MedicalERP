using FluentValidation;
using MedicalERP.Application.Companies.Dtos;
using MedicalERP.Application.Identity.Dtos;
using MedicalERP.Application.Stores.Dtos;
using MedicalERP.Application.Warehouses.Dtos;

namespace MedicalERP.Application.Validators;

public sealed class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyRequestValidator() 
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3); RuleFor(x => x.TimeZone).NotEmpty().MaximumLength(100); }
}
public sealed class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyRequestValidator() { RuleFor(x => x.Name).NotEmpty().MaximumLength(150); RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)); RuleFor(x => x.CurrencyCode).NotEmpty().Length(3); RuleFor(x => x.TimeZone).NotEmpty().MaximumLength(100); }
}
public sealed class CreateStoreRequestValidator : AbstractValidator<CreateStoreRequest>
{
    public CreateStoreRequestValidator() { RuleFor(x => x.Name).NotEmpty().MaximumLength(150); RuleFor(x => x.Code).NotEmpty().MaximumLength(40); RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)); RuleFor(x => x.CurrencyCode).NotEmpty().Length(3); }
}
public sealed class CreateWarehouseRequestValidator : AbstractValidator<CreateWarehouseRequest>
{
    public CreateWarehouseRequestValidator() { RuleFor(x => x.StoreId).NotEmpty(); RuleFor(x => x.Name).NotEmpty().MaximumLength(150); RuleFor(x => x.Code).NotEmpty().MaximumLength(40); }
}
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator() { RuleFor(x => x.Email).NotEmpty().EmailAddress(); RuleFor(x => x.Password).NotEmpty(); }
}
public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator() { RuleFor(x => x.Email).NotEmpty().EmailAddress(); RuleFor(x => x.Password).NotEmpty().MinimumLength(8); RuleFor(x => x.FirstName).NotEmpty().MaximumLength(80); RuleFor(x => x.LastName).NotEmpty().MaximumLength(80); }
}
