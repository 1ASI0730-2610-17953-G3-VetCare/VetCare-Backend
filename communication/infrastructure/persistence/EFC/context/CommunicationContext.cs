using Microsoft.EntityFrameworkCore;
using VetCare.communication.domain.model.aggregates;

namespace VetCare.communication.infrastructure.persistence.EFC.context;

public class CommunicationContext : DbContext
{
    public CommunicationContext(DbContextOptions<CommunicationContext> options) : base(options)
    {
    }

    public DbSet<DirectMessage> DirectMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DirectMessage>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.ClientId).IsRequired();
            entity.Property(m => m.VeterinarianId).IsRequired();
            entity.Property(m => m.Channel).IsRequired().HasMaxLength(50);
            entity.Property(m => m.Content).IsRequired().HasColumnType("text");
            entity.Property(m => m.SentAt).IsRequired();
            entity.Property(m => m.Status).IsRequired().HasMaxLength(50);
            entity.ToTable("direct_messages");
        });
    }
}
