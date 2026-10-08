using CustomerServiceCampaign.Api.Contracts.Requests;
using CustomerServiceCampaign.Api.Services.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CustomerServiceCampaign.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var result = authService.Login(
            request.Username,
            request.Password);

        if (result is null)
        {
            return Unauthorized(new
            {
                error = "Invalid username or password."
            });
        }

        return Ok(result);
    }
}