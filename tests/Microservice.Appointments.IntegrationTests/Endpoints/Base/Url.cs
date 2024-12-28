namespace Microservice.Appointments.IntegrationTests.Endpoints.Base;

public class Url
{
    public static class Appointments
    {
        private static string BaseEndpoint => "Appointments";
        public static string GetAll => $"{BaseEndpoint}";
        public static string GetById(int id)=> $"{BaseEndpoint}/{id}";
    }
}