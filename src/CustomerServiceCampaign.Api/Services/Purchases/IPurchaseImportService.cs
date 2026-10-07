namespace CustomerServiceCampaign.Api.Services.Purchases;

public interface IPurchaseImportService
{
    Task<int> ImportAsync(Stream csvStream);
}