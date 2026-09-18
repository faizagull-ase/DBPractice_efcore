namespace ClinicFlowDemo.Models;

// Junction table resolving the many-to-many relationship between Provider and Specialty.
public class ProviderSpecialty
{
    public int ProviderId { get; set; }
    public Provider Provider { get; set; } = null!;

    public int SpecialtyId { get; set; }
    public Specialty Specialty { get; set; } = null!;
}
