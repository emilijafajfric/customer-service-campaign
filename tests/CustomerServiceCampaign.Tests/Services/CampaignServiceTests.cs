using CustomerServiceCampaign.Api.Data;
using CustomerServiceCampaign.Api.Domain.Entities;
using CustomerServiceCampaign.Api.Services.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace CustomerServiceCampaign.Tests.Services;

public class CampaignServiceTests
{
    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetResultsAsync_ReturnsMergedRewardAndPurchaseData()
    {
        await using var dbContext = CreateDbContext();

        var campaignId = Guid.NewGuid();
        var agentId = Guid.NewGuid();

        dbContext.Campaigns.Add(new Campaign
        {
            Id = campaignId,
            Name = "Test Campaign",
            StartDate = new DateOnly(2026, 10, 1),
            EndDate = new DateOnly(2026, 10, 7),
            DailyRewardLimit = 5,
            DiscountPercentage = 10
        });

        dbContext.Agents.Add(new Agent
        {
            Id = agentId,
            Name = "Test Agent"
        });

        dbContext.Rewards.AddRange(
            new Reward
            {
                Id = Guid.NewGuid(),
                CampaignId = campaignId,
                AgentId = agentId,
                CustomerId = "C-001",
                RewardedAt = DateTimeOffset.UtcNow
            },
            new Reward
            {
                Id = Guid.NewGuid(),
                CampaignId = campaignId,
                AgentId = agentId,
                CustomerId = "C-002",
                RewardedAt = DateTimeOffset.UtcNow.AddMinutes(1)
            });

        dbContext.Purchases.Add(new Purchase
        {
            Id = Guid.NewGuid(),
            CustomerId = "C-001",
            PurchaseReference = "PUR-001",
            Amount = 299.99m,
            PurchaseDate = DateTimeOffset.UtcNow.AddDays(30),
            ImportedAt = DateTimeOffset.UtcNow
        });

        await dbContext.SaveChangesAsync();

        var service = new CampaignService(dbContext);

        var results = await service.GetResultsAsync(campaignId);

        Assert.Equal(2, results.Count);

        var customerWithPurchase =
            results.Single(x => x.CustomerId == "C-001");

        Assert.True(customerWithPurchase.SuccessfulPurchase);
        Assert.Equal(1, customerWithPurchase.PurchaseCount);
        Assert.Equal(299.99m, customerWithPurchase.TotalPurchaseAmount);

        var customerWithoutPurchase =
            results.Single(x => x.CustomerId == "C-002");

        Assert.False(customerWithoutPurchase.SuccessfulPurchase);
        Assert.Equal(0, customerWithoutPurchase.PurchaseCount);
        Assert.Equal(0m, customerWithoutPurchase.TotalPurchaseAmount);
    }

    [Fact]
    public async Task GetResultsAsync_Throws_WhenCampaignDoesNotExist()
    {
        await using var dbContext = CreateDbContext();

        var service = new CampaignService(dbContext);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GetResultsAsync(Guid.NewGuid()));

        Assert.Equal(
            "Campaign not found.",
            exception.Message);
    }
}