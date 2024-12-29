using RestSharp;

namespace Microservice.Appointments.IntegrationTests.Helpers;

public static class RequestHelper
{
    private const string AcceptHeader = "Accept";
    private const string ApplicationJson = "application/json";

    public static RestRequest GetRequest(string endpoint)
    {
        var request = CreateRequest(endpoint, Method.Get);
        return request;
    }

    public static RestRequest PostRequest<T>(string endpoint, T payload) where T : class
    {
        var request = CreateRequest(endpoint, Method.Post);
        request.AddJsonBody(payload);
        return request;
    }

    public static RestRequest PutRequest<T>(string endpoint, T payload) where T : class
    {
        var request = CreateRequest(endpoint, Method.Put);
        request.AddJsonBody(payload);
        return request;
    }

    public static RestRequest PatchRequest<T>(string endpoint, T payload) where T : class
    {
        var request = CreateRequest(endpoint, Method.Patch);
        request.AddJsonBody(payload);
        return request;
    }

    public static RestRequest DeleteRequest(string endpoint)
    {
        var request = CreateRequest(endpoint, Method.Delete);
        return request;
    }

    private static RestRequest CreateRequest(string endpoint, Method method)
    {
        var request = new RestRequest(endpoint, method);
        request.AddHeader(AcceptHeader, ApplicationJson);
        return request;
    }
}