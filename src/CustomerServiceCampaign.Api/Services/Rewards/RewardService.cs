using CustomerServiceCampaign.Api.Data;
using CustomerServiceCampaign.Api.Domain.Entities;
using CustomerServiceCampaign.Api.Services.Customers;
using Microsoft.EntityFrameworkCore;

namespace CustomerServiceCampaign.Api.Services.Rewards;

public class RewardService(
    ApplicationDbContext dbContext,
    ICustomerService customerService) : IRewardService
{
    public async Task<Reward> CreateRewardAsync(
        Guid campaignId,
        Guid agentId,
        string customerId)
    {
        var campaign = await dbContext.Campaigns
            .SingleOrDefaultAsync(x => x.Id == campaignId);

        if (campaign is null)
        {
            throw new InvalidOperationException("Campaign not found.");
        }

        var agentExists = await dbContext.Agents
            .AnyAsync(x => x.Id == agentId);

        if (!agentExists)
        {
            throw new InvalidOperationException("Agent not found.");
        }

        var customerExists =
            await customerService.CustomerExistsAsync(customerId);

        if (!customerExists)
        {
            throw new InvalidOperationException("Customer not found.");
        }

        var now = DateTimeOffset.UtcNow;
        var today = DateOnly.FromDateTime(now.UtcDateTime);

        if (today < campaign.StartDate || today > campaign.EndDate)
        {
            throw new InvalidOperationException("Campaign is not active.");
        }

        var alreadyRewarded = await dbContext.Rewards.AnyAsync(x =>
            x.CampaignId == campaignId &&
            x.CustomerId == customerId);

        if (alreadyRewarded)
        {
            throw new InvalidOperationException(
                "Customer has already been rewarded in this campaign.");
        }

        var startOfDay = new DateTimeOffset(
            now.Year,
            now.Month,
            now.Day,
            0,
            0,
            0,
            TimeSpan.Zero);

        var endOfDay = startOfDay.AddDays(1);

        var agentRewards = await dbContext.Rewards
            .Where(x =>
                x.CampaignId == campaignId &&
                x.AgentId == agentId)
            .ToListAsync();

        var rewardsToday = agentRewards.Count(x =>
            x.RewardedAt >= startOfDay &&
            x.RewardedAt < endOfDay);

        if (rewardsToday >= campaign.DailyRewardLimit)
        {
            throw new InvalidOperationException(
                "Daily reward limit reached.");
        }

        var reward = new Reward
        {
            Id = Guid.NewGuid(),
            CampaignId = campaignId,
            AgentId = agentId,
            CustomerId = customerId,
            RewardedAt = now
        };

        dbContext.Rewards.Add(reward);
        await dbContext.SaveChangesAsync();

        return reward;
    }
}