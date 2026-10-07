using CustomerServiceCampaign.Api.Domain.Entities;

namespace CustomerServiceCampaign.Api.Services.Rewards;
public interface IRewardService
{
    Task<Reward> CreateRewardAsync(
        Guid campaignId,
        Guid agentId,
        string customerId);
}