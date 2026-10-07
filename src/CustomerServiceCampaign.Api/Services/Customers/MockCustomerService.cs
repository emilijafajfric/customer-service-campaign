namespace CustomerServiceCampaign.Api.Services.Customers;

public class MockCustomerService : ICustomerService
{
    private static readonly HashSet<string> CustomerIds =
    [
        "C-001",
        "C-002",
        "C-003",
        "C-004",
        "C-005",
        "C-006"
    ];

    public Task<bool> CustomerExistsAsync(string customerId)
    {
        return Task.FromResult(CustomerIds.Contains(customerId));
    }
}