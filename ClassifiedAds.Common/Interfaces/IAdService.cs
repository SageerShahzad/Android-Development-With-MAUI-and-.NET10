using ClassifiedAds.Common.Dtos;

namespace ClassifiedAds.Common.Interfaces
{
    public interface IAdService
    {
        Task<AdDTO> GetAdByIdAsync(int adId, string userId);
    }
}
