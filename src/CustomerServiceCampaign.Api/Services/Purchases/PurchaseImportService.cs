using System.Globalization;
using CustomerServiceCampaign.Api.Data;
using CustomerServiceCampaign.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerServiceCampaign.Api.Services.Purchases;

public class PurchaseImportService(ApplicationDbContext dbContext)
    : IPurchaseImportService
{
    public async Task<int> ImportAsync(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);

        var header = await reader.ReadLineAsync();

        if (header is null)
        {
            throw new InvalidOperationException("CSV file is empty.");
        }

        var importedCount = 0;

        while (await reader.ReadLineAsync() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var values = line.Split(',');

            if (values.Length != 4)
            {
                throw new InvalidOperationException(
                    $"Invalid CSV row: {line}");
            }

            var customerId = values[0].Trim();
            var purchaseReference = values[1].Trim();

            if (!decimal.TryParse(
                    values[2],
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var amount))
            {
                throw new InvalidOperationException(
                    $"Invalid purchase amount: {values[2]}");
            }

            if (!DateTimeOffset.TryParse(
                    values[3],
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out var purchaseDate))
            {
                throw new InvalidOperationException(
                    $"Invalid purchase date: {values[3]}");
            }

            var alreadyExists = await dbContext.Purchases
                .AnyAsync(x => x.PurchaseReference == purchaseReference);

            if (alreadyExists)
            {
                continue;
            }

            dbContext.Purchases.Add(new Purchase
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                PurchaseReference = purchaseReference,
                Amount = amount,
                PurchaseDate = purchaseDate,
                ImportedAt = DateTimeOffset.UtcNow
            });

            importedCount++;
        }

        await dbContext.SaveChangesAsync();

        return importedCount;
    }
}