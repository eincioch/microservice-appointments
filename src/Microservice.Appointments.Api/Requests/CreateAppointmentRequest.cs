namespace Microservice.Appointments.Api.Requests;

public record CreateAppointmentRequest(
    string Title,
    DateTime StartTime,
    DateTime EndTime,
    string Description
);