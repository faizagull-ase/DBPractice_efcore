namespace ClinicFlowDemo.Models;

// Junction table resolving the many-to-many relationship between Patient and Policy.
public class PatientPolicy
{
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

    public string MemberId { get; set; } = null!;
    public bool IsPrimary { get; set; }
}
