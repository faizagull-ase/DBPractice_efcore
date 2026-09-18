namespace ClinicFlowDemo.Models;

public class Provider
{
    public int ProviderId { get; set; }
    public string Name { get; set; } = null!;
    public string Credentials { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;

    public ICollection<ProviderSpecialty> ProviderSpecialties { get; set; } = new List<ProviderSpecialty>();
    public ICollection<ProviderLocation> ProviderLocations { get; set; } = new List<ProviderLocation>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
