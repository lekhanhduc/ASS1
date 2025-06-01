using System.Security.Claims;

namespace FUNewsManagementSystem.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(Claim[] claims);
        string GenerateRefreshToken(Claim[] claims);
    }
}
