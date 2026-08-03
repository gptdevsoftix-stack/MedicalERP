using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Domain.Common;
using MedicalERP.Domain.Companies;
using MedicalERP.Domain.Identity;
using MedicalERP.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedicalERP.Infrastructure.Persistence;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly ICompanyContext? _companyContext;
    private readonly ICurrentUserService? _currentUser;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICompanyContext? companyContext = null, ICurrentUserService? currentUser = null) : base(options)
    {
        _companyContext = companyContext;
        _currentUser = currentUser;
    }

    public Guid? CurrentCompanyId => _companyContext?.CompanyId;
    public bool IsPlatformAdmin => _currentUser?.IsPlatformAdmin == true;

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<UserStoreAccess> UserStoreAccesses => Set<UserStoreAccess>();
    public DbSet<UserCompanyAccess> UserCompanyAccesses => Set<UserCompanyAccess>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        foreach (var fk in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) fk.DeleteBehavior = DeleteBehavior.Restrict;
        builder.Entity<Store>().HasQueryFilter(x => CurrentCompanyId == null || x.CompanyId == CurrentCompanyId);
        builder.Entity<Warehouse>().HasQueryFilter(x => CurrentCompanyId == null || x.CompanyId == CurrentCompanyId);
        builder.Entity<UserStoreAccess>().HasQueryFilter(x => CurrentCompanyId == null || x.CompanyId == CurrentCompanyId);
        builder.Entity<UserCompanyAccess>().HasQueryFilter(x => CurrentCompanyId == null || x.CompanyId == CurrentCompanyId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedByUserId = _currentUser?.UserId;
            }
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedByUserId = _currentUser?.UserId;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
