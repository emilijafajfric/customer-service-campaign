namespace CustomerServiceCampaign.Api.Contracts.Responses;

public class RewardResponse
{
    public Guid Id { get; set; }

    public Guid CampaignId { get; set; }

    public Guid AgentId { get; set; }

    public required string CustomerId { get; set; }

    public DateTimeOffset RewardedAt { get; set; }
}