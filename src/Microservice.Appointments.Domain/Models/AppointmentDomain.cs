using Microservice.Appointments.Domain.Enums;
using Microservice.Appointments.Domain.Exceptions;

namespace Microservice.Appointments.Domain.Models;

public class AppointmentDomain
{
    private const int MinimumAllowedId = 1;
    private const int UnassignedId = 0;

    public int Id { get; private set; }
    public string Title { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string Description { get; private set; }
    public AppointmentStatus Status { get; private set; }

    public AppointmentDomain(string title, DateTime startTime, DateTime endTime, string description)
    {
        ValidateTitle(title);
        ValidateDates(startTime, endTime);

        Title = title;
        StartTime = startTime;
        EndTime = endTime;
        Description = description;
        Status = AppointmentStatus.Scheduled;
    }

    public void Update(string title, DateTime startTime, DateTime endTime, string description, AppointmentStatus status)
    {
        ValidateTitle(title);
        ValidateDates(startTime, endTime);
        UpdateStatus(status);

        Title = title;
        StartTime = startTime;
        EndTime = endTime;
        Description = description;
        Status = status;
    }

    /// <summary>
    /// Constructor for hydrating an existing appointment when fetching.
    /// </summary>
    private AppointmentDomain(int id, string title, DateTime startTime, DateTime endTime, string description, AppointmentStatus status)
    {
        Id = id;
        Title = title;
        StartTime = startTime;
        EndTime = endTime;
        Description = description;
        Status = status;
    }

    public void AssignId(int id)
    {
        if (id < MinimumAllowedId)
            throw new DomainValidationException($"Appointment id must be greater than or equal to {MinimumAllowedId}. Provided value: {id}");

        if (Id != UnassignedId)
            throw new DomainValidationException($"Appointment id has already been assigned. Current id: {Id}");

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

    public void UpdateStatus(AppointmentStatus status)
    {
        if (status == Status)
            return;

        switch (status)
        {
            case AppointmentStatus.Canceled:
                Cancel();
                break;
            case AppointmentStatus.Completed:
                Complete();
                break;
            default:
                throw new DomainValidationException($"Invalid status: {status}");
        }
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

    /// <summary>
    /// Hydrates an appointment domain object from persisted data.
    /// Uses the internal constructor to initialize the instance.
    /// </summary>
    /// <returns>Fully initialized <see cref="AppointmentDomain"/> instance.</returns>
    public static AppointmentDomain Hydrate(int id, string title, DateTime startTime, DateTime endTime, string description, AppointmentStatus status)
        => new(id, title, startTime, endTime, description, status);
}