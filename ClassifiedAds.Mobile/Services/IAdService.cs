using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.Services;

public interface IAdService
{
    Task<List<AdDTO>> GetAds();
    Task<AdDTO?> GetAdById(int id);
}