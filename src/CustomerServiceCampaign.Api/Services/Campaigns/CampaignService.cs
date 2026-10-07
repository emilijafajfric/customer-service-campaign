using CustomerServiceCampaign.Api.Contracts.Responses;
using CustomerServiceCampaign.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerServiceCampaign.Api.Services.Campaigns;

public class CampaignService(ApplicationDbContext dbContext)
    : ICampaignService
{
    public async Task<IReadOnlyList<CampaignResultResponse>> GetResultsAsync(
        Guid campaignId)
    {
        var campaignExists = await dbContext.Campaigns
            .AnyAsync(x => x.Id == campaignId);

        if (!campaignExists)
        {
            throw new InvalidOperationException("Campaign not found.");
        }

        var rewards = await dbContext.Rewards
            .AsNoTracking()
            .Where(x => x.CampaignId == campaignId)
            .ToListAsync();

        rewards = rewards
            .OrderBy(x => x.RewardedAt)
            .ToList();

        var customerIds = rewards
            .Select(x => x.CustomerId)
            .Distinct()
            .ToList();

        var purchases = await dbContext.Purchases
            .AsNoTracking()
            .Where(x => customerIds.Contains(x.CustomerId))
            .ToListAsync();

        var results = rewards.Select(reward =>
        {
            var customerPurchases = purchases
                .Where(x => x.CustomerId == reward.CustomerId)
                .ToList();

            return new CampaignResultResponse
            {
                CustomerId = reward.CustomerId,
                AgentId = reward.AgentId,
                RewardedAt = reward.RewardedAt,
                SuccessfulPurchase = customerPurchases.Count > 0,
                PurchaseCount = customerPurchases.Count,
                TotalPurchaseAmount =
                    customerPurchases.Sum(x => x.Amount)
            };
        }).ToList();

        return results;
    }
}