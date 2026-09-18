namespace ClinicFlowDemo.Models;

public class Specialty
{
    public int SpecialtyId { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<ProviderSpecialty> ProviderSpecialties { get; set; } = new List<ProviderSpecialty>();
}
