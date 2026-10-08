using CustomerServiceCampaign.Api.Contracts.Requests;
using CustomerServiceCampaign.Api.Contracts.Responses;
using CustomerServiceCampaign.Api.Services.Rewards;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CustomerServiceCampaign.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RewardsController(IRewardService rewardService) : ControllerBase
{
    [Authorize(Roles = "Agent")]
    [HttpPost]
    public async Task<IActionResult> CreateReward(CreateRewardRequest request)
    {
        try
        {
            var reward = await rewardService.CreateRewardAsync(
                request.CampaignId,
                request.AgentId,
                request.CustomerId);

            var response = new RewardResponse
            {
                Id = reward.Id,
                CampaignId = reward.CampaignId,
                AgentId = reward.AgentId,
                CustomerId = reward.CustomerId,
                RewardedAt = reward.RewardedAt
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                error = exception.Message
            });
        }
    }
}