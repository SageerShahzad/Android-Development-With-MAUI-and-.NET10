using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.Repositories;

public interface IAdRepository
{
    Task<List<AdDTO>> GetAds();
    Task<AdDTO?> GetAd(int id);
}