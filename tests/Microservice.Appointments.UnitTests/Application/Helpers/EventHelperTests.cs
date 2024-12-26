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
    public void Given_Sample_Class_Event_When_Get_Event_Name_Is_Called_Then_Returns_Formatted_Event_Name()
    {
        // Act
        var eventName = EventHelper.GetEventName<SampleClassEvent>();

        // Assert
        Assert.Equal(SampleClassEventExpectedName, eventName);
    }

    [Fact]
    public void Given_Another_Sample_Event_When_Get_Event_Name_Is_Called_Then_Returns_Formatted_Event_Name()
    {
        // Act
        var eventName = EventHelper.GetEventName<AnotherSampleEvent>();

        // Assert
        Assert.Equal(AnotherSampleEventExpectedName, eventName);
    }

    [Fact]
    public void Given_Event_With_Event_Suffix_When_Get_Event_Name_Is_Called_Then_Removes_Event_Suffix()
    {
        // Act
        var eventName = EventHelper.GetEventName<SampleClassEvent>();

        // Assert
        Assert.DoesNotContain(EventSuffix, eventName);
    }
}