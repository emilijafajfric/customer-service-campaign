using System.Text;
using CustomerServiceCampaign.Api.Data;
using CustomerServiceCampaign.Api.Services.Purchases;
using Microsoft.EntityFrameworkCore;

namespace CustomerServiceCampaign.Tests.Services;

public class PurchaseImportServiceTests
{
    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task ImportAsync_ImportsValidPurchases()
    {
        await using var dbContext = CreateDbContext();

        var service = new PurchaseImportService(dbContext);

        const string csv = """
            CustomerId,PurchaseReference,Amount,PurchaseDate
            C-001,PUR-001,100.50,2026-11-07T10:30:00Z
            C-002,PUR-002,200.00,2026-11-08T11:00:00Z
            """;

        await using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(csv));

        var importedCount = await service.ImportAsync(stream);

        Assert.Equal(2, importedCount);
        Assert.Equal(2, dbContext.Purchases.Count());
    }

    [Fact]
    public async Task ImportAsync_SkipsDuplicatePurchaseReference()
    {
        await using var dbContext = CreateDbContext();

        var service = new PurchaseImportService(dbContext);

        const string csv = """
            CustomerId,PurchaseReference,Amount,PurchaseDate
            C-001,PUR-001,100.50,2026-11-07T10:30:00Z
            """;

        await using var firstStream = new MemoryStream(
            Encoding.UTF8.GetBytes(csv));

        await service.ImportAsync(firstStream);

        await using var secondStream = new MemoryStream(
            Encoding.UTF8.GetBytes(csv));

        var importedCount = await service.ImportAsync(secondStream);

        Assert.Equal(0, importedCount);
        Assert.Single(dbContext.Purchases);
    }

    [Fact]
    public async Task ImportAsync_Throws_WhenAmountIsInvalid()
    {
        await using var dbContext = CreateDbContext();

        var service = new PurchaseImportService(dbContext);

        const string csv = """
            CustomerId,PurchaseReference,Amount,PurchaseDate
            C-001,PUR-001,invalid,2026-11-07T10:30:00Z
            """;

        await using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(csv));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ImportAsync(stream));

        Assert.Equal(
            "Invalid purchase amount: invalid",
            exception.Message);
    }
}