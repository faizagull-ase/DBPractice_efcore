namespace ClinicFlowDemo.Models;

// Junction table resolving the many-to-many relationship between Encounter and DiagnosisCode.
public class EncounterDiagnosis
{
    public int EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;

    public string DiagnosisCodeValue { get; set; } = null!;
    public DiagnosisCode DiagnosisCode { get; set; } = null!;
}
