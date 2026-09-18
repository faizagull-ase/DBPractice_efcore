namespace ClinicFlowDemo.Models;

public class Room
{
    public int RoomId { get; set; }

    public int LocationId { get; set; }
    public ClinicLocation ClinicLocation { get; set; } = null!;

    public string Name { get; set; } = null!;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
