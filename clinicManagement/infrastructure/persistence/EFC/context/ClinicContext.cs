using Microsoft.EntityFrameworkCore;
using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.shared.persistence.EFC.extensions;

namespace VetCare.clinicManagement.infrastructure.persistence.EFC.context;

public class ClinicContext : DbContext
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Consultation> Consultations => Set<Consultation>();
    public DbSet<VaccineRecord> VaccineRecords => Set<VaccineRecord>();
    public DbSet<HospitalizationAdmission> HospitalizationAdmissions => Set<HospitalizationAdmission>();
    public DbSet<HospitalizationTask> HospitalizationTasks => Set<HospitalizationTask>();

    public ClinicContext(DbContextOptions<ClinicContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseSnakeCaseNamingConventions();

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedOnAdd();
            
            entity.Property(c => c.Code).IsRequired().HasMaxLength(20);
            entity.Property(c => c.FullName).IsRequired().HasMaxLength(150);
            entity.Property(c => c.DocumentId).IsRequired().HasMaxLength(20);
            entity.Property(c => c.Phone).HasMaxLength(20);
            entity.Property(c => c.Email).HasMaxLength(100);
            entity.Property(c => c.Status).HasMaxLength(20);
            
            entity.ToTable("clients");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            
            entity.Property(p => p.Code).IsRequired().HasMaxLength(20);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Status).HasMaxLength(20);
            
            entity.OwnsOne(p => p.Species, s => {
                s.ToTable("patients");
                s.Property(x => x.Name).HasColumnName("species").IsRequired().HasMaxLength(50);
            });
            
            entity.OwnsOne(p => p.Weight, w => {
                w.ToTable("patients");
                w.Property(x => x.Value).HasColumnName("weight_value").IsRequired();
                w.Property(x => x.Unit).HasColumnName("weight_unit").IsRequired().HasMaxLength(10);
            });

            // Note: In DDD we map OwnerId without navigation properties if they cross aggregates.
            // OwnerId is just an int column.
            entity.Property(p => p.OwnerId).IsRequired();

            entity.ToTable("patients");
        });

        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedOnAdd();
            
            entity.Property(c => c.DoctorName).IsRequired().HasMaxLength(150);
            entity.Property(c => c.Type).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Status).IsRequired().HasMaxLength(50);
            
            entity.Property(c => c.Subjective).HasColumnType("text");
            entity.Property(c => c.Objective).HasColumnType("text");
            entity.Property(c => c.Analysis).HasColumnType("text");
            entity.Property(c => c.Plan).HasColumnType("text");
            
            entity.Property(c => c.Temperature).HasColumnType("decimal(5,2)");
            entity.Property(c => c.Weight).HasColumnType("decimal(8,2)");
            
            entity.Property(c => c.PatientId).IsRequired();

            var navigation = entity.Metadata.FindNavigation(nameof(Consultation.Items));
            navigation?.SetPropertyAccessMode(Microsoft.EntityFrameworkCore.PropertyAccessMode.Field);

            entity.HasMany(c => c.Items)
                  .WithOne()
                  .HasForeignKey(ci => ci.ConsultationId)
                  .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);

            entity.ToTable("consultations");
        });

        modelBuilder.Entity<VetCare.clinicManagement.domain.model.entities.ConsultationItem>(entity =>
        {
            entity.HasKey(ci => ci.Id);
            entity.Property(ci => ci.ProductId).IsRequired();
            entity.Property(ci => ci.ProductName).IsRequired().HasMaxLength(150);
            entity.Property(ci => ci.Quantity).IsRequired();
            entity.Property(ci => ci.UnitPrice).HasColumnType("decimal(18,2)");
            entity.ToTable("consultation_items");
        });

        modelBuilder.Entity<VaccineRecord>(entity =>
        {
            entity.HasKey(vr => vr.Id);
            entity.Property(vr => vr.Id).ValueGeneratedOnAdd();

            entity.Property(vr => vr.VaccineName).IsRequired().HasMaxLength(100);
            entity.Property(vr => vr.Disease).IsRequired().HasMaxLength(150);
            entity.Property(vr => vr.LastApplication).IsRequired();
            entity.Property(vr => vr.NextDose).IsRequired();
            entity.Property(vr => vr.PatientId).IsRequired();

            entity.ToTable("vaccine_records");
        });

        modelBuilder.Entity<HospitalizationAdmission>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.PatientId).IsRequired();
            entity.Property(a => a.Status).IsRequired().HasMaxLength(20);
            entity.Property(a => a.Diagnosis).IsRequired().HasMaxLength(200);
            entity.Property(a => a.AdmissionDate).IsRequired();
            entity.Property(a => a.TreatmentsJson).IsRequired().HasColumnType("text");
            entity.Ignore(a => a.IsActive);
            entity.ToTable("hospitalization_admissions");
        });

        modelBuilder.Entity<HospitalizationTask>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).ValueGeneratedOnAdd();
            entity.Property(t => t.PatientId).IsRequired();
            entity.Property(t => t.Status).IsRequired().HasMaxLength(20);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(150);
            entity.Property(t => t.Description).IsRequired().HasMaxLength(300);
            entity.Property(t => t.TaskDate).IsRequired();
            entity.Property(t => t.TaskTime).IsRequired().HasMaxLength(5);
            entity.ToTable("hospitalization_tasks");
        });
    }
}
