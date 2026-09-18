namespace ClinicFlowDemo.Models;

// Junction table resolving the many-to-many relationship between Provider and ClinicLocation.
public class ProviderLocation
{
    public int ProviderId { get; set; }
    public Provider Provider { get; set; } = null!;

    public int LocationId { get; set; }
    public ClinicLocation ClinicLocation { get; set; } = null!;
}
