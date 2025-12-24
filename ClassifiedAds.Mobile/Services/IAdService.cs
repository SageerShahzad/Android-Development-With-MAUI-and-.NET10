using ClassifiedAds.Mobile.Models;



namespace ClassifiedAds.Mobile.Services;



public interface IAdService

{

    Task<AdDTO?> GetAdById(int id);

}