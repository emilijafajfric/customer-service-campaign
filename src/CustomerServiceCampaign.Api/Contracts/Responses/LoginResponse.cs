namespace CustomerServiceCampaign.Api.Contracts.Responses;

public class LoginResponse
{
    public required string AccessToken { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }
}