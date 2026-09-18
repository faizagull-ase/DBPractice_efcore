namespace ClinicFlowDemo.Models;

public class Encounter
{
    public int EncounterId { get; set; }

    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public DateOnly EncounterDate { get; set; }
    public string NoteText { get; set; } = null!;

    public ICollection<EncounterDiagnosis> EncounterDiagnoses { get; set; } = new List<EncounterDiagnosis>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
