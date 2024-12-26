using AutoFixture;
using Microservice.Appointments.Domain.Enums;
using Microservice.Appointments.Domain.Exceptions;
using Microservice.Appointments.Domain.Models;
using Xunit;

namespace Microservice.Appointments.UnitTests.Domain.Models;

public class AppointmentDomainTests
{
    private const int InvalidId = 0;
    private const int NegativeId = -1;
    private const int ValidHoursToAdd = 1;
    private const int InvalidHoursToAdd = -1;

    private readonly Fixture _fixture = new();
    public static IEnumerable<object[]> InvalidStatuses =>
        new List<object[]>
        {
            new object[] { AppointmentStatus.Completed, "completed" },
            new object[] { AppointmentStatus.Canceled, "canceled" }
        };

    [Fact]
    public void Given_Null_Or_Empty_Title_When_Constructed_Then_Throws_DomainValidationException()
    {
        // Arrange
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddHours(ValidHoursToAdd);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() =>
            new AppointmentDomain(null!, startTime, endTime, _fixture.Create<string>()));

        Assert.Throws<DomainValidationException>(() =>
            new AppointmentDomain(string.Empty, startTime, endTime, _fixture.Create<string>()));
    }

    [Fact]
    public void Given_Invalid_Dates_When_Constructed_Then_Throws_DomainValidationException()
    {
        // Arrange
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddHours(InvalidHoursToAdd);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() =>
            new AppointmentDomain(_fixture.Create<string>(), startTime, endTime, _fixture.Create<string>()));
    }

    [Fact]
    public void Given_Valid_Data_When_Constructed_Then_Properties_Are_Set_Correctly()
    {
        // Arrange
        var title = _fixture.Create<string>();
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddHours(ValidHoursToAdd);
        var description = _fixture.Create<string>();

        // Act
        var appointment = new AppointmentDomain(title, startTime, endTime, description);

        // Assert
        Assert.Equal(title, appointment.Title);
        Assert.Equal(startTime, appointment.StartTime);
        Assert.Equal(endTime, appointment.EndTime);
        Assert.Equal(description, appointment.Description);
        Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
    }

    [Fact]
    public void Given_Invalid_Id_When_AssignId_Called_Then_Throws_DomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => appointment.AssignId(InvalidId));
        Assert.Throws<DomainValidationException>(() => appointment.AssignId(NegativeId));
    }

    [Fact]
    public void Given_Id_Already_Assigned_When_AssignId_Called_Then_Throws_DomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        var id = _fixture.Create<int>();
        appointment.AssignId(id);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => appointment.AssignId(_fixture.Create<int>()));
    }

    [Fact]
    public void Given_Valid_Id_When_AssignId_Called_Then_Id_Is_Assigned()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        var id = _fixture.Create<int>();

        // Act
        appointment.AssignId(id);

        // Assert
        Assert.Equal(id, appointment.Id);
    }

    [Fact]
    public void Given_Status_Already_Completed_When_Complete_Called_Then_Throws_DomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        appointment.Complete();

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => appointment.Complete());
    }

    [Fact]
    public void Given_Valid_Status_When_Complete_Called_Then_Status_Is_Updated()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();

        // Act
        appointment.Complete();

        // Assert
        Assert.Equal(AppointmentStatus.Completed, appointment.Status);
    }

    [Fact]
    public void Given_Status_Already_Canceled_When_Cancel_Called_Then_Throws_DomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        appointment.Cancel();

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => appointment.Cancel());
    }

    [Fact]
    public void Given_Valid_Status_When_Cancel_Called_Then_Status_Is_Updated()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();

        // Act
        appointment.Cancel();

        // Assert
        Assert.Equal(AppointmentStatus.Canceled, appointment.Status);
    }

    [Fact]
    public void Given_Valid_Data_When_Hydrate_Called_Then_Properties_Are_Set_Correctly()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var title = _fixture.Create<string>();
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddHours(ValidHoursToAdd);
        var description = _fixture.Create<string>();
        var status = AppointmentStatus.Scheduled;

        // Act
        var appointment = AppointmentDomain.Hydrate(id, title, startTime, endTime, description, status);

        // Assert
        Assert.Equal(id, appointment.Id);
        Assert.Equal(title, appointment.Title);
        Assert.Equal(startTime, appointment.StartTime);
        Assert.Equal(endTime, appointment.EndTime);
        Assert.Equal(description, appointment.Description);
        Assert.Equal(status, appointment.Status);
    }

    [Fact]
    public void Given_Valid_Data_When_Update_Is_Called_Then_Properties_Are_Updated_Correctly()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        var newTitle = _fixture.Create<string>();
        var newStartTime = DateTime.UtcNow;
        var newEndTime = newStartTime.AddHours(ValidHoursToAdd);
        var newDescription = _fixture.Create<string>();
        var newStatus = AppointmentStatus.Completed;

        // Act
        appointment.Update(newTitle, newStartTime, newEndTime, newDescription, newStatus);

        // Assert
        Assert.Equal(newTitle, appointment.Title);
        Assert.Equal(newStartTime, appointment.StartTime);
        Assert.Equal(newEndTime, appointment.EndTime);
        Assert.Equal(newDescription, appointment.Description);
        Assert.Equal(newStatus, appointment.Status);
    }

    [Fact]
    public void Given_Invalid_Status_When_Update_Is_Called_Then_Throws_DomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        var newTitle = _fixture.Create<string>();
        var newStartTime = DateTime.UtcNow;
        var newEndTime = newStartTime.AddHours(ValidHoursToAdd);
        var newDescription = _fixture.Create<string>();

        // Act & Assert
        Assert.Throws<DomainValidationException>(() =>
            appointment.Update(newTitle, newStartTime, newEndTime, newDescription, (AppointmentStatus)999));
    }

    [Fact]
    public void Given_Status_Already_Set_When_Update_Status_Called_Then_No_Change()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        var initialStatus = appointment.Status;

        // Act
        appointment.UpdateStatus(initialStatus);

        // Assert
        Assert.Equal(initialStatus, appointment.Status);
    }

    [Fact]
    public void Given_Valid_Status_When_Update_Status_To_Canceled_Then_Status_Is_Updated()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();

        // Act
        appointment.UpdateStatus(AppointmentStatus.Canceled);

        // Assert
        Assert.Equal(AppointmentStatus.Canceled, appointment.Status);
    }

    [Fact]
    public void Given_Valid_Status_When_Update_Status_To_Completed_Then_Status_Is_Updated()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();

        // Act
        appointment.UpdateStatus(AppointmentStatus.Completed);

        // Assert
        Assert.Equal(AppointmentStatus.Completed, appointment.Status);
    }

    [Fact]
    public void Given_Invalid_Status_When_Update_Status_Called_Then_Throws_DomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => appointment.UpdateStatus((AppointmentStatus)999));
    }

    [Fact]
    public void Given_Status_Is_Completed_When_Validate_Deletable_Called_Then_Throws_DomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        appointment.Complete();

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.ValidateDeletable());
        Assert.Equal($"Appointment with ID {appointment.Id} cannot be deleted because it is already completed.", exception.Message);
    }

    [Fact]
    public void Given_Status_Is_Canceled_When_Validate_Deletable_Called_Then_Throws_DomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        appointment.Cancel();

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.ValidateDeletable());
        Assert.Equal($"Appointment with ID {appointment.Id} cannot be deleted because it is already canceled.", exception.Message);
    }

    [Theory]
    [MemberData(nameof(InvalidStatuses))]
    public void Given_Invalid_Status_When_Validate_Deletable_Called_Then_Throws_DomainValidationException(AppointmentStatus status, string statusDescription)
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();

        switch (status)
        {
            case AppointmentStatus.Completed:
                appointment.Complete();
                break;
            case AppointmentStatus.Canceled:
                appointment.Cancel();
                break;
        }

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.ValidateDeletable());
        Assert.Equal($"Appointment with ID {appointment.Id} cannot be deleted because it is already {statusDescription}.", exception.Message);
    }

    [Fact]
    public void Given_Notification_Status_Already_Sent_When_Mark_Notification_Sent_Called_Then_Throws_DomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        appointment.MarkNotificationSent();

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => appointment.MarkNotificationSent());
    }

    [Fact]
    public void Given_Notification_Status_Not_Sent_When_Mark_Notification_Sent_Called_Then_Notification_Status_Is_Updated()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();

        // Act
        appointment.MarkNotificationSent();

        // Assert
        Assert.Equal(NotificationStatus.Sent, appointment.NotificationStatus);
    }

    private AppointmentDomain CreateValidAppointmentDomain()
    {
        return new AppointmentDomain(
            _fixture.Create<string>(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(ValidHoursToAdd),
            _fixture.Create<string>()
        );
    }
}