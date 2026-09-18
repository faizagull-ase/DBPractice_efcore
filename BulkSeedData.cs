using ClinicFlowDemo.Models;

namespace ClinicFlowDemo;


public static class BulkSeedData
{
    private const int PatientCount = 1000;
    private const int ProviderCount = 50;
    private const int AppointmentCount = 50000;
    private const int AppointmentBatchSize = 5000;
    private const int EncounterBatchSize = 5000;

    private static readonly string[] PatientFirstNames =
        { "James", "Mary", "Robert", "Patricia", "John", "Jennifer", "Michael", "Linda", "William", "Elizabeth" };
    private static readonly string[] PatientLastNames =
        { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };
    private static readonly string[] ProviderFirstNames = { "Sarah", "Alan", "Emily", "David", "Laura" };
    private static readonly string[] ProviderLastNames = { "Kim", "Reyes", "Chen", "Patel" };
    private static readonly string[] Credentials = { "MD", "DO", "NP", "PA" };
    private static readonly string[] Reasons =
        { "Annual physical", "Follow-up visit", "Acute illness visit", "Vaccination", "Consultation" };

    public static void Generate(ClinicFlowDbContext db)
    {
        if (db.Patients.Any(p => p.Email.StartsWith("bulk_patient")))
        {
            Console.WriteLine("Bulk data already generated — skipping.");
            return;
        }

        db.ChangeTracker.AutoDetectChangesEnabled = false;
        try
        {
            var patientIds = GeneratePatients(db);
            var providerIds = GenerateProviders(db);
            var roomIds = GenerateRooms(db);

            GenerateProviderRelationships(db, providerIds);
            GeneratePatientPolicies(db, patientIds);

            var completedAppointmentIds = GenerateAppointments(db, patientIds, providerIds, roomIds);
            GenerateEncountersAndDiagnoses(db, completedAppointmentIds);
        }
        finally
        {
            db.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    private static List<int> GeneratePatients(ClinicFlowDbContext db)
    {
        var patients = new List<Patient>(PatientCount);
        for (int n = 1; n <= PatientCount; n++)
        {
            var first = PatientFirstNames[(n - 1) % PatientFirstNames.Length];
            var last = PatientLastNames[((n - 1) / PatientFirstNames.Length) % PatientLastNames.Length];
            patients.Add(new Patient
            {
                FirstName = first,
                LastName = last,
                DateOfBirth = DateOnly.FromDateTime(new DateTime(2026, 1, 1).AddYears(-(18 + (n % 65)))),
                Gender = (Gender)(((n - 1) % 3) + 1),
                Phone = $"555-9{n:0000}",
                Email = $"bulk_patient{n}@example.com",
                HomeAddress = $"{n} Bulk Ave"
            });
        }
        db.Patients.AddRange(patients);
        db.SaveChanges();
        var ids = patients.Select(p => p.PatientId).ToList();
        db.ChangeTracker.Clear();
        Console.WriteLine($"Generated {ids.Count} patients.");
        return ids;
    }

    private static List<int> GenerateProviders(ClinicFlowDbContext db)
    {
        var providers = new List<Provider>(ProviderCount);
        for (int n = 1; n <= ProviderCount; n++)
        {
            var first = ProviderFirstNames[(n - 1) % ProviderFirstNames.Length];
            var last = ProviderLastNames[((n - 1) / ProviderFirstNames.Length) % ProviderLastNames.Length];
            providers.Add(new Provider
            {
                Name = $"Dr. {first} {last}",
                Credentials = Credentials[(n - 1) % Credentials.Length],
                Phone = $"555-8{n:0000}",
                Email = $"bulk_provider{n}@clinicflow.example"
            });
        }
        db.Providers.AddRange(providers);
        db.SaveChanges();
        var ids = providers.Select(p => p.ProviderId).ToList();
        db.ChangeTracker.Clear();
        Console.WriteLine($"Generated {ids.Count} providers.");
        return ids;
    }

    private static List<int> GenerateRooms(ClinicFlowDbContext db)
    {
        var locationIds = db.ClinicLocations.Select(c => c.LocationId).OrderBy(id => id).ToList();
        var rooms = new List<Room>();
        foreach (var locationId in locationIds)
        {
            rooms.Add(new Room { LocationId = locationId, Name = "Bulk Room 1" });
            rooms.Add(new Room { LocationId = locationId, Name = "Bulk Room 2" });
        }
        db.Rooms.AddRange(rooms);
        db.SaveChanges();
        var ids = rooms.Select(r => r.RoomId).ToList();
        db.ChangeTracker.Clear();
        Console.WriteLine($"Generated {ids.Count} rooms.");
        return ids;
    }

    private static void GenerateProviderRelationships(ClinicFlowDbContext db, List<int> providerIds)
    {
        var specialtyIds = db.Specialties.Select(s => s.SpecialtyId).OrderBy(id => id).ToList();
        var locationIds = db.ClinicLocations.Select(c => c.LocationId).OrderBy(id => id).ToList();

        var providerSpecialties = new List<ProviderSpecialty>();
        var providerLocations = new List<ProviderLocation>();

        for (int i = 0; i < providerIds.Count; i++)
        {
            var providerId = providerIds[i];

            // every provider gets one specialty; every other one gets a 2nd, offset so it never repeats the 1st
            providerSpecialties.Add(new ProviderSpecialty
            {
                ProviderId = providerId,
                SpecialtyId = specialtyIds[i % specialtyIds.Count]
            });
            if (i % 2 == 0)
            {
                providerSpecialties.Add(new ProviderSpecialty
                {
                    ProviderId = providerId,
                    SpecialtyId = specialtyIds[(i + 3) % specialtyIds.Count]
                });
            }

            providerLocations.Add(new ProviderLocation
            {
                ProviderId = providerId,
                LocationId = locationIds[i % locationIds.Count]
            });
        }

        db.ProviderSpecialties.AddRange(providerSpecialties);
        db.ProviderLocations.AddRange(providerLocations);
        db.SaveChanges();
        db.ChangeTracker.Clear();
        Console.WriteLine($"Generated {providerSpecialties.Count} ProviderSpecialty and {providerLocations.Count} ProviderLocation rows.");
    }

    private static void GeneratePatientPolicies(ClinicFlowDbContext db, List<int> patientIds)
    {
        var policyIds = db.Policies.Select(p => p.PolicyId).OrderBy(id => id).ToList();
        var patientPolicies = new List<PatientPolicy>();

        for (int i = 0; i < patientIds.Count; i++)
        {
            var patientId = patientIds[i];

            patientPolicies.Add(new PatientPolicy
            {
                PatientId = patientId,
                PolicyId = policyIds[i % policyIds.Count],
                MemberId = $"MEM-{i + 1}",
                IsPrimary = true
            });

            if (i % 3 == 0)
            {
                patientPolicies.Add(new PatientPolicy
                {
                    PatientId = patientId,
                    PolicyId = policyIds[(i + 3) % policyIds.Count],
                    MemberId = $"MEM-S-{i + 1}",
                    IsPrimary = false
                });
            }
        }

        db.PatientPolicies.AddRange(patientPolicies);
        db.SaveChanges();
        db.ChangeTracker.Clear();
        Console.WriteLine($"Generated {patientPolicies.Count} PatientPolicy rows.");
    }

    private static List<int> GenerateAppointments(ClinicFlowDbContext db, List<int> patientIds, List<int> providerIds, List<int> roomIds)
    {
        var completedAppointmentIds = new List<int>();
        var baseDate = new DateTime(2024, 1, 1);
        var batch = new List<Appointment>(AppointmentBatchSize);

        for (int n = 1; n <= AppointmentCount; n++)
        {
            var status = (n % 100) switch
            {
                < 60 => AppointmentStatus.Completed,
                < 80 => AppointmentStatus.Scheduled,
                < 90 => AppointmentStatus.Cancelled,
                < 95 => AppointmentStatus.NoShow,
                _ => AppointmentStatus.CheckedIn
            };

            // UQ_Appointment_Provider_Datetime requires each provider's own appointments to
            // never share a timestamp. Using a per-provider sequence number (0, 1, 2, ...) instead
            // of the raw global counter guarantees that: with 1,000 appointments per provider and
            // a 1,095-day spread, no provider's own slot number ever wraps around and repeats a day.
            int providerIndex = (n - 1) % providerIds.Count;
            int slotForThisProvider = (n - 1) / providerIds.Count;

            batch.Add(new Appointment
            {
                PatientId = patientIds[(n - 1) % patientIds.Count],
                ProviderId = providerIds[providerIndex],
                RoomId = roomIds[(n - 1) % roomIds.Count],
                AppointmentDatetime = baseDate.AddDays(slotForThisProvider % 1095).AddHours((slotForThisProvider % 8) + 9),
                Reason = Reasons[n % Reasons.Length],
                Status = status
            });

            if (batch.Count == AppointmentBatchSize || n == AppointmentCount)
            {
                db.Appointments.AddRange(batch);
                db.SaveChanges();

                completedAppointmentIds.AddRange(
                    batch.Where(a => a.Status == AppointmentStatus.Completed).Select(a => a.AppointmentId));

                db.ChangeTracker.Clear();
                batch.Clear();
                Console.WriteLine($"  ...appointments: {n} of {AppointmentCount}");
            }
        }

        Console.WriteLine($"Generated {AppointmentCount} appointments ({completedAppointmentIds.Count} completed).");
        return completedAppointmentIds;
    }

    private static void GenerateEncountersAndDiagnoses(ClinicFlowDbContext db, List<int> completedAppointmentIds)
    {
        var diagnosisCodes = db.DiagnosisCodes.Select(d => d.DiagnosisCodeValue).OrderBy(c => c).ToList();
        var medicationIds = db.Medications.Select(m => m.MedicationId).OrderBy(id => id).ToList();

        int totalEncounters = 0, totalDiagnoses = 0, totalPrescriptions = 0;

        for (int offset = 0; offset < completedAppointmentIds.Count; offset += EncounterBatchSize)
        {
            var appointmentIdBatch = completedAppointmentIds.Skip(offset).Take(EncounterBatchSize).ToList();

            var encounters = appointmentIdBatch.Select(apptId => new Encounter
            {
                AppointmentId = apptId,
                EncounterDate = DateOnly.FromDateTime(new DateTime(2024, 1, 1)), // placeholder date, kept simple for bulk data
                NoteText = "Visit completed without complications. Standard follow-up recommended."
            }).ToList();

            db.Encounters.AddRange(encounters);
            db.SaveChanges();

            var encounterIds = encounters.Select(e => e.EncounterId).ToList();
            db.ChangeTracker.Clear();

            var encounterDiagnoses = new List<EncounterDiagnosis>();
            var prescriptions = new List<Prescription>();

            for (int i = 0; i < encounterIds.Count; i++)
            {
                var encounterId = encounterIds[i];

                encounterDiagnoses.Add(new EncounterDiagnosis
                {
                    EncounterId = encounterId,
                    DiagnosisCodeValue = diagnosisCodes[i % diagnosisCodes.Count]
                });
                if (i % 2 == 0)
                {
                    encounterDiagnoses.Add(new EncounterDiagnosis
                    {
                        EncounterId = encounterId,
                        DiagnosisCodeValue = diagnosisCodes[(i + 3) % diagnosisCodes.Count]
                    });
                }

                prescriptions.Add(new Prescription
                {
                    EncounterId = encounterId,
                    MedicationId = medicationIds[i % medicationIds.Count],
                    Dosage = "As directed",
                    Instructions = "Follow package instructions"
                });
                if (i % 3 == 0)
                {
                    prescriptions.Add(new Prescription
                    {
                        EncounterId = encounterId,
                        MedicationId = medicationIds[(i + 3) % medicationIds.Count],
                        Dosage = "As directed",
                        Instructions = "Follow package instructions"
                    });
                }
            }

            db.EncounterDiagnoses.AddRange(encounterDiagnoses);
            db.Prescriptions.AddRange(prescriptions);
            db.SaveChanges();
            db.ChangeTracker.Clear();

            totalEncounters += encounters.Count;
            totalDiagnoses += encounterDiagnoses.Count;
            totalPrescriptions += prescriptions.Count;
            Console.WriteLine($"  ...encounters: {totalEncounters} of {completedAppointmentIds.Count}");
        }

        Console.WriteLine($"Generated {totalEncounters} Encounter, {totalDiagnoses} EncounterDiagnosis, {totalPrescriptions} Prescription rows.");
    }
}
