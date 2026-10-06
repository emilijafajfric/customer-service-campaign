namespace CustomerServiceCampaign.Api.Domain.Entities;

public class Campaign
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int DailyRewardLimit { get; set; } = 5;

    public decimal DiscountPercentage { get; set; }
}