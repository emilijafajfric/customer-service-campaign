using CustomerServiceCampaign.Api.Contracts.Responses;

namespace CustomerServiceCampaign.Api.Services.Campaigns;

public interface ICampaignService
{
    Task<IReadOnlyList<CampaignResultResponse>> GetResultsAsync(
        Guid campaignId);
}