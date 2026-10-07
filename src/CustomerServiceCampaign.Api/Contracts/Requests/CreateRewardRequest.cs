namespace CustomerServiceCampaign.Api.Contracts.Requests;

public class CreateRewardRequest
{
    public Guid CampaignId { get; set; }

    public Guid AgentId { get; set; }

    public required string CustomerId { get; set; }
}