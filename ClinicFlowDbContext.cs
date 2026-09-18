using ClinicFlowDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClinicFlowDemo;

public class ClinicFlowDbContext : DbContext
{
    private readonly string _connectionString;

    public ClinicFlowDbContext(IOptions<ConnectionStringsOptions> connectionStringsOptions)
    {
        _connectionString = connectionStringsOptions.Value.DefaultConnection;
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<PatientPolicy> PatientPolicies => Set<PatientPolicy>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<ClinicLocation> ClinicLocations => Set<ClinicLocation>();
    public DbSet<ProviderSpecialty> ProviderSpecialties => Set<ProviderSpecialty>();
    public DbSet<ProviderLocation> ProviderLocations => Set<ProviderLocation>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Encounter> Encounters => Set<Encounter>();
    public DbSet<DiagnosisCode> DiagnosisCodes => Set<DiagnosisCode>();
    public DbSet<EncounterDiagnosis> EncounterDiagnoses => Set<EncounterDiagnosis>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // =====================================================
        // Patient
        // =====================================================
        modelBuilder.Entity<Patient>(e =>
        {
            e.ToTable("Patient");
            e.HasKey(p => p.PatientId);
            e.Property(p => p.FirstName).HasColumnType("varchar(50)").IsRequired();
            e.Property(p => p.LastName).HasColumnType("varchar(50)").IsRequired();
            e.Property(p => p.DateOfBirth).HasColumnType("date").IsRequired();
            e.Property(p => p.Gender).IsRequired(); // int enum, no CHECK — validated in app code
            e.Property(p => p.Phone).HasColumnType("varchar(20)").IsRequired();
            e.Property(p => p.Email).HasColumnType("varchar(100)").IsRequired();
            e.Property(p => p.HomeAddress).HasColumnType("varchar(255)");
            e.HasIndex(p => p.Email).IsUnique();
            e.HasIndex(p => p.Phone).IsUnique();
        });

        // =====================================================
        // Policy
        // =====================================================
        modelBuilder.Entity<Policy>(e =>
        {
            e.ToTable("Policy");
            e.HasKey(p => p.PolicyId);
            e.Property(p => p.PayerName).HasColumnType("varchar(100)").IsRequired();
            e.Property(p => p.PlanName).HasColumnType("varchar(100)").IsRequired();
            e.HasIndex(p => new { p.PayerName, p.PlanName }).IsUnique();
        });

        // =====================================================
        // PatientPolicy (junction: Patient <-> Policy)
        // =====================================================
        modelBuilder.Entity<PatientPolicy>(e =>
        {
            e.ToTable("PatientPolicy");
            e.HasKey(pp => new { pp.PatientId, pp.PolicyId });
            e.Property(pp => pp.MemberId).HasColumnType("varchar(50)").IsRequired();
            e.Property(pp => pp.IsPrimary).IsRequired();

            e.HasOne(pp => pp.Patient)
                .WithMany(p => p.PatientPolicies)
                .HasForeignKey(pp => pp.PatientId)
                .OnDelete(DeleteBehavior.Cascade); // join row is meaningless without the patient

            e.HasOne(pp => pp.Policy)
                .WithMany(p => p.PatientPolicies)
                .HasForeignKey(pp => pp.PolicyId)
                .OnDelete(DeleteBehavior.Restrict); // protect the catalog

            e.HasIndex(pp => new { pp.PolicyId, pp.MemberId }).IsUnique();

            // Enforces "at most one primary policy per patient" via a filtered unique index.
            e.HasIndex(pp => pp.PatientId)
                .IsUnique()
                .HasFilter("[IsPrimary] = 1")
                .HasDatabaseName("UQ_PatientPolicy_OnePrimary");
        });

        // =====================================================
        // Specialty
        // =====================================================
        modelBuilder.Entity<Specialty>(e =>
        {
            e.ToTable("Specialty");
            e.HasKey(s => s.SpecialtyId);
            e.Property(s => s.Name).HasColumnType("varchar(100)").IsRequired();
            e.HasIndex(s => s.Name).IsUnique();
        });

        // =====================================================
        // Provider
        // =====================================================
        modelBuilder.Entity<Provider>(e =>
        {
            e.ToTable("Provider");
            e.HasKey(p => p.ProviderId);
            e.Property(p => p.Name).HasColumnType("varchar(100)").IsRequired();
            e.Property(p => p.Credentials).HasColumnType("varchar(50)").IsRequired();
            e.Property(p => p.Phone).HasColumnType("varchar(20)").IsRequired();
            e.Property(p => p.Email).HasColumnType("varchar(100)").IsRequired();
            e.HasIndex(p => p.Email).IsUnique();
            e.HasIndex(p => p.Phone).IsUnique();
        });

        // =====================================================
        // ClinicLocation
        // =====================================================
        modelBuilder.Entity<ClinicLocation>(e =>
        {
            e.ToTable("ClinicLocation");
            e.HasKey(c => c.LocationId);
            e.Property(c => c.Name).HasColumnType("varchar(100)").IsRequired();
            e.Property(c => c.Address).HasColumnType("varchar(255)").IsRequired();
            e.HasIndex(c => c.Name).IsUnique();
            e.HasIndex(c => c.Address).IsUnique();
        });

        // =====================================================
        // ProviderSpecialty (junction: Provider <-> Specialty)
        // =====================================================
        modelBuilder.Entity<ProviderSpecialty>(e =>
        {
            e.ToTable("ProviderSpecialty");
            e.HasKey(ps => new { ps.ProviderId, ps.SpecialtyId });

            e.HasOne(ps => ps.Provider)
                .WithMany(p => p.ProviderSpecialties)
                .HasForeignKey(ps => ps.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(ps => ps.Specialty)
                .WithMany(s => s.ProviderSpecialties)
                .HasForeignKey(ps => ps.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // ProviderLocation (junction: Provider <-> ClinicLocation)
        // =====================================================
        modelBuilder.Entity<ProviderLocation>(e =>
        {
            e.ToTable("ProviderLocation");
            e.HasKey(pl => new { pl.ProviderId, pl.LocationId });

            e.HasOne(pl => pl.Provider)
                .WithMany(p => p.ProviderLocations)
                .HasForeignKey(pl => pl.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(pl => pl.ClinicLocation)
                .WithMany(c => c.ProviderLocations)
                .HasForeignKey(pl => pl.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // Room
        // =====================================================
        modelBuilder.Entity<Room>(e =>
        {
            e.ToTable("Room");
            e.HasKey(r => r.RoomId);
            e.Property(r => r.Name).HasColumnType("varchar(50)").IsRequired();

            e.HasOne(r => r.ClinicLocation)
                .WithMany(c => c.Rooms)
                .HasForeignKey(r => r.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(r => new { r.LocationId, r.Name }).IsUnique();
        });

        // =====================================================
        // Appointment
        // =====================================================
        modelBuilder.Entity<Appointment>(e =>
        {
            e.ToTable("Appointment");
            e.HasKey(a => a.AppointmentId);
            e.Property(a => a.AppointmentDatetime).HasColumnType("datetime").IsRequired();
            e.Property(a => a.Reason).HasColumnType("varchar(255)").IsRequired();
            e.Property(a => a.Status).IsRequired(); // int enum, no CHECK

            e.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict); // don't wipe appointment history via patient delete

            e.HasOne(a => a.Provider)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(a => a.Room)
                .WithMany(r => r.Appointments)
                .HasForeignKey(a => a.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Added during Stage 5 (raw-SQL side): prevents double-booking the
            // exact same provider at the exact same timestamp.
            e.HasIndex(a => new { a.ProviderId, a.AppointmentDatetime })
                .IsUnique()
                .HasDatabaseName("UQ_Appointment_Provider_Datetime");
        });

        // =====================================================
        // Encounter
        // =====================================================
        modelBuilder.Entity<Encounter>(e =>
        {
            e.ToTable("Encounter");
            e.HasKey(en => en.EncounterId);
            e.Property(en => en.EncounterDate).HasColumnType("date").IsRequired();
            e.Property(en => en.NoteText).HasColumnType("varchar(max)").IsRequired();

            e.HasOne(en => en.Appointment)
                .WithOne(a => a.Encounter)
                .HasForeignKey<Encounter>(en => en.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade); // encounter is meaningless without its appointment

            e.HasIndex(en => en.AppointmentId).IsUnique();
        });

        // =====================================================
        // DiagnosisCode
        // =====================================================
        modelBuilder.Entity<DiagnosisCode>(e =>
        {
            e.ToTable("DiagnosisCode");
            e.HasKey(d => d.DiagnosisCodeValue);
            e.Property(d => d.DiagnosisCodeValue).HasColumnName("diagnosis_code").HasColumnType("varchar(10)");
            e.Property(d => d.Description).HasColumnType("varchar(255)").IsRequired();
        });

        // =====================================================
        // EncounterDiagnosis (junction: Encounter <-> DiagnosisCode)
        // =====================================================
        modelBuilder.Entity<EncounterDiagnosis>(e =>
        {
            e.ToTable("EncounterDiagnosis");
            e.HasKey(ed => new { ed.EncounterId, ed.DiagnosisCodeValue });
            e.Property(ed => ed.DiagnosisCodeValue).HasColumnName("diagnosis_code").HasColumnType("varchar(10)");

            e.HasOne(ed => ed.Encounter)
                .WithMany(en => en.EncounterDiagnoses)
                .HasForeignKey(ed => ed.EncounterId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(ed => ed.DiagnosisCode)
                .WithMany(d => d.EncounterDiagnoses)
                .HasForeignKey(ed => ed.DiagnosisCodeValue)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // Medication
        // =====================================================
        modelBuilder.Entity<Medication>(e =>
        {
            e.ToTable("Medication");
            e.HasKey(m => m.MedicationId);
            e.Property(m => m.Name).HasColumnType("varchar(100)").IsRequired();
            e.HasIndex(m => m.Name).IsUnique();
        });

        // =====================================================
        // Prescription
        // =====================================================
        modelBuilder.Entity<Prescription>(e =>
        {
            e.ToTable("Prescription");
            e.HasKey(p => p.PrescriptionId);
            e.Property(p => p.Dosage).HasColumnType("varchar(50)").IsRequired();
            e.Property(p => p.Instructions).HasColumnType("varchar(255)").IsRequired();

            e.HasOne(p => p.Encounter)
                .WithMany(en => en.Prescriptions)
                .HasForeignKey(p => p.EncounterId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(p => p.Medication)
                .WithMany(m => m.Prescriptions)
                .HasForeignKey(p => p.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
