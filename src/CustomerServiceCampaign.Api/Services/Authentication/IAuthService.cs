using CustomerServiceCampaign.Api.Contracts.Responses;

namespace CustomerServiceCampaign.Api.Services.Authentication;

public interface IAuthService
{
    LoginResponse? Login(string username, string password);
}