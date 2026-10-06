namespace CustomerServiceCampaign.Api.Domain.Entities;

public class Reward
{
    public Guid Id { get; set; }

    public Guid CampaignId { get; set; }

    public Guid AgentId { get; set; }

    public required string CustomerId { get; set; }

    public DateTimeOffset RewardedAt { get; set; }

    public Campaign? Campaign { get; set; }

    public Agent? Agent { get; set; }
}