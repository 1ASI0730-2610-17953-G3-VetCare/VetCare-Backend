using Microsoft.EntityFrameworkCore;
using VetCare.iam.domain.model.aggregates;
using VetCare.shared.persistence.EFC.extensions;

namespace VetCare.iam.infrastructure.persistence.EFC.context;

public class IamContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public IamContext(DbContextOptions<IamContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.UseSnakeCaseNamingConventions();
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedOnAdd();
            
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).IsRequired().HasMaxLength(50);
            
            entity.OwnsOne(u => u.Email, email =>
            {
                email.ToTable("users");
                email.Property(e => e.Address)
                     .HasColumnName("email")
                     .IsRequired()
                     .HasMaxLength(150);
            });
            
            entity.ToTable("users");
        });
    }
}
