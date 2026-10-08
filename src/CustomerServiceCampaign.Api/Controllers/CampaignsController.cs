using CustomerServiceCampaign.Api.Services.Campaigns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CustomerServiceCampaign.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignsController(
    ICampaignService campaignService) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("{campaignId:guid}/results")]
    public async Task<IActionResult> GetResults(Guid campaignId)
    {
        try
        {
            var results =
                await campaignService.GetResultsAsync(campaignId);

            return Ok(results);
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new
            {
                error = exception.Message
            });
        }
    }
}