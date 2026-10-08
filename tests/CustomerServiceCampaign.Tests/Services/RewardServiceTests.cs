using CustomerServiceCampaign.Api.Data;
using CustomerServiceCampaign.Api.Domain.Entities;
using CustomerServiceCampaign.Api.Services.Rewards;
using CustomerServiceCampaign.Tests.Fakes;
using Microsoft.EntityFrameworkCore;

namespace CustomerServiceCampaign.Tests.Services;

public class RewardServiceTests
{
    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreateRewardAsync_CreatesReward_WhenRequestIsValid()
    {
        await using var dbContext = CreateDbContext();

        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            Name = "Test Campaign",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
            DailyRewardLimit = 5,
            DiscountPercentage = 10
        };

        var agent = new Agent
        {
            Id = Guid.NewGuid(),
            Name = "Test Agent"
        };

        dbContext.Campaigns.Add(campaign);
        dbContext.Agents.Add(agent);
        await dbContext.SaveChangesAsync();

        var customerService = new FakeCustomerService("C-001");

        var service = new RewardService(
            dbContext,
            customerService);

        var reward = await service.CreateRewardAsync(
            campaign.Id,
            agent.Id,
            "C-001");

        Assert.Equal("C-001", reward.CustomerId);
        Assert.Equal(agent.Id, reward.AgentId);
        Assert.Equal(campaign.Id, reward.CampaignId);

        Assert.Single(dbContext.Rewards);
    }

    [Fact]
    public async Task CreateRewardAsync_Throws_WhenCustomerDoesNotExist()
    {
        await using var dbContext = CreateDbContext();

        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            Name = "Test Campaign",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
            DailyRewardLimit = 5,
            DiscountPercentage = 10
        };

        var agent = new Agent
        {
            Id = Guid.NewGuid(),
            Name = "Test Agent"
        };

        dbContext.Campaigns.Add(campaign);
        dbContext.Agents.Add(agent);
        await dbContext.SaveChangesAsync();

        var customerService = new FakeCustomerService();

        var service = new RewardService(
            dbContext,
            customerService);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateRewardAsync(
                campaign.Id,
                agent.Id,
                "C-999"));

        Assert.Equal(
            "Customer not found.",
            exception.Message);
    }

    [Fact]
    public async Task CreateRewardAsync_Throws_WhenDailyLimitIsReached()
    {
        await using var dbContext = CreateDbContext();

        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            Name = "Test Campaign",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
            DailyRewardLimit = 5,
            DiscountPercentage = 10
        };

        var agent = new Agent
        {
            Id = Guid.NewGuid(),
            Name = "Test Agent"
        };

        dbContext.Campaigns.Add(campaign);
        dbContext.Agents.Add(agent);

        var now = DateTimeOffset.UtcNow;

        for (var i = 1; i <= 5; i++)
        {
            dbContext.Rewards.Add(new Reward
            {
                Id = Guid.NewGuid(),
                CampaignId = campaign.Id,
                AgentId = agent.Id,
                CustomerId = $"C-{i:000}",
                RewardedAt = now
            });
        }

        await dbContext.SaveChangesAsync();

        var customerService =
            new FakeCustomerService("C-006");

        var service = new RewardService(
            dbContext,
            customerService);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateRewardAsync(
                campaign.Id,
                agent.Id,
                "C-006"));

        Assert.Equal(
            "Daily reward limit reached.",
            exception.Message);
    }
    
    [Fact]
    public async Task CreateRewardAsync_Throws_WhenCustomerWasAlreadyRewarded()
    {
        await using var dbContext = CreateDbContext();

        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            Name = "Test Campaign",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
            DailyRewardLimit = 5,
            DiscountPercentage = 10
        };

        var agent = new Agent
        {
            Id = Guid.NewGuid(),
            Name = "Test Agent"
        };

        dbContext.Campaigns.Add(campaign);
        dbContext.Agents.Add(agent);

        dbContext.Rewards.Add(new Reward
        {
            Id = Guid.NewGuid(),
            CampaignId = campaign.Id,
            AgentId = agent.Id,
            CustomerId = "C-001",
            RewardedAt = DateTimeOffset.UtcNow
        });

        await dbContext.SaveChangesAsync();

        var customerService = new FakeCustomerService("C-001");

        var service = new RewardService(
            dbContext,
            customerService);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateRewardAsync(
                campaign.Id,
                agent.Id,
                "C-001"));

        Assert.Equal(
            "Customer has already been rewarded in this campaign.",
            exception.Message);
    }
}