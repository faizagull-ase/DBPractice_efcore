namespace ClinicFlowDemo.Models;

public class Policy
{
    public int PolicyId { get; set; }
    public string PayerName { get; set; } = null!;
    public string PlanName { get; set; } = null!;

    public ICollection<PatientPolicy> PatientPolicies { get; set; } = new List<PatientPolicy>();
}
