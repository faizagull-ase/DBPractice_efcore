namespace ClinicFlowDemo.Models;

public class Patient
{
    public int PatientId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? HomeAddress { get; set; }

    public ICollection<PatientPolicy> PatientPolicies { get; set; } = new List<PatientPolicy>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
