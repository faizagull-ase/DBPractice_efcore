namespace ClinicFlowDemo.Models;

public class Medication
{
    public int MedicationId { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
