using Microsoft.EntityFrameworkCore;
using VetCare.scheduling.domain.model.aggregates;
using VetCare.shared.persistence.EFC.extensions;
using VetCare.shared.domain;
using VetCare.shared.persistence.EFC.extensions.eventDispatcher;

namespace VetCare.scheduling.infrastructure.persistence.EFC.context;

public class SchedulingContext : DbContext
{
    public DbSet<Appointment> Appointments => Set<Appointment>();
    private readonly IDomainEventDispatcher _dispatcher;

    public SchedulingContext(DbContextOptions<SchedulingContext> options, IDomainEventDispatcher dispatcher) : base(options) 
    {
        _dispatcher = dispatcher;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        
        var entitiesWithEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToArray();

        await _dispatcher.DispatchEventsAsync(entitiesWithEvents);
        
        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseSnakeCaseNamingConventions();

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            
            entity.Property(a => a.PatientId).IsRequired();
            entity.Property(a => a.ClientId).IsRequired();
            entity.Property(a => a.Date).IsRequired();
            entity.Property(a => a.Status).IsRequired().HasMaxLength(50);
            entity.Property(a => a.Type).IsRequired().HasMaxLength(50);
            
            entity.OwnsOne(a => a.TimeSlot, ts => {
                ts.ToTable("appointments");
                ts.Property(x => x.StartTime).HasColumnName("time_start").IsRequired();
                ts.Property(x => x.EndTime).HasColumnName("time_end").IsRequired();
            });

            entity.ToTable("appointments");
        });
    }
}
