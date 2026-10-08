using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomerServiceCampaign.Api.Contracts.Responses;
using Microsoft.IdentityModel.Tokens;

namespace CustomerServiceCampaign.Api.Services.Authentication;

public class AuthService(IConfiguration configuration) : IAuthService
{
    public LoginResponse? Login(string username, string password)
    {
        string? role = null;

        if (username == "agent1" && password == "Agent123!")
        {
            role = "Agent";
        }
        else if (username == "admin" && password == "Admin123!")
        {
            role = "Admin";
        }

        if (role is null)
        {
            return null;
        }

        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT signing key is not configured.");

        var issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT issuer is not configured.");

        var audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT audience is not configured.");

        var expirationMinutes =
            configuration.GetValue<int>("Jwt:ExpirationMinutes");

        var expiresAt =
            DateTimeOffset.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, role)
        };

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new LoginResponse
        {
            AccessToken =
                new JwtSecurityTokenHandler().WriteToken(token),

            ExpiresAt = expiresAt
        };
    }
}