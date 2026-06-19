using Microsoft.EntityFrameworkCore;
using VetCare.backoffice.domain.model.aggregates;
using VetCare.shared.persistence.EFC.extensions;

namespace VetCare.backoffice.infrastructure.persistence.EFC.context;

public class BackofficeContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Entry> Entries => Set<Entry>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

    public BackofficeContext(DbContextOptions<BackofficeContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseSnakeCaseNamingConventions();

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Code).IsRequired().HasMaxLength(20);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
            entity.Property(p => p.Category).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            
            var navigation = entity.Metadata.FindNavigation(nameof(Product.Lots));
            navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
            
            entity.HasMany(p => p.Lots)
                  .WithOne()
                  .HasForeignKey(pl => pl.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VetCare.backoffice.Domain.Model.Entities.ProductLot>(entity =>
        {
            entity.ToTable("product_lots");
            entity.HasKey(pl => pl.Id);
            entity.Property(pl => pl.LotNumber).IsRequired().HasMaxLength(50);
            entity.Property(pl => pl.InitialQuantity).IsRequired();
            entity.Property(pl => pl.CurrentQuantity).IsRequired();
            entity.Property(pl => pl.ExpiryDate).IsRequired();
            entity.Property(pl => pl.ProductId).IsRequired();
        });

        modelBuilder.Entity<Entry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).IsRequired().HasMaxLength(150);
            entity.Property(s => s.Contact).HasMaxLength(150);
            entity.Property(s => s.Phone).HasMaxLength(20);
            entity.Property(s => s.Email).HasMaxLength(150);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(t => t.ConsultationBasePrice).HasColumnType("decimal(18,2)");
            entity.Property(t => t.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(t => t.Status).IsRequired().HasMaxLength(50);
            entity.Property(t => t.PaymentMethod).HasMaxLength(50);

            var navigation = entity.Metadata.FindNavigation(nameof(Ticket.Items));
            navigation?.SetPropertyAccessMode(Microsoft.EntityFrameworkCore.PropertyAccessMode.Field);

            entity.HasMany(t => t.Items)
                  .WithOne()
                  .HasForeignKey(ti => ti.TicketId)
                  .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TicketItem>(entity =>
        {
            entity.ToTable("ticket_items");
            entity.HasKey(ti => ti.Id);
            entity.Property(ti => ti.ProductName).IsRequired().HasMaxLength(150);
            entity.Property(ti => ti.UnitPrice).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(it => it.Id);
            entity.Property(it => it.ProductId).IsRequired();
            entity.Property(it => it.Type).IsRequired().HasMaxLength(50);
            entity.Property(it => it.Reason).HasMaxLength(255);
            entity.Property(it => it.ResponsibleUser).HasMaxLength(100);
            entity.ToTable("inventory_transactions");
        });
    }
}
