using Microsoft.AspNetCore.Mvc;

namespace Microservice.Appointments.Api.Helpers;

public static class ActionResultHelper
{
    public static IActionResult Created<T>(string actionName, string controllerName, T id, object response)
    {
        return new CreatedAtActionResult(
            actionName,
            controllerName,
            new { id },
            response
        );
    }
}