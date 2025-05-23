﻿namespace Microservice.Appointments.IntegrationTests.Endpoints.Base;

public class Url
{
    public static class Appointments
    {
        private static string BaseEndpoint => "Appointments";
        public static string GetAll => $"{BaseEndpoint}";
        public static string GetById(int id) => $"{BaseEndpoint}/{id}";
        public static string Post => $"{BaseEndpoint}";
        public static string Put(int id) => $"{BaseEndpoint}/{id}";
        public static string Patch(int id) => $"{BaseEndpoint}/{id}/status";
        public static string Delete(int id) => $"{BaseEndpoint}/{id}";
    }
}