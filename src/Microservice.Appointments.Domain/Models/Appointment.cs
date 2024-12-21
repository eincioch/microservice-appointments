using Microservice.Appointments.Domain.Enums;
using Microservice.Appointments.Domain.Exceptions;

namespace Microservice.Appointments.Domain.Models;

public class Appointment
{
    private const int UnassignedId = 0;

    public int Id { get; private set; }
    public string Title { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string Description { get; private set; }
    public AppointmentStatus Status { get; private set; }

    public Appointment(string title, DateTime startTime, DateTime endTime, string description)
    {
        ValidateTitle(title);
        ValidateDates(startTime, endTime);

        Title = title;
        StartTime = startTime;
        EndTime = endTime;
        Description = description;
        Status = AppointmentStatus.Scheduled;
    }

    public void AssignId(int id)
    {
        if (id <= UnassignedId)
            throw new DomainValidationException($"Appointment id must be greater than {UnassignedId}. Provided value: {id}");

        if (Id != UnassignedId)
            throw new DomainValidationException($"Appointment Id has already been assigned for this appointment. Current ID: {Id}");

        Id = id;
    }

    public void Complete()
    {
        if (Status == AppointmentStatus.Completed)
            throw new DomainValidationException($"Appointment with ID {Id} is already {AppointmentStatus.Completed}.");

        Status = AppointmentStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == AppointmentStatus.Canceled)
            throw new DomainValidationException($"Appointment with ID {Id} is already {AppointmentStatus.Canceled}.");

        Status = AppointmentStatus.Canceled;
    }

    private static void ValidateDates(DateTime startTime, DateTime endTime)
    {
        if (startTime >= endTime)
            throw new DomainValidationException($"{nameof(startTime)} ({startTime}) must be earlier than {nameof(endTime)} ({endTime}).");
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainValidationException($"{nameof(title)} cannot be null or empty.");
    }
}