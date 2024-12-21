using Microservice.Appointments.Domain.Enums;

namespace Microservice.Appointments.Infrastructure.Entities;

public class Appointment
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Description { get; set; }
    public AppointmentStatus Status { get; set; }
}