using Microservice.Appointments.Domain.Enums;

namespace Microservice.Appointments.Infrastructure.Entities;

public class Appointment
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Description { get; set; } = null!;
    public AppointmentStatus Status { get; set; }
}