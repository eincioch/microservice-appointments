namespace Microservice.Appointments.IntegrationTests.Responses;

public record AppointmentResponse(
    int Id,
    string Title,
    DateTime StartTime,
    DateTime EndTime,
    string Description,
    int Status
);