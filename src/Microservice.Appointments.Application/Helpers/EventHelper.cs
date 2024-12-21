namespace Microservice.Appointments.Application.Helpers;

public static class EventHelper
{
    private const string EventString = ".event";

    public static string GetEventName<TEvent>() where TEvent : class
    {
        var typeName = typeof(TEvent).Name;
        return string.Concat(
            typeName.Select(
                (character, number) =>
                    number > decimal.Zero && char.IsUpper(character)
                        ? $".{char.ToLower(character)}"
                        : char.ToLower(character).ToString()
            )
        ).Replace(EventString, string.Empty);
    }
}