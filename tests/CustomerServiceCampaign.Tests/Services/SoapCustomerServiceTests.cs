using System.Net;
using System.Text;
using CustomerServiceCampaign.Api.Services.Customers;
using Microsoft.Extensions.Configuration;

namespace CustomerServiceCampaign.Tests.Services;

public class SoapCustomerServiceTests
{
    [Fact]
    public async Task CustomerExistsAsync_ReturnsTrue_WhenFindPersonResultExists()
    {
        const string soapResponse = """
            <SOAP-ENV:Envelope
                xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/">
              <SOAP-ENV:Body>
                <FindPersonResponse xmlns="http://tempuri.org">
                  <FindPersonResult>
                    <Name>Newton,Dave R.</Name>
                    <SSN>384-10-6538</SSN>
                    <Age>26</Age>
                  </FindPersonResult>
                </FindPersonResponse>
              </SOAP-ENV:Body>
            </SOAP-ENV:Envelope>
            """;

        var httpClient = CreateHttpClient(
            HttpStatusCode.OK,
            soapResponse);

        var configuration = CreateConfiguration();

        var service = new SoapCustomerService(
            httpClient,
            configuration);

        var exists = await service.CustomerExistsAsync("1");

        Assert.True(exists);
    }

    [Fact]
    public async Task CustomerExistsAsync_ReturnsFalse_WhenFindPersonResultDoesNotExist()
    {
        const string soapResponse = """
            <SOAP-ENV:Envelope
                xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/">
              <SOAP-ENV:Body>
                <FindPersonResponse xmlns="http://tempuri.org"/>
              </SOAP-ENV:Body>
            </SOAP-ENV:Envelope>
            """;

        var httpClient = CreateHttpClient(
            HttpStatusCode.OK,
            soapResponse);

        var configuration = CreateConfiguration();

        var service = new SoapCustomerService(
            httpClient,
            configuration);

        var exists = await service.CustomerExistsAsync("999999");

        Assert.False(exists);
    }

    private static HttpClient CreateHttpClient(
        HttpStatusCode statusCode,
        string responseContent)
    {
        var handler = new FakeHttpMessageHandler(
            statusCode,
            responseContent);

        return new HttpClient(handler);
    }

    private static IConfiguration CreateConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            ["CustomerService:SoapEndpoint"] =
                "https://example.com/soap"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _responseContent;

        public FakeHttpMessageHandler(
            HttpStatusCode statusCode,
            string responseContent)
        {
            _statusCode = statusCode;
            _responseContent = responseContent;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(
                    _responseContent,
                    Encoding.UTF8,
                    "text/xml")
            };

            return Task.FromResult(response);
        }
    }
}