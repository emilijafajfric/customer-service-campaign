namespace CustomerServiceCampaign.Api.Contracts.Responses;

public class CampaignResultResponse
{
    public required string CustomerId { get; set; }

    public Guid AgentId { get; set; }

    public DateTimeOffset RewardedAt { get; set; }

    public bool SuccessfulPurchase { get; set; }

    public int PurchaseCount { get; set; }

    public decimal TotalPurchaseAmount { get; set; }
}