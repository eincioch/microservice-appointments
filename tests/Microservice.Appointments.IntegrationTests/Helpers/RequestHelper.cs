using RestSharp;

namespace Microservice.Appointments.IntegrationTests.Helpers;

public static class RequestHelper
{
    private const string AcceptHeader = "Accept";
    private const string ApplicationJson = "application/json";

    public static RestRequest GetRequest(string endpoint)
    {
        var request = new RestRequest(endpoint, Method.Get);
        request.AddHeader(AcceptHeader, ApplicationJson);
        return request;
    }
}