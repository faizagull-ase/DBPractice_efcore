namespace ClinicFlowDemo.Models;

public class Appointment
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int ProviderId { get; set; }
    public Provider Provider { get; set; } = null!;

    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public DateTime AppointmentDatetime { get; set; }
    public string Reason { get; set; } = null!;

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public Encounter? Encounter { get; set; }
}
