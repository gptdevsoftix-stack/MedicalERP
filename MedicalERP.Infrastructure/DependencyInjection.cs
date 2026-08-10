using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Services;
using MedicalERP.Domain.Interfaces;
using MedicalERP.Infrastructure.Authentication;
using MedicalERP.Infrastructure.Identity;
using MedicalERP.Infrastructure.Persistence;
using MedicalERP.Infrastructure.Repositories;
using MedicalERP.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MedicalERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
        }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        var jwt = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationClaimsPrincipalFactory>();
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Home/AccessDenied";
        });
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "MedicalERP";
            options.DefaultChallengeScheme = "MedicalERP";
            options.DefaultScheme = "MedicalERP";
        })
        .AddPolicyScheme("MedicalERP", "MedicalERP", options =>
        {
            options.ForwardDefaultSelector = context =>
                context.Request.Headers.Authorization.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? JwtBearerDefaults.AuthenticationScheme
                    : IdentityConstants.ApplicationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICompanyContext, CompanyContext>();
        services.AddScoped<IStoreContext, StoreContext>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<ICompanyService, MedicalERP.Application.Services.CompanyService>();
        services.AddScoped<IStoreService, MedicalERP.Application.Services.StoreService>();
        services.AddScoped<IWarehouseService, MedicalERP.Application.Services.WarehouseService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICatalogMasterRepository, CatalogMasterRepository>();
        services.AddScoped<ICatalogMasterService, CatalogMasterService>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductUnitRepository, ProductUnitRepository>();
        services.AddScoped<IProductUnitService, ProductUnitService>();
        services.AddScoped<IProductBarcodeRepository, ProductBarcodeRepository>();
        services.AddScoped<IProductBarcodeService, ProductBarcodeService>();
        services.AddScoped<IStoreProductRepository, StoreProductRepository>();
        services.AddScoped<IStoreProductService, StoreProductService>();
        services.AddScoped<IProductBatchRepository, ProductBatchRepository>();
        services.AddScoped<IProductBatchService, ProductBatchService>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ISaleService, SaleService>();
        return services;
    }
}



