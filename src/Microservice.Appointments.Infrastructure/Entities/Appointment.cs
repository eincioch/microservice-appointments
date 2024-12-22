using System.ComponentModel.DataAnnotations;
using Microservice.Appointments.Domain.Enums;

namespace Microservice.Appointments.Infrastructure.Entities;

public class Appointment
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = null!;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = null!;

    [Required]
    public AppointmentStatus Status { get; set; }
}