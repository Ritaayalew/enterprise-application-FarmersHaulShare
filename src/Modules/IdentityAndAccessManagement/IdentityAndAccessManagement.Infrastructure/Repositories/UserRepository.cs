using IdentityAndAccessManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityAndAccessManagement.Infrastructure.DbContexts;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);

            b.OwnsOne(u => u.Email, e => e.Property(x => x.Value).HasColumnName("Email").IsRequired());
            b.OwnsOne(u => u.Name, n => n.Property(x => x.Value).HasColumnName("FullName").IsRequired());
            b.OwnsOne(u => u.PhoneNumber, p => p.Property(x => x.Value).HasColumnName("PhoneNumber"));
            b.OwnsOne(u => u.CooperativeId, c => c.Property(x => x.Value).HasColumnName("CooperativeId"));

            b.Ignore(u => u.Roles); // Roles can be stored in a join table if needed later
            b.Property(u => u.HasCompletedOnboarding);
            b.Property(u => u.LastLoginAt);
            b.Property(u => u.KeycloakSubjectId).IsRequired();
        });
    }
}
