using Microsoft.EntityFrameworkCore;
using VetCare.profile.domain.model.aggregates;
using VetCare.shared.persistence.EFC.extensions;

namespace VetCare.profile.infrastructure.persistence.EFC.context;

public class ProfileContext : DbContext
{
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    public ProfileContext(DbContextOptions<ProfileContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseSnakeCaseNamingConventions();

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.UserId).IsRequired();
            entity.Property(p => p.Theme).HasMaxLength(20);
            entity.Property(p => p.Language).HasMaxLength(10);
            entity.Property(p => p.ReceiveNotifications).IsRequired();
            entity.Property(p => p.AvatarUrl).HasMaxLength(500);

            entity.HasIndex(p => p.UserId).IsUnique();
        });
    }
}
