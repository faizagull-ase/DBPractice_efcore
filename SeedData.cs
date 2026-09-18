using ClinicFlowDemo.Models;

namespace ClinicFlowDemo;

public static class SeedData
{
    public static void Seed(ClinicFlowDbContext db)
    {
        if (db.Patients.Any())
        {
            Console.WriteLine("Data already seeded — skipping.");
            return;
        }

        // ---- Catalogs (matching ClinicApp's seed.sql catalogs — 10 entries each) ----
        var familyMed = new Specialty { Name = "Family Medicine" };
        var pediatrics = new Specialty { Name = "Pediatrics" };
        var cardiology = new Specialty { Name = "Cardiology" };
        var dermatology = new Specialty { Name = "Dermatology" };
        var orthopedics = new Specialty { Name = "Orthopedics" };
        var neurology = new Specialty { Name = "Neurology" };
        var psychiatry = new Specialty { Name = "Psychiatry" };
        var endocrinology = new Specialty { Name = "Endocrinology" };
        var gastroenterology = new Specialty { Name = "Gastroenterology" };
        var obGyn = new Specialty { Name = "Obstetrics & Gynecology" };
        db.Specialties.AddRange(familyMed, pediatrics, cardiology, dermatology, orthopedics,
            neurology, psychiatry, endocrinology, gastroenterology, obGyn);

        var downtown = new ClinicLocation { Name = "Downtown Clinic", Address = "100 Main St, Suite 1" };
        var westside = new ClinicLocation { Name = "Westside Clinic", Address = "200 Elm St, Suite 2" };
        var eastside = new ClinicLocation { Name = "Eastside Clinic", Address = "300 Birch St, Suite 3" };
        var northgate = new ClinicLocation { Name = "Northgate Clinic", Address = "400 Cedar Ave, Suite 4" };
        var southpark = new ClinicLocation { Name = "Southpark Clinic", Address = "500 Maple Dr, Suite 5" };
        var riverside = new ClinicLocation { Name = "Riverside Clinic", Address = "600 River Rd, Suite 6" };
        var lakeside = new ClinicLocation { Name = "Lakeside Clinic", Address = "700 Lake Shore Blvd, Suite 7" };
        var midtown = new ClinicLocation { Name = "Midtown Clinic", Address = "800 Center St, Suite 8" };
        var uptown = new ClinicLocation { Name = "Uptown Clinic", Address = "900 Highland Ave, Suite 9" };
        var harborview = new ClinicLocation { Name = "Harborview Clinic", Address = "1000 Harbor Way, Suite 10" };
        db.ClinicLocations.AddRange(downtown, westside, eastside, northgate, southpark,
            riverside, lakeside, midtown, uptown, harborview);

        var diabetes = new DiagnosisCode { DiagnosisCodeValue = "E11.9", Description = "Type 2 diabetes without complications" };
        var hypertension = new DiagnosisCode { DiagnosisCodeValue = "I10", Description = "Essential hypertension" };
        var urti = new DiagnosisCode { DiagnosisCodeValue = "J06.9", Description = "Acute upper respiratory infection" };
        var lowBackPain = new DiagnosisCode { DiagnosisCodeValue = "M54.5", Description = "Low back pain" };
        var asthma = new DiagnosisCode { DiagnosisCodeValue = "J45.909", Description = "Unspecified asthma, uncomplicated" };
        var gerd = new DiagnosisCode { DiagnosisCodeValue = "K21.9", Description = "Gastro-esophageal reflux disease without esophagitis" };
        var anxiety = new DiagnosisCode { DiagnosisCodeValue = "F41.1", Description = "Generalized anxiety disorder" };
        var hyperlipidemia = new DiagnosisCode { DiagnosisCodeValue = "E78.5", Description = "Hyperlipidemia, unspecified" };
        var uti = new DiagnosisCode { DiagnosisCodeValue = "N39.0", Description = "Urinary tract infection, site not specified" };
        var dermatitis = new DiagnosisCode { DiagnosisCodeValue = "L20.9", Description = "Atopic dermatitis, unspecified" };
        db.DiagnosisCodes.AddRange(diabetes, hypertension, urti, lowBackPain, asthma,
            gerd, anxiety, hyperlipidemia, uti, dermatitis);

        var metformin = new Medication { Name = "Metformin 500mg" };
        var lisinopril = new Medication { Name = "Lisinopril 10mg" };
        var amoxicillin = new Medication { Name = "Amoxicillin 500mg" };
        var atorvastatin = new Medication { Name = "Atorvastatin 20mg" };
        var albuterol = new Medication { Name = "Albuterol 90mcg Inhaler" };
        var omeprazole = new Medication { Name = "Omeprazole 20mg" };
        var sertraline = new Medication { Name = "Sertraline 50mg" };
        var levothyroxine = new Medication { Name = "Levothyroxine 75mcg" };
        var ibuprofen = new Medication { Name = "Ibuprofen 400mg" };
        var amlodipine = new Medication { Name = "Amlodipine 5mg" };
        db.Medications.AddRange(metformin, lisinopril, amoxicillin, atorvastatin, albuterol,
            omeprazole, sertraline, levothyroxine, ibuprofen, amlodipine);

        var blueCross = new Policy { PayerName = "BlueCross", PlanName = "PPO Gold" };
        var aetna = new Policy { PayerName = "Aetna", PlanName = "HMO Silver" };
        var unitedHealthcare = new Policy { PayerName = "UnitedHealthcare", PlanName = "PPO Silver" };
        var cigna = new Policy { PayerName = "Cigna", PlanName = "HMO Bronze" };
        var humana = new Policy { PayerName = "Humana", PlanName = "PPO Bronze" };
        var kaiser = new Policy { PayerName = "Kaiser Permanente", PlanName = "HMO Gold" };
        var blueCrossHmo = new Policy { PayerName = "BlueCross", PlanName = "HMO Silver" };
        var aetnaPpo = new Policy { PayerName = "Aetna", PlanName = "PPO Gold" };
        var medicare = new Policy { PayerName = "Medicare", PlanName = "Part B" };
        var medicaid = new Policy { PayerName = "Medicaid", PlanName = "State Plan" };
        db.Policies.AddRange(blueCross, aetna, unitedHealthcare, cigna, humana,
            kaiser, blueCrossHmo, aetnaPpo, medicare, medicaid);

        // ---- Patients ----
        var patientA = new Patient
        {
            FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateOnly(1985, 4, 12),
            Gender = Gender.Female, Phone = "555-0101", Email = "jane.doe@example.com", HomeAddress = "12 Oak Ave"
        };
        var patientB = new Patient
        {
            FirstName = "Mark", LastName = "Lee", DateOfBirth = new DateOnly(1990, 9, 3),
            Gender = Gender.Male, Phone = "555-0102", Email = "mark.lee@example.com", HomeAddress = "34 Pine Rd"
        };
        db.Patients.AddRange(patientA, patientB);

        // Jane Doe: two policies (primary + secondary) — the "patient with two insurance policies" case
        patientA.PatientPolicies.Add(new PatientPolicy { Policy = blueCross, MemberId = "BC-1001", IsPrimary = true });
        patientA.PatientPolicies.Add(new PatientPolicy { Policy = aetna, MemberId = "AE-2002", IsPrimary = false });
        patientB.PatientPolicies.Add(new PatientPolicy { Policy = blueCross, MemberId = "BC-1002", IsPrimary = true });

        // ---- Providers ----
        var providerA = new Provider
        {
            Name = "Dr. Sarah Kim", Credentials = "MD", Phone = "555-0201", Email = "s.kim@clinicflow.example"
        };
        var providerB = new Provider
        {
            Name = "Dr. Alan Reyes", Credentials = "DO", Phone = "555-0202", Email = "a.reyes@clinicflow.example"
        };
        db.Providers.AddRange(providerA, providerB);

        // Dr. Kim: two specialties — the "provider with two specialties" case
        providerA.ProviderSpecialties.Add(new ProviderSpecialty { Specialty = familyMed });
        providerA.ProviderSpecialties.Add(new ProviderSpecialty { Specialty = pediatrics });
        providerB.ProviderSpecialties.Add(new ProviderSpecialty { Specialty = cardiology });

        providerA.ProviderLocations.Add(new ProviderLocation { ClinicLocation = downtown });
        providerB.ProviderLocations.Add(new ProviderLocation { ClinicLocation = westside });

        // ---- Rooms ----
        var room1 = new Room { ClinicLocation = downtown, Name = "Room 1" };
        var room2 = new Room { ClinicLocation = westside, Name = "Room 1" }; // same name, different location — allowed
        db.Rooms.AddRange(room1, room2);

        // ---- Appointments (covers every status) ----
        var completedAppt = new Appointment
        {
            Patient = patientA, Provider = providerA, Room = room1,
            AppointmentDatetime = new DateTime(2026, 8, 10, 9, 0, 0),
            Reason = "Diabetes follow-up", Status = AppointmentStatus.Completed
        };
        var cancelledAppt = new Appointment
        {
            Patient = patientA, Provider = providerA, Room = room1,
            AppointmentDatetime = new DateTime(2026, 8, 17, 9, 0, 0),
            Reason = "Routine check-up", Status = AppointmentStatus.Cancelled
        };
        var noShowAppt = new Appointment
        {
            Patient = patientB, Provider = providerB, Room = room2,
            AppointmentDatetime = new DateTime(2026, 8, 18, 14, 0, 0),
            Reason = "Cardiology consult", Status = AppointmentStatus.NoShow
        };
        var scheduledAppt = new Appointment
        {
            Patient = patientB, Provider = providerA, Room = room1,
            AppointmentDatetime = new DateTime(2026, 9, 22, 10, 30, 0),
            Reason = "New patient visit", Status = AppointmentStatus.Scheduled
        };
        db.Appointments.AddRange(completedAppt, cancelledAppt, noShowAppt, scheduledAppt);

        // ---- Encounter for the completed appointment ----
        var encounter = new Encounter
        {
            Appointment = completedAppt,
            EncounterDate = new DateOnly(2026, 8, 10),
            NoteText = "Patient reports stable blood sugar levels. Continue current medication regimen. " +
                       "Blood pressure elevated; starting antihypertensive."
        };
        db.Encounters.Add(encounter);

        // Multiple diagnoses on this encounter
        encounter.EncounterDiagnoses.Add(new EncounterDiagnosis { DiagnosisCode = diabetes });
        encounter.EncounterDiagnoses.Add(new EncounterDiagnosis { DiagnosisCode = hypertension });

        // Multiple prescriptions on this encounter
        encounter.Prescriptions.Add(new Prescription
        {
            Medication = metformin, Dosage = "500mg twice daily", Instructions = "Take with meals"
        });
        encounter.Prescriptions.Add(new Prescription
        {
            Medication = lisinopril, Dosage = "10mg once daily", Instructions = "Take in the morning"
        });

        db.SaveChanges();
    }
}
