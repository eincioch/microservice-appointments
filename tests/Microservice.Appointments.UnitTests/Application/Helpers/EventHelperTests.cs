using Microservice.Appointments.Application.Helpers;
using Xunit;

namespace Microservice.Appointments.UnitTests.Application.Helpers;

public class EventHelperTests
{
    private const string SampleClassEventExpectedName = "sample.class";
    private const string AnotherSampleEventExpectedName = "another.sample";
    private const string EventSuffix = ".event";

    private class SampleClassEvent { }
    private class AnotherSampleEvent { }

    [Fact]
    public void Given_SampleClassEvent_When_GetEventNameIsCalled_Then_ReturnsFormattedEventName()
    {
        // Act
        var eventName = EventHelper.GetEventName<SampleClassEvent>();

        // Assert
        Assert.Equal(SampleClassEventExpectedName, eventName);
    }

    [Fact]
    public void Given_AnotherSampleEvent_When_GetEventNameIsCalled_Then_ReturnsFormattedEventName()
    {
        // Act
        var eventName = EventHelper.GetEventName<AnotherSampleEvent>();

        // Assert
        Assert.Equal(AnotherSampleEventExpectedName, eventName);
    }

    [Fact]
    public void Given_EventWithEventSuffix_When_GetEventNameIsCalled_Then_RemovesEventSuffix()
    {
        // Act
        var eventName = EventHelper.GetEventName<SampleClassEvent>();

        // Assert
        Assert.DoesNotContain(EventSuffix, eventName);
    }
}