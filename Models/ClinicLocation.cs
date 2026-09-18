namespace ClinicFlowDemo.Models;

public class ClinicLocation
{
    public int LocationId { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;

    public ICollection<ProviderLocation> ProviderLocations { get; set; } = new List<ProviderLocation>();
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
