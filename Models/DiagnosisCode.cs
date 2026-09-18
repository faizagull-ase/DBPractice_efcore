namespace ClinicFlowDemo.Models;

public class DiagnosisCode
{
    // Natural key (e.g. ICD-10-style code) — kept as the PK per the ERD.
    public string DiagnosisCodeValue { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<EncounterDiagnosis> EncounterDiagnoses { get; set; } = new List<EncounterDiagnosis>();
}
