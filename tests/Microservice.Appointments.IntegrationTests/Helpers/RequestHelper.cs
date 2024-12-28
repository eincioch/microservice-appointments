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

    public static RestRequest PostRequest<T>(string endpoint, T payload) where T : class
    {
        var request = new RestRequest(endpoint, Method.Post);
        request.AddHeader(AcceptHeader, ApplicationJson);
        request.AddJsonBody(payload);
        return request;
    }
}