using FluentValidation;
using Hangfire;
using Hangfire.SqlServer;
using MedicalERP.Application.Validators;
using MedicalERP.Infrastructure;

namespace MedicalERP.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssemblyContaining<CreateCompanyRequestValidator>();
        services.AddInfrastructure(configuration);
        return services;
    }

    public static IServiceCollection AddBackgroundJobServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("BackgroundJobs:Enabled"))
        {
            return services;
        }

        services.AddHangfire(config => config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions { PrepareSchemaIfNecessary = true }));
        services.AddHangfireServer();
        return services;
    }
}

