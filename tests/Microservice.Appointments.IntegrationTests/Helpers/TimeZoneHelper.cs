namespace Microservice.Appointments.IntegrationTests.Helpers;

public static class TimeZoneHelper
{
    public static DateTime ToUtc(DateTime dateTime)
    {
        var localTimeZone = TimeZoneInfo.Local;
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, localTimeZone);
    }

    public static DateTime ToLocalTime(DateTime dateTime)
    {
        var localTimeZone = TimeZoneInfo.Local;
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, localTimeZone);
    }
}