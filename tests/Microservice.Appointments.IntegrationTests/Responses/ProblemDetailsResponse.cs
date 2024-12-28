namespace Microservice.Appointments.IntegrationTests.Responses;

public record ProblemDetailsResponse(
    string Title,
    int Status,
    string Detail,
    string Instance
);