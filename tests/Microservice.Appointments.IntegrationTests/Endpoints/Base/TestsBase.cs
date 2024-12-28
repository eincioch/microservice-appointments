using Microservice.Appointments.IntegrationTests.Configuration;
using RestSharp;

namespace Microservice.Appointments.IntegrationTests.Endpoints.Base;

public class TestsBase
{
    protected readonly RestClient Client = new(ConfigurationFile.Instance.BaseUrl);
}