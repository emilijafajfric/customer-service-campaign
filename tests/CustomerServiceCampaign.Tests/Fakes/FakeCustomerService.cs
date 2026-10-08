using CustomerServiceCampaign.Api.Services.Customers;

namespace CustomerServiceCampaign.Tests.Fakes;

public class FakeCustomerService : ICustomerService
{
    private readonly HashSet<string> _customerIds;

    public FakeCustomerService(params string[] customerIds)
    {
        _customerIds = customerIds.ToHashSet();
    }

    public Task<bool> CustomerExistsAsync(string customerId)
    {
        return Task.FromResult(_customerIds.Contains(customerId));
    }
}