using ClassifiedAds.Common.Entities;

namespace ClassifiedAds.Common.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
    string GenerateRefreshToken();
}
