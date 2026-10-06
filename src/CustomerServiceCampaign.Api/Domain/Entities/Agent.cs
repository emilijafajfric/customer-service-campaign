namespace CustomerServiceCampaign.Api.Domain.Entities;

public class Agent
{
    public Guid Id { get; set; }

    public required string Name { get; set; }
}