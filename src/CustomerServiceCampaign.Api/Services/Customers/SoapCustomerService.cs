using System.Text;
using System.Xml.Linq;

namespace CustomerServiceCampaign.Api.Services.Customers;

public class SoapCustomerService : ICustomerService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public SoapCustomerService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<bool> CustomerExistsAsync(string customerId)
    {
        var endpoint = _configuration["CustomerService:SoapEndpoint"];

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException(
                "SOAP customer service endpoint is not configured.");
        }

        XNamespace soapNamespace =
            "http://schemas.xmlsoap.org/soap/envelope/";

        XNamespace serviceNamespace =
            "http://tempuri.org";

        var soapEnvelope = new XDocument(
            new XElement(
                soapNamespace + "Envelope",
                new XElement(
                    soapNamespace + "Body",
                    new XElement(
                        serviceNamespace + "FindPerson",
                        new XElement(
                            serviceNamespace + "id",
                            customerId)))));

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            endpoint);

        request.Content = new StringContent(
            soapEnvelope.ToString(),
            Encoding.UTF8,
            "text/xml");

        request.Headers.Add(
            "SOAPAction",
            "\"http://tempuri.org/SOAP.Demo.FindPerson\"");

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                "Customer service request failed.");
        }

        var xml = await response.Content.ReadAsStringAsync();

        var document = XDocument.Parse(xml);

        var personResult = document
            .Descendants(serviceNamespace + "FindPersonResult")
            .FirstOrDefault();

        return personResult is not null &&
               personResult.Elements().Any();
    }
}