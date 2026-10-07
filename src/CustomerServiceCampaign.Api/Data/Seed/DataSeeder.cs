using CustomerServiceCampaign.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerServiceCampaign.Api.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext)
    {
        if (!await dbContext.Agents.AnyAsync())
        {
            dbContext.Agents.AddRange(
                new Agent
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Agent One"
                },
                new Agent
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Agent Two"
                });
        }

        if (!await dbContext.Campaigns.AnyAsync())
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            dbContext.Campaigns.Add(
                new Campaign
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Loyal Customer Campaign",
                    StartDate = today,
                    EndDate = today.AddDays(6),
                    DailyRewardLimit = 5,
                    DiscountPercentage = 10
                });
        }

        await dbContext.SaveChangesAsync();
    }
}