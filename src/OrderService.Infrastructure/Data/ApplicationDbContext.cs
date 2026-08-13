using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


 protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Order>()
        .Property(x => x.Version)
        .IsRowVersion();

 modelBuilder.Entity<User>().HasData(
    new User
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        FirstName = "System",
        LastName = "Admin",
        Email = "admin@orderservice.com",
        PasswordHash = "Admin123!",
        UserRole = Domain.Enums.UserRole.Admin,
        UserStatus = Domain.Enums.UserStatus.Active,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    }
);
}
    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Order>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.MarkAsUpdated();
            }
        }
        

        return await base.SaveChangesAsync(cancellationToken);
    }
}