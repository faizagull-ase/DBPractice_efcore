namespace ClinicFlowDemo.Models;

public class Prescription
{
    public int PrescriptionId { get; set; }

    public int EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;

    public int MedicationId { get; set; }
    public Medication Medication { get; set; } = null!;

    public string Dosage { get; set; } = null!;
    public string Instructions { get; set; } = null!;
}
