using IdentityAndAccessManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityAndAccessManagement.Infrastructure.DbContexts;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);

            // Email value object
            b.OwnsOne(u => u.Email, e =>
            {
                e.Property(x => x.Value)
                    .HasColumnName("Email")
                    .IsRequired();
            });

            // FullName value object (multi-column)
            b.OwnsOne(u => u.Name, n =>
            {
                n.Property(x => x.FirstName)
                    .HasColumnName("FirstName")
                    .IsRequired();

                n.Property(x => x.LastName)
                    .HasColumnName("LastName")
                    .IsRequired();
            });

            // PhoneNumber value object (optional)
            b.OwnsOne(u => u.PhoneNumber, p =>
            {
                p.Property(x => x.Value)
                    .HasColumnName("PhoneNumber");
            });

            // CooperativeId value object (optional)
            b.OwnsOne(u => u.CooperativeId, c =>
            {
                c.Property(x => x.Value)
                    .HasColumnName("CooperativeId");
            });

            b.Property(u => u.LastLoginAt);
            b.Property(u => u.HasCompletedOnboarding);
            b.Property(u => u.KeycloakSubjectId).IsRequired();

            // Roles are not persisted directly (will use join-table or claims later)
            b.Ignore(u => u.Roles);
        });
    }
}
