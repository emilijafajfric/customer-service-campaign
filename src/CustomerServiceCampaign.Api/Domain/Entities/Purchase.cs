namespace CustomerServiceCampaign.Api.Domain.Entities;

public class Purchase
{
    public Guid Id { get; set; }

    public required string CustomerId { get; set; }

    public required string PurchaseReference { get; set; }

    public decimal Amount { get; set; }

    public DateTimeOffset PurchaseDate { get; set; }

    public DateTimeOffset ImportedAt { get; set; }
}