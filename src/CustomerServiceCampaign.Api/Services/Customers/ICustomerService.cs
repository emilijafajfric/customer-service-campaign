namespace CustomerServiceCampaign.Api.Services.Customers;

public interface ICustomerService
{
    Task<bool> CustomerExistsAsync(string customerId);
}