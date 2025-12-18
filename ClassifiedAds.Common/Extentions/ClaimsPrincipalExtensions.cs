using System.Security.Claims;

namespace ClassifiedAds.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetMemberId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new Exception("User ID claim not found.");
    }
}
