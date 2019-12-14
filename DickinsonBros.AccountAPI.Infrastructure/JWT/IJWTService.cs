using System.Collections.Generic;
using System.Security.Claims;

namespace DickinsonBros.AccountAPI.Infrastructure.JWT
{
    public interface IJWTService
    {
        string GenerateJWT(string nameIdentifier, System.DateTime expiresDateTime, bool isRefresh = false);
        string GenerateJWT(IEnumerable<Claim> claims, System.DateTime expiresDateTime, bool isRefresh = false);
        ClaimsPrincipal GetPrincipal(string token, bool isRefresh = false, bool vaildateLifetime = true);
    }
}